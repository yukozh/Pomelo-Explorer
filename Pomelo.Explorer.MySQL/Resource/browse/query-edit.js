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
        timestamp: new Date().getTime()
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
                for (var i = 0; i < data.length; ++i) {
                    var vm = await self.$cont.open('/static/mysql/Resource/browse/table-view', { instance: this.instance, database: this.database, isQuery: true });
                    vm.rows = data[i].rows;
                    var columns = [];
                    for (var j = 0; j < data[i].columns.length; ++j) {
                        var col = {};
                        col.field = data[i].columns[j];
                        col.type = data[i].columnTypes[j];
                        col.null = data[i].nullable[j];
                        col.key = data[i].keys.some(x => x === col.field) ? "PRI" : null;
                        columns.push(col);
                    }
                    vm.columns = columns;
                    vm.origin = JSON.parse(JSON.stringify(data[i].rows));
                    var status = [];
                    for (var i = 0; i < vm.rows.length; ++i) {
                        status[i] = 'plain';
                    }
                    vm.status = status;
                    vm.$forceUpdate();
                }
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
    }
};