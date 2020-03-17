component.data = function () {
    return {
        extensions: [],
        provider: null
    };
};

component.methods = {
    getExtensionList: function () {
        app.getMenu().extensions = qv.get('/extension/list', { creatable: false })
            .then((data) => {
                app.getMenu().extensions = data;
            });
    }
};

component.created = function () {
    var menu = app.getMenu();
    menu.getExtensionList();
};