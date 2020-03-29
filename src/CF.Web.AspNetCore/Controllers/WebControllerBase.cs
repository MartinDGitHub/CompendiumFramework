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
            return await PostRedirectGetAsync(postOperationAsync,
                async () => await Task.FromResult(this.Redirect(redirectUrl.ToString())).ConfigureAwait(false),
                async () =>
                {
                    if (string.IsNullOrWhiteSpace(viewName))
                    {
                        return await Task.FromResult(View(model)).ConfigureAwait(false);
                    }
                    else
                    {
                        return await Task.FromResult(View(viewName, model)).ConfigureAwait(false);
                    }
                }
            ).ConfigureAwait(false);
        }

        protected async Task<IActionResult> PostRedirectGetAsync(Func<Task> postOperationAsync, Func<Task<IActionResult>> redirectOperationAsync, Func<Task<IActionResult>> errorOperationAsync)
        {
            if (postOperationAsync == null)
            {
                throw new ArgumentNullException(nameof(postOperationAsync));
            }

            if (this.ModelState.IsValid)
            {
                // Perform the operation which may succeed or fail with validation errors.
                //
                // Unexpected errors are expected to be thrown and hit the global exception
                // handler middleware. This call should only return and continue on success
                // or failure due to user-facing validation rule violations.
                await postOperationAsync().ConfigureAwait(false);

                // The operation failed with validation errors, so re-render the page
                if (this.IsErrorState)
                {
                    // If the operation incurred validation errors, clear the model state to ensure that
                    // it does not interfere with the re-render of the page with submitted form content
                    // and the validation errors returned by the operation.
                    this.ModelState.Clear();
                }
                else
                {
                    // If the operation did not incur any errors, perform the redirect-get part of the PRG pattern.
                    await redirectOperationAsync().ConfigureAwait(false);

                    // Here we transfer any recorded messages to the redirect message recorder so that they will
                    // be preserved across the redirect. This is done after performing the redirect operation
                    // so that any messages recorded during that operation will be preserved.
                    if (this.ScopedMessageRecorder.Messages.Any())
                    {
                        foreach (var message in this.ScopedMessageRecorder.Messages)
                        {
                            this.ScopedRedirectMessageRecorder.RecordOutbound(message);
                        }
                    }
                }
            }

            // If a redirect did not occur due to errors, perform the error operation which typically consists
            // of re-rendering the view from model state. This will re-render the invalid form content along
            // with model state errors and/or validation errors returned from the operation.
            return await errorOperationAsync().ConfigureAwait(false);
        }

    }
}
