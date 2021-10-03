namespace TagHelpers.TagHelpers
{
    /// https://andrewlock.net/creating-an-if-tag-helper-to-conditionally-render-content/
 
    using Microsoft.AspNetCore.Razor.TagHelpers;

    /// <summary>
    /// This tag helper renders its CONTENT, only if the 'is' value evaluates as true.
    /// Use: <if is="true"><p>True</p></if> 
    /// Use: <if is="1==1"><p>True</p></if>
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
