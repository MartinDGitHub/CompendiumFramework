using CF.Web.AspNetCore.Models.Shared;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers
{
    [HtmlTargetElement("span", Attributes = AspValidationForEditorAttributeName)]
    public class ValidationMessageForEditorTagHelper : ValidationMessageTagHelper, IForEditorTagHelper
    {
        private const string AspValidationForEditorAttributeName = "asp-validation-for-editor";

        [HtmlAttributeName(AspValidationForEditorAttributeName)]
        public EditorTagHelperViewModel EditorTagHelperViewModel { get; set; }

        public ValidationMessageForEditorTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await ForEditorUtility.ProcessAsync(this, () => base.ProcessAsync(context, output));
        }
    }
}
