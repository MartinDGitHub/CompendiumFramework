using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.Helpers
{
    public interface IPartialViewHelper
    {
        Task<string> GetPartialViewHtmlStringAsync(string viewPath, object model, ActionContext actionContext, ViewDataDictionary viewData, ITempDataDictionary tempData);
    }
}