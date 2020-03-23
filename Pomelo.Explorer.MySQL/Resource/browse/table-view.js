component.menu = '/static/mysql/Resource/browse/menu';

component.created = function () {
    console.warn(this);
    this.getTable({ database: this.database, table: this.table, expression: null, page: 0 });
};

component.data = function () {
    return {
        instance: null,
        database: null,
        table: null,
        rows: [],
        columns: [],
        page: 0
    };
};

component.methods = {
    getTable: function (params) {
        var self = this;
        qv.post('/mysql/gettablerows/' + self.instance, params)
            .then(data => {
                self.columns = data.columns;
                self.rows = data.values;
            });
    }
};