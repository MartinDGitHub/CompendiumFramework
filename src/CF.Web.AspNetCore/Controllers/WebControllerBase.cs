using CF.Common.Messaging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.Controllers
{
    // Set up the default routing.
    [Route("[controller]/[action]/{id?}")]
    public abstract class WebControllerBase : Controller
    {
        protected IScopedMessageRecorder ScopedMessageRecorder { get; }

        protected IScopedRedirectMessageRecorder ScopedRedirectMessageRecorder { get; }

        protected bool IsErrorState => !this.ModelState.IsValid || this.ScopedMessageRecorder.HasErrors;

        protected bool HasMessages => this.IsErrorState || this.ScopedMessageRecorder.Messages.Any();

        protected WebControllerBase(IScopedMessageRecorder scopedMessageRecorder, IScopedRedirectMessageRecorder scopedRedirectMessageRecorder)
        {
            this.ScopedMessageRecorder = scopedMessageRecorder ?? throw new ArgumentNullException(nameof(scopedMessageRecorder));
            this.ScopedRedirectMessageRecorder = scopedRedirectMessageRecorder ?? throw new ArgumentNullException(nameof(scopedRedirectMessageRecorder));
        }
        
        protected async Task<IActionResult> PostRedirectGetAsync<TModel>(TModel model, Func<Task> postOperationAsync, Uri redirectUrl)
        {
            return await this.PostRedirectGetAsync(model, postOperationAsync, redirectUrl, null).ConfigureAwait(false);

        }

        protected async Task<IActionResult> PostRedirectGetAsync<TModel>(TModel model, Func<Task> postOperationAsync, Uri redirectUrl, string viewName)
        {
            if (postOperationAsync == null)
            {
                throw new ArgumentNullException(nameof(postOperationAsync));
            }

            if (this.ModelState.IsValid)
            {
                // Model state must only be preserved when it is invalid, so that the page 
                // can be re-rendered with related errors. Otherwise, it should be cleared to 
                // ensure that it does not interfere with re-rendering the page without redirecting.
                this.ModelState.Clear();

                // Perform the operation.
                await postOperationAsync().ConfigureAwait(false);

                // If the operation did not incur any errors, perform the Get part of the PRG pattern.
                // Messages unrelated to an error that prevented success of the operation will be relayed
                // via cookies.
                if (!this.IsErrorState)
                {
                    if (this.ScopedMessageRecorder.Messages.Any())
                    {
                        foreach (var message in this.ScopedMessageRecorder.Messages)
                        {
                            this.ScopedRedirectMessageRecorder.RecordOutbound(message);
                        }
                    }

                    return Redirect(redirectUrl.ToString());
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
