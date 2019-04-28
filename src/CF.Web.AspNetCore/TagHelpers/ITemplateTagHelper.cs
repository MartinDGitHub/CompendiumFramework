using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CF.Web.AspNetCore.TagHelpers
{
    public interface ITemplateTagHelper
    {
        ModelExpression For { get; set; }

        IModelExpressionWrapper ModelExpressionWrapper { get; set; }
    }
}