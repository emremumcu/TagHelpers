``` csharp

[HtmlTargetElement("breadcrumb-item", ParentTag = "breadcrumb")]
[OutputElementHint("li")]
[OutputElementHint("ol")]
[RestrictChildren("breadcrumb-item")]
[HtmlTargetElement("divider", ParentTag = "button-dropdown", TagStructure = TagStructure.WithoutEndTag)]
[OutputElementHint("button")]
[RestrictChildren("a", "text", "header", "divider")]
[HtmlTargetElement("button-dropdown")]
[GenerateId("dropdown-", false)]
public class BreadcrumbItemTagHelper : BreconsTagHelperBase

[HtmlAttributeName("my-href")]
public string Href { get; set; }


        public override void Init(TagHelperContext context)
        {
            base.Init(context);
        }

public override void Process(TagHelperContext context, TagHelperOutput output)
{
    output.TagName = "li";
    output.AddCssClass("breadcrumb-item");
    output.TagMode = TagMode.StartTagAndEndTag;
    output.PreContent.SetHtmlContent($"<a href=\"{this.Href}\">");
    output.PostContent.SetHtmlContent("</a>");

output.PreContent.AppendHtml($"<h4 class=\"card-title\">{this.Title}</h4>");
output.AddDataAttribute("ride", "carousel");

}

public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
{
var items = await output.GetChildContentAsync();

    output.TagName = "button";

    output.Attributes.Add("type", "submit");

    output.AddCssClass("btn");

    output.AddCssClass("btn-primary");

            

    var content = await output.GetChildContentAsync();

    //var target = content.GetContent() + "@mumcu.net";

    //output.Attributes.SetAttribute("href", "mailto:" + target);

    //output.Content.SetContent(target);
            output.PostElement.AppendHtml($"<div class=\"{(this.RightAlignment ? "dropdown-menu dropdown-menu-right" : "dropdown-menu")}\" aria-labelledby=\"{this.Id}\">");
            output.PostElement.AppendHtml(items);
            output.PostElement.AppendHtml("</div>");

TagBuilder button = new TagBuilder("button");
button.InnerHtml.AppendHtml($"<span class=\"sr-only\">{this.Title}</span>");

output.PreContent.AppendHtml("<div class=\"carousel-caption d-none d-md-block\">");
output.PostContent.PrependHtml("</div>");
output.Content.SetHtmlContent(await output.GetChildContentAsync());
}


```