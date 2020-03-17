var app = new Vue({
    router: router,
    data: {
        menu: false
    },
    methods: {
        redirect: function (path, params) {
            window.appBuilder.redirect(path, params);
        },
        getMain: function () {
            if (this._router.history.current.matched.length)
                return this._router.history.current.matched[0].instances.default;
            else
                return null;
        },
        getMenu: function () {
            var matched = this._router.matcher.match(router.history.current.path + '.menu').matched;
            if (matched.length)
                return matched[0].instances.default;
            else
                return null;
        }
    }
});

$.get('/layout.html', {}, function (template) {
    Vue.component('render-layout', {
        template: template
    });
    app.$mount('#app');
});