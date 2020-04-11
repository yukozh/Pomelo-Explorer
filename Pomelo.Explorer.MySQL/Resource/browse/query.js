component.menu = '/static/mysql/Resource/browse/menu';

component.created = function () {
    var id = 'pomelo-mysql-query-' + this.instance + '-' + this.database;
    this.$cont = new PomeloComponentContainer('#' + id, this);
    this.$cont.open('/static/mysql/Resource/browse/query-edit', { instance: this.instance, database: this.database });
};

component.data = function () {
    return {
        instance: null,
        database: null,
        opened: [],
        active: null
    };
};

component.computed = {
    tabbarHeight: function () {
        return this.$refs.tabbar.offsetHeight;
    }
};

component.methods = {
    openList: function () {
        this.active = null;
        this.$cont.open('/static/mysql/Resource/browse/table-list', { instance: this.instance, database: this.database });
    },
    open: function (type, params, title, e) {
        if (e && $(e.target).hasClass('close')) {
            return;
        }
        var url = '/static/mysql/Resource/browse/' + type;
        var id = this.$cont.toQueryString(url, params);
        if (this.opened.filter(x => x.id === id).length === 0) {
            this.opened.push({ id: id, title: title, params: params, type: type });
        }
        this.active = id;
        this.$cont.open(url, params);
    },
    destroy: function (id) {
        this.$cont.closeById(id);
        var rmIndex = 0;
        for (var i = 0; i < this.opened.length; ++i) {
            if (this.opened[i].id === id) {
                this.opened.splice(i, 1);
                rmIndex = i;
                break;
            }
        }

        if (this.opened.length === 0) {
            this.$cont.open('/static/mysql/Resource/browse/table-list', { instance: this.instance, database: this.database });
        } else {
            --rmIndex;
            if (rmIndex < 0) {
                rmIndex = 0;
            }
            this.$cont.reactive(this.opened[rmIndex].id);
        }
    }
};