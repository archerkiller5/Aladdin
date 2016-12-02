var wc = wc || {};
//微信对象封装
(function() {
    if (typeof ($) === "undefined") {
        console.error("请引用zepto.js。");
        return;
    }
    wc.weiChat = wc.weiChat || {};
    var self = wc.weiChat;
    //检查wx对象
    if (typeof (wx) == "undefined") {
        console.error("没有定义wx，可能是没有引用JSSDK脚本。");
        return;
    }
    //确保微信配置初始化完成后调用
    self.ready = function(callback) {
        wx.ready(function() {
            callback && callback();
        });
    };


    //配置相关
    self.config = {};
    //JSSDK列表
    self.config.jsApiList = [
        "checkJsApi",
        "onMenuShareTimeline",
        "onMenuShareAppMessage",
        "onMenuShareQQ",
        "onMenuShareWeibo",
        "onMenuShareQZone",
        "hideMenuItems",
        "showMenuItems",
        "hideAllNonBaseMenuItem",
        "showAllNonBaseMenuItem",
        "translateVoice",
        "startRecord",
        "stopRecord",
        "onVoiceRecordEnd",
        "playVoice",
        "onVoicePlayEnd",
        "pauseVoice",
        "stopVoice",
        "uploadVoice",
        "downloadVoice",
        "chooseImage",
        "previewImage",
        "uploadImage",
        "downloadImage",
        "getNetworkType",
        "openLocation",
        "getLocation",
        "hideOptionMenu",
        "showOptionMenu",
        "closeWindow",
        "scanQRCode",
        "chooseWXPay",
        "openProductSpecificView",
        "addCard",
        "chooseCard",
        "openCard"
    ];
    //初始化配置
    self.init = function(config) {
        self.config = $.extend(self.config, config);
        wx.config(self.config);
    };

    //是否为微信请求
    self.isWechat = function() {
        return /micromessenger/.test(navigator.userAgent.toLowerCase());
    };

    //是否WiFi
    self.isWifi = function(func) {
        wx.getNetworkType({
            success: function(res) {
                var isWifi = res.networkType == "wifi"; // 返回网络类型2g，3g，4g，wifi
                func && func(isWifi);
            }
        });
    };

    //上传处理
    self.uploader = {};
    self.uploader.upload = function(setting, type) {
        console.debug("self.uploader.upload", setting, type);
        //解决IOS无法上传的坑
        setting.localId = setting.localId.indexOf("wxlocalresource") != -1
            ? setting.localId.replace("wxlocalresource", "wxLocalResource")
            : setting.localId;
        var options = $.extend(
            {
                //localId: localId, // 需要上传的图片的本地ID，由chooseImage接口获得。此参数是必须的
                isShowProgressTips: 1, // 默认为1，显示进度提示
                fail: function(res) {
                    wc.notify.error("上传图片失败，请检查网络或稍后再试！");
                    console.warn(res);
                }
            },
            setting);
        console.debug("preUpload", setting, type);
        switch (type) {
        case "img":
            wx.uploadImage(options);
            break;
        case "voice":
            wx.uploadVoice(options);
            break;
        }

    };
    self.uploader.imgs = [];
    self.uploader.voices = [];
    self.uploader.serverIds = { imgs: [], voices: [] };
    //批量上传
    self.uploader.uploads = function(setting, localIds, type) {

        setting = setting || {};
        console.debug("uploads", setting, localIds, type);

        function uploadImages(localImagesIds) {
            if (localImagesIds.length === 0) {
                console.debug(type + "上传完成");
                setting.allSuccess && setting.allSuccess();
            }
            var localId = localImagesIds[0];
            //解决IOS无法上传的坑
            if (localId.indexOf("wxlocalresource") != -1) {
                localId = localId.replace("wxlocalresource", "wxLocalResource");
            }
            wx.uploadImage({
                localId: localId, // 需要上传的图片的本地ID，由chooseImage接口获得
                isShowProgressTips: 1, // 默认为1，显示进度提示
                success: function(res) {
                    type == "img"
                        ? self.uploader.serverIds.imgs.push(res.serverId)
                        : self.uploader.serverIds.voices.push(res.serverId);
                    localImagesIds.shift();
                    uploadImages(localImagesIds);
                },
                fail: function(res) {
                    console.error("上传失败！", res);
                }
            });
        }


        //如果没有传递localids，则自动上传缓存的本地id
        if (typeof (localIds) === "undefined") {
            console.error("请设置localIds！");
            return;
        } else {
            uploadImages(localIds);
            //upload(localIds, setting, type);
        }
        //var serverIds = [];


        //function upload(ids, setting, type) {
        //    console.debug('upload', ids, setting, type);
        //    if (ids.length == 0) {
        //        console.error('参数localIds不能为空！');
        //        return;
        //    }
        //    var _success = setting.success;
        //    var uploadSetting = setting;
        //    uploadSetting.localId = ids[0];
        //    uploadSetting.success = function (res) {
        //        console.debug('uploadSetting.success', res, type);
        //        //添加ServerId
        //        type == 'img' ? self.uploader.serverIds.imgs.push(res.serverId) : self.uploader.serverIds.voices.push(res.serverId);
        //        serverIds.push(res.serverId);
        //        ids.shift();
        //        _success && _success(res);
        //        //全部上传完毕，执行回调函数，返回serverId列表
        //        if (ids.length === 0) {
        //            console.debug(type + '上传完成');
        //            setting.allSuccess && setting.allSuccess(serverIds);
        //            return;
        //        } else
        //            upload(ids, setting, type);
        //    };
        //    self.uploader.upload(uploadSetting, type || 'img');
        ////}

    };

    //图片相关
    self.image = {};
    self.image.choose = function(setting) {
        setting = setting || {};
        setting._success = setting.success;

        setting.success = function(res) {
            for (var i = 0; i < res.localIds.length; i++) {
                self.uploader.imgs.push(res.localIds[i]);
            }
            setting._success && setting._success(res.localIds);
        };
        var options = $.extend(
            {
                count: 9, // 默认9
                sizeType: ["original", "compressed"], // 可以指定是原图还是压缩图，默认二者都有
                sourceType: ["album", "camera"], // 可以指定来源是相册还是相机，默认二者都有
                fail: function(res) {
                    wc.notify.error("选择图片失败，请检查网络或稍后再试！");
                    console.warn(res);
                }
            },
            setting);
        wx.chooseImage(options);
    };
    self.image.preview = function(current, urls) {
        wx.previewImage({
            current: current,
            urls: urls
        });
    };

    //音频相关
    self.voice = {};
    //监听录音自动停止接口。录音时间超过一分钟没有停止的时候会执行 complete 回调
    self.voice.onRecordEnd = function(callback) {
        wx.onVoiceRecordEnd({
            complete: function(res) {
                self.uploader.voices.push(res.localId);
                callback && callback(res);
            }
        });
    };
    //监听语音播放完毕接口
    self.voice.onPlayEnd = function(success) {
        wx.onVoicePlayEnd({
            success: function(res) {
                success && success(res);
            }
        });
    };
    //开始录音
    self.voice.startRecord = function() {
        wx.startRecord();
    };
    //停止录音接口
    self.voice.stopRecord = function(success) {
        wx.stopRecord({
            success: function(res) {
                self.uploader.voices.push(res.localId);
                success && success(res);
            }
        });
    };
    //播放
    self.voice.play = function(localId) {
        wx.playVoice({
            localId: localId
        });
    };
    //暂停
    self.voice.pause = function(localId) {
        wx.pauseVoice({
            localId: localId
        });
    };
    //停止
    self.voice.stop = function(localId) {
        wx.stopVoice({
            localId: localId
        });
    };

    //位置相关
    self.location = {};
    //使用微信内置地图查看位置接口
    //{
    //    latitude: 0, // 纬度，浮点数，范围为90 ~ -90
    //    longitude: 0, // 经度，浮点数，范围为180 ~ -180。
    //    name: '', // 位置名
    //    address: '', // 地址详情说明
    //    scale: 1, // 地图缩放级别,整形值,范围从1~28。默认为最大
    //    infoUrl: '' // 在查看位置界面底部显示的超链接,可点击跳转
    //}
    self.location.open = function(setting) {
        wx.openLocation(setting);
    };
    //获取地理位置接口
    //{
    //    type: 'wgs84', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
    //    success: function (res) {
    //        var latitude = res.latitude; // 纬度，浮点数，范围为90 ~ -90
    //        var longitude = res.longitude; // 经度，浮点数，范围为180 ~ -180。
    //        var speed = res.speed; // 速度，以米/每秒计
    //        var accuracy = res.accuracy; // 位置精度
    //    }
    //}
    self.location.get = function(setting) {
        wx.getLocation(setting);
    };

    //window相关
    self.window = {};
    //关闭当前网页窗口接口
    self.window.close = function() {
        wx.closeWindow();
    };

    //二维码相关
    self.qrcode = {};
    //扫描。可以由微信处理，也可以自行处理
    self.qrcode.scan = function(scanType, success) {
        //可以指定扫二维码还是一维码，默认二者都有
        scanType = scanType || ["qrCode", "barCode"];
        if (typeof (success) !== "undefined") {
            wx.scanQRCode({
                needResult: 1, // 默认为0，扫描结果由微信处理，1则直接返回扫描结果，
                scanType: scanType,
                success: function(res) {
                    //当needResult 为 1 时，扫码返回的结果
                    success && success(res.resultStr);
                }
            });
        } else {
            wx.scanQRCode({
                needResult: 0, // 默认为0，扫描结果由微信处理，1则直接返回扫描结果，
                scanType: scanType
            });
        }
    };

})();