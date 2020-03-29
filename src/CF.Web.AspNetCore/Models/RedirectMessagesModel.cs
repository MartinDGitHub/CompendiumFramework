using CF.Common.Dto.Messaging;
using System;
using System.Collections.Generic;

namespace CF.Web.AspNetCore.Models
{
    public class RedirectMessagesModel
    {
        /// <summary>
        /// Gets the redirect target URL path and query that the messages are intended for.
        /// </summary>
        public string TargetPathAndQuery { get; set; }

        /// <summary>
        /// Gets the referrer path and query that the messages are intended to come from.
        /// </summary>
        public string ReferrerPathAndQuery { get; set; }

        /// <summary>
        /// Gets when the messages expires.
        /// </summary>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public IEnumerable<Message> Messages { get; set; }
    }
}
