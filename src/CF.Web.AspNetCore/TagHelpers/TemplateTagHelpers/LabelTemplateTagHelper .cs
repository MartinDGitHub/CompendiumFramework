using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace CF.Web.AspNetCore.TagHelpers.TemplateTagHelpers
{
    [HtmlTargetElement("label", Attributes = AspForExprAttributeName)]
    public class LabelTemplateTagHelper : LabelTagHelper, ITemplateTagHelper
    {
        private const string AspForExprAttributeName = "asp-for-expr";

        protected IHtmlGenerator HtmlGenerator { get; }

        [HtmlAttributeName(AspForExprAttributeName)]
        public IModelExpressionWrapper ModelExpressionWrapper { get; set; }

        /// <summary>
        /// Gets classes to apply when the input that would be generated for the model expression
        /// would be of a particular type. See <see cref="InputTemplateTagHelper.InputTypeClasses"/>
        /// for more details on how to specify this value.
        /// </summary>
        public string InputTypeClasses { get; set; }

        public LabelTemplateTagHelper(IHtmlGenerator generator) : base(generator)
        {
            this.HtmlGenerator = generator ?? throw new ArgumentNullException(nameof(generator));
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

            await TemplateTagHelperUtility.ProcessAsync(this, () => base.ProcessAsync(context, output)).ConfigureAwait(false);

            // If there are classes that are specific to an input type, some extra work must
            // be performed to artificially create an input tag helper to resolve the type of
            // input from the model metadata.
            if (!string.IsNullOrWhiteSpace(this.InputTypeClasses))
            {
                var inputTagHelper = new InputTagHelper(this.HtmlGenerator);
                inputTagHelper.For = this.For;
                inputTagHelper.ViewContext = this.ViewContext;
                var inputTagHelperOutput = new TagHelperOutput("input", new TagHelperAttributeList(), (_, __) => Task.FromResult((TagHelperContent)new DefaultTagHelperContent()));
                inputTagHelper.Process(context, inputTagHelperOutput);
            }
        }
    }
}
