app.filter('capitalizeFirst', function () {
    return function (input) {
        return (!!input) ? input.charAt(0).toUpperCase() + input.substr(1).toLowerCase() : '';
    }
});

app.filter('truncateStringWithEllipsis', function () {
    return function (input, maxCharacters) {
        if (input && input.length > maxCharacters) {
            // Replace this with the real implementation
            return input.substring(0, maxCharacters) + '...';
        } else
            return input;
    }
});

app.filter('domain', function () {
    return function (input) {
        var matches,
            output = "",
            urls = /\w+:\/\/([\w|\.]+)/;

        matches = urls.exec(input);

        if (matches !== null) output = matches[1];

        return output;
    };
});