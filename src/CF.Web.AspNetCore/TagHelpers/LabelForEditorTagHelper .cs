using CF.Web.AspNetCore.Models.Shared;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers
{
    [HtmlTargetElement("label", Attributes = AspForEditorAttributeName)]
    public class LabelForEditorTagHelper : LabelTagHelper, IForEditorTagHelper
    {
        private const string AspForEditorAttributeName = "asp-for-editor";

        [HtmlAttributeName(AspForEditorAttributeName)]
        public EditorTagHelperViewModel EditorTagHelperViewModel { get; set; }

        public LabelForEditorTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await ForEditorUtility.ProcessAsync(this, () => base.ProcessAsync(context, output));
        }
    }
}
