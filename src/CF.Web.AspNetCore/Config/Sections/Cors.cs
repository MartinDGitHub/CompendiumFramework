using System.Collections.Generic;

namespace CF.Web.AspNetCore.Config.Sections
{
    internal class Cors
    {
        public Dictionary<string, string[]> OriginsByPolicy { get; set; }
    }
}
