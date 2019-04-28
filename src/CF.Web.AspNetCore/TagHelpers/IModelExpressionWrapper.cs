using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CF.Web.AspNetCore.TagHelpers
{
    public interface IModelExpressionWrapper
    {
        ModelExpression ModelExpression { get; set; }
    }
}
