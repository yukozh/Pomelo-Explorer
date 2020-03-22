String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
};

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
});

var appBuilder = {
    router: router,
    menuContainer: {},
    activeMenu: null,
    menuHiddenFunc: null,
    menuShowFunc: null,
    loadMenu: async function (menu) {
        var destroy = function () {
            if (this.activeMenu) {
                this.activeMenu.$destroy();
                this.activeMenu = null;
                if (this.menuHiddenFunc) {
                    this.menuHiddenFunc();
                }
            }
            return;
        };

        if (!menu) {
            destroy();
        }

        var name = 'menu' + menu.replaceAll('\/', '-').replaceAll('\\.', '-');
        if (this.activeMenu && name === this.activeMenu.$options.name) {
            return;
        } else {
            var html = await this.getTemplate(menu + '.menu');
            if (!html) {
                return this.loadMenu(null);
            }
            var js = await this.getScript(menu + '.menu');

            if ($('#menu-css').length === 0) {
                $('head').append(`<link id="component-css" href="${(menu + '.menu.css')}" rel="stylesheet" />`);
            } else {
                $('#menu-css').attr('href', url + '.menu.css');
            }

            var component = { template: html };
            eval(js);
            var ComponentBuilder = Vue.component('menu' + menu.replaceAll('\/', '-').replaceAll('\\.', '-'), component);
            var instance = new ComponentBuilder();
            instance.$root = app;
            destroy();
            this.activeMenu = instance;

            $('#menu').html('<div id="menu-anchor"></div>');
            this.activeMenu.$mount('#menu-anchor');
            if (this.menuShowFunc) {
                this.menuShowFunc();
            }
        }
    },
    onMenuShow: function (func) {
        this.menuShowFunc = func;
    },
    onMenuHidden: function (func) {
        this.menuHiddenFunc = func;
    },
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
    getCss: function (url) {
        return fetch(url + '.css')
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
    buildCreatedFunction: function (component, func) {
        if (component.created === undefined) {
            component.created = function () { };
        }
        var origin = component.created;
        component.created = function () {
            // Others
            origin.call(this);
            func.call(this);
        };
    },
    loadComponentIfNotExist: async function (url) {
        if (this.router.apps.some(x => x._route.path === url))
            return;

        var self = this;
        html = await this.getTemplate(url);
        js = await this.getScript(url);
        var component = this.buildComponent(html, js, menu);
        if ($('#component-css').length === 0) {
            $('head').append(`<link id="component-css" rel="stylesheet" />`);
        }
        this.buildCreatedFunction(component, function () {
            $('#component-css').attr('href', url + '.css');
            self.loadMenu(component.menu);
        });
        this.addRoute(url, component);
    },
    redirect: async function (path, queries) {
        await this.loadComponentIfNotExist(path);
        this.router.push({ path: path, query: queries });
    }
};