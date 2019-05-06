using CF.Common.Dto.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Web.AspNetCore.Models
{
    public class CookieMessagesModel
    {
        /// <summary>
        /// Gets the URL that the messages are for.
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// Gets when the cookie expires in UTC.
        /// </summary>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// Gets the messages.
        /// </summary>

        public IEnumerable<Message> Messages { get; set; }
    }
}
