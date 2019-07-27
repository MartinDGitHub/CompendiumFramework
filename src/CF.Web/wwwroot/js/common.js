/*! CF v1.0.0 | Compendium Framework |  */

// Establish the root and most common framework namespaces.
window.CF = { __namespace: true };
CF.Common = { __namespace: true };
CF.Common.Constants = { __namespace: true };

CF.Common.Constants.ROOT_NAMESPACE = "CF";

// Replaces placeholders in the format string with values in the replacements
// array at the specified placeholder indexes. For example, a format string
// of "Fo{0} Ba{0} Ba{1}" and replacements of ["r", "z"] will result in a formatted
// string of "For Bar Baz".
CF.Common.stringFormat = function (formatString, replacements) {
    let formattedString = formatString;

    if (_.isString(formatString) && _.isArray(replacements)) {
        for (let i = 0; i < replacements.length; i++) {
            formattedString = formattedString.replace("{" + i + "}", replacements[i]);
        }
    }

    return formatString;
};

// Ensure that the specified, period-delimited namespace exists. Returns the namespace
// that was ensured to exist.
CF.Common.ensureNamespaceExists = function (namespace) {
    if (_.isString(namespace)) {
        const parts = namespace.split(".");

        if (parts[0] !== CF.Common.Constants.ROOT_NAMESPACE) {
            throw new Error(CF.Common.stringFormat("The namespace of [{0}] does not have a root of [{1}].", [namespace, CF.Common.Constants.ROOT_NAMESPACE]));
        }

        let namespacePath = CF.Common.Constants.ROOT_NAMESPACE;
        for (let i = 1; i < parts.length; i++) {
            namespacePath = namespacePath + "." + parts[i];
            if (_.isUndefined(window[namespacePath])) {
                window[namespacePath] = { __namespace: true };
            }
        }

        return window[namespacePath];
    }
};