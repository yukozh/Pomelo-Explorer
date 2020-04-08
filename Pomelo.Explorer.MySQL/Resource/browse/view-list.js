component.menu = '/static/mysql/Resource/browse/menu';

component.created = function () {
    this.getViews();
};

component.data = function () {
    return {
        instance: null,
        database: null,
        selected: null,
        items: [],
        views: {
            views: null
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
    getViews: function () {
        var self = this;
        self.views.views = qv.createView('/mysql/getviews', { id: self.instance, database: self.database });
        self.views.views.fetch((data) => {
            self.items = data;
        });
    }
};