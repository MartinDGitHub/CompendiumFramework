using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace CF.Web.AspNetCore.TagHelpers.TemplateTagHelpers
{
    [HtmlTargetElement("input", Attributes = AspForExprAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class InputTemplateTagHelper : InputTagHelper, ITemplateTagHelper
    {
        private const string AspForExprAttributeName = "asp-for-expr";

        protected readonly HtmlEncoder _htmlEncoder;

        // A regex to split a "." delimited input type class value into the following parts:
        // e.g. (match checkbox input types): checkbox.form-check-input
        // 1 = checkbox
        // 2 = form-check-input
        // e.g. (match all non-checkbox input types): ^((?!checkbox).)*$.form-control
        // 1 = ^((?!checkbox).)*$
        // 2 = form-control
        protected static Regex InputTypeClassValueSplitRegex = new Regex(@"(.*)\.(-*[_a-zA-Z]+[_a-zA-Z0-9-]*)", RegexOptions.Compiled);

        [HtmlAttributeName(AspForExprAttributeName)]
        public IModelExpressionWrapper ModelExpressionWrapper { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets and sets a classes for specific input types.
        /// </summary><remarks>
        /// Specify a semicolon-delimited set of input type classes where each value is a regex that 
        /// matches input types the class belongs to followed by a "." and the name of the class 
        /// to apply. For example: "[date|datetime-local].dt-class;checkbox.cb-class" will apply 
        /// dt-class to date and datetime-local inputs and the cb-class to checkbox inputs.
        /// For available types, see: https://www.w3.org/TR/html52/sec-forms.html#element-attrdef-input-type
        /// </remarks>
        public string InputTypeClasses { get; set; }

        public InputTemplateTagHelper(IHtmlGenerator generator, HtmlEncoder htmlEncoder) : base(generator)
        {
            this._htmlEncoder = htmlEncoder ?? throw new ArgumentNullException(nameof(htmlEncoder));
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (this.IsReadOnly)
            {
                output.Attributes.Add(new TagHelperAttribute("readonly"));
            }

            if (this.IsDisabled)
            {
                output.Attributes.Add(new TagHelperAttribute("disabled"));
            }

            TemplateTagHelperUtility.Process(this, () => base.Process(context, output));

            // Apply type-specific input classes.
            var typeAttribute = output.Attributes["type"];
            if (typeAttribute != null && !string.IsNullOrWhiteSpace(this.InputTypeClasses))
            {
                var values = this.InputTypeClasses.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in values)
                {
                    var match = InputTypeClassValueSplitRegex.Match(value);
                    if (match.Groups.Count == 3)
                    {
                        var regex = match.Groups[1].Value;
                        var className = match.Groups[2].Value;

                        if (Regex.IsMatch((string)typeAttribute.Value, regex))
                        {
                            output.AddClass(className, this._htmlEncoder);
                        }

                        Console.WriteLine($"Regex = [{regex}], Class Name = [{className}]");
                    }
                    else
                    {
                        throw new InvalidOperationException($"The input type class value of [{value}] could not be successfully parsed into a regex and class name. Ensure: 1) values are semicolon delimited; 2) the class name is prefixed with a period; 3) the class name only contains valid characters.");
                    }
                }
            }
        }
    }
}
