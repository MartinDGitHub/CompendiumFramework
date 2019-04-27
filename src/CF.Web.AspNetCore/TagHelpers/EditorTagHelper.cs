using CF.Web.AspNetCore.Models.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers
{
    [HtmlTargetElement("editor", Attributes = AspForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class EditorTagHelper : TagHelper
    {
        private const string AspForAttributeName = "asp-for";
        private const string TemplateViewPath = "~/Views/Shared/EditorTemplates/";
        private const string TemplateSuffix = "EditorTemplate.cshtml";

        private readonly ICompositeViewEngine _viewEngine;
        private readonly IViewBufferScope _viewBufferScope;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(AspForAttributeName)]
        public ModelExpression For { get; set; }

        public string Name { get; set; }

        public ViewDataDictionary ViewData { get; set; }

        public EditorTagHelper(ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope)
        {
            this._viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
            this._viewBufferScope = viewBufferScope ?? throw new ArgumentNullException(nameof(viewBufferScope));
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            // Suppress "editor" tag rendering.
            output.TagName = null;

            var editorViewPath = $"{(this.Name.StartsWith(TemplateViewPath) ? string.Empty: TemplateViewPath)}{Name}{(this.Name.EndsWith(TemplateSuffix) ? string.Empty : TemplateSuffix)}";
            var viewEngineResult = _viewEngine.GetView(this.ViewContext.ExecutingFilePath, editorViewPath, isMainPage: false);

            viewEngineResult.EnsureSuccessful(new string[] { editorViewPath });

            var viewBuffer = new ViewBuffer(this._viewBufferScope, viewEngineResult.ViewName, ViewBuffer.PartialViewPageSize);
            using (var writer = new ViewBufferTextWriter(viewBuffer, Encoding.UTF8))
            {
                var baseViewData = ViewData ?? ViewContext.ViewData;
                var newViewData = new ViewDataDictionary<object>(baseViewData, new EditorTagHelperViewModel { For = this.For });
                var partialViewContext = new ViewContext(ViewContext, viewEngineResult.View, newViewData, writer);

                using (viewEngineResult.View as IDisposable)
                {
                    await viewEngineResult.View.RenderAsync(partialViewContext);
                }

                output.Content.SetHtmlContent(viewBuffer);
            }
        }
    }
}
