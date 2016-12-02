
var instance = function(params, componentInfo) {
    var self = this;
    this.Name = ko.observable();
    this.Url = ko.observable("/Content/patterns/congruent_pentagon.png");
    this.Id = ko.observable();
    //Id发生改变时，获取新的图片
    self.Id.subscribe(function(newValue) {
        mwc.log.warn(newValue);
        mwc.restApi.get({
            //请求地址
            url: "/api/News/" + newValue,
            //是否锁定UI
            isBlockUI: true,
            //可选，锁定元素
            blockUI: componentInfo.element,
            //成功函数
            success: function(data) {
                self.Name(data.Title);
                self.Url("/MediaFiles/thumb/" + data.ThumbMediaId + ".jpg");
            }
        });
    });

    if (params && typeof (params.value()) == "string") {
        params.value.subscribe(function(newValue) {
            self.Id(newValue);
        });
        params.value().length > 0 && self.Id(params.value());
    }

    this.showLoader = ko.observable(true);
    this.currentImage = ko.observable({
        "Id": "",
        "Name": "请选择素材（多图文）",
        "Url": "/Content/patterns/congruent_pentagon.png"
    });
    this.pages = ko.observable([]);
    this.dataRows = ko.observableArray([]);
    this.pageSize = ko.observable(6);
    this.totalCount = ko.observable(0);
    this.currentPageIndex = ko.observable(1);
    this.nextPage = function() {
        self.currentPageIndex() < self.getTotalPages() && self.currentPageIndex(self.currentPageIndex() + 1);
    };
    this.previousPage = function() {
        self.currentPageIndex() > 1 && self.currentPageIndex(self.currentPageIndex() - 1);
    };
    this.getTotalPages = function() {
        var i = self.totalCount() / self.pageSize();
        return Math.floor(i) + (i > Math.floor(i) ? 1 : 0);
    };
    this.getPagesArr = function() {
        var totalPages = self.getTotalPages();
        var limitCount = totalPages > 10 ? 10 : totalPages;
        var currentPageIndex = self.currentPageIndex();
        var min = 1, max = limitCount;
        if (currentPageIndex > 5) {
            if (totalPages - currentPageIndex > 5) {
                min = currentPageIndex - 4;
                max = currentPageIndex + 5;
            } else {
                min = currentPageIndex - 9;
                max = totalPages;
            }
        }
        if (min < 1) min = 1;
        return ko.utils.range(min, max);
    };
    //订阅  当前页
    self.currentPageIndex.subscribe(function(newValue) {
        self.loadData();
    });
    //订阅  分页数
    self.pageSize.subscribe(function(newValue) {
        var t = self.getTotalPages();
        self.pages(self.getPagesArr());
        if (self.currentPageIndex() > t)
            self.currentPageIndex(1);
        self.loadData();
    });
    //加载数据
    this.loadData = function() {
        mwc.restApi.get({
            //请求地址
            url: "/api/News/" + self.currentPageIndex() + "/" + self.pageSize(),
            //是否锁定UI
            isBlockUI: true,
            //可选，锁定元素
            blockUI: componentInfo.element,
            //成功函数
            success: function(data) {
                self.dataRows(data.DataRows);
                self.totalCount(data["TotalItemCount"]);
                self.pages(self.getPagesArr());
                self.showLoader(false);
            }
        });
    };
    this.ImageClick = function(data) {
        self.Id(data.Id);
        mwc.log.debug(params.value());
        params.value(data.Id);
        $("#newsModal").modal("hide");
    };
    self.loadData();
};
var newschoiceViewModelInstance = {
    createViewModel: function(params, componentInfo) {
        return new instance(params, componentInfo);
    }
};
//多图文选择
ko.components.register("news-choice-button",
{
    viewModel: newschoiceViewModelInstance,
    template: "" +
        '<div class="file-box">' +
        '<div class="file-box">' +
        '<div class="file">' +
        '<a href="#" data-toggle="modal" id="news-imageshow" data-target="#newsModal">' +
        '<span class="corner"></span>' +
        '<div class="image">' +
        '<img alt="image" class="img-responsive" data-bind="attr:{src:Url}">' +
        "</div>" +
        '<div class="file-name" data-bind="text:Name">' +
        "</div>" +
        "</a>" +
        "</div>" +
        "</div>" +
        "</div>" +
        '<button type="button" class="btn btn-primary" data-toggle="modal" data-target="#newsModal">' +
        "选择素材" +
        "</button>"
});

//多图文选择
ko.components.register("news-choice-modal",
{
    viewModel: newschoiceViewModelInstance,
    template: "" +
        '<template id="pagesTemplate">' +
        '<button type="button" class="btn btn-white" data-bind="css: { disabled: currentPageIndex() <= 1 }, click: function () { previousPage(); }"><i class="fa fa-chevron-left"></i></button>' +
        "<!-- ko foreach: pages -->" +
        '<button class="btn btn-white" data-bind="css: { active: $data == $parent.currentPageIndex() },text: $data, click: function () { $parent.currentPageIndex($data); }"></button>' +
        "<!-- /ko -->" +
        '<button type="button" class="btn btn-white" data-bind="css: { disabled: currentPageIndex() >= getTotalPages() }, click: function () { nextPage(); }"><i class="fa fa-chevron-right"></i> </button>' +
        "</template>" +
        '<div class="modal inmodal fade" id="newsModal" tabindex="-1" role="dialog" aria-hidden="true">' +
        '<div class="modal-dialog modal-lg">' +
        '<div class="modal-content">' +
        '<div class="modal-header">' +
        '<button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>' +
        '<h4 class="modal-title">请选择素材（点击图片）</h4>' +
        "</div>" +
        '<div class="modal-body">' +
        '<div class="row" data-bind="visible:showLoader()">' +
        '<div class="col-lg-12 center-block middle-box">' +
        '<div class="sk-spinner sk-spinner-pulse"></div>' +
        "</div>" +
        "</div>" +
        '<div class="row" data-bind="visible:!showLoader()">' +
        '<div class="col-lg-12 animated fadeInRight" data-bind="foreach: dataRows">' +
        '<div class="file-box" data-bind="click:$parent.ImageClick">' +
        '<div class="file">' +
        '<a data-bind="attr:{href:Url}">' +
        '<span class="corner"></span>' +
        '<div class="image">' +
        '<img alt="image" class="img-responsive" data-bind="attr:{src:\'/MediaFiles/thumb/\'+ThumbMediaId+\'.jpg\'}">' +
        "</div>" +
        '<div class="file-name" data-bind="text:Title">' +
        "</div>" +
        "</a>" +
        "</div>" +
        "</div>" +
        "</div>" +
        '<div class="col-lg-12 animated fadeInLeft">' +
        '<div class="form-group">' +
        '<div class="col-sm-4">' +
        '<div>当前为第<span data-bind="text: currentPageIndex()"></span>页，总共有<span data-bind="    text: getTotalPages()"></span>页</div>' +
        "</div>" +
        '<div class="col-sm-8">' +
        '<div class="btn-group pull-right" data-bind="template: { name: \'pagesTemplate\',replaceChildren:$root }"></div>' +
        "</div>" +
        "</div>" +
        "</div>" +
        "</div>" +
        "</div>" +
        '<div class="modal-footer">' +
        '<button type="button" class="btn btn-primary" data-dismiss="modal">关闭</button>' +
        //'<button type="button" class="btn btn-primary">确定</button>' +
        "</div>" +
        "</div>" +
        "</div>" +
        "</div>"

});