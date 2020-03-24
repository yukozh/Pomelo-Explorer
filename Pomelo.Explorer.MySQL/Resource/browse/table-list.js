component.menu = '/static/mysql/Resource/browse/menu';

component.created = function () {
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
    getTables: function () {
        var self = this;
        self.views.tables = qv.createView('/mysql/gettables', { id: self.instance, database: self.database });
        self.views.tables.fetch((data) => {
            self.tables = data;
        });
    }
};