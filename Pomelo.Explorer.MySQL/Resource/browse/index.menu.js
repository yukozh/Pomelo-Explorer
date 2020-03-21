component.created = function () {
    app.getMenu().instance = router.history.current.query.id;
    qv.createView('/mysql/getdatabases/' + app.getMenu().instance)
        .fetch((data) => {
            app.getMenu().databases = data;
        });
};

component.data = function () {
    return {
        active: {
            db: null,
            action: null,
            detail: null
        },
        instance: null,
        databases: [],
        tables: {},
    };
};

component.methods = {
    
};