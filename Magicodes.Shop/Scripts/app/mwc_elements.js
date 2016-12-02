var mwc = mwc || {};
(function() {
    if (!toastr) {
        return;
    }
    /* 设置默认值 *************************************************/

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "1000",
        "timeOut": "7000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    /* 通知 *********************************************/

    var showNotification = function(type, message, title) {
        toastr[type](message, title);
    };

    mwc.notify.success = function(message, title) {
        showNotification("success", message, title);
    };

    mwc.notify.info = function(message, title) {
        showNotification("info", message, title);
    };

    mwc.notify.warn = function(message, title) {
        showNotification("warning", message, title);
    };

    mwc.notify.error = function(message, title) {
        showNotification("error", message, title);
    };

})();

(function() {
    if (!$.blockUI) {
        return;
    }

    $.extend($.blockUI.defaults,
    {
        message: " ",
        css: {},
        overlayCSS: {
            backgroundColor: "#AAA",
            opacity: 0.3,
            cursor: "wait"
        }
    });

    mwc.ui.block = function(elm) {
        if (!elm) {
            $.blockUI();
        } else {
            $(elm).block();
        }
    };

    mwc.ui.unblock = function(elm) {
        if (!elm) {
            $.unblockUI();
        } else {
            $(elm).unblock();
        }
    };

})();

(function($) {
    if (!sweetAlert || !$) {
        return;
    }

    /* 设置默认配置 *************************************************/

    mwc.libs = mwc.libs || {};
    mwc.libs.sweetAlert = {
        config: {
            'default': {
                confirmButtonText: "确定",
                cancelButtonText: "取消",
            },
            info: {
                type: "info"
            },
            success: {
                type: "success"
            },
            warn: {
                type: "warning"
            },
            error: {
                type: "error"
            },
            confirm: {
                type: "warning",
                title: "确定进行以下操作么？",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55"
            }
        }
    };

    /* 弹出消息 **************************************************/

    var showMessage = function(type, message, title) {
        if (!title) {
            title = message;
            message = undefined;
        }

        var opts = $.extend(
            {},
            mwc.libs.sweetAlert.config.default,
            mwc.libs.sweetAlert.config[type],
            {
                title: title,
                text: message,
                html: true
            }
        );

        return $.Deferred(function($dfd) {
            sweetAlert(opts,
                function() {
                    $dfd.resolve();
                });
        });
    };

    mwc.message.info = function(message, title) {
        return showMessage("info", message, title);
    };

    mwc.message.success = function(message, title) {
        return showMessage("success", message, title);
    };

    mwc.message.warn = function(message, title) {
        return showMessage("warn", message, title);
    };

    mwc.message.error = function(message, title) {
        return showMessage("error", message, title);
    };

    mwc.message.confirm = function(message, titleOrCallback, callback) {
        var userOpts = {
            text: message,
            showLoaderOnConfirm: true,
            closeOnConfirm: false
        };

        if ($.isFunction(titleOrCallback)) {
            callback = titleOrCallback;
        } else if (titleOrCallback) {
            userOpts.title = titleOrCallback;
        };

        var opts = $.extend(
            {},
            mwc.libs.sweetAlert.config.default,
            mwc.libs.sweetAlert.config.confirm,
            userOpts
        );

        return $.Deferred(function($dfd) {
            sweetAlert(opts,
                function(isConfirmed) {
                    callback && callback(isConfirmed);
                    $dfd.resolve();
                });
        });
    };

    mwc.message.prompt = function(message, inputValue, titleOrCallback, callback) {
        var userOpts = {
            text: message,
            type: "input",
            animation: "slide-from-top",
            showLoaderOnConfirm: true,
            closeOnConfirm: false,
            inputValue: inputValue
        };

        if ($.isFunction(titleOrCallback)) {
            callback = titleOrCallback;
        } else if (titleOrCallback) {
            userOpts.title = titleOrCallback;
            userOpts.inputPlaceholder = titleOrCallback;
        };

        var opts = $.extend(
            {},
            mwc.libs.sweetAlert.config.default,
            mwc.libs.sweetAlert.config.confirm,
            userOpts
        );

        return $.Deferred(function($dfd) {
            sweetAlert(opts,
                function(inputValue) {
                    if (inputValue === false) return false;
                    if (inputValue === "") {
                        sweetAlert.showInputError("请输入内容！");
                        return false
                    }
                    callback && callback(inputValue);
                    $dfd.resolve();
                });
        });
    };
})(jQuery);

(function() {
    if (!layer) {
        mwc.log.warn("layer is not implemented!");
        return;
    }
    mwc.window.show = function(title, url, width, height, fullScreen) {
        var _width = width || 1000;
        var _height = height || 600;
        //判断是否存在父级弹窗
        if (parent.layer && parent.layer.getFrameIndex(window.name)) {
            _width = _width - 100;
            _height = _height - 50;
        }
        fullScreen = fullScreen || false;
        var index = layer.open({
            type: 2,
            //skin: 'layui-layer-lan',
            title: title,
            fix: false,
            shadeClose: true,
            maxmin: true,
            area: [_width + "px", _height + "px"],
            content: url
        });
        if (fullScreen) {
            layer.full(index);
        }
        if (typeof (width) === "undefined") {
            layer.iframeAuto(index);
        }
    };
    mwc.window.closeSeft = function() {
        //当你在iframe页面关闭自身时
        var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
        parent.layer.close(index); //再执行关闭    
    };
    mwc.window.closeAll = function() {
        layer.closeAll();
    };
})();

(function() {

    if (!$.fn.spin) {
        return;
    }

    mwc.libs = mwc.libs || {};

    mwc.libs.spinjs = {
        spinner_config: {
            lines: 11,
            length: 0,
            width: 10,
            radius: 20,
            corners: 1.0,
            trail: 60,
            speed: 1.2
        },

        //Config for busy indicator in element's inner element that has '.mwc-busy-indicator' class.
        spinner_config_inner_busy_indicator: {
            lines: 11,
            length: 0,
            width: 4,
            radius: 7,
            corners: 1.0,
            trail: 60,
            speed: 1.2
        }

    };

    mwc.ui.setBusy = function(elm, optionsOrPromise) {
        optionsOrPromise = optionsOrPromise || {};
        if (optionsOrPromise.always || optionsOrPromise["finally"]) { //Check if it's promise
            optionsOrPromise = {
                promise: optionsOrPromise
            };
        }

        var options = $.extend({}, optionsOrPromise);

        if (!elm) {
            if (options.blockUI != false) {
                mwc.ui.block();
            }

            $("body").spin(mwc.libs.spinjs.spinner_config);
        } else {
            var $elm = $(elm);
            var $busyIndicator = $elm
                .find(".mwc-busy-indicator");
//TODO@Halil: What if  more than one element. What if there are nested elements?
            if ($busyIndicator.length) {
                $busyIndicator.spin(mwc.libs.spinjs.spinner_config_inner_busy_indicator);
            } else {
                if (options.blockUI != false) {
                    mwc.ui.block(elm);
                }

                $elm.spin(mwc.libs.spinjs.spinner_config);
            }
        }

        if (options.promise) { //Supports Q and jQuery.Deferred
            if (options.promise.always) {
                options.promise.always(function() {
                    mwc.ui.clearBusy(elm);
                });
            } else if (options.promise["finally"]) {
                options.promise["finally"](function() {
                    mwc.ui.clearBusy(elm);
                });
            }
        }
    };

    mwc.ui.clearBusy = function(elm) {
        if (!elm) {
            mwc.ui.unblock();
            $("body").spin(false);
        } else {
            var $elm = $(elm);
            var $busyIndicator = $elm.find(".mwc-busy-indicator");
            if ($busyIndicator.length) {
                $busyIndicator.spin(false);
            } else {
                mwc.ui.unblock(elm);
                $elm.spin(false);
            }
        }
    };

})();
/* restApi 适用于调用WebApi，定义了GET、DELETE、PUT、POST这4中方法*******************************************************/
(function($) {

    if (!$) {
        return;
    }
    /* mwcAjaxApiHelper *******************************************************/
    var mwcAjaxApiHelper = {
        blockUI: function(options) {
            options.blockUI === !0 ? mwc.ui.setBusy() : mwc.ui.setBusy(options.blockUI);
        },
        unblockUI: function(options) {
            options.blockUI === !0 ? mwc.ui.clearBusy() : mwc.ui.clearBusy(options.blockUI);
        },
        handleApiData: function(data, userOptions, $dfd, statusCode, type) {
            var messagePromise = null;
            switch (type) {
            case "success":
            {
                function success() {
                    if (data) {
                        $dfd && $dfd.resolve(data.result !== undefined ? data.result : data, data);
                        userOptions
                            .success &&
                            userOptions.success(data.result !== undefined ? data.result : data, data);
                    } else {
                        $dfd && $dfd.resolve();
                        userOptions.success && userOptions.success();
                    }
                }

                if (data && (data.message || data.Message)) {
                    var message = data.message || data.Message;
                    messagePromise = (data.success !== undefined && data.success === true ||
                            data.Success !== undefined && data.Success === true)
                        ? mwc.message.success(message)
                        : mwc.message.error(message);
                }
                if (messagePromise) {
                    messagePromise.done(function() {
                        success();
                        if (data.targetUrl || data.TargetUrl)
                            location.href = data.targetUrl || data.TargetUrl;
                    });
                } else {
                    success();
                    if (data.targetUrl || data.TargetUrl)
                        location.href = data.targetUrl || data.TargetUrl;
                }
                break;
            }
            case "error":
            {
                if ($.isFunction(userOptions.error)) {
                    userOptions.error(data);
                } else {
                    //取消阻碍UI
                    mwcAjaxApiHelper.unblockUI(userOptions);
                    if (data.error) {
                        messagePromise = data.error.details
                            ? mwc.message.error(data.error.details.replace(/\r\n/gi, "<br />"), data.error.message)
                            : mwc.message.error(data.error.message.replace(/\r\n/gi, "<br />"));
                        $dfd && $dfd.reject(data.error);
                    } else if (data.ModelState) {
                        var detail = "";
                        for (var p in data.ModelState) {
                            detail += data.ModelState[p] + "\n";
                        }
                        messagePromise = mwc.message.error(detail, data.Message);
                    } else if (data.Message || data.message) {
                        var message = data.Message || data.message;
                        messagePromise = mwc.message.error(message.replace(/\r\n/gi, "<br />"));
                    }

                    if (data.targetUrl || data.TargetUrl) {
                        if (messagePromise) {
                            messagePromise.done(function() {
                                location.href = data.targetUrl || data.TargetUrl;
                            });
                        } else {
                            location.href = data.targetUrl || data.TargetUrl;
                        }
                    }
                }
            }
            default:
            {
                break;
            }

            }
        }
    };

    mwc.restApi.defaultOpts = {
        cache: false,
        accepts: "application/json",
        dataType: "json",
        contentType: "application/json; charset=utf-8"
    };

    mwc.restApi.ajax = function(userOptions) {
        userOptions = userOptions || {};
        return $.Deferred(function($dfd) {
            mwc.restApi.defaultOpts.statusCode =
            {
                200 /*成功响应*/: function(data) {
                    mwcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 200, "success");
                },
                201 /*创建成功*/: function(data) {
                    mwcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 201, "success");
                },
                401 /*需要验证*/: function(jqXHR, textStatus, errorThrown) {
                    var data = { targetUrl: "/Account/Login", error: { message: "登陆失效，请重新登陆！" } };
                    mwcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 401, "error", data);
                },
                204 /*成功响应，无内容返回*/: function(data) {
                    mwcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 204, "success");
                },
                404 /*不存在*/: function() {
                    var data = { error: { message: "您访问的资源已被删除或不存在！" } };
                    mwcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 404, "error", data);
                },
                400 /*请求失败*/: function(jqxhr) {
                    var data = $.parseJSON(jqxhr.responseText);
                    mwcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 400, "error", data);
                },
                409 /*数据冲突*/: function(jqxhr) {
                    var data = { error: { message: "数据冲突，无法处理您的请求！" } };
                    mwcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 409, "error", data);
                },
                500 /*服务器错误*/: function(jqxhr) {
                    try {
                        var data = $.parseJSON(jqxhr.responseText);
                        mwcAjaxApiHelper.handleApiData(data, userOptions, $dfd, 500, "error", data);
                    } catch (e) {
                        mwc.message.error("服务器错误，请重试或联系管理员！");
                    }
                }
            };
            var options = $.extend({}, mwc.restApi.defaultOpts, userOptions);
            options.success = undefined;
            options.error = undefined;
            //mwc.log.debug(options);
            //阻碍UI
            userOptions.isBlockUI !== false && mwcAjaxApiHelper.blockUI(userOptions);
            $.ajax(options)
                .done(function(data) {

                })
                .fail(function() {
                    $dfd.reject.apply(this, arguments);
                })
                .complete(function() {
                    //取消阻碍UI
                    userOptions.isBlockUI !== false && mwcAjaxApiHelper.unblockUI(userOptions);
                });
        });
    };
    mwc.restApi.get = function(userOptions) {
        userOptions = userOptions || {};
        userOptions.type = "GET";
        mwc.restApi.ajax(userOptions);
    };
    mwc.restApi.delete = function(userOptions) {
        userOptions = userOptions || {};
        userOptions.type = "DELETE";
        if (typeof (userOptions.contentType) == 'undefined' && typeof (userOptions.data) == "object")
            userOptions.data = JSON.stringify(userOptions.data);
        mwc.restApi.ajax(userOptions);
    };
    mwc.restApi.put = function(userOptions) {
        userOptions = userOptions || {};
        userOptions.type = "PUT";
        if (typeof (userOptions.contentType)=='undefined' && typeof (userOptions.data) == "object")
            userOptions.data = JSON.stringify(userOptions.data);
        mwc.restApi.ajax(userOptions);
    };
    mwc.restApi.post = function(userOptions) {
        userOptions = userOptions || {};
        userOptions.type = "POST";
        if ($.isArray(userOptions.data))
            userOptions.data = JSON.stringify(userOptions.data);
        else if (typeof (userOptions.contentType) == 'undefined' && typeof (userOptions.data) == "object" && typeof (userOptions.contentType) === "undefined")
            userOptions.contentType = "application/x-www-form-urlencoded; charset=UTF-8";

        mwc.restApi.ajax(userOptions);
    };
})(jQuery);