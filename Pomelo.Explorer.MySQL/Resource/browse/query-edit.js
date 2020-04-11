component.menu = '/static/mysql/Resource/browse/menu';

component.mounted = function () {
    var id = 'pomelo-mysql-query-result-' + this.instance + '-' + this.database;
    this.$cont = new PomeloComponentContainer('#' + id, this);

    app.loadModule('/static/mysql/Resource/lib/ace/ace.js');
    var domId = 'editor-' + this.timestamp;
    var editor = ace.edit(domId);
    editor.setTheme("ace/theme/monokai");
    editor.session.setMode("ace/mode/mysql");
    $('#' + domId)[0].editor = editor;
};

component.data = function () {
    return {
        instance: null,
        database: null,
        timestamp: new Date().getTime(),
        opened: [],
        active: null
    };
};

component.computed = {
    resultHeight: function () {
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
    execute: function () {
        var self = this;
        var domId = '#editor-' + self.timestamp;
        var sql = $(domId)[0].editor.getValue();
        qv.post('/mysql/executeresult/' + self.instance, { database: self.database, sql: sql })
            .then(async data => {
                for (let i = 0; i < data.length; ++i) {
                    var params = { instance: self.instance, database: self.database, table: data[i].table, isQuery: true, readonly: data[i].readonly, timeExecuted: data[i].timeSpan, rowsAffected: data[i].rowsAffected, timestamp: new Date().getTime() };
                    var vm = await self.$cont.open('/static/mysql/Resource/browse/table-view', params);
                    vm.rows = data[i].rows;
                    var columns = [];
                    for (var j = 0; j < data[i].columns.length; ++j) {
                        var col = {};
                        col.field = data[i].columns ? data[i].columns[j] : null;
                        col.type = data[i].columnTypes ? data[i].columnTypes[j] : null;
                        col.null = data[i].nullable ? data[i].nullable[j] : null;
                        col.key = data[i].keys && data[i].keys.some(x => x === col.field) ? "PRI" : null;
                        columns.push(col);
                    }
                    vm.columns = columns;
                    vm.origin = JSON.parse(JSON.stringify(data[i].rows));
                    for (var k = 0; k < data[i].rows.length; ++k) {
                        vm.$data.status.push('plain');
                    }
                    vm.$forceUpdate();

                    var id = self.$cont.toQueryString('/static/mysql/Resource/browse/table-view', params);
                    self.opened.push({ id: id, params: params });
                    self.active = id;
                }
            })
            .catch(function (data) {
                app.dialog('error', 'MySQL Error', data.responseJSON.code + ' - ' + data.responseJSON.message);
                return Promise.resolve();
            });
    },
    reset: function () {
        var domId = '#editor-' + this.timestamp;
        $(domId)[0].editor.setValue('');
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
    open: function (params, e) {
        if (e && $(e.target).hasClass('close')) {
            return;
        }
        var url = '/static/mysql/Resource/browse/table-view';
        var id = this.$cont.toQueryString(url, params);
        if (this.opened.filter(x => x.id === id).length === 0) {
            this.opened.push({ id: id, params: params});
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

        if (this.opened.length > 0) {
            --rmIndex;
            if (rmIndex < 0) {
                rmIndex = 0;
            }
            this.$cont.reactive(this.opened[rmIndex].id);
        }
    }
};