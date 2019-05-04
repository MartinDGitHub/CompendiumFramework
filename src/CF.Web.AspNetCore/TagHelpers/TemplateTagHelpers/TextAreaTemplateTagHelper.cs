using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace CF.Web.AspNetCore.TagHelpers.TemplateTagHelpers
{
    [HtmlTargetElement("textarea", Attributes = AspForExprAttributeName)]
    public class TextAreaTemplateTagHelper : TextAreaTagHelper, ITemplateTagHelper
    {
        private const string AspForExprAttributeName = "asp-for-expr";

        [HtmlAttributeName(AspForExprAttributeName)]
        public IModelExpressionWrapper ModelExpressionWrapper { get; set; }

        public TextAreaTemplateTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            TemplateTagHelperUtility.Process(this, () => base.Process(context, output));
        }
    }
}
