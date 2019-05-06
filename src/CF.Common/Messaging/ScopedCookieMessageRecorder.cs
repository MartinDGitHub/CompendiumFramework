using System;

namespace CF.Common.Messaging
{
    class ScopedCookieMessageRecorder : ScopedMessageRecorder, IScopedCookieMessageRecorder
    {
        public string TargetUrl { get; set; }

        public string Key { get; set; } = Guid.NewGuid().ToString();

        public TimeSpan ExpiresAfter { get; set; } = TimeSpan.FromMinutes(5);
    }
}
