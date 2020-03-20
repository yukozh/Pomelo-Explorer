menu('/create');

component.data = function () {
    return {
        form: {
            address: null,
            port: 3306,
            ssl: 'None',
            username: null,
            password: null
        }
    };
};

component.methods = {
    connect: function () {
        qv.post('/mysql/createconnection', this.form)
            .then(function (data) {
                return qv.post('/mysql/openconnection/' + data.id, { });
            })
            .then(function () {
                alert('OK');
            })
            .catch(function (data) {
                app.dialog('error', 'MySQL Error', data.responseJSON.code + ' - ' + data.responseJSON.message);
            });
    }
};

component.created = function () {
    app.getMenu().provider = 'mysql';
};