/*! CF v1.0.0 | Compendium Framework |  */
$(function () {
    const MESSAGES_COOKIE_NAME = 'cf-messages';
    let messagesCookieRawValue = null;
    if (Cookies && (messagesCookieRawValue = Cookies.get(MESSAGES_COOKIE_NAME))) {
        var messages = JSON.parse(messagesCookieRawValue);
        if (messages && messages.TargetUrl && messages.TargetUrl.toLowerCase() === window.location.pathname.toLowerCase()) {
            // Act on the messages.

            // Delete the cookie so that it isn't re-sent.
            Cookies.remove(MESSAGES_COOKIE_NAME);
        }
    }
});