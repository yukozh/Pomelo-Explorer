component.created = function () {
    var self = this;
    self.instance = router.history.current.query.id;
    qv.createView('/mysql/getdatabases/' + self.instance)
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
        instance: null,
        databases: [],
        tables: {},
    };
};

component.watch = {
    toggled: function () {
        this.$forceUpdate();
    }
};

component.methods = {
};