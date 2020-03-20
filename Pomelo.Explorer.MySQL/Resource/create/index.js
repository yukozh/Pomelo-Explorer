menu('/create');

component.data = function () {
    return {
        form: {
            address: null,
            port: 3306,
            ssl: 'None',
            username: null,
            password: null
        },
        working: false
    };
};

component.methods = {
    connect: function () {
        var self = this;
        self.working = true;
        qv.post('/mysql/createconnection', self.form)
            .then(function (data) {
                return qv.post('/mysql/openconnection/' + data.id, { });
            })
            .then(function () {
                // TODO: Refresh tab
                return Promise.resolve();
            })
            .catch(function (data) {
                app.dialog('error', 'MySQL Error', data.responseJSON.code + ' - ' + data.responseJSON.message);
                return Promise.resolve();
            })
            .finally(function () {
                self.working = false;
            });
    },
    test: function () {
        var self = this;
        self.working = true;
        qv.post('/mysql/testconnection', self.form)
            .then(function () {
                app.dialog('info', 'MySQL', `Connect to ${self.form.username}@${self.form.address} succeeded.`);
                return Promise.resolve();
            })
            .catch(function (data) {
                app.dialog('error', 'MySQL Error', data.responseJSON.code + ' - ' + data.responseJSON.message);
                return Promise.resolve();
            })
            .finally(function () {
                self.working = false;
            });
    }
};

component.created = function () {
    app.getMenu().provider = 'mysql';
};