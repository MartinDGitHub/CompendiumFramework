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

        protected async Task<IActionResult> TryPostRedirectGetAsync(Func<Task> postOperationAsync, Func<Task<IActionResult>> redirectAsync, Func<Task<IActionResult>> showMessagesAsync)
        {
            return await this.TryPostRedirectGetAsync<object>(
                async () => { await postOperationAsync(); return Task.FromResult(default(object)); },
                async _ => await redirectAsync(),
                async _ => await showMessagesAsync());
        }

        protected async Task<IActionResult> TryPostRedirectGetAsync(Func<Task> postOperationAsync, Func<IActionResult> redirect, Func<Task<IActionResult>> showMessagesAsync)
        {
            return await this.TryPostRedirectGetAsync<object>(
                async () => { await postOperationAsync(); return Task.FromResult(default(object)); },
                _ => Task.FromResult(redirect()),
                async _ => await showMessagesAsync());
        }

        protected async Task<IActionResult> TryPostRedirectGetAsync(Func<Task> postOperationAsync, Func<Task<IActionResult>> redirectAsync, Func<IActionResult> showMessages)
        {
            return await this.TryPostRedirectGetAsync<object>(
                async () => { await postOperationAsync(); return Task.FromResult(default(object)); },
                async _ => await redirectAsync(),
                _ => Task.FromResult(showMessages()));
        }

        protected async Task<IActionResult> TryPostRedirectGetAsync(Func<Task> postOperationAsync, Func<IActionResult> redirect, Func<IActionResult> showMessages)
        {
            return await this.TryPostRedirectGetAsync<object>(
                async () => { await postOperationAsync(); return Task.FromResult(default(object)); },
                _ => Task.FromResult(redirect()),
                _ => Task.FromResult(showMessages()));
        }

        protected async Task<IActionResult> TryPostRedirectGetAsync<TResult>(Func<Task<TResult>> postOperationAsync, Func<TResult, ActionResult> redirect, Func<TResult, Task<IActionResult>> showMessages)
        {
            return await this.TryPostRedirectGetAsync<TResult>(
                postOperationAsync,
                async result => await Task.FromResult(redirect(result)),
                async result => await showMessages(result));
        }

        protected async Task<IActionResult> TryPostRedirectGetAsync<TResult>(Func<Task<TResult>> postOperationAsync, Func<TResult, Task<IActionResult>> redirectAsync, Func<TResult, IActionResult> showMessages)
        {
            return await this.TryPostRedirectGetAsync<TResult>(
                postOperationAsync,
                async result => await redirectAsync(result),
                result => Task.FromResult(showMessages(result)));
        }

        protected async Task<IActionResult> TryPostRedirectGetAsync<TResult>(Func<Task<TResult>> postOperationAsync, Func<TResult, IActionResult> redirect, Func<TResult, IActionResult> showMessages)
        {
            return await this.TryPostRedirectGetAsync<TResult>(
                postOperationAsync,
                result => Task.FromResult(redirect(result)),
                result => Task.FromResult(showMessages(result)));
        }

        /// <summary>
        /// Applies the post-redirect-get (PRG) pattern where if after the post operation, there are no messages to show, the redirect
        /// action is performed; however, if there are messages to show, the handle messages function is run.
        /// </summary><remarks>
        /// When a post operation succeeds without messages, it is best to redirect to a GET action so that: 1) the user can refresh the browser
        /// without re-posting the form, or incurring the corresponding warning message; 2) a POST URL that is different than the GET URL will
        /// not be bookmarked; 3) the code to rebuild the model after an operation that changes persisted state will not need to be performed
        /// as part of the POST action; 4) there is no risk of mutating a model in unexpected ways over a series of POST operations.
        /// 
        /// Unfortunately, when there are messages - error, or other - to show the user, redirecting will lose those messages. Therefore, 
        /// the POST operation will need to relay the posted model along with the messages. There are no reliable ways to work around this that
        /// do not require server-affinity by using server-side session state, or client-side redirect hacks (i.e. using local storage to store messages).
        /// </remarks>
        protected async Task<IActionResult> TryPostRedirectGetAsync<TResult>(Func<Task<TResult>> postOperationAsync, Func<TResult, Task<IActionResult>> redirectAsync, Func<TResult, Task<IActionResult>> showMessagesAsync)
        {
            var result = await postOperationAsync();

            if (this.IsErrorState)
            {
                return await showMessagesAsync(result);
            }
            else
            {
                return await redirectAsync(result);
            }
        }

        protected async Task<IActionResult> PostRedirectGetAsync(Func<Task> postOperationAsync, string redirectUrl, Func<IActionResult> onErrorHandlerAsync)
        {
            return await this.PostRedirectGetAsync(postOperationAsync, redirectUrl, () => Task.FromResult(onErrorHandlerAsync()));

        }

        protected async Task<IActionResult> PostRedirectGetAsync(Func<Task> postOperationAsync, string redirectUrl, Func<Task<IActionResult>> onErrorHandlerAsync)
        {
            await postOperationAsync();

            if (this.IsErrorState)
            {
                return await onErrorHandlerAsync();
            }
            else
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
    }
}
