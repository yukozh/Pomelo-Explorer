var router = new VueRouter({
    mode: 'history'
});
router.afterEach((to, from) => {
    $(window).scrollTop(0);
    if (to.matched.length && to.matched[0].instances.default) {
        var current = to.matched[0].instances.default;
        if (current && current.$options.created.length) {
            current.$options.created[0].call(current);
        }
    }
    if (to.matched.length) {
        var menu = app._router.matcher.match(to.matched[0].path).matched;
        if (menu.length > 0) {
            var menuCom = menu[0].instances.default;
            if (menuCom && menuCom.$options.created.length) {
                menuCom.$options.created[0].call(current);
            }
        }
    }
});

var appBuilder = {
    router: router,
    getTemplate: function (url) {
        return fetch(url + '.html')
            .then((data) => { return data.text() })
            .catch((err) => { return Promise.resolve('') });
    },
    getScript: function (url) {
        return fetch(url + '.js')
            .then((data) => { return data.text() })
            .catch((err) => { return Promise.resolve('') });
    },
    buildComponent: function (template, script) {
        var component = { template: template };
        eval(script);
        return component;
    },
    addRoute: function (url, component) {
        router.addRoutes([{ path: url, name: url, component: component }]);
    },
    addMenuRoute: function (url, component) {
        router.addRoutes([{ path: url + '.menu', name: url + '.menu', component: component }]);
    },
    loadComponentIfNotExist: async function (url, isMenu) {
        if (this.router.apps.some(x => x._route.path === url))
            return;

        if (!isMenu) {
            html = await this.getTemplate(url);
            js = await this.getScript(url);
            var component = this.buildComponent(html, js);
            if (component.computed === undefined) {
                component.computed = {};
            }
            component.computed.menu = function () {
                var menuPath = this.$route.path + '.menu';
                var matched = this.$root._router.matcher.match(menuPath).matched;
                if (matched.length)
                    return matched[0].instances.default;
                else
                    return null;
            };
            this.addRoute(url, component);
        } else {
            html = await this.getTemplate(url + '.menu');
            var nomenu = false;
            if (!html) {
                nomenu = true;
                html = '<div data-no-menu></div>';
            }
            js = await this.getScript(url + '.menu');
            if (!js) {
                js = 'component.created = function () { this.$root.menu = false; }';
            }
            var component = this.buildComponent(html, js);
            if (!nomenu) {
                if (component.created === undefined) {
                    component.created = function () { };
                }
                var tmpFunc = component.created;
                component.created = function () { tmpFunc(); this.$root.menu = true; };
            }
            this.addMenuRoute(url, component);
        }
    },
    redirect: async function (path, params) {
        await this.loadComponentIfNotExist(path, false);
        await this.loadComponentIfNotExist(path, true);
        this.router.push({ path: path, params: params });
    }
};