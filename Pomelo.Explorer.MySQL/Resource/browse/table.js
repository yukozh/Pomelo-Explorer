component.menu = '/static/mysql/Resource/browse/menu';

component.created = function () {
    var id = 'pomelo-mysql-table-' + this.instance + '-' + this.database;
    this.$cont = new PomeloComponentContainer('#' + id, this);
    this.$cont.open('/static/mysql/Resource/browse/table-list', { instance: this.instance, database: this.database });
};

component.data = function () {
    return {
        instance: null,
        database: null,
        opened: [],
        active: null
    };
};

component.methods = {
    openList: function () {
        this.active = null;
        this.$cont.open('/static/mysql/Resource/browse/table-list', { instance: this.instance, database: this.database });
    },
    open: function (type, params, title) {
        var url = '/static/mysql/Resource/browse/' + type;
        var id = this.$cont.toQueryString(url, params);
        if (this.opened.filter(x => x.id === id).length === 0) {
            this.opened.push({ id: id, title: title, params: params, type: type });
        }
        this.active = id;
        this.$cont.open(url, params);
    }
};