component.menu = '/static/mysql/Resource/browse/menu';

component.created = function () {
    if (!this.isQuery) {
        this.getTable({ database: this.database, table: this.table, expression: null, page: 0 });
    }
};

component.data = function () {
    return {
        isQuery: false,
        readonly: false,
        isView: false,
        instance: null,
        database: null,
        table: null,
        rows: [],
        origin: [],
        status: [],
        columns: [],
        page: 0,
        showEditMenu: false,
        selected: {
            row: null,
            col: null
        },
        menuLeft: 0
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
    save: function () {
        var self = this;
        var request = self.generateSql();
        qv.post('/mysql/executenonquery/' + self.instance, request)
            .then(data => {
                // TODO: Update status bar, clean dirty
                self.cleanDirty();
                alert('OK');
            });
    },
    getTable: function (params) {
        var self = this;
        qv.post('/mysql/gettablecolumns/' + self.instance, params)
            .then(data => {
                self.columns = data;
                return qv.post('/mysql/gettablerows/' + self.instance, params);
            })
            .then(data => {
                self.status = data.values.map(x => 'plain');
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
        var index = $(e.target).parents('tr').attr('data-row-index');
        if ($(e.target).parents('tr').hasClass('new-record')) {
            this.status[index] = 'new';
        } else if ($(e.target).parents('tr').hasClass('remove-record')) {
            this.status[index] = 'remove';
        } else if ($(e.target).parents('tr').hasClass('removed-record')) {
            this.status[index] = 'removed';
        } else if ($(e.target).parents('tr').find('.dirty').length > 0) {
            this.status[index] = 'dirty';
        } else {
            this.status[index] = 'plain';
        }
    },
    cleanDirty: function () {
        for (var i = 0; i < this.rows.length; ++i) {
            this.cleanSingle(i);
        }
    },
    cleanSingle: function (index) {
        if (this.status[index] === 'remove') {
            this.status[index] = 'removed';
            $('tr[data-row-index="' + index + '"]').hide();
        } else {
            this.status[index] = 'plain';
            console.warn('clean ' + index);
            $('tr[data-row-index="' + index + '"]').removeClass('new-record');
            $('tr[data-row-index="' + index + '"]').removeClass('dirty');
            var fields = $('tr[data-row-index="' + index + '"]').find('.dirty');
            for (var i = 0; i < fields.length; ++i) {
                this.origin[index][i] = this.rows[index][i];
                $(fields[i]).attr('data-origin', $(fields[i]).val());
                $(fields[i]).removeClass('dirty');
            }
        }
    },
    generateSql: function () {
        var request = {
            sql: '',
            database: this.database,
            parameters: [],
            placeholders: [],
            dbTypes: []
        };
        for (var i = 0; i < this.rows.length; ++i) {
            this.generateSingleSql(i, request);
        }
        return request;
    },
    generateSingleSql: function (index, request) {
        var sql = '';
        if (!this.status[index]) return '';
        else if (this.status[index] === 'new') {
            request.sql += ('INSERT INTO `' + this.table + '` (' + this.generateColumns() + ') VALUES (' + this.generateValues(index, request) + ');\r\n');
        } else if (this.status[index] === 'remove') {
            request.sql += ('DELETE FROM `' + this.table + '` WHERE ' + this.generateCondition(index, request) + ' LIMIT 1;\r\n');
        } else if (this.status[index] === 'dirty') {
            request.sql += ('UPDATE `' + this.table + '` SET ' + this.generateUpdate(index, request) + ' WHERE ' + this.generateCondition(index, request) + ' LIMIT 1;\r\n');
        }
    },
    generateColumns: function () {
        return this.columns.map(x => '`' + x.field + '`').join(', ');
    },
    generateUpdate: function (index, request) {
        var doms = $('tr[data-row-index="' + index + '"]').find('td');
        var set = [];
        for (var i = 0; i < this.columns.length; ++i) {
            if ($(doms[i]).find('.dirty').length === 0) {
                continue;
            }
            var placeholder = `value_${index}_${i}`;
            set.push('`' + this.columns[i].field + '` = @' + placeholder);
            request.placeholders.push(placeholder);
            request.dbTypes.push(this.columns[i].type);
            request.parameters.push(this.rows[index][i]);
        }
        return set.join(', ');
    },
    generateValues: function (index, request) {
        var doms = $('tr[data-row-index="' + index + '"]').find('input');
        var values = [];
        for (var i = 0; i < this.columns.length; ++i) {
            var placeholder = `value_${index}_${i}`;
            values.push('@' + placeholder);
            request.placeholders.push(placeholder);
            request.dbTypes.push(this.columns[i].type);
            request.parameters.push(this.rows[index][i]);
        }
        return values.join(', ');
    },
    generateCondition: function (index, request) {
        var conditions = [];
        var doms = $('tr[data-row-index="' + index + '"]').find('input');
        for (var i = 0; i < this.columns.length; ++i) {
            var placeholder = `condition_${index}_${i}`;
            if (this.origin[index][i] !== null) {
                conditions.push('`' + this.columns[i].field + '` = @' + placeholder);
            } else {
                conditions.push('`' + this.columns[i].field + '` IS NULL');
            }
            request.placeholders.push(placeholder);
            request.dbTypes.push(this.columns[i].type);
            request.parameters.push(this.origin[index][i]);
        }
        return conditions.join(' AND ');
    },
    newRow: function () {
        var row = [];
        for (var i = 0; i < this.columns.length; ++i) {
            row.push(null);
        }
        this.$data.status.push('new');
        this.$data.rows.push(row);
        this.$data.origin.push(JSON.parse(JSON.stringify(row)));
    },
    setEditMenuLeft: function (e) {
        this.menuLeft = $(e.target).offset().left;
    },
    openEditor: function (type) {
        var self = this;
        var id = 'Special_Value_' + new Date().getTime().toString() + Math.round(Math.random() * 10000);
        var row = self.selected.row;
        var col = self.selected.col;
        var ipc = window.nodeRequire('electron').ipcRenderer;
        var channel = 'mysql-' + id;
        var doms = $('tr[data-row-index="' + row + '"]').find('input');
        var dom = $(doms[col]);
        var listen = function () {
            ipc.on(channel, (event, val, keepListen) => {
                console.log(val);
                app.$set(self.$data.rows[row], col, val);
                self.handleDirty({ target: dom[0] });
                if (!keepListen) {
                    ipc.removeAllListeners(channel);
                }
            });
        };
        if (type === 'text' || type === 'json') {
            qv.post('/mysql/editor/set-string-special-value', { key: id, value: dom.val(), keepListen: true })
                .then(() => {
                    listen();
                    return qv.post('/windows/open', {
                        url: '/mysql/editor/text/' + type + '/' + id
                    });
                });
        } else if (type === 'blob') {
            qv.post('/mysql/editor/set-blob-special-value', { key: id, value: null })
                .then(() => {
                    listen();
                });
        } else if (type === 'hex') {
            qv.post('/mysql/editor/set-string-special-value', { key: id, value: dom.val(), keepListen: true })
                .then(() => {
                    listen();
                    return qv.post('/windows/open', {
                        url: '/mysql/editor/hex/' + id
                    });
                });
        }
    },
    discard: function () {
        for (var i = 0; i < this.rows.length; ++i) {
            for (var j = 0; j < this.columns.length; ++j) {
                app.$set(this.$data.rows[i], j, this.$data.origin[i][j]);
                if (this.status[i] === 'new') {
                    app.$set(this.$data.status, i, 'removed');
                } else {
                    app.$set(this.$data.status, i, 'plain');
                }
            }
        }
    }
};