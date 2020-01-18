using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers.TemplateTagHelpers
{
    [HtmlTargetElement("template", Attributes = AspForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class TemplateTagHelper : TagHelper
    {
        private class DefaultForModel : IModelExpressionWrapper
        {
            public ModelExpression ModelExpression { get; set; }
        }

        protected const string AspForAttributeName = "asp-for";
        protected const string TemplateViewPath = "~/Views/Shared/Templates/";
        protected const string TemplateSuffix = "Template.cshtml";

        [HtmlAttributeNotBound]
        protected ICompositeViewEngine ViewEngine { get; }

        [HtmlAttributeNotBound]
        protected IViewBufferScope ViewBufferScope { get; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName(AspForAttributeName)]
        public ModelExpression For { get; set; }

        public virtual string Name { get; set; }

        public ViewDataDictionary ViewData { get; }

        [HtmlAttributeNotBound]
        protected virtual IModelExpressionWrapper TemplateModel { get; set; }

        public TemplateTagHelper(ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope)
        {
            this.ViewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
            this.ViewBufferScope = viewBufferScope ?? throw new ArgumentNullException(nameof(viewBufferScope));
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

            if (this.For == null)
            {
                throw new InvalidOperationException("No for model epxression was provided.");
            }

            if (string.IsNullOrWhiteSpace(this.Name))
            {
                throw new InvalidOperationException("No template name was provided.");
            }

            // Suppress "editor" tag rendering.
            output.TagName = null;

            var editorViewPath = $"{(this.Name.StartsWith(TemplateViewPath) ? string.Empty: TemplateViewPath)}{Name}{(this.Name.EndsWith(TemplateSuffix) ? string.Empty : TemplateSuffix)}";
            var viewEngineResult = this.ViewEngine.GetView(this.ViewContext.ExecutingFilePath, editorViewPath, isMainPage: false);

            viewEngineResult.EnsureSuccessful(new string[] { editorViewPath });

            if (this.TemplateModel == null)
            {
                this.TemplateModel = new DefaultForModel { ModelExpression = this.For };
            }
            else if (this.TemplateModel.ModelExpression == null)
            {
                this.TemplateModel.ModelExpression = this.For;
            }
            var baseViewData = this.ViewData ?? ViewContext.ViewData;
            var newViewData = new ViewDataDictionary<object>(baseViewData, this.TemplateModel);
            
            using var writer = new StringWriter();
            var partialViewContext = new ViewContext(ViewContext, viewEngineResult.View, newViewData, writer);

            using (viewEngineResult.View as IDisposable)
            {
                await viewEngineResult.View.RenderAsync(partialViewContext).ConfigureAwait(false);
            }

            output.Content.SetHtmlContent(writer.ToString());
        }
    }
}
