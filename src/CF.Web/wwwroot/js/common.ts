/*! CF v1.0.0 | Compendium Framework |  */

import * as $ from "../lib/jquery/jquery";
import * as _ from "../lib/lodash.js/lodash";

export class StringUtility {
    // Replaces placeholders in the format string with values in the replacements
    // array at the specified placeholder indexes. For example, a format string
    // of "Fo{0} Ba{0} Ba{1}" and replacements of ["r", "z"] will result in a formatted
    // string of "For Bar Baz".
    stringFormat(formatString: string, replacements: Array<string>): string {
        let formattedString = formatString;

        if (_.isString(formatString) && _.isArray(replacements)) {
            for (let i = 0; i < replacements.length; i++) {
                formattedString = formattedString.replace("{" + i + "}", replacements[i]);
            }
        }

        return formatString;
    };
}
