using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers.TemplateTagHelpers
{
    internal static class TemplateTagHelperUtility
    {
        public static void Process(ITemplateTagHelper templateTagHelper, Action process)
        {
            Init(templateTagHelper);
            process();
        }
        public static async Task ProcessAsync(ITemplateTagHelper forEditorTagHelper, Func<Task> processAsync)
        {
            Init(forEditorTagHelper);
            await processAsync();
        }

        private static void Init(ITemplateTagHelper templateTagHelper)
        {
            if (templateTagHelper.For != null)
            {
                throw new Exception($"The asp-for attribute must not be specified when using asp-for-expr.");
            }

            if (templateTagHelper.ModelExpressionWrapper != null)
            {
                templateTagHelper.For = templateTagHelper.ModelExpressionWrapper.ModelExpression;
            }
            else
            {
                throw new InvalidOperationException($"No model expression wrapper was set on the template tag helper.");
            }
        }
    }
}
