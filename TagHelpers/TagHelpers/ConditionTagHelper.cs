namespace TagHelpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;

    /// <summary>
    /// Applies to all html elements having a condition attribute.
    /// If the specified condition is not true, then the element is not rendered at all.
    /// Use: <p Condition="false" >This is a paragraph but not rendered since condition is false</p>
    /// Use: <header Condition="User.Identity.IsAuthenticated" class="navbar-margin">
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
