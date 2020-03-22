component.menu = '/static/mysql/Resource/browse/index';

component.created = function () {
    console.log(this);
    this.instance = router.history.current.query.instance;
    this.database = router.history.current.query.database;
    this.getTables();
};

component.data = function () {
    return {
        instance: null,
        database: null,
        selected: null,
        tables: [],
        views: {
            tables: null
        }
    };
};

component.methods = {
    getTables: function () {
        var self = this;
        self.views.tables = qv.createView('/mysql/gettables', { id: self.instance, database: self.database });
        self.views.tables.fetch((data) => {
            self.tables = data;
        });
    }
};