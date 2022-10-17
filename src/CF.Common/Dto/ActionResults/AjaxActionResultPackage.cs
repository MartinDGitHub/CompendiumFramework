using CF.Common.Dto.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Common.Dto.ActionResults
{
    public class AjaxActionResultPackage<TResult> : AjaxActionResultPackage
    {
        /// <summary>
        /// Gets and sets the result of an API operation.
        /// </summary>
        public TResult Result { get; set; }
    }

    public class AjaxActionResultPackage : ActionResultPackage
    {
        /// <summary>
        /// Gets the URI to redirect to on various HTTP error response statuses.
        ///
        /// NOTE: this will only be used when the initiator of the AJAX request is aware of this value so as
        /// to redirect the browser to the URL. The default behavior is for the AJAX operation to implicitly 
        /// follow the redirect and return the redirected response content to the caller. In other words,
        /// unless we have custom client-side code written to redirect the browser to this URL, no browser 
        /// redirect will occur.
        /// </summary>
        public Uri RedirectUri { get; set; }

        /// <summary>
        /// Gets and sets model state error messages.
        /// </summary>
        public IEnumerable<string> ModelStateErrorMessages { get; set; }
    }
}
