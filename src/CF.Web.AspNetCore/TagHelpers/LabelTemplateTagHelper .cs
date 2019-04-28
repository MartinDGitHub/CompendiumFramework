using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers
{
    [HtmlTargetElement("label", Attributes = AspForExprAttributeName)]
    public class LabelTemplateTagHelper : LabelTagHelper, ITemplateTagHelper
    {
        private const string AspForExprAttributeName = "asp-for-expr";

        [HtmlAttributeName(AspForExprAttributeName)]
        public IModelExpressionWrapper ModelExpressionWrapper { get; set; }

        public LabelTemplateTagHelper(IHtmlGenerator generator) : base(generator)
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

            await TemplateTagHelperUtility.ProcessAsync(this, () => base.ProcessAsync(context, output));
        }
    }
}
