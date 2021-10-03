``` csharp

namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;


    [HtmlTargetElement(Attributes = "bold")]
    public class BoldTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            /// In our sample, the Process method removes the "bold" attribute and surrounds the containing markup with <strong></strong>
            /// Because you don't want to replace the existing tag content, you must write the opening <strong> tag with the PreContent.SetHtmlContent method and the closing </strong> tag with the PostContent.SetHtmlContent method.
            /// 
            output.Attributes.RemoveAll("bold");
            output.PreContent.SetHtmlContent("<strong>");
            output.PostContent.SetHtmlContent("</strong>");
        }
    }
}

namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using System.Threading.Tasks;

    [HtmlTargetElement("BsSubmit")]
    [OutputElementHint("button")]
    public class SubmitTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "button";

            output.Attributes.Add("type", "submit");

            output.AddCssClass("btn");

            output.AddCssClass("btn-primary");

            

            var content = await output.GetChildContentAsync();

            //var target = content.GetContent() + "@mumcu.net";

            //output.Attributes.SetAttribute("href", "mailto:" + target);

            //output.Content.SetContent(target);
        }
    }
}


namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Mvc.TagHelpers;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// This is a custom implementation of partial tag helper in Asp.NET.
    /// Partial control is rendered based on the value of if parameter.
    /// ex: <partial-c when="true" name="/ViewPartials/_ThemeSelector.cshtml" model="null" />
    /// ex: <partial-c when=true name="/ViewPartials/_ThemeSelector.cshtml" model="null" />
    /// ex: <partial-c when="(DateTime.UtcNow.Year==2021)?(true):(false)" name="/ViewPartials/_ThemeSelector.cshtml" model="null" />
    /// ex: <partial-c if="User.Identity.IsAuthenticated" name="/ViewPartials/_Header.cshtml" />
    /// https://github.com/reZach/minifynetcore/blob/master/MinifyNETCore/MinifyNETCore/TagHelpers/MPartialTagHelper.cs
    /// </summary>
    [HtmlTargetElement("partial-c", TagStructure = TagStructure.WithoutEndTag)]
    public class ConditionalPartialTagHelper : PartialTagHelper
    {
        [HtmlAttributeName("when")]
        public bool Include { get; set; } = true;

        public ConditionalPartialTagHelper(ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope) : base(viewEngine, viewBufferScope) { }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!Include)
                return Task.CompletedTask;
            else
                return base.ProcessAsync(context, output);
        }

        private bool IsDevelopment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;
        }
    }
}



namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;


    /// <summary>
    /// Applies to all html elements having a condition attribute.
    /// If the specified condition is not true, then the element is not rendered at all.
    /// ex: <p Condition="false" >This is a paragraph but not rendered since condition is false</p>
    /// ex: <header Condition="User.Identity.IsAuthenticated" class="navbar-margin">
    /// Use the nameof operator to specify the attribute to target rather than specifying a string as you did with the bold tag helper.
    /// The nameof operator will protect the code should it ever be refactored (we might want to change the name to RedCondition).
    /// </summary>
    [HtmlTargetElement(Attributes = nameof(Condition))]
    public class ConditionTagHelper : TagHelper
    {
        public bool Condition { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Condition)
            {
                output.SuppressOutput();
            }
        }
    }
}



namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using System.Threading.Tasks;

    public class EmailAsyncTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";

            var content = await output.GetChildContentAsync();

            var target = content.GetContent() + "@mumcu.net";

            output.Attributes.SetAttribute("href", "mailto:" + target);

            output.Content.SetContent(target);
        }
    }
}



namespace TagHelpers.TagHelpers
{

    using Microsoft.AspNetCore.Razor.TagHelpers;



    public class EmailTagHelper : TagHelper
    {
        // Can be passed via <email mail-to="..." />.
        // PascalCase gets translated into kebab-case.
        public string MailTo { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";    // Replaces <email> with <a> tag

            output.TagMode = TagMode.StartTagAndEndTag;

            var address = MailTo + "@mumcu.net";

            // This approach works for the attribute "href" as long as it doesn't currently exist in the attributes collection.
            // You can also use the output.Attributes.Add method to add a tag helper attribute to the end of the collection of tag attributes.
            output.Attributes.SetAttribute("href", "mailto:" + address);

            output.Content.SetContent(address);
        }


    }
}




namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Extentions
    {
        public static void AddCssClass(this TagHelperOutput output, String cssClass)
        {
            AddCssClass(output, new List<string>() { cssClass });
        }

        public static void AddCssClass(this TagHelperOutput output, List<string> cssClasses)
        {
            // Create a function that takes 2 strings and returns one (concat 2 input string as the output):
            Func<string, string, string> f_join = new Func<string, string, string>((string s1, string s2) => string.Concat(s1, " ", s2));

            if (output.Attributes.ContainsName("class"))
            {
                string existinClassValues = output.Attributes["class"].Value?.ToString().Trim();
                if (!string.IsNullOrEmpty(existinClassValues)) cssClasses.Insert(0, existinClassValues);
            }

            // TagHelperAttribute classAttribute = new TagHelperAttribute("class", string.Join(" ", cssClasses));
            TagHelperAttribute classAttribute = new TagHelperAttribute("class", Enumerable.Aggregate<string>(cssClasses, f_join));

            output.Attributes.SetAttribute(classAttribute);
        }
    }
}


namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// This tag helper renders its content only if the itis value is true.
    /// ex: <if is="true"><p>True</p></if>
    /// ex: <if is="1 == 1"><p>True</p></if>
    /// https://andrewlock.net/creating-an-if-tag-helper-to-conditionally-render-content/
    /// </summary>
    [HtmlTargetElement("if", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class IfTagHelper : TagHelper
    {
        public override int Order => -1000;

        [HtmlAttributeName("is")]
        public bool Include { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Always strip the outer tag name as we never want <if> to render
            output.TagName = null;

            if (Include) return;
            else output.SuppressOutput();
        }
    }
}



namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using System.Linq;
    using System.Security.Claims;

    [HtmlTargetElement("UserInfo", TagStructure = TagStructure.WithoutEndTag)]
    public class UserInfoTagHelper : TagHelper
    {
        private ClaimsPrincipal user { get; set; }

        // Requires: services.AddHttpContextAccessor();
        public UserInfoTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            user = httpContextAccessor.HttpContext.User;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "table";

            output.TagMode = TagMode.StartTagAndEndTag;

            TagBuilder tableBuilder = new TagBuilder("table");

            tableBuilder.AddCssClass("table table-sm table-hover table-bordered");

            string[] captions = { "ISSUER", "TYPE", "NAME", "VALUE" };

            TagBuilder theadBuilder = new TagBuilder("thead");
            TagBuilder tbodyBuilder = new TagBuilder("tbody");
            TagBuilder trBuilder = new TagBuilder("tr");
            TagBuilder thBuilder;
            TagBuilder tdBuilder;

            foreach (string s in captions)
            {
                thBuilder = new TagBuilder("th");
                thBuilder.Attributes["scope"] = "col";
                thBuilder.InnerHtml.Append(s);

                trBuilder.InnerHtml.AppendHtml(thBuilder);
            }

            theadBuilder.AddCssClass("elegant-color white-text");

            theadBuilder.InnerHtml.AppendHtml(trBuilder);

            foreach (Claim c in user.Claims)
            {
                trBuilder = new TagBuilder("tr");

                tdBuilder = new TagBuilder("td");
                tdBuilder.InnerHtml.Append(c.Issuer);
                trBuilder.InnerHtml.AppendHtml(tdBuilder);

                tdBuilder = new TagBuilder("td");
                tdBuilder.InnerHtml.Append(c.Type);
                trBuilder.InnerHtml.AppendHtml(tdBuilder);

                tdBuilder = new TagBuilder("td");
                tdBuilder.InnerHtml.Append((c.Type.StartsWith("http")) ? (c.Type.Split('/').LastOrDefault()) : (c.Type));
                trBuilder.InnerHtml.AppendHtml(tdBuilder);

                tdBuilder = new TagBuilder("td");
                tdBuilder.InnerHtml.Append(c.Value);
                trBuilder.InnerHtml.AppendHtml(tdBuilder);

                tbodyBuilder.InnerHtml.AppendHtml(trBuilder);
            }

            tableBuilder.InnerHtml.AppendHtml(theadBuilder).AppendHtml(tbodyBuilder);

            output.PreContent.AppendHtml(tableBuilder);

        }

        private void BuildTable()
        {
            TagBuilder table = new TagBuilder("table");
            TagBuilder thead = new TagBuilder("thead");
            TagBuilder tbody = new TagBuilder("tbody");
            TagBuilder tfoot = new TagBuilder("tfoot");
            TagBuilder tr = new TagBuilder("tr");
            TagBuilder th = new TagBuilder("th");
            TagBuilder td = new TagBuilder("td");

            string[] captions = { "ISSUER", "TYPE", "NAME", "VALUE" };



            foreach (string s in captions)
            {
                th = new TagBuilder("th");
                th.Attributes["scope"] = "col";
                th.InnerHtml.Append(s);

                tr.InnerHtml.AppendHtml(th);
            }

            thead.AddCssClass("elegant-color white-text");

            thead.InnerHtml.AppendHtml(tr);

            foreach (Claim c in user.Claims)
            {
                tr = new TagBuilder("tr");

                td = new TagBuilder("td");
                td.InnerHtml.Append(c.Issuer);
                tr.InnerHtml.AppendHtml(td);

                td = new TagBuilder("td");
                td.InnerHtml.Append(c.Type);
                tr.InnerHtml.AppendHtml(td);

                td = new TagBuilder("td");
                td.InnerHtml.Append((c.Type.StartsWith("http")) ? (c.Type.Split('/').LastOrDefault()) : (c.Type));
                tr.InnerHtml.AppendHtml(td);

                td = new TagBuilder("td");
                td.InnerHtml.Append(c.Value);
                tr.InnerHtml.AppendHtml(td);

                tbody.InnerHtml.AppendHtml(tr);
            }

            table.AddCssClass("table table-sm table-hover table-bordered");
            table.InnerHtml.AppendHtml(thead).AppendHtml(tbody);
        }

    }
}



namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TagHelpers.Models;


    public class WebsiteInformationTagHelper : TagHelper
    {
        public WebsiteContext Info { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "section";
            output.Content.SetHtmlContent(
               $@"<ul><li><strong>Version:</strong> {Info.Version}</li>
            <li><strong>Copyright Year:</strong> {Info.CopyrightYear}</li>
            <li><strong>Approved:</strong> {Info.Approved}</li>
            <li><strong>Number of tags to show:</strong> {Info.TagsToShow}</li></ul>");

            // You can use the following markup with a closing tag and remove the line with TagMode.StartTagAndEndTag in the tag helper:
            // <website-information info="webContext" ></website-information>
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}







```



``` html

<style>
    xmp {
        margin: 0px;
    }
</style>

@using TagHelpers.Models
@{
    ViewData["Title"] = "About";
    WebsiteContext webContext = new WebsiteContext
    {
        Version = new Version(1, 3),
        CopyrightYear = 1638,
        Approved = true,
        TagsToShow = 131
    };
}


<h1>Tag Helpers</h1>

<div class="row">

    <div class="col my-5">

        <table class="table table-striped">

            <tr>
                <th>Tag Helper</th>
                <th>Razor Markup</th>
                <th>Converted Markup</th>
                <th>Razor Result</th>
            </tr>

            <tr>
                <td>Email Tag Helper</td>
                <td><xmp>@Html.Raw(@"<email mail-to=""support""></email>")</xmp></td>
                <td><xmp><email mail-to="support"></email></xmp></td>
                <td><email mail-to="support"></email></td>
            </tr>

            <tr>
                <td>Email Async Tag Helper</td>
                <td><xmp>@Html.Raw(@"<email-async>support</email-async>")</xmp></td>
                <td><xmp><email-async>support</email-async></xmp></td>
                <td><email-async>support</email-async></td>
            </tr>

            <tr>
                <td>Bold Tag Helper</td>
                <td><xmp>@Html.Raw(@"<p bold>Bold content.</p>")</xmp></td>
                <td><xmp><p bold>Bold content.</p></xmp></td>
                <td><p bold>Bold content.</p></td>
            </tr>

            <tr>
                <td>Tag Helper with Model</td>
                <td><xmp>@Html.Raw(@"<website-information info=""webContext"" />")</xmp></td>
                <td><xmp><website-information info="webContext" /></xmp></td>
                <td><website-information info="webContext" /></td>
            </tr>

            <tr>
                <td>Button Tag Helper</td>
                <td><xmp>@Html.Raw(@"<BsSubmit>OK</BsSubmit>")</xmp></td>
                <td><xmp><BsSubmit>OK</BsSubmit></xmp></td>
                <td><BsSubmit>OK</BsSubmit></td>
            </tr>

        </table>

        <div condition="true">
            <p>
                This div is visible since condition is true...
            </p>
        </div>

        <UserInfo />

        <cache expires-sliding="@TimeSpan.FromSeconds(15)">
            <p>Timestamp from cache tag helper @DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")</p>
        </cache>

    </div>

</div>


@*<ul class="nav nav-tabs" id="myTab" role="tablist">
        <li class="nav-item" role="presentation">
            <a class="nav-link active" id="home-tab" data-bs-toggle="tab" href="#tabid-1" role="tab" aria-controls="tab1" aria-selected="true">Tab-1</a>
        </li>
        <li class="nav-item" role="presentation">
            <a class="nav-link" id="profile-tab" data-bs-toggle="tab" href="#tabid-2" role="tab" aria-controls="tab2" aria-selected="false">Tab-2</a>
        </li>
        <li class="nav-item" role="presentation">
            <a class="nav-link" id="contact-tab" data-bs-toggle="tab" href="#tabid-3" role="tab" aria-controls="tab3" aria-selected="false">Tab-3</a>
        </li>
    </ul>

    <div class="tab-content my-2" id="myTabContent">
        <div class="tab-pane fade show active" id="tabid-1" role="tabpanel" aria-labelledby="tab-1">1</div>
        <div class="tab-pane fade" id="tabid-2" role="tabpanel" aria-labelledby="tab-2">2</div>
        <div class="tab-pane fade" id="tabid-3" role="tabpanel" aria-labelledby="tab-3">3</div>
    </div>*@


```