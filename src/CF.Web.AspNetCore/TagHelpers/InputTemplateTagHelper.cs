using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace CF.Web.AspNetCore.TagHelpers
{
    [HtmlTargetElement("input", Attributes = AspForExprAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class InputTemplateTagHelper : InputTagHelper, ITemplateTagHelper
    {
        private const string AspForExprAttributeName = "asp-for-expr";

        [HtmlAttributeName(AspForExprAttributeName)]
        public IModelExpressionWrapper ModelExpressionWrapper { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsDisabled { get; set; }

        public InputTemplateTagHelper(IHtmlGenerator generator) : base(generator)
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

            if (this.IsReadOnly)
            {
                output.Attributes.Add(new TagHelperAttribute("readonly"));
            }

            if (this.IsDisabled)
            {
                output.Attributes.Add(new TagHelperAttribute("disabled"));
            }

            TemplateTagHelperUtility.Process(this, () => base.Process(context, output));
        }
    }
}
