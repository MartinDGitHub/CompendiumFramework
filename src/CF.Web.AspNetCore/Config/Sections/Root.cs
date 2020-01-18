namespace CF.Web.AspNetCore.Config.Sections
{
    internal class Root
    {
        public Cors Cors { get; set; }

        public Environment Environment { get; set; }

        public Domain Domain { get; set;}
    }
}
