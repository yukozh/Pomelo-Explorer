var mainContainer = new PomeloComponentContainer('#main', app, function (view) {
    return fetch(view + '.css')
        .then(data => {
            return data.text();
        })
        .then(data => {
            $('#component-css').html(data);
        });
}, function () { });

var menuContainer = new PomeloComponentContainer('#menu', app, function (view) {
    return fetch(view + '.css')
        .then(data => {
            return data.text();
        })
        .then(data => {
            $('#menu-css').html(data);
            app.menu = true;
        });
}, function (view) {
        if (!view) {
            app.menu = false;
        } else {
            app.menu = true;
        }
});

var app = new Vue({
    data: {
        active: null,
        menu: false,
        tab: {
            instances:[]
        },
        extensions: [],
        height: 0
    },
    created: function () {
        var self = this;
        self.getExtensions();
        self.height = window.innerHeight;
    },
    methods: {
        open: function (view, params) {
            app.$main.open(view, params);
        },
        openMenu: function (view, params) {
            app.$menu.open(view, params);
        },
        openInstance: function (id, provider) {
            this.active = 'instance-' + id;
            var view = '/static/' + provider + this.extensions.filter(x => x.id === provider)[0].browse;
            app.$main.open();
            app.$menu.open(view, { id: id });
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
    },
    mounted: function () {
        this.$nextTick(() => {
            window.addEventListener('resize', () => { this.height = window.innerHeight; });
        });
    }
});

app.$main = mainContainer;
app.$menu = menuContainer;
app.$mount('#app');

var __split_down = false;

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