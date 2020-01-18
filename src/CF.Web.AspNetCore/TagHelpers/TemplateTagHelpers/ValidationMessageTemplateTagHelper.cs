using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers.TemplateTagHelpers
{
    [HtmlTargetElement("span", Attributes = AspValidationForExprAttributeName)]
    public class ValidationMessageTemplateTagHelper : ValidationMessageTagHelper, ITemplateTagHelper
    {
        private const string AspValidationForExprAttributeName = "asp-validation-for-expr";

        [HtmlAttributeName(AspValidationForExprAttributeName)]
        public IModelExpressionWrapper ModelExpressionWrapper { get; set; }

        public ValidationMessageTemplateTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            await TemplateTagHelperUtility.ProcessAsync(this, () => base.ProcessAsync(context, output)).ConfigureAwait(false);
        }
    }
}
