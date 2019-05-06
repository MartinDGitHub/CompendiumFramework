using CF.Common.Dto.Messaging;
using System;
using System.Collections.Generic;

namespace CF.Common.Messaging
{
    public interface IScopedCookieMessageRecorder : IScopedMessageRecorder
    {
        string TargetUrl { get; set; }

        string Key { get; set; }

        TimeSpan ExpiresAfter { get; set; }
    }
}
