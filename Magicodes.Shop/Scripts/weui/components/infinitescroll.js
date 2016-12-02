//无限滚动
//参数：
//  ajaxUrl:Ajax加载路径，默认会带上pageStart、pageSize参数
//  pageSize:分页数量
//  isBlockUI:是否显示加载遮罩层
var instance = function(params, componentInfo) {
    var self = this;
    this.Lists = ko.observableArray([]);
    this.ajaxUrl = null;
    this.load = function() {
        self.Lists([]);
        this.pageSize = params.pageSize || 10;
        this.pageStart = 0;
        this.ajaxStart = -1;
        // dropload
        $(componentInfo.element)
            .dropload({
                scrollArea: window,
                loadDownFn: function (me) {
                    if (self.ajaxStart != -1 && self.ajaxStart == self.pageStart)
                        return;
                    self.ajaxStart = self.pageStart;
                    wc.restApi.get({
                        url: self.ajaxUrl,
                        data: { pageStart: self.pageStart, pageSize: self.pageSize },
                        isBlockUI: self.isBlockUI,
                        success: function(data) {
                            self.pageStart += data.length;
                            if (data.length == 0) {
                                //锁定
                                me.lock();
                                //无数据
                                me.noData();
                                $("div.dropload-down").remove();
                                return;
                            }
                            for (var i = 0; i < data.length; i++) {
                                self.Lists.push(data[i]);
                            }
                            $("div.dropload-down").remove();
                            // 每次数据加载完，必须重置
                            me.resetload();
                        }
                    });
                }
            });
    };
    if (params) {
        this.pageSize = params.pageSize || 10;
        this.pageStart = 0;
        this.isBlockUI = params.isBlockUI || false;
        if ($.isFunction(params.ajaxUrl)) {
            self.ajaxUrl = params.ajaxUrl();
            //添加订阅
            params.ajaxUrl.subscribe(function(newValue) {
                self.ajaxUrl = newValue;
                self.load();
            });
            self.load();
        } else {
            this.ajaxUrl = params.ajaxUrl;
            self.load();
        }
    }

};
var newschoiceViewModelInstance = {
    createViewModel: function(params, componentInfo) {
        return new instance(params, componentInfo);
    }
};
//按钮组选择组件
ko.components.register("infinitescroll",
{
    viewModel: newschoiceViewModelInstance,
    template: '<div data-bind="template: { name: \'infinitescroll-template\', data: Lists }"></div>'
});