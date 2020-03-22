var app = new Vue({
    router: router,
    data: {
        active: null,
        menu: false,
        tab: {
            instances:[]
        },
        extensions:[]
    },
    created: function () {
        var self = this;
        self.getExtensions();
        appBuilder.onMenuHidden(function () {
            self.menu = false;
        });
        appBuilder.onMenuShow(function () {
            var menu = self.getMenu();
            menu.$root = self;
            menu.$parent = self;
            self.menu = true;
        });
    },
    methods: {
        redirect: function (path, queries) {
            window.appBuilder.redirect(path, queries);
        },
        redirectToInstance: function (id, provider) {
            this.active = 'instance-' + id;
            this.redirect('/static/' + provider + this.extensions.filter(x => x.id === provider)[0].browse, { id: id });
        },
        useMenu: function (menu) {
            appBuilder.loadMenu(menu);
        },
        getMain: function () {
            if (this._router.history.current.matched.length)
                return this._router.history.current.matched[0].instances.default;
            else
                return null;
        },
        getMenu: function () {
            return appBuilder.activeMenu;
        },
        dialog: function (icon, title, message) {
            qv.post('/dialog/show', { icon: icon, title: title, message: message });
        },
        pushInstance: function (id, provider) {
            this.tab.instances.push({
                id: id,
                provider: provider
            });
        },
        popInstance: function (id) {
            this.tab.instances.pop(this.tab.instances.pop.filter(x => x.id === id)[0]);
        },
        getExtensions: function () {
            var self = this;
            self.extensions = qv.get('/extension/list', { creatable: false })
                .then((data) => {
                    self.extensions = data;
                });
        }
    }
});

var __split_down = false;
$.get('/layout.html', {}, function (template) {
    Vue.component('render-layout', {
        template: template
    });
    app.$mount('#app');
});

$(window).mousedown(function (e) {
    var dom = $(e.target);
    if (dom.hasClass('pomelo-instance-menu-drag')) {
        __split_down = true;
    }
});

$(window).mousemove(function (e) {
    if (__split_down) {
        var x = e.pageX;
        var width = $(window).width();
        var left = x - $('.pomelo-instance-tab').outerWidth();
        var right = width - x;
        $('.pomelo-instance-menu').css('width', left + 'px');
        $('.pomelo-instance-main').css('left', x);
        $('.pomelo-instance-menu-drag').css('left', (x - 3) + 'px');
    }
});

$(window).mouseup(function (e) {
    __split_down = false;
});

Vue.component('svg-icon', {
    props: ['src'],
    data: function () {
        return {
            inline: ''
        };
    },
    created: function () {
        var self = this;
        fetch(self.src).then(data => {
            return data.text();
        }).then(txt => {
            self.inline = txt;
        });
    },
    template: '<svg v-html="inline"></svg>'
});