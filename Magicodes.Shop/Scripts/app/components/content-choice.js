(function() {
    var current = {};
    var instance = function(params, componentInfo) {
        var self = this;
        this.Name = ko.observable("请选择素材");
        this.Url = ko.observable("/Content/patterns/congruent_pentagon.png");
        this.ContentId = ko.observable("");
        this.MediaId = ko.observable("");
        //默认选择的类型
        this.KeyWordContentType = ko.observable("1");
        //选择列表
        this.ContentTypes = params.contentTypes;
        //是否禁用选择框
        this.DisabledChoice = ko.observable(false);

        this.ShowChoiceWindow = function(data) {
            var type = self.KeyWordContentType().toString();
            var url = "";
            switch (type) {
                //文本
            case "0":
                url = "/WeiChat_KeyWordTextContent?lightLayout=1";
                break;
            //图文
            case "5":
                url = "/WeiChat_KeyWordNewsContent?lightLayout=1";
                break;
            //图片
            case "1":
                url = "/Site_Resources?resourceType=0&lightLayout=1";
                break;
            //语音
            case "3":
                url = "/Site_Resources?resourceType=1&lightLayout=1";
                break;
            //视频
            case "4":
                url = "/Site_Resources?resourceType=2&lightLayout=1";
                break;
            //对接客服
            case "6":
                return;
            }
            mwc.window.show("选择素材", url);
        };
        if (params) {
            if (params.keyWordContentType()) {
                self.KeyWordContentType(params.keyWordContentType());
            }
            //self.ContentTypes = params.contentTypes;
            //if (params.contentTypes().length > 0) {
            //    self.ContentTypes(params.contentTypes());
            //}
            //params.contentTypes.subscribe(function (newValue) {
            //    console.debug(newValue)
            //    self.ContentTypes(newValue);
            //});

            if (params.disabledChoice) {
                self.DisabledChoice(params.disabledChoice);
            }

            if (params.contentId().length > 0) {
                self.ContentId(params.contentId());
                mwc.restApi.get({
                    //请求地址
                    url: "/WeiChat_KeyWordAutoReplay/Data/" + self.KeyWordContentType() + "/" + self.ContentId(),
                    //是否锁定UI
                    isBlockUI: true,
                    //可选，锁定元素
                    blockUI: componentInfo.element,
                    //成功函数
                    success: function(model) {
                        console.debug(model);
                        self.Name(model.Name);
                        self.Url(encodeURI(model.FrontCoverImageUrl || model.PicUrl || model.SiteUrl || model.Url));
                        self.MediaId(model.MediaId ||
                            model.ImageMediaId ||
                            model.ThumbMediaId ||
                            model.VoiceMediaId ||
                            model.Text);
                    }
                });
            }

            //添加订阅
            self.MediaId.subscribe(function(newValue) {
                params.mediaId(newValue);
            });

            self.ContentId.subscribe(function(newValue) {
                params.contentId(newValue);
            });
            self.KeyWordContentType.subscribe(function(newValue) {
                if (newValue != params.keyWordContentType()) {
                    self.Name("请选择素材");
                    self.Url("/Content/patterns/congruent_pentagon.png");
                    self.ContentId("");
                    self.MediaId("");
                }
                params.keyWordContentType(newValue);

            });
        }
        window.choice = function(data) {
            console.debug(data);
            self.MediaId(data.MediaId);
            self.ContentId(data.Id);
            self.Name(data.Name);
            self.Url(data.FrontCoverImageUrl || data.SiteUrl || data.Url);

        };
        //文本选择和图文选择触发
        window.setContent = function(type, contentId, data) {
            self.ContentId(contentId);
            self.MediaId(data);
            self.Url(data);
        };
    };
    var newschoiceViewModelInstance = {
        createViewModel: function(params, componentInfo) {
            current.viewModel = new instance(params, componentInfo);
            return current.viewModel;
        }
    };
    //资源选择
    ko.components.register("content-choice-button",
    {
        viewModel: newschoiceViewModelInstance,
        template: "" +
            '<div class="form-group">' +
            '<label class="col-sm-2 control-label">类型：</label>' +
            '<div class="col-sm-10">' +
            '<select class="form-control" data-bind="options:ContentTypes,optionsText:\'text\',optionsValue:\'value\',value:KeyWordContentType,disable:DisabledChoice" data-val="true" data-val-required="类型 字段是必需的。">' +
            "</select>" +
            "</div>" +
            "</div>" +
            '<div class="form-group" data-bind="visible:KeyWordContentType()!=6">' +
            '<label class="col-sm-2 control-label">内容：</label>' +
            '<div class="col-sm-10">' +
            "<div>" +
            '<div class="panel panel-default" data-bind="click:ShowChoiceWindow">' +
            '<div class="panel-body" id="content">' +
            '<p data-bind="if:KeyWordContentType()==0">' +
            '<span data-bind="text:MediaId"></span>' +
            "</p>" +
            '<p data-bind="if:KeyWordContentType()==1">' +
            '<img style="width:150px;height:100px" data-bind="attr:{src:Url}" />' +
            "</p>" +
            '<p data-bind="if:KeyWordContentType()==3">' +
            '<audio style="width:250px;height:100px" controls="controls" data-bind="attr:{src:Url}" />' +
            "</p>" +
            '<p data-bind="if:KeyWordContentType()==4 && MediaId()!=\'\'">' +
            '<video style="height:100px !important;width:250px !important" controls="controls" data-bind="attr:{src:Url}" />' +
            "</p>" +
            '<p data-bind="if:KeyWordContentType()==5">' +
            '<img style="width:150px;height:100px" data-bind="attr:{src:Url}" />' +
            "</p>" +
            '<div data-bind="if:KeyWordContentType()!=0 && KeyWordContentType()!=5">' +
            '<p data-bind="text:Name">' +
            "</p>" +
            "</div>" +
            '<!-- ko if:MediaId()==\"\" -->' +
            "<p>" +
            '<a class="btn btn-primary btn-block m-t" data-bind="click:$root.ShowChoiceWindow">' +
            '<i class="fa fa-plus">' +
            "</i>选择素材</a>" +
            "</p>" +
            "<!-- /ko -->" +
            "</div>" +
            "</div>" +
            "</div>" +
            "</div>" +
            "</div>"
    });
}());