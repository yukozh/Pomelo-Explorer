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