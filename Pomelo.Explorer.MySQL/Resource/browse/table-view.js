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
        origin:[],
        columns: [],
        page: 0
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
    getTable: function (params) {
        var self = this;
        qv.post('/mysql/gettablerows/' + self.instance, params)
            .then(data => {
                self.columns = data.columns;
                self.rows = data.values;
                self.origin = JSON.parse(JSON.stringify(data.values));
            });
    },
    getWindowHeight: function () {
        $(window).height();
    },
    getTableHeight: function () {
        try {
            return (getWindowHeight() - this.$refs.toolbar.offsetHeight - this.$root.$refs.tabbar.offsetHeight - 25).toString() + 'px';
        } catch (ex) {
            return null;
        }
    },
    handleDirty: function (e) {
        if (e.target.value === $(e.target).attr('data-origin')) {
            $(e.target).removeClass('dirty');
        } else {
            $(e.target).addClass('dirty');
        }
    }
};