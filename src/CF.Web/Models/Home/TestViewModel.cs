using System;

namespace CF.Web.Models.Home
{
    public class TestViewModel
    {
        public bool Flag { get; set; } = true;

        public string Text { get; set; } = "Foo!";

        public DateTimeOffset Today { get; set; } = DateTimeOffset.Now;
    }
}
