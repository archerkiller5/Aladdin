var mwc = mwc || {};
(function() {

    /* 日志记录 ***************************************************/
    mwc.log = mwc.log || {};

    mwc.log.levels = {
        DEBUG: 1,
        INFO: 2,
        WARN: 3,
        ERROR: 4,
        FATAL: 5
    };
    mwc.log.level = mwc.log.levels.DEBUG;

    mwc.log.log = function(logObject, logLevel) {
        if (!window.console || !window.console.log) {
            return;
        }

        if (logLevel != undefined && logLevel < mwc.log.level) {
            return;
        }
        if (window.console.error && logLevel >= 4)
            console.error(logObject);
        else if (window.console.warn && logLevel >= 3)
            console.warn(logObject);
        else
            console.log(logObject);
    };

    mwc.log.debug = function(logObject) {
        mwc.log.log(logObject, mwc.log.levels.DEBUG);
    };

    mwc.log.info = function(logObject) {
        mwc.log.log(logObject, mwc.log.levels.INFO);
    };

    mwc.log.warn = function(logObject) {
        mwc.log.log(logObject, mwc.log.levels.WARN);
    };

    mwc.log.error = function(logObject) {
        mwc.log.log(logObject, mwc.log.levels.ERROR);
    };

    mwc.log.fatal = function(logObject) {
        mwc.log.log(logObject, mwc.log.levels.FATAL);
    };

    /* 通知 *********************************************/
    //定义通知接口，需要自定义实现

    mwc.notify = mwc.notify || {};

    mwc.notify.success = function(message, title) {
        mwc.log.warn("mwc.notify.success is not implemented!");
    };

    mwc.notify.info = function(message, title) {
        mwc.log.warn("mwc.notify.info is not implemented!");
    };

    mwc.notify.warn = function(message, title) {
        mwc.log.warn("mwc.notify.warn is not implemented!");
    };

    mwc.notify.error = function(message, title) {
        mwc.log.warn("mwc.notify.error is not implemented!");
    };

    /* 弹窗消息 **************************************************/
    //定义弹窗消息接口，需要自定义实现

    mwc.message = mwc.message || {};
    mwc.message.info = function(message, title) {
        mwc.log.warn("mwc.message.info is not implemented!");
    };

    mwc.message.success = function(message, title) {
        mwc.log.warn("mwc.message.success is not implemented!");
    };

    mwc.message.warn = function(message, title) {
        mwc.log.warn("mwc.message.warn is not implemented!");
    };

    mwc.message.error = function(message, title) {
        mwc.log.warn("mwc.message.error is not implemented!");
    };

    mwc.message.confirm = function(message, titleOrCallback, callback) {
        mwc.log.warn("mwc.message.confirm is not implemented!");
    };
    mwc.message.prompt = function(message, inputValue, titleOrCallback, callback) {
        mwc.log.warn("mwc.message.prompt is not implemented!");
    };

    /* 弹出窗口 **************************************************/
    mwc.window = mwc.window || {};
    mwc.window.show = function(title, url, width, height) {
        mwc.log.warn("mwc.window.show is not implemented!");
    };
    //从当前窗口关闭自己
    mwc.window.closeSeft = function() {
        mwc.log.warn("mwc.window.closeSeft is not implemented!");
    };
    mwc.window.closeAll = function() {
        mwc.log.warn("mwc.window.closeAll is not implemented!");
    };

    /* restApi 适用于调用WebApi，定义了GET、DELET、PUT、POST这4中方法*******************************************************/
    mwc.restApi = mwc.restApi || {};
    mwc.restApi.get = function(setting) {
        mwc.log.warn("mwc.restApi.get is not implemented!");
    };
    mwc.restApi.delete = function(setting) {
        mwc.log.warn("mwc.restApi.delete is not implemented!");
    };
    mwc.restApi.put = function(setting) {
        mwc.log.warn("mwc.restApi.put is not implemented!");
    };
    mwc.restApi.post = function(setting) {
        mwc.log.warn("mwc.restApi.post is not implemented!");
    };

    /* UI *******************************************************/

    mwc.ui = mwc.ui || {};

    /* UI 阻塞 */
    //定义UI 阻塞接口，需要自定义实现

    mwc.ui.block = function(elm) {
        mwc.log.warn("mwc.ui.block is not implemented!");
    };

    mwc.ui.unblock = function(elm) {
        mwc.log.warn("mwc.ui.unblock is not implemented!");
    };

    /* UI Busy */
    //定义UI Busy接口，需要自定义实现

    mwc.ui.setBusy = function(elm, optionsOrPromise) {
        mwc.log.warn("mwc.ui.setBusy is not implemented!");
    };

    mwc.ui.clearBusy = function(elm) {
        mwc.log.warn("mwc.ui.clearBusy is not implemented!");
    };

    /* EVENT BUS *****************************************/

    mwc.event = (function() {

        var _callbacks = {};

        var on = function(eventName, callback) {
            if (!_callbacks[eventName]) {
                _callbacks[eventName] = [];
            }

            _callbacks[eventName].push(callback);
        };

        var trigger = function(eventName) {
            var callbacks = _callbacks[eventName];
            if (!callbacks || !callbacks.length) {
                return;
            }

            var args = Array.prototype.slice.call(arguments, 1);
            for (var i = 0; i < callbacks.length; i++) {
                callbacks[i].apply(this, args);
            }
        };

        // 公共接口 ///////////////////////////////////////////////////

        return {
            on: on,
            trigger: trigger
        };
    })();


    /* 通用方法 ***************************************************/

    mwc.utils = mwc.utils || {};

    /* 创建命名空间别名
    *  例如：:
    *  var logger = mwc.utils.createNamespace(mwc, 'log');
    *  logger就等同于nwc.log
    *  第一个参数是必须已定义的变量
    ************************************************************/
    mwc.utils.createNamespace = function(root, ns) {
        var parts = ns.split(".");
        for (var i = 0; i < parts.length; i++) {
            if (typeof root[parts[i]] == "undefined") {
                root[parts[i]] = {};
            }

            root = root[parts[i]];
        }

        return root;
    };

    /* 类似于C#中的 String.Format
    *  例如：
    *  var str = mwc.utils.formatString('Hello {0}!', 'World'); //str = 'Hello World!'
    ************************************************************/
    mwc.utils.formatString = function() {
        if (arguments.length < 1) {
            return null;
        }

        var str = arguments[0];

        for (var i = 1; i < arguments.length; i++) {
            var placeHolder = "{" + (i - 1) + "}";
            str = str.replace(placeHolder, arguments[i]);
        }

        return str;
    };

})();