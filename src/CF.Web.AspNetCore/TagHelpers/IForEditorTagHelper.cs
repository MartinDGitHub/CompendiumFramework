using CF.Web.AspNetCore.Models.Shared;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CF.Web.AspNetCore.TagHelpers
{
    public interface IForEditorTagHelper
    {
        ModelExpression For { get; set; }

        EditorTagHelperViewModel EditorTagHelperViewModel { get; set; }
    }
}