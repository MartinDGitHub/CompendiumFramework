using CF.Common.Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.Controllers
{
    public abstract class WebControllerBase : Controller
    {
        protected readonly IScopedMessageRecorder _scopedMessageRecorder;
        protected readonly IScopedCookieMessageRecorder _scopedCookieMessageRecorder;

        protected bool IsErrorState => !this.ModelState.IsValid || this._scopedMessageRecorder.HasErrors;

        protected bool HasMessages => this.IsErrorState || this._scopedMessageRecorder.Messages.Any();

        protected WebControllerBase(IScopedMessageRecorder scopedMessageRecorder, IScopedCookieMessageRecorder scopedCookieMessageRecorder)
        {
            this._scopedMessageRecorder = scopedMessageRecorder ?? throw new ArgumentNullException(nameof(scopedMessageRecorder));
            this._scopedCookieMessageRecorder = scopedCookieMessageRecorder ?? throw new ArgumentNullException(nameof(scopedCookieMessageRecorder));
        }
        
        protected async Task<IActionResult> PostRedirectGetAsync<TModel>(TModel model, Func<Task> postOperationAsync, string redirectUrl)
        {
            return await this.PostRedirectGetAsync(model, postOperationAsync, redirectUrl, null);

        }

        protected async Task<IActionResult> PostRedirectGetAsync<TModel>(TModel model, Func<Task> postOperationAsync, string redirectUrl, string viewName)
        {
            if (this.ModelState.IsValid)
            {
                // Model state must only be preserved when it is invalid, so that the page 
                // can be re-rendered with related errors. Otherwise, it should be cleared to 
                // ensure that it does not interfere with re-rendering the page without redirecting.
                this.ModelState.Clear();

                // Perform the operation.
                await postOperationAsync();

                // If the operation did not incur any errors, perform the Get part of the PRG pattern.
                // Messages unrelated to an error that prevented success of the operation will be relayed
                // via cookies.
                if (!this.IsErrorState)
                {
                    if (this._scopedMessageRecorder.Messages.Any())
                    {
                        foreach (var message in this._scopedMessageRecorder.Messages)
                        {
                            this._scopedCookieMessageRecorder.Record(message);
                        }

                        this._scopedCookieMessageRecorder.TargetUrl = redirectUrl;
                    }

                    return Redirect(redirectUrl);
                }
            }

            // If a redirect did not occur, re-render the model on the view with errors, etc.
            if (string.IsNullOrWhiteSpace(viewName))
            {
                return View(model);
            }
            else
            {
                return View(viewName, model);
            }
        }
    }
}
