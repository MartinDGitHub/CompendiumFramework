using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System;

namespace CF.Web.AspNetCore.Helpers
{
    /// <summary>
    /// A helper for partial view operations.
    /// </summary>
    internal class PartialViewHelper : IPartialViewHelper
    {
        private readonly IHtmlHelper _htmlHelper;
        private readonly ICompositeViewEngine _viewEngine;

        public PartialViewHelper(ICompositeViewEngine viewEngine, IHtmlHelper htmlHelper)
        {
            this._viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
            this._htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));
        }

        /// <summary>
        /// Gets a partial view as its HTML string representation.
        /// </summary><remarks>
        /// This is typically used for rendering a partial view into its serialized HTML string format for return
        /// on an AJAX call where it can in turn be inserted into the DOM.
        /// </remarks>
        public async Task<string> GetPartialViewHtmlStringAsync(string viewPath, object model, ActionContext actionContext, ViewDataDictionary viewData, ITempDataDictionary tempData)
        {
            string htmlString;
            using (var stringWriter = new StringWriter())
            {
                var viewEngineResult = this._viewEngine.GetView(null, viewPath, false);
                if (!viewEngineResult.Success)
                {
                    throw new InvalidOperationException($"The no view was resolved for view path [{viewPath}].");
                }
                var viewContext = new ViewContext(actionContext, viewEngineResult.View, viewData, tempData, TextWriter.Null, new HtmlHelperOptions());
                (this._htmlHelper as IViewContextAware).Contextualize(viewContext);
                var htmlContext = await this._htmlHelper.PartialAsync(viewPath, model);

                htmlContext.WriteTo(stringWriter, HtmlEncoder.Default);

                htmlString = stringWriter.ToString();
            }

            return htmlString;
        }
    }
}
