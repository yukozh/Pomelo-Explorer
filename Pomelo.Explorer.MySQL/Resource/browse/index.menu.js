component.created = function () {
    alert(router.history.current.query.id);
    app.getMenu().instance = router.history.current.query.id;
    qv.createView('/mysql/getdatabases/' + app.getMenu().instance)
        .fetch((data) => {
            app.getMenu().databases = data;
        });
};

component.data = function () {
    return {
        active: null,
        instance: null,
        databases: []
    };
};

component.methods = {
};