using CF.Web.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CF.Web.Models.Shared
{
    public class FormGroupEditorTemplateModel : IModelExpressionWrapper
    {
        public bool IsReadOnly { get; set; }

        public bool IsDisabled { get; set; }

        public string ColClass { get; set; }

        public string LabelColClass { get; set; }

        public string ControlColClass { get; set; }

        public ModelExpression ModelExpression { get; set; }
    }
}
