using CF.Web.Models.Shared;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers
{
    [HtmlTargetElement("form-group-editor", Attributes = AspForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class FormGroupEditorTemplateTagHelper : TemplateTagHelper
    {
        [HtmlAttributeName("readonly")]
        public bool IsReadOnly { get; set; }

        [HtmlAttributeName("disabled")]
        public bool IsDisabled { get; set; }

        public string ColClass { get; set; } = "col-4";

        public string LabelColClass { get; set; } = "col-4";

        public string ControlColClass { get; set; } = "col-8";

        public override string Name { get; set; } = "FormGroupEditor";

        public FormGroupEditorTemplateTagHelper(ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope) : base (viewEngine, viewBufferScope)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            this.TemplateModel = new FormGroupEditorTemplateModel
            {
                IsReadOnly = this.IsReadOnly,
                IsDisabled = this.IsDisabled,
                ColClass = this.ColClass,
                LabelColClass = this.LabelColClass,
                ControlColClass = this.ControlColClass,
            };

            await base.ProcessAsync(context, output);
        }
    }
}
