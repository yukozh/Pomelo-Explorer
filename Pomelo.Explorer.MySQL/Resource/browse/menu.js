component.created = function () {
    var self = this;
    qv.createView('/mysql/getdatabases/' + self.id)
        .fetch((data) => {
            self.databases = data;
            for (var i = 0; i < data.length; ++i) {
                self.toggled.push(false);
            }
        });
};

component.data = function () {
    return {
        toggled: [],
        id: null,
        databases: [],
        tables: {},
        active: null
    };
};

component.watch = {
    toggled: function () {
        this.$forceUpdate();
    }
};

component.methods = {
};