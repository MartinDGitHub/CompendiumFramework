using CF.Web.AspNetCore.Models.Shared;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers
{
    internal static class ForEditorUtility
    {
        public static void Process(IForEditorTagHelper forEditorTagHelper, Action process)
        {
            Init(forEditorTagHelper);
            process();
        }
        public static async Task ProcessAsync(IForEditorTagHelper forEditorTagHelper, Func<Task> processAsync)
        {
            Init(forEditorTagHelper);
            await processAsync();
        }

        private static void Init(IForEditorTagHelper forEditorTagHelper)
        {
            if (forEditorTagHelper.For != null)
            {
                throw new Exception($"The asp-for attribute must not be specified when using asp-for-expr.");
            }

            var editorTagHelperViewModel = forEditorTagHelper.EditorTagHelperViewModel as EditorTagHelperViewModel;
            if (editorTagHelperViewModel != null)
            {
                forEditorTagHelper.For = editorTagHelperViewModel.For;
            }
            else
            {
                throw new InvalidOperationException($"The model must be of type [{typeof(EditorTagHelperViewModel).FullName}]. Instead it was of type [{forEditorTagHelper.EditorTagHelperViewModel.GetType().FullName}].");
            }
        }
    }
}
