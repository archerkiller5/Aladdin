var mwc = mwc || {};
mwc.bs = mwc.bs || {};
(function () {
    mwc.bs.init = function (setting) {
        var defaultSetting = {
            //是否启用ICheck
            iCheck: true,
            //是否启用选择所有
            checkAll: true,
            //是否启用模式框链接触发
            modalClick: true,
            //是否启用批量操作
            batchOperation: true,
            //是否启用loadingButton
            loadingButton: false,
            //初始化所以的tr下的checkbox
            initAllTrCheck:false
        };
        var options = $.extend(defaultSetting, setting);
        if (options.iCheck) {
            $("input[type=checkbox]")
                .iCheck({
                    checkboxClass: "icheckbox_square-green",
                    radioClass: "iradio_square-green"
                });
        }
        if (options.initAllTrCheck) {
            $("input[type=checkbox]").each(function () {
                var $cb = $(this);
                if ($cb.closest('div.checkbox').length == 0) {
                    var $td = $cb.closest('td');
                    if ($td.length > 0) {
                        var $cbDiv = $('<div class="checkbox checkbox-primary" style="margin-top:0;margin-bottom:0;"><label></label></div>');
                        $td.append($cbDiv);
                        $cb.prependTo($cbDiv);
                    }
                }
            });
        }
        if (options.checkAll) {
            $("#chkCheckAll")
                .on("ifToggled",
                    function (e) {
                        $("input[type=checkbox][name=ids]").iCheck(e.currentTarget.checked ? "check" : "uncheck");
                    })
                .on('change', function (e) {
                    $("input[type=checkbox][name=ids]").prop('checked', e.currentTarget.checked);
                });
        }
        if (options.modalClick) {
            //将clicktype='modal'属性的链接均用弹窗打开
            $("a[data-clicktype='modal']")
                .on("click",
                    function () {
                        mwc.window.show($(this).data('title'), $(this).data('url'), $(this).data("width"), $(this).data("height"));
                    });
        }
        if (options.batchOperation) {
            //批量操作
            $("#toolBar button[data-action], a[data-action]")
                .on("click",
                    function () {
                        var action = $(this).data("action") || "BatchOperation";
                        mwc.bs.batchOperation(action,
                            $(this).data("comfirmmessage"),
                            $(this).data("param"),
                            $(this).data("reloadparent"),
                            $(this).data("controller"));
                    });
        }
        if (options.loadingButton) {
            var $btns = $("button.loadingButton, a.loadingButton").ladda();
            if ($btns.ladda) {
                $btns.on("click",
                    function () {
                        var $btn = $(this);
                        $btn.ladda("start");
                        var ajaxUrl = $btn.data("ajaxurl");
                        mwc.restApi.get({
                            url: ajaxUrl,
                            isBlockUI: false,
                            success: function () {
                                $btn.ladda("stop");

                                if ($btn.data('notify'))
                                    mwc.notify.info($btn.data('notify'));

                                if ($btn.data("reload") === true) {
                                    setTimeout(function () { window.location.reload(); }, 800);
                                }
                            },
                            error: function () {
                                $btn.ladda("stop");
                            }
                        });

                    });
            } else {
                console.error("请在页面添加LoadingButton的相关脚本资源！");
            }
        }
    };
    mwc.bs.postBatchOperation = function (operation, param, reloadParent, controller) {
        var ids = new Array();
        $.each($("input[type=checkbox][name=ids]:checked").serializeArray(),
            function (i, v) {
                ids.push(v.value);
            });
        var apiData = {};
        //全局
        if ($.isFunction(mwc.bs.batchOperationInitParams)) {
            apiData = mwc.bs.batchOperationInitParams();
        }
        if (mwc.bs[param] && $.isFunction(mwc.bs[param])) {
            var data = mwc.bs[param]();
            for (var name in data) {
                apiData[name] = data[name];
            }
        } else {
            if (param)
                apiData.param = param;
        }
        //设置ids
        apiData.ids = ids;
        var url = controller
            ? (controller + "/BatchOperation/" + operation)
            : (location.pathname + "/BatchOperation/" + operation);
        console.debug(url);
        mwc.restApi.post({
            url: url,
            contentType: "application/x-www-form-urlencoded",
            data: apiData,
            success: function (data) {
                if (reloadParent)
                    parent.location.reload();
                else
                    location.reload();
            }
        });
    };
    mwc.bs.batchOperation = function (operation, comfirmMessage, param, reloadParent, controller) {
        var $checkInputs = $("input[type=checkbox][name=ids]:checked");
        if ($checkInputs.length === 0) {
            mwc.message.warn("请至少选择一项！");
            return;
        }
        if (comfirmMessage) {
            mwc.message.confirm("",
                comfirmMessage,
                function (isConfirmed) {
                    isConfirmed && mwc.bs.postBatchOperation(operation, param, reloadParent, controller);
                });
        } else {
            mwc.bs.postBatchOperation(operation, param, reloadParent);
        }
    };
    mwc.bs.initCheckedChange = function () {
        function setStatus() {
            $("input[type=checkbox][data-target]")
                .each(function () {
                    var $id = $(this).data("target");
                    if ($id) {
                        var checked = $(this).get(0).checked;
                        checked ? $($id).slideDown() : $($id).slideUp();
                    }
                });
        }

        setStatus();
        $("input[type=checkbox][data-target]")
            .change(function () {
                setStatus();
            });
    };
    mwc.bs.initFormControls = function () {
        //数值框
        {
            var $inputs = $("input[data-type=number]");
            if ($inputs.length > 0) {
                $inputs.each(function () {
                    if ($(this).TouchSpin) {
                        var settings = {
                            buttondown_class: "btn btn-white",
                            buttonup_class: "btn btn-white"
                        };
                        var min = $(this).data("val-range-min") || $(this).data("min");
                        if (min)
                            settings.min = min;
                        var max = $(this).data("val-range-max") || $(this).data("max");
                        if (max)
                            settings.max = max;
                        $(this).TouchSpin(settings);
                    } else {
                        console.error("请在页面上引用TouchSpin脚本。");
                    }
                });
            }
        }
        //百分比
        {
            var $inputs = $("input[data-type=percent]");
            if ($inputs.length > 0) {
                $inputs.each(function () {
                    var settings = {
                        step: 0.1,
                        decimals: 2,
                        boostat: 5,
                        maxboostedstep: 10,
                        postfix: "%",
                        buttondown_class: "btn btn-white",
                        buttonup_class: "btn btn-white"
                    };
                    var min = $(this).data("val-range-min") || $(this).data("min");
                    if (min)
                        settings.min = min;
                    var max = $(this).data("val-range-max") || $(this).data("max");
                    if (max)
                        settings.max = max;
                    $(this).TouchSpin(settings);
                });
            }
        }
        //百分比
        {
            var $inputs = $("input[data-type=switch]");
            if ($inputs.length > 0) {
                $inputs.each(function () {
                    var switchery = new Switchery($(this).get(0), { color: "#1AB394" });
                });
            }
        }
        //日期
        {
            var $inputs = $("input[type=date], input[data-type=date]");
            if ($inputs.length > 0) {
                $inputs.each(function () {
                    $(this)
                        .datepicker({
                            todayBtn: "linked",
                            //keyboardNavigation: false,
                            forceParse: false,
                            calendarWeeks: true,
                            autoclose: true,
                            todayHighlight: true,
                            language: "zh-CN"
                        });
                });
            }
        }
        //日期和时间
        {
            var $inputs = $("input[type=datetime], input[data-type=datetime]");
            if ($inputs.length > 0) {
                $inputs.each(function () {
                    $(this)
                        .datepicker({
                            todayBtn: "linked",
                            //keyboardNavigation: false,
                            forceParse: false,
                            calendarWeeks: true,
                            autoclose: true,
                            todayHighlight: true,
                            language: "zh-CN"
                        });
                });
            }
        }
    };
})();