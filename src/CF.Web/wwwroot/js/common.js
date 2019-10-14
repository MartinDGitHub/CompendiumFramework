"use strict";
/*! CF v1.0.0 | Compendium Framework |  */
Object.defineProperty(exports, "__esModule", { value: true });
var _ = require("../lib/lodash.js/lodash");
var StringUtility = /** @class */ (function () {
    function StringUtility() {
    }
    // Replaces placeholders in the format string with values in the replacements
    // array at the specified placeholder indexes. For example, a format string
    // of "Fo{0} Ba{0} Ba{1}" and replacements of ["r", "z"] will result in a formatted
    // string of "For Bar Baz".
    StringUtility.prototype.stringFormat = function (formatString, replacements) {
        var formattedString = formatString;
        if (_.isString(formatString) && _.isArray(replacements)) {
            for (var i = 0; i < replacements.length; i++) {
                formattedString = formattedString.replace("{" + i + "}", replacements[i]);
            }
        }
        return formatString;
    };
    ;
    return StringUtility;
}());
exports.StringUtility = StringUtility;
//# sourceMappingURL=common.js.map