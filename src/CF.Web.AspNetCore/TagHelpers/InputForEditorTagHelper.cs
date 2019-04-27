using CF.Web.AspNetCore.Models.Shared;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace CF.Web.AspNetCore.TagHelpers
{
    [HtmlTargetElement("input", Attributes = AspForEditorAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class InputForEditorTagHelper : InputTagHelper, IForEditorTagHelper
    {
        private const string AspForEditorAttributeName = "asp-for-editor";

        [HtmlAttributeName(AspForEditorAttributeName)]
        public EditorTagHelperViewModel EditorTagHelperViewModel { get; set; }

        public InputForEditorTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ForEditorUtility.Process(this, () => base.Process(context, output));
        }
    }
}
