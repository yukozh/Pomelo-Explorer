component.data = function () {
    return {
        extensions: []
    };
};

component.methods = {
    getExtensionList: function () {
        var self = this;
        self.extensions = qv.createView('/extension/list', { creatable: false })
            .fetch((data) => {
                self.extensions = data;
            });
    }
};

component.created = function () {
    var menu = app.getMenu();
    menu.getExtensionList();
};