using System;
using System.ComponentModel.DataAnnotations;

namespace CF.Web.Models.Home
{
    public class TestViewModel
    {
        public bool Flag { get; set; } = true;

        [Required]
        [MaxLength(10)]
        public string Text { get; set; } = "Foo!";

        [DataType(DataType.Date)]
        public DateTimeOffset Today { get; set; } = DateTimeOffset.Now;

        [DataType(DataType.PhoneNumber)]
        public string HomePhone { get; set; } = "123-456-7890";

        [DataType(DataType.PhoneNumber)]
        public string BusinessPhone { get; set; } = "456-789-0123";

        [DataType(DataType.DateTime)]
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

    }
}
