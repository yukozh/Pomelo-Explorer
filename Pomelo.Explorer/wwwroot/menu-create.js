component.data = function () {
    return {
        extensions: [],
        provider: null
    };
};

component.methods = {
    getExtensionList: function () {
        var self = this;
        self.extensions = qv.get('/extension/list', { creatable: false })
            .then((data) => {
                self.extensions = data;
            });
    }
};

component.created = function () {
    this.getExtensionList();
};

component.active = function () {
    app.active = 'new-connection';
};