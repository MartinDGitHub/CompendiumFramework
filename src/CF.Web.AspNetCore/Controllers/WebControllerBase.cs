using CF.Common.Dto.ActionResults;
using CF.Common.Dto.Messaging;
using CF.Common.Messaging;
using CF.Web.AspNetCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.Controllers
{
    // Set up the default routing.
    [Route("[controller]/[action]/{id?}")]
    public abstract class WebControllerBase : Controller
    {
        private readonly IPartialViewHelper _partialViewHelper;

        protected IScopedMessageRecorder ScopedMessageRecorder { get; }

        protected IScopedRedirectMessageRecorder ScopedRedirectMessageRecorder { get; }

        protected bool IsErrorState => !this.ModelState.IsValid || this.ScopedMessageRecorder.HasErrors;

        protected bool HasMessages => this.IsErrorState || this.ScopedMessageRecorder.Messages.Any();

        protected WebControllerBase(IScopedMessageRecorder scopedMessageRecorder, IScopedRedirectMessageRecorder scopedRedirectMessageRecorder, IPartialViewHelper partialViewHelper)
        {
            this.ScopedMessageRecorder = scopedMessageRecorder ?? throw new ArgumentNullException(nameof(scopedMessageRecorder));
            this.ScopedRedirectMessageRecorder = scopedRedirectMessageRecorder ?? throw new ArgumentNullException(nameof(scopedRedirectMessageRecorder));
            this._partialViewHelper = partialViewHelper ?? throw new ArgumentNullException(nameof(partialViewHelper));
        }
        /// <summary>
        /// Returns an action result package without a result, with messages recorded on it.
        /// </summary>
        [NonAction]
        protected AjaxActionResultPackage AjaxActionResultPackage()
        {
            return this.ConfigureAjaxActionResultPackage(new AjaxActionResultPackage());
        }

        /// <summary>
        /// Returns an action result package with messages recorded on it and the specified view will be rendered as an HTML string and returned 
        /// as the result on the package.
        /// </summary>
        [NonAction]
        protected async Task<AjaxActionResultPackage> AjaxActionResultPackageAsync(string viewPath, object viewModel)
        {
            var viewString = await this.GetPartialViewHtmlStringAsync(viewPath, viewModel);

            return this.AjaxActionResultPackage(viewString);
        }

        /// <summary>
        /// Returns an action result package with a result and messages recorded on it.
        /// </summary>
        [NonAction]
        protected AjaxActionResultPackage AjaxActionResultPackage<TResult>(TResult result)
        {
            var ajaxActionResultPackage = new AjaxActionResultPackage<TResult>()
            {
                Result = result,
            };

            return this.ConfigureAjaxActionResultPackage(ajaxActionResultPackage);
        }

        [NonAction]
        private AjaxActionResultPackage ConfigureAjaxActionResultPackage(AjaxActionResultPackage ajaxActionResultPackage)
        {
            ajaxActionResultPackage.Success = !this.IsErrorState;

            this.RecordScopedMessages(ajaxActionResultPackage);
            this.RecordInvalidModelState(ajaxActionResultPackage);

            return ajaxActionResultPackage;
        }

        [NonAction]
        protected async Task<IActionResult> AjaxOperationAsync<TResult>(Func<Task> operationAsync, Func<TResult> successResult, Func<Task<AjaxActionResultPackage>> errorOperationAsync = null)
        {
            return await this.ResolveAjaxOperationActionResultAsync(
                operationAsync,
                async () => await Task.FromResult(AjaxActionResultPackage(successResult())),
                async () => errorOperationAsync == null ? await Task.FromResult(AjaxActionResultPackage()) : await errorOperationAsync()
            );
        }

        [NonAction]
        protected async Task<IActionResult> AjaxOperationAsync<TResult>(Func<Task> operationAsync, Func<Task<TResult>> successResultAsync, Func<Task<AjaxActionResultPackage>> errorOperationAsync = null)
        {
            return await this.ResolveAjaxOperationActionResultAsync(
                operationAsync,
                async () => this.AjaxActionResultPackage(await successResultAsync()),
                async () => errorOperationAsync == null ? await Task.FromResult(AjaxActionResultPackage()) : await errorOperationAsync()
            );
        }

        [NonAction]
        protected async Task<IActionResult> AjaxOperationAsync(Func<Task> operationAsync, params string[] successMessages)
        {
            return await this.ResolveAjaxOperationActionResultAsync(
                operationAsync,
                async () =>
                {
                    if (successMessages != null)
                    {
                        foreach (var successMessage in successMessages)
                        {
                            if (!string.IsNullOrWhiteSpace(successMessage))
                            {
                                this.SetSuccessMessage(successMessage);
                            }
                        }
                    }

                    return await Task.FromResult(AjaxActionResultPackage());
                },
                async () => await Task.FromResult(AjaxActionResultPackage())
            );
        }

        [NonAction]
        protected async Task<IActionResult> AjaxOperationAsync(Func<Task> operationAsync, string successMessage, Func<Task> errorOperationAsync)
        {
            return await this.ResolveAjaxOperationActionResultAsync(
                operationAsync,
                async () =>
                {
                    if (!string.IsNullOrWhiteSpace(successMessage))
                    {
                        this.SetSuccessMessage(successMessage);
                    }

                    return await Task.FromResult(AjaxActionResultPackage());
                },
                async () =>
                {
                    if (errorOperationAsync != null)
                    {
                        await errorOperationAsync.Invoke();
                    }

                    return this.AjaxActionResultPackage();
                }
            );
        }

        [NonAction]
        protected async Task<IActionResult> AjaxOperationAsync(Func<Task> operationAsync, string successMessage, Func<Task<AjaxActionResultPackage>> errorOperationAsync)
        {
            return await this.ResolveAjaxOperationActionResultAsync(
                operationAsync,
                async () =>
                {
                    if (!string.IsNullOrWhiteSpace(successMessage))
                    {
                        this.SetSuccessMessage(successMessage);
                    }

                    return await Task.FromResult(AjaxActionResultPackage());
                },
                errorOperationAsync
            );
        }

        [NonAction]
        protected async Task<IActionResult> AjaxOperationAsync(Func<Task> operationAsync, Func<Task> successOperationAsync = null, Func<Task> errorOperationAsync = null)
        {
            return await this.ResolveAjaxOperationActionResultAsync(
                operationAsync,
                async () => {
                    // Perform the operation first so that any messages that are recorded are included on the package.
                    if (successOperationAsync != null)
                    {
                        await successOperationAsync.Invoke();
                    }

                    return this.AjaxActionResultPackage();
                },
                async () =>
                {
                    // Perform the operation first so that any messages that are recorded are included on the package.
                    if (errorOperationAsync != null)
                    {
                        await errorOperationAsync.Invoke();
                    }

                    return this.AjaxActionResultPackage();
                }
            );
        }

        [NonAction]
        protected async Task<IActionResult> RedirectAjaxOperationAsync(Func<Task> postOperationAsync, Func<RedirectToActionResult> redirectOperation, Func<Task<AjaxActionResultPackage>> errorOperationAsync = null)
        {
            return await this.ResolveRedirectAjaxOperationAsync(
                postOperationAsync,
                async () => await Task.FromResult(BuildAjaxActionResultPackage()),
                errorOperationAsync
            );

            AjaxActionResultPackage BuildAjaxActionResultPackage()
            {
                var result = redirectOperation();

                return new AjaxActionResultPackage { RedirectUri = new Uri(result.UrlHelper.Action(result.ActionName, result.ControllerName, result.RouteValues, protocol: null, host: null, fragment: result.Fragment), UriKind.RelativeOrAbsolute) };
            };
        }

        [NonAction]
        protected async Task<IActionResult> RedirectAjaxOperationAsync(Func<Task> postOperationAsync, Func<Task<RedirectToActionResult>> redirectOperationAsync, Func<Task<AjaxActionResultPackage>> errorOperationAsync = null)
        {
            return await this.ResolveRedirectAjaxOperationAsync(
                postOperationAsync,
                BuildAjaxActionResultPackageAsync,
                errorOperationAsync
            );

            async Task<AjaxActionResultPackage> BuildAjaxActionResultPackageAsync()
            {
                var result = await redirectOperationAsync();

                return new AjaxActionResultPackage { RedirectUri = new Uri(result.UrlHelper.Action(result.ActionName, result.ControllerName, result.RouteValues, protocol: null, host: null, fragment: result.Fragment), UriKind.RelativeOrAbsolute) };
            };
        }

        [NonAction]
        protected async Task<IActionResult> RedirectAjaxOperationAsync(Func<Task> postOperationAsync, Func<RedirectResult> redirectOperation, Func<Task<AjaxActionResultPackage>> errorOperationAsync = null)
        {
            return await this.ResolveRedirectAjaxOperationAsync(
                postOperationAsync,
                async () => await Task.FromResult(BuildAjaxActionResultPackage()),
                errorOperationAsync
            );

            AjaxActionResultPackage BuildAjaxActionResultPackage()
            {
                var result = redirectOperation();

                return new AjaxActionResultPackage { RedirectUri = new Uri(result.Url, UriKind.RelativeOrAbsolute) };
            };
        }

        [NonAction]
        protected async Task<IActionResult> RedirectAjaxOperationAsync(Func<Task> postOperationAsync, Func<Task<RedirectResult>> redirectOperationAsync, Func<Task<AjaxActionResultPackage>> errorOperationAsync = null)
        {
            return await this.ResolveRedirectAjaxOperationAsync(
                postOperationAsync,
                BuildAjaxActionResultPackageAsync,
                errorOperationAsync
            );

            async Task<AjaxActionResultPackage> BuildAjaxActionResultPackageAsync()
            {
                var result = await redirectOperationAsync();

                return new AjaxActionResultPackage { RedirectUri = new Uri(result.Url, UriKind.RelativeOrAbsolute) };
            };
        }

        [NonAction]
        private async Task<IActionResult> ResolveRedirectAjaxOperationAsync(Func<Task> postOperationAsync, Func<Task<AjaxActionResultPackage>> redirectOperationAsync, Func<Task<AjaxActionResultPackage>> errorOperationAsync)
        {
            return await this.ResolveAjaxOperationActionResultAsync(
                postOperationAsync,
                async () => {

                    // First perform the redirect operation to ensure any additional messages are logged before transferring them.
                    var redirectActionResultPackage = await redirectOperationAsync();

                    // Record all the messages from the post operation and the redirect operation.
                    this.TransferMessagesForRedirect();

                    return await Task.FromResult(this.ConfigureAjaxActionResultPackage(redirectActionResultPackage));
                },
                async () => errorOperationAsync == null ? await Task.FromResult(this.AjaxActionResultPackage()) : await errorOperationAsync()
            );
        }

        [NonAction]
        private async Task<IActionResult> ResolveAjaxOperationActionResultAsync(Func<Task> operationAsync, Func<Task<AjaxActionResultPackage>> successOperationAsync, Func<Task<AjaxActionResultPackage>> errorOperationAsync)
        {
            // AJAX calls always respond with JSON, so convert success and error operations to JSON results.
            return await this.PerformSuccessErrorOperationAsync(
                operationAsync,
                async () => new JsonResult(await successOperationAsync()),
                async () =>
                {
                    // Set the HTTP status code to 400 to indicate an validation error condition.
                    this.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    return new JsonResult(await errorOperationAsync());

                }
            );
        }

        [NonAction]
        protected async Task<IActionResult> PostRedirectGetAsync(Func<Task> postOperationAsync, Func<RedirectToActionResult> redirectOperation, Func<IActionResult> errorOperation)
        {
            return await this.ResolvePostRedirectGetActionResultAsync(
                postOperationAsync,
                async () => await Task.FromResult(redirectOperation()),
                async () => await Task.FromResult(errorOperation()));
        }

        [NonAction]
        protected async Task<IActionResult> PostRedirectGetAsync(Func<Task> postOperationAsync, Func<RedirectToActionResult> redirectOperation, Func<Task<IActionResult>> errorOperationAsync)
        {
            return await this.ResolvePostRedirectGetActionResultAsync(
                postOperationAsync,
                async () => await Task.FromResult(redirectOperation()),
                async () => await errorOperationAsync());
        }

        [NonAction]
        protected async Task<IActionResult> PostRedirectGetAsync(Func<Task> postOperationAsync, Func<Task<RedirectToActionResult>> redirectOperationAsync, Func<Task<IActionResult>> errorOperationAsync)
        {
            return await this.ResolvePostRedirectGetActionResultAsync(
                postOperationAsync,
                async () => await redirectOperationAsync(),
                errorOperationAsync);
        }

        [NonAction]
        protected async Task<IActionResult> PostRedirectGetAsync(Func<Task> postOperationAsync, Func<RedirectResult> redirectOperation, Func<IActionResult> errorOperation)
        {
            return await this.ResolvePostRedirectGetActionResultAsync(
                postOperationAsync,
                async () => await Task.FromResult(redirectOperation()),
                async () => await Task.FromResult(errorOperation()));
        }

        [NonAction]
        protected async Task<IActionResult> PostRedirectGetAsync(Func<Task> postOperationAsync, Func<RedirectResult> redirectOperation, Func<Task<IActionResult>> errorOperationAsync)
        {
            return await this.ResolvePostRedirectGetActionResultAsync(
                postOperationAsync,
                async () => await Task.FromResult(redirectOperation()),
                errorOperationAsync);
        }

        [NonAction]
        protected async Task<IActionResult> PostRedirectGetAsync(Func<Task> postOperationAsync, Func<Task<RedirectResult>> redirectOperationAsync, Func<Task<IActionResult>> errorOperationAsync)
        {
            return await this.ResolvePostRedirectGetActionResultAsync(
                postOperationAsync,
                async () => await redirectOperationAsync(),
                errorOperationAsync);
        }

        [NonAction]
        private async Task<IActionResult> ResolvePostRedirectGetActionResultAsync(Func<Task> postOperationAsync, Func<Task<IActionResult>> redirectOperationAsync, Func<Task<IActionResult>> errorOperationAsync)
        {
            return await this.PerformSuccessErrorOperationAsync(
                postOperationAsync,
                async () => {

                    var redirectOperationResult = await redirectOperationAsync();

                    // The redirect operation may have logged additional messages, so transfer messages after.
                    this.TransferMessagesForRedirect();

                    return redirectOperationResult;
                },
                errorOperationAsync);
        }

        [NonAction]
        private async Task<IActionResult> PerformSuccessErrorOperationAsync<TSuccess, TError>(Func<Task> operationAsync, Func<Task<TSuccess>> successOperationAsync, Func<Task<TError>> errorOperationAsync)
            where TSuccess : IActionResult
            where TError : IActionResult
        {
            if (this.ModelState.IsValid)
            {
                // If an unhandled exception occurs during the operation, it should not be caught here. Instead it should
                // propagate up to the global exception handler middleware so that it can be uniformly, and safely
                // handled.
                //
                // The operation should record any validation errors that are user-actionable on the scoped validation
                // message recorder. Doing so will ensure that an error state is detected below and that the on error
                // function is invoked.
                await operationAsync();

                // If the operation incurred user-facing validation errors, clear the model state to ensure
                // it does not impact re-rendering of a partial view returned by the error operation.
                if (this.IsErrorState)
                {
                    this.ModelState.Clear();
                }
                // If the operation did not incur any errors, perform the success operation.
                else
                {
                    return await successOperationAsync();
                }
            }
            else if (this.ModelState.ValidationState == ModelValidationState.Unvalidated)
            {
                throw new InvalidOperationException($"The model state was [{ModelValidationState.Unvalidated}]. Ensure that the model was successfully bound, including child collections. To debug, check the entries on the model state for items with a {nameof(ModelStateEntry.ValidationState)} of [{ModelValidationState.Unvalidated}].");
            }

            // Errors operation was prevented the operation from succeeding, so perform the error operation.
            return await errorOperationAsync();
        }

        [NonAction]
        protected IActionResult RedirectToActionWithMessages(string actionName, string controllerName, object routeValues = null)
        {
            this.PreserveRedirectMessages();
            this.TransferMessagesForRedirect();
            return RedirectToAction(actionName, controllerName, routeValues);
        }

        [NonAction]
        protected IActionResult RedirectWithMessages(string url)
        {
            this.PreserveRedirectMessages();
            this.TransferMessagesForRedirect();
            return Redirect(url);
        }

        [NonAction]
        protected void RecordInvalidModelState(AjaxActionResultPackage actionResultPackage)
        {
            if (!this.ModelState.IsValid)
            {
                var modelStateErrorMessages = new List<string>();
                foreach (var value in this.ModelState.Values.Where(x => x.ValidationState == ModelValidationState.Invalid))
                {
                    foreach (var error in value.Errors)
                    {
                        modelStateErrorMessages.Add(error.ErrorMessage);
                    }
                }
                actionResultPackage.ModelStateErrorMessages = actionResultPackage.ModelStateErrorMessages?.Concat(modelStateErrorMessages) ?? modelStateErrorMessages;
            }
        }

        [NonAction]
        protected void RecordScopedMessages(ActionResultPackage actionResultPackage)
        {
            if (actionResultPackage == null)
            {
                throw new ArgumentNullException(nameof(actionResultPackage));
            }

            if (actionResultPackage.Messages == null)
            {
                actionResultPackage.Messages = new Message[] { };
            }

            actionResultPackage.Messages = actionResultPackage.Messages
                .Concat(this.ScopedMessageRecorder.Messages.Select(x => new Message(x)));
        }

        [NonAction]
        protected void PreserveRedirectMessages()
        {
            if (this.ScopedRedirectMessageRecorder.InboundMessages?.Any() != null)
            {
                foreach (var message in this.ScopedRedirectMessageRecorder.InboundMessages)
                {
                    this.ScopedRedirectMessageRecorder.RecordOutbound(message);
                }
            }
        }

        [NonAction]
        protected void TransferMessagesForRedirect()
        {
            if (this.ScopedMessageRecorder.Messages.Any())
            {
                foreach (var message in this.ScopedMessageRecorder.Messages)
                {
                    this.ScopedRedirectMessageRecorder.RecordOutbound(message);
                }
            }
        }

        [NonAction]
        protected void SetSuccessMessage(string message, bool suppressOnErrorState = true)
        {
            if (!suppressOnErrorState || !this.IsErrorState)
            {
                this.ScopedMessageRecorder.Record(MessageSeverity.Success, message);
            }
        }

        [NonAction]
        protected void SetErrorMessage(string message)
        {
            this.ScopedMessageRecorder.Record(MessageSeverity.Error, message);
        }

        [NonAction]
        protected void SetWarningMessage(string message)
        {
            this.ScopedMessageRecorder.Record(MessageSeverity.Warning, message);
        }

        /// <summary>
        /// Gets the HTML string representation of a partial view.
        /// </summary><remarks>
        /// This is typically used to include partial view content in the response for an AJAX call 
        /// that returns other JSON-serialized data in addition to the partial view HTML.
        /// </remarks>
        [NonAction]
        protected async Task<string> GetPartialViewHtmlStringAsync(string viewPath, object model)
        {
            return await this._partialViewHelper.GetPartialViewHtmlStringAsync(viewPath, model, this.ControllerContext, this.ViewData, this.TempData);
        }

    }
}
