component.menu = '/static/mysql/Resource/browse/menu';

component.created = function () {
    this.getColumns();
};

component.data = function () {
    return {
        instance: null,
        database: null,
        table: null,
        active: 'columns',
        data: {
            columns: null
        },
        origin: {
            columns: null
        },
        toggle: []
    };
};

component.computed = {
    tableHeight: function () {
        try {
            return (this.$root.$root.height - this.toolbarHeight - this.$root.tabbarHeight - 25) + 'px';
        } catch {
            return 0;
        }
    },
    toolbarHeight: function () {
        if (this.$refs.toolbar) {
            return this.$refs.toolbar.offsetHeight;
        } else {
            return 42;
        }
    }
};

component.methods = {
    getColumns: function () {
        var self = this;
        qv.post('/mysql/getfulltablecolumns/' + self.instance, { database: self.database, table: self.table })
            .then(data => {
                self.data.columns = data;
                self.origin.columns = JSON.parse(JSON.stringify(data));
                var toggle = [];
                for (var i = 0; i < data.length; ++i) {
                    toggle.push(false);
                }
                self.toggle = toggle;

            });
    },
    save: function () { },
    discard: function () { }
};