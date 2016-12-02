var wc = wc || {};
/* 通知 *********************************************/
//定义通知接口，需要自定义实现
wc.notify = wc.notify || {};
wc.notify._show = function(icon, msg, duration, extraclass) {
    wc.ui.block();
    var $toast = $('<div class="aui-toast ' +
            (extraclass || "") +
            '" style="z-index:1000;"><i class="aui-iconfont ' +
            (icon || "aui-icon-check") +
            '"></i><div class="aui-toast-content">' +
            (msg || "操作成功！") +
            "</div></div>")
        .appendTo(document.body);
    setTimeout(function() {
            wc.ui.unblock();
            $toast.remove();
        },
        duration || 2000);
};
wc.notify.success = function(msg, duration) {
    wc.notify._show("aui-icon-check", (msg || "操作成功！"), (duration || 2000));
};
wc.notify.info = function(msg, duration) {
    wc.notify._show("aui-icon-info", (msg), (duration || 2000));
};
wc.notify.warn = function(msg, duration) {
    wc.notify._show("aui-icon-warn", (msg), (duration || 2000));
};
wc.notify.error = function(msg, duration) {
    wc.notify._show("aui-icon-roundclose", (msg), (duration || 2000));
};
/* 弹窗消息 **************************************************/
//定义弹窗消息接口，需要自定义实现
wc.message = wc.message || {};
wc.message._show = function(message, title, buttons) {
    title = title || "温馨提示";
    wc.ui.block();
    var buttonStr = "";
    $.each(buttons,
        function(i, v) {
            buttonStr += '<div class="aui-dialog-btn ' + v.cls + " " + v.type + '" tapmode>' + v.text + "</div>";
        });
    var $message = $('<div class="aui-dialog" id="dialog">' +
            '<div class="aui-dialog-header">' +
            title +
            "</div>" +
            '<div class="aui-dialog-body aui-text-left">' +
            message +
            "</div>" +
            '<div class="aui-dialog-footer">' +
            buttonStr +
            "</div>" +
            "</div>")
        .appendTo(document.body);
    $.each(buttons,
        function(i, v) {
            $message.find("div.aui-dialog-btn." + v.type)
                .on("click",
                    function() {
                        wc.ui.unblock();
                        $message.remove();
                        v.func && v.func();
                    });
        });
};
wc.message.info = function(message, title, func) {
    wc.message._show(message, title, [{ cls: "aui-text-info", type: "ok", text: "确定", func: func }]);
};
wc.message.success = function(message, title) {
    wc.message._show(message, title, [{ cls: "aui-text-success", type: "ok", text: "确定" }]);
};
wc.message.warn = function(message, title) {
    wc.message._show(message, title, [{ cls: "aui-text-warn", type: "ok", text: "确定" }]);
};
wc.message.error = function(message, title) {
    wc.message._show(message, title, [{ cls: "aui-text-error", type: "ok", text: "确定" }]);
};
wc.message.confirm = function(message, title, callback) {
    wc.message._show(message,
        title,
        [
            { cls: "aui-text-error", type: "ok", text: "取消" },
            { cls: "aui-text-info", type: "confirm", text: "确定", func: callback }
        ]);
};
/* restApi 适用于调用WebApi，定义了GET、DELET、PUT、POST这4中方法*******************************************************/
wc.restApi = wc.restApi || {};
/* wcAjaxApiHelper *******************************************************/
var wcAjaxApiHelper = {
    blockUI: function(options) {
        wc.ui.setBusy();
    },
    unblockUI: function(options) {
        wc.ui.clearBusy();
    },
    handleApiData: function(data, userOptions, $dfd, statusCode, type) {
        var messagePromise = null;
        switch (type) {
        case "success":
        {
            function success() {
                if (data) {
                    $dfd && $dfd.resolve(data.result !== undefined ? data.result : data, data);
                    userOptions.success && userOptions.success(data.result !== undefined ? data.result : data, data);
                } else {
                    $dfd && $dfd.resolve();
                    userOptions.success && userOptions.success();
                }
            }

            if (data && (data.message || data.Message)) {
                var message = data.message || data.Message;
                messagePromise = data.success !== undefined && data.success === true
                    ? wc.message.success(message)
                    : wc.message.warn(message);
            }
            if (messagePromise) {
                messagePromise.done(function() {
                    success();
                    if (data.targetUrl)
                        location.href = data.targetUrl;
                });
            } else {
                success();
                if (data && data.targetUrl)
                    location.href = data.targetUrl;
            }
            break;
        }
        case "error":
        {
            if (data.error) {
                messagePromise = data.error.details
                    ? wc.message.error(data.error.details.replace(/\r\n/gi, "<br />"), data.error.message)
                    : wc.message.error(data.error.message.replace(/\r\n/gi, "<br />"), "错误");
            } else if (data.ModelState) {
                var detail = "";
                for (var p in data.ModelState) {
                    detail += data.ModelState[p] + "\n";
                }
                messagePromise = wc.message.error(detail, data.Message);
            } else if (data.Message || data.message) {
                var message = data.Message || data.message;
                messagePromise = wc.message.error(message.replace(/\r\n/gi, "<br />"), "错误");
            }
            $dfd && $dfd.reject(data.error);
            userOptions.error && userOptions.error(data.error);
        }
        default:
        {
            break;
        }

        }
    }
};
wc.restApi.defaultOpts = {
    cache: false,
    accepts: "application/json",
    dataType: "json",
    contentType: "application/json; charset=utf-8"
};
wc.restApi.ajax = function(userOptions) {
    userOptions = userOptions || {};
    return $.Deferred(function($dfd) {
        var options = $.extend({}, wc.restApi.defaultOpts, userOptions);
        options.success = undefined;
        options.error = undefined;
        //阻碍UI
        userOptions.isBlockUI !== false && wcAjaxApiHelper.blockUI(userOptions);
        $.ajax(options)
            .done(function(data, status, xhr) {
                console.debug(data, status, xhr);
                switch (xhr.status) {
                    //创建成功
                case 201:
                //成功响应
                case 200:
                {
                    wcAjaxApiHelper.handleApiData(data, userOptions, $dfd, xhr.status, "success");
                    break;
                }
                //需要验证
                case 401:
                {
                    var data = { targetUrl: "/Account/Login", error: { message: "登陆失效，请重新登陆！" } };
                    wcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 401, "error", data);
                    break;
                }
                //不存在
                case 404:
                {
                    var data = { error: { message: "您访问的资源已被删除或不存在！" } };
                    wcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 404, "error", data);
                    break;
                }

                }
                console.debug(data, status, xhr);
            })
            .fail(function() {
                var xhr = arguments[0];
                switch (xhr.status) {
					                    //创建成功
                case 201:
                //成功响应
                case 200:
                {
                    wcAjaxApiHelper.handleApiData(null, userOptions, $dfd, xhr.status, "success");
                    break;
                }
                    //请求失败
                case 400:
                {
                    var data = $.parseJSON(xhr.responseText);
                    wcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 400, "error", data);
                    break;
                }
                //服务器错误
                case 500:
                {
                    try {
                        var data = $.parseJSON(xhr.responseText);
                        wcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 500, "error", data);
                    } catch (e) {
                        wc.message.error("服务器错误，请重试或联系管理员！");
                    }
                }
                }
            })
            .always(function() {
                //取消阻碍UI
                userOptions.isBlockUI !== false && wcAjaxApiHelper.unblockUI(userOptions);
            });
    });
};
//Demo:wc.restApi.get({ url: '/api/checkwork', success: function (data) { console.debug(data); } })
wc.restApi.get = function(userOptions) {
    userOptions = userOptions || {};
    userOptions.type = "GET";
    wc.restApi.ajax(userOptions);
};
wc.restApi.delete = function(userOptions) {
    userOptions = userOptions || {};
    userOptions.type = "DELETE";
    if (typeof (userOptions.data) == "object")
        userOptions.data = JSON.stringify(userOptions.data);
    wc.restApi.ajax(userOptions);
};
wc.restApi.put = function(userOptions) {
    userOptions = userOptions || {};
    userOptions.type = "PUT";
    if (typeof (userOptions.data) == "object")
        userOptions.data = JSON.stringify(userOptions.data);
    wc.restApi.ajax(userOptions);
};
wc.restApi.post = function(userOptions) {
    userOptions = userOptions || {};
    userOptions.type = "POST";
    if (typeof (userOptions.data) == "object" && typeof (userOptions.contentType) === "undefined") {
        userOptions.contentType = "application/x-www-form-urlencoded; charset=UTF-8";
    }
    wc.restApi.ajax(userOptions);
};
/* UI *******************************************************/
wc.ui = wc.ui || {};
/* UI 阻塞 */
//定义UI 阻塞接口，需要自定义实现
wc.ui.block = function() {
    $('<div class="aui-mask"></div>').appendTo(document.body);
};
wc.ui.unblock = function(elm) {
    $("div.aui-mask").remove();
};
/* UI Busy */
//定义UI Busy接口，需要自定义实现
wc.ui.setBusy = function(msg) {
    $('<div class="aui-toast" style="z-index:1000;"><div class="aui-toast-loading"></div><div class="aui-toast-content">' + (msg || "加载中") + "</div></div>").appendTo(document.body);
};
wc.ui.clearBusy = function() {
    $("div.aui-toast").remove();
};