component.created = function () {
    app.getMenu().instance = router.history.current.query.id;
    qv.createView('/mysql/getdatabases/' + app.getMenu().instance)
        .fetch((data) => {
            app.getMenu().databases = data;
            for (var i = 0; i < data.length; ++i) {
                app.getMenu().toggled.push(false);
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
        app.getMenu().$forceUpdate();
    }
};

component.methods = {
    
};