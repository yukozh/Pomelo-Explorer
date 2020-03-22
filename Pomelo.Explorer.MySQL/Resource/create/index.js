menu('/create');

component.created = function () {
    app.getMenu().provider = 'mysql';
};