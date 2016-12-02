var instance = function(params, componentInfo) {
    var self = this;
    this.Name = ko.observable("请选择素材");
    this.Url = ko.observable("/Content/patterns/congruent_pentagon.png");
    this.Id = ko.observable("");
    //默认选择的类型
    this.MediaType = ko.observable("1");
    //是否禁用选择框
    this.DisabledChoice = ko.observable(false);
    //订阅 
    self.MediaType.subscribe(function(newValue) {
        console.debug(newValue);
        self.Name("请选择素材");
        self.Url("/Content/patterns/congruent_pentagon.png");
        self.Id("");
    });
    this.ShowChoiceWindow = function(data) {
        var type = self.MediaType();
        var url = "";
        switch (type) {
            //图文
        case "5":
            url = "/Site_News?resourceType=5&lightLayout=1";
            break;
        //图片
        case "0":
            url = "/Site_Resources?resourceType=0&lightLayout=1";
            break;
        //语音
        case "1":
            url = "/Site_Resources?resourceType=1&lightLayout=1";
            break;
        //视频
        case "2":
            url = "/Site_Resources?resourceType=2&lightLayout=1";
            break;
        }
        mwc.window.show("选择素材", url);
    };
    if (params && typeof (params.value()) == "string") {

        params.value.subscribe(function(newValue) {
            self.Id(newValue);
        });
        if (params.value().length > 0) {
            self.Id(params.value());
            mwc.restApi.get({
                //请求地址
                url: "/Site_Resources/GetJsonDataByMediaId/" + self.Id(),
                //是否锁定UI
                isBlockUI: true,
                //可选，锁定元素
                blockUI: componentInfo.element,
                //成功函数
                success: function(model) {
                    console.debug(model);
                    var data = model.FileBase;
                    if (data == null) {
                        return;
                    }
                    self.MediaType(model.ResourceType);
                    //一定要放后面，不然因为触发了订阅事件而导致相关数据被清空
                    self.Name(data.Name);
                    if (data.FrontCoverImageUrl)
                        self.Url(data.FrontCoverImageUrl);
                    else
                        self.Url(data.SiteUrl || data.Url);
                    self.Id(data.MediaId);
                    console.debug(ko.toJS(self));
                }
            });
        }
        if (params.mediaType) {
            console.debug("params.mediaType：", params.mediaType);
            self.MediaType(params.mediaType);
        }
        if (params.disabledChoice) {
            self.DisabledChoice(params.disabledChoice);
        }
    }
    window.choice = function(data) {
        params.value(data.MediaId);
        self.Id(data.MediaId);
        self.Name(data.Name);
        if (data.FrontCoverImageUrl) {
            self.Url(data.FrontCoverImageUrl);
        } else
            self.Url(data.SiteUrl || data.Url);
    };
};
var newschoiceViewModelInstance = {
    createViewModel: function(params, componentInfo) {
        return new instance(params, componentInfo);
    }
};
//资源选择
ko.components.register("media-choice-button",
{
    viewModel: newschoiceViewModelInstance,
    template: "" +
        '<div class="form-group">' +
        '<label class="col-sm-2 control-label">类型：</label>' +
        '<div class="col-sm-10">' +
        '<select class="form-control" data-bind="value:MediaType,disable:DisabledChoice" data-val="true" data-val-required="类型 字段是必需的。">' +
        '<option value="0">图片</option>' +
        '<option value="1">语音</option>' +
        '<option value="2">视频</option>' +
        '<option value="5">图文</option>' +
        "</select>" +
        "</div>" +
        "</div>" +
        '<div class="form-group">' +
        '<div class="col-sm-12">' +
        "<div>" +
        '<div class="panel panel-default" data-bind="click:ShowChoiceWindow">' +
        '<div class="panel-body" id="content">' +
        '<p data-bind="if:MediaType()==0">' +
        '<img style="width:150px;height:100px" data-bind="attr:{src:Url}" />' +
        "</p>" +
        '<p data-bind="if:MediaType()==1 && Id()!=\'\'">' +
        '<audio style="width:250px;height:100px" controls="controls" data-bind="attr:{src:Url}" />' +
        "</p>" +
        '<p data-bind="if:MediaType()==2 && Id()!=\'\'">' +
        '<video style="height:100px !important;width:250px !important" controls="controls" data-bind="attr:{src:Url}" />' +
        "</p>" +
        '<p data-bind="if:MediaType()==5">' +
        '<img style="width:150px;height:100px" data-bind="attr:{src:Url}" />' +
        "</p>" +
        "<div>" +
        '<p data-bind="text:Name">' +
        "</p>" +
        "</div>" +
        '<!-- ko if:Id()==\"\" && "1,2".indexOf(MediaType())!=-1 -->' +
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