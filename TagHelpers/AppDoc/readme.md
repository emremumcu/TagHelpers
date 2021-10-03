# Tag Helpers in ASP.NET Core
https://docs.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/authoring?view=aspnetcore-5.0#get-started-with-tag-helpers

## What are Tag Helpers

Tag Helpers enable server-side code to participate in creating and rendering HTML elements in Razor files. Tag Helpers are authored in C#, 
and they target HTML elements based on element name, attribute name, or parent tag. Tag Helpers reduce the explicit transitions between 
HTML and C# in Razor views. In many cases, HTML Helpers provide an alternative approach to a specific Tag Helper, but it's important to 
recognize that Tag Helpers don't replace HTML Helpers and there's not a Tag Helper for each HTML Helper.

## Writing Tag Helpers

To create custom tag helper, the first step is to create a class that inherits from "TagHelper" class. This class has a virtual method to 
generate HTML tags. It contains both synchronous (Process) and asynchronous (ProcessAsync) implementation of the virtual method.

> public virtual Task ProcessAsync(TagHelperContext context, TagHelperOutput output);  
> public virtual void Process(TagHelperContext context, TagHelperOutput output);  

We can implement either one of these two or both, based on our requirement. The Process method (or ProcessAsync) is responsible to generate 
the HTML that is rendered by the browser. It receives context of tag helper instance and TegHelperOuter which we can be used to read and 
change the content of our tag helper that is within scope.

Tag helpers use a naming convention that targets elements of the root class name (minus the TagHelper portion of the class name). For example, 
if we have a tag helper class named EmailTagHelper, since the root name of EmailTagHelper is email, so the <email> tag will be targeted. 
This naming convention should work for most tag helpers.

The tag helper class must be derived from TagHelper class (in Microsoft.AspNetCore.Razor.TagHelpers). The TagHelper class provides methods and 
properties for writing Tag Helpers.

The overridden Process method controls what the tag helper does when executed. The TagHelper class also provides an asynchronous version 
(ProcessAsync) with the same parameters.

The context parameter to Process (and ProcessAsync) contains information associated with the execution of the current HTML tag.

The output parameter to Process (and ProcessAsync) contains a stateful HTML element representative of the original source used to generate 
an HTML tag and content.

Sample EmailTagHelper class name has a suffix of TagHelper, which is not required, but it's considered a best practice convention. 

Tag helpers translates Pascal-cased C# class names and properties for tag helpers into kebab case. Therefore, to use the 
WebsiteInformationTagHelper in Razor, you'll write <website-information />

## Notice

Pascal-cased class and property names for tag helpers are translated into their kebab case. Therefore, to use the MailTo attribute, 
you'll use <email mail-to="value"/> equivalent.

As mentioned previously, tag helpers translates Pascal-cased C# class names and properties for tag helpers into kebab case. Therefore, 
to use the WebsiteInformationTagHelper in Razor, you'll write <website-information />.

You are not explicitly identifying the target element with the [HtmlTargetElement] attribute, so the default of website-information 
will be targeted. If you applied the following attribute (note it's not kebab case but matches the class name):

[HtmlTargetElement("WebsiteInformation")]

The kebab case tag <website-information /> wouldn't match. If you want use the [HtmlTargetElement] attribute, you would use kebab 
case as shown below:

[HtmlTargetElement("Website-Information")]

If you were to write the email tag self-closing (<email mail-to="Rick" />), the final output would also be self-closing. 
To enable the ability to write the tag with only a start tag (<email mail-to="Rick">) you must mark the class with the following:

> [HtmlTargetElement("email", TagStructure = TagStructure.WithoutEndTag)] 
> public class EmailVoidTagHelper : TagHelper

With a self-closing email tag helper, the output would be <a href="mailto:Rick@contoso.com" />. 
Self-closing anchor tags are not valid HTML, so you wouldn't want to create one, but you might want to create a tag helper that's 
self-closing. Tag helpers set the type of the TagMode property after reading a tag.

The [HtmlTargetElement] attribute passes an attribute parameter that specifies that any HTML element that contains an HTML attribute 
named "bold" will match, and the Process override method in the class will run. 

<p bold>...</p>

The [HtmlTargetElement] attribute above only targets HTML markup that provides an attribute name of "bold". The <bold> element 
wasn't modified by the tag helper. If you comment out the [HtmlTargetElement] attribute line and it will default to targeting <bold> 
tags, that is, HTML markup of the form <bold>. Remember, the default naming convention will match the class name BoldTagHelper to 
<bold> tags.

Decorating a class with multiple [HtmlTargetElement] attributes results in a logical-OR of the targets. 

[HtmlTargetElement("bold")]
[HtmlTargetElement(Attributes = "bold")]
public class BoldTagHelper : TagHelper

When multiple attributes are added to the same statement, the runtime treats them as a logical-AND.

[HtmlTargetElement("bold", Attributes = "bold")]

You can also use the [HtmlTargetElement] to change the name of the targeted element. For example if you wanted the BoldTagHelper to target 
<MyBold> tags, you would use the following attribute:

[HtmlTargetElement("MyBold")]

Use the nameof operator to specify the attribute to target rather than specifying a string as you did with the bold tag helper:

[HtmlTargetElement(Attributes = nameof(Condition))]
 //   [HtmlTargetElement(Attributes = "condition")]
 public class ConditionTagHelper : TagHelper
{
   public bool Condition { get; set; }

   public override void Process(TagHelperContext context, TagHelperOutput output)

The nameof operator will protect the code should it ever be refactored (we might want to change the name to RedCondition).

## ProcessAsync

```csharp

public class EmailTagHelper : TagHelper
{
    private const string EmailDomain = "contoso.com";
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";                                 // Replaces <email> with <a> tag
        var content = await output.GetChildContentAsync();
        var target = content.GetContent() + "@" + EmailDomain;
        output.Attributes.SetAttribute("href", "mailto:" + target);
        output.Content.SetContent(target);
    }
}

```

This version uses the asynchronous ProcessAsync method. The asynchronous GetChildContentAsync returns a Task containing the 
TagHelperContent.

## Managing Tag Helper scope

Tag Helpers scope is controlled by a combination of @addTagHelper, @removeTagHelper, and the "!" opt-out character.

### @addTagHelper makes Tag Helpers available

The @addTagHelper directive makes Tag Helpers available to the view. 
/Views/_ViewImports.cshtml by default is inherited by all files in the Views folder and subfolders adding the @addTagHelper directive to 
the Views/_ViewImports.cshtml file makes the Tag Helper available to all view files in the Views directory and subdirectories. You can 
use the @addTagHelper directive in specific view files if you want to opt-in to exposing the Tag Helper to only those views.

> @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

The first parameter after @addTagHelper specifies the Tag Helpers to load (we are using "*" for all Tag Helpers), and the second parameter 
"Microsoft.AspNetCore.Mvc.TagHelpers" specifies the assembly containing the Tag Helpers. Microsoft.AspNetCore.Mvc.TagHelpers is the assembly 
for the built-in ASP.NET Core Tag Helpers.

To expose all of the Tag Helpers in this project (which creates an assembly named AspNetDemo_TagHelpers), you would use the following:

> @using AspNetDemo_TagHelpers
> @addTagHelper *, AspNetDemo_TagHelpers

If the project contains an EmailTagHelper with the default namespace (AspNetDemo_TagHelpers.TagHelpers.EmailTagHelper), you can provide the 
fully qualified name (FQN) of the Tag Helper:

> @using AspNetDemo_TagHelpers
> @addTagHelper AspNetDemo_TagHelpers.TagHelpers.EmailTagHelper, AspNetDemo_TagHelpers

To add a Tag Helper to a view using an FQN, you first add the FQN (AspNetDemo_TagHelpers.TagHelpers.EmailTagHelper), and then the assembly 
name (AspNetDemo_TagHelpers). Most developers prefer to use the "*" wildcard syntax. The wildcard syntax allows you to insert the wildcard 
character "*" as the suffix in an FQN. For example, any of the following directives will bring in the EmailTagHelper:

> @addTagHelper AspNetDemo_TagHelpers.TagHelpers.E*, AspNetDemo_TagHelpers
> @addTagHelper AspNetDemo_TagHelpers.TagHelpers.Email*, AspNetDemo_TagHelpers

### @removeTagHelper removes Tag Helpers

The @removeTagHelper has the same two parameters as @addTagHelper, and it removes a Tag Helper that was previously added. For example, 
@removeTagHelper applied to a specific view removes the specified Tag Helper from the view. 

Using @removeTagHelper in a Views/Folder/_ViewImports.cshtml file removes the specified Tag Helper from all of the views in Folder.

### Controlling Tag Helper scope with the _ViewImports.cshtml file

You can add a _ViewImports.cshtml to any view folder, and the view engine applies the directives from both that file and the 
Views/_ViewImports.cshtml file. If you added an empty Views/Home/_ViewImports.cshtml file for the Home views, there would be no change 
because the _ViewImports.cshtml file is additive. Any @addTagHelper directives you add to the Views/Home/_ViewImports.cshtml file 
(that are not in the default Views/_ViewImports.cshtml file) would expose those Tag Helpers to views only in the Home folder.

### Opting out of individual elements

You can disable a Tag Helper at the element level with the Tag Helper opt-out character ("!"). 

You must apply the Tag Helper opt-out character to the opening and closing tag. (The Visual Studio editor automatically adds the opt-out 
character to the closing tag when you add one to the opening tag). After you add the opt-out character, the element and Tag Helper 
attributes are no longer displayed in a distinctive font.

```html
<!span asp-validation-for="Email" class="text-danger"></!span>
```

### Using @tagHelperPrefix to make Tag Helper usage explicit

The @tagHelperPrefix directive allows you to specify a tag prefix string to enable Tag Helper support and to make Tag Helper usage 
explicit. For example, you could add the following markup to the Views/_ViewImports.cshtml file:

> @tagHelperPrefix th:

In the code image below, the Tag Helper prefix is set to th:, so only those elements using the prefix th: support Tag Helpers 
(Tag Helper-enabled elements have a distinctive font).

<th:span class="text-danger"></th:span>

### Self-closing Tag Helpers

Many Tag Helpers can't be used as self-closing tags. Some Tag Helpers are designed to be self-closing tags. Using a Tag Helper 
that was not designed to be self-closing suppresses the rendered output. Self-closing a Tag Helper results in a self-closing 
tag in the rendered output.

### C# in Tag Helpers attribute/declaration

Tag Helpers do not allow C# in the element's attribute or tag declaration area. For example, the following code is not valid:

<input asp-for="LastName" @(Model?.LicenseId == null ? "disabled" : string.Empty) />

The preceding code can be written as:

<input asp-for="LastName" disabled="@(Model?.LicenseId == null)" />

### Tag Helpers compared to HTML Helpers

Tag Helpers attach to HTML elements in Razor views, while HTML Helpers are invoked as methods interspersed with HTML in Razor views. 
Consider the following Razor markup, which creates an HTML label with the CSS class "caption":

@Html.Label("FirstName", "First Name:", new {@class="caption"})

The at (@) symbol tells Razor this is the start of code. The next two parameters ("FirstName" and "First Name:") are strings, so 
IntelliSense can't help. The last argument:

new {@class="caption"}

Is an anonymous object used to represent attributes. Because class is a reserved keyword in C#, you use the @ symbol to force C# 
to interpret @class= as a symbol (property name). To a front-end designer (someone familiar with HTML/CSS/JavaScript and other client 
technologies but not familiar with C# and Razor), most of the line is foreign. The entire line must be authored with no help from 
IntelliSense.

Using the LabelTagHelper, the same markup can be written as:

<label class="caption" asp-for="FirstName"></label>

With the Tag Helper version, the Visual Studio editor, IntelliSense displays matching elements.

The markup is much cleaner and easier to read, edit, and maintain than the HTML Helpers approach. The C# code is reduced to the 
minimum that the server needs to know about. The Visual Studio editor displays markup targeted by a Tag Helper in a distinctive font.

### Tag Helpers compared to Web Server Controls

- Tag Helpers don't own the element they're associated with; they simply participate in the rendering of the element and content.
- Web Server controls allow you to add functionality to the client Document Object Model (DOM) elements by using a client control. 
Tag Helpers have no DOM.
- Web Server controls include automatic browser detection. Tag Helpers have no knowledge of the browser.
- Multiple Tag Helpers can act on the same element (see Avoiding Tag Helper conflicts ) while you typically can't compose Web Server c
ontrols.
- Tag Helpers can modify the tag and content of HTML elements that they're scoped to, but don't directly modify anything else on a page. 
Web Server controls have a less specific scope and can perform actions that affect other parts of your page; enabling unintended side 
effects.
- Web Server controls use type converters to convert strings into objects. With Tag Helpers, you work natively in C#, 
so you don't need to do type conversion.
- Web Server controls use System.ComponentModel to implement the run-time and design-time behavior of components and controls. 
System.ComponentModel includes the base classes and interfaces for implementing attributes and type converters, binding to data sources, 
and licensing components. Contrast that to Tag Helpers, which typically derive from TagHelper, and the TagHelper base class exposes only 
two methods, Process and ProcessAsync.

### Customizing the Tag Helper element font

You can customize the font and colorization from Tools > Options > Environment > Fonts and Colors:

- HTML Razor Tag Helper Attribute
- HTML Razor Tag Helper Element

# Creating Custom Tag Helpers With ASP.NET Core MVC

https://www.c-sharpcorner.com/article/creating-custom-tag-helpers-with-asp-net-core-mvc/

To create custom tag helper, the first step is to create a class that inherits from "TagHelper" class. This class has a virtual method to generate HTML tags. It contains both synchronous (Process) and asynchronous (ProcessAsync) implementation of the virtual method.

``` csharp
public virtual Task ProcessAsync(TagHelperContext context, TagHelperOutput output);  
public virtual void Process(TagHelperContext context, TagHelperOutput output); 
```

We can implement either one of these two or both, based on our requirement. The Process method (or ProcessAsync) is responsible to generate the HTML that is rendered by the browser. It receives context of tag helper instance and TegHelperOuter which we can be used to read and change the content of our tag helper that is within scope.

# Code


    /// <summary>
    /// The [HtmlTargetElement] attribute passes an attribute parameter that specifies that any HTML element that contains an HTML attribute named "bold" will match, and the Process override method in the class will run.
    /// The [HtmlTargetElement] attribute above only targets HTML markup that provides an attribute name of "bold". The <bold> element wasn't modified by the tag helper.
    /// If you comment out the [HtmlTargetElement] attribute line, it will default to targeting <bold> tags, that is, HTML markup of the form <bold>. Remember, the default naming convention will match the class name BoldTagHelper to <bold> tags (matching name without the TagHelper suffix).
    /// -----------------------------------------
    /// Decorating a class with multiple [HtmlTargetElement] attributes results in a logical-OR of the targets. For example, using the code below, a bold tag or a bold attribute will match.
    /// [HtmlTargetElement("bold")]
    /// [HtmlTargetElement(Attributes = "bold")]
    /// -----------------------------------------
    /// When multiple attributes are added to the same statement, the runtime treats them as a logical-AND. For example, in the code below, an HTML element must be named "bold" with an attribute named "bold" (<bold bold />) to match.
    /// [HtmlTargetElement("bold", Attributes = "bold")]
    /// -----------------------------------------
    /// You can also use the [HtmlTargetElement] to change the name of the targeted element. For example if you wanted the BoldTagHelper to target <MyBold> tags, you would use the following attribute:
    /// [HtmlTargetElement("MyBold")]
    /// </summary>


    /*
     * Tag helpers use a naming convention that targets elements of the root class name (minus the TagHelper portion of the class name). In this example, the root name of EmailTagHelper is email, so the <email> tag will be targeted. This naming convention should work for most tag helpers, it can be overriden.
     * 
     * The EmailTagHelper class derives from TagHelper. The TagHelper class provides methods and properties for writing Tag Helpers. The overridden Process method controls what the tag helper does when executed. The TagHelper class also provides an asynchronous version (ProcessAsync) with the same parameters. The context parameter to Process (and ProcessAsync) contains information associated with the execution of the current HTML tag.
     * 
     * The output parameter to Process (and ProcessAsync) contains a stateful HTML element representative of the original source used to generate an HTML tag and content. Our class name has a suffix of TagHelper, which is not required, but it's considered a best practice convention.
     * 
     * To make the EmailTagHelper class available to all our Razor views, add the addTagHelper directive to the Views/_ViewImports.cshtml file: @addTagHelper *, TagHelpers (where TagHelpers is the name of the project)
     * 
     */

    /*
     * If you were to write the email tag self-closing (<email mail-to="Rick" />), the final output would also be self-closing. To enable the ability to write the tag with only a start tag (<email mail-to="Rick">) you must mark the class with the following:
     * 
     [HtmlTargetElement("email", TagStructure = TagStructure.WithoutEndTag)] 
     public class EmailVoidTagHelper : TagHelper 

     With a self-closing email tag helper, the output would be <a href="mailto:Rick@contoso.com" />. Self-closing anchor tags are not valid HTML, so you wouldn't want to create one, but you might want to create a tag helper that's self-closing. Tag helpers set the type of the TagMode property after reading a tag.

     */

    /// <summary>
    /// Tag helpers translates PascalCased C# class names and properties for tag helpers into kebab-case. Therefore, to use the WebsiteInformationTagHelper in Razor, you'll write <website-information />.
    /// </summary>