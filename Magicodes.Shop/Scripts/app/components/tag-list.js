//参数说明：
//type：站点资源类型
//showUploadButton：是否显示上传按钮
//loadByUrl：是否根据Url参数加载默认的选择
(function() {
    var current = {};
    var instance = function(params, componentInfo) {
        var self = this;
        //站点资源类型
        this.SiteResourceType = ko.observable("Gallery");
        //选择模型
        this.TagList = ko.observableArray([]);
        //是否显示上传按钮
        this.ShowUploadButton = ko.observable(false);
        //图标
        //Gallery = 0,
        //Voice = 1,
        //Video = 2,
        //Thumb = 3,
        //Article = 4,
        //News = 5
        this.Icons = ["fa-picture-o", "fa-volume-up", "fa-film", "fa-picture-o", "fa-newspaper-o", "fa-newspaper-o"];
        //选择项
        this.ChoiceItem = null;
        //重新加载
        this.reload = function(success) {
            mwc.restApi.get({
                //请求地址
                url: "/api/Site_ResourceType/" + self.SiteResourceType(),
                //是否锁定UI
                isBlockUI: true,
                //可选，锁定元素
                blockUI: componentInfo.element,
                //成功函数
                success: function(model) {
                    for (var i = 0; i < model.length; i++) {
                        var item = model[i];
                        item.Checked = false;
                    }
                    self.TagList(ko.mapping.fromJS(model));
                    success && success(model);
                }
            });
        };
        //获取JS数据
        this.getJsData = function() {
            return ko.mapping.toJS(self.TagList());
        };
        //循环修改项
        this.setEachItem = function(setFunc) {
            var list = self.getJsData();
            for (var i = 0; i < list.length; i++) {
                var item = list[i];
                setFunc && setFunc(item);
            }
            self.TagList(ko.mapping.fromJS(list));
        };
        //更新选择状态
        this.setCheckedStatus = function() {
            self.setEachItem(function(item) {
                item.Checked = self.ChoiceItem != null && self.ChoiceItem.Id() == item.Id;
            });
        };
        //标签点击事件
        this.tagClick = function(item) {
            self.ChoiceItem = item;
            self.setCheckedStatus();
            //console.debug(item);
            window.tagClick && window.tagClick(ko.mapping.toJS(self.ChoiceItem));
        };
        //获取选择项
        this.getCheckedItem = function() {
            if (self.ChoiceItem != null) {
                return self.ChoiceItem;
            }
            var list = self.TagList()();
            for (var i = 0; i < list.length; i++) {
                var item = list[i];
                if (item.Checked()) {
                    self.ChoiceItem = item;
                    return self.ChoiceItem;
                }
            }
            return null;
        };
        //标签添加事件
        this.addClick = function() {
            mwc.message.prompt("请输入标签名称",
                "",
                "添加标签",
                function(inputValue) {
                    var data = { Title: inputValue, ResourceType: self.SiteResourceType() };
                    mwc.restApi.post({
                        //请求地址
                        url: "/api/Site_ResourceType/",
                        data: data,
                        //是否锁定UI
                        isBlockUI: true,
                        //可选，锁定元素
                        blockUI: componentInfo.element,
                        //成功函数
                        success: function(data) {
                            mwc.message.success("添加成功！");
                            self.reload(function() {
                                self.setCheckedStatus();
                            });
                        }
                    });
                });
        };
        //上传事件
        this.uploadClick = function() {
            if (self.ChoiceItem == null) {
                mwc.message.error("请选择标签！");
                return;
            }
            var url = "/Site_Resources/Upload/" + self.ChoiceItem.Id();
            var title = "上传素材";
            mwc.window.show(title, url, 996, 485);
        };
        //删除事件
        this.removeClick = function() {
            var tagName = this.Title();
            var id = this.Id();
            mwc.message.confirm("确定要删除【" + tagName + "】么？删除之后，其相应的资源均会移动到默认标签下。",
                function(result) {
                    if (result) {
                        mwc.restApi.delete({
                            url: "api/Site_ResourceType/" + id,
                            success: function(data) {
                                mwc.message.success("删除成功！");
                                self.reload(function() {
                                    if (id == self.ChoiceItem.Id()) {
                                        self.tagClick(self.TagList()()[0]);
                                    } else
                                        self.setCheckedStatus();
                                });
                            }
                        });
                    }
                });
        };
        //初始化
        if (params) {
            console.debug(params);
            params.type && self.SiteResourceType(params.type);
            params.showUploadButton && self.ShowUploadButton(params.showUploadButton);
            console.debug(self.ShowUploadButton());
            //加载数据
            self.reload(function() {
                //console.debug(self.TagList()()[0]);
                if (params.loadByUrl) {
                    var list = self.getJsData();
                    self.setEachItem(function(item) {
                        item.Checked = location.href.indexOf(item.Id) != -1;
                    });
                }
                var item = self.getCheckedItem();
                self.tagClick(item == null ? self.TagList()()[0] : item);
            });
        }

    };
    var newsViewModelInstance = {
        createViewModel: function(params, componentInfo) {
            current.viewModel = new instance(params, componentInfo);
            return current.viewModel;
        }
    };
    //按钮组选择组件
    ko.components.register("tag-list",
    {
        viewModel: newsViewModelInstance,
        template: '<ul class="tag-list" id="file-taglist" data-bind="foreach:TagList()" style="padding: 0">' +
            '<li class="tag" data-bind="css:Checked()?\'text-warning\':\'\'">' +
            '<span class="close" style="font-size:18px;" data-bind="click:$parent.removeClick,visible:!IsSystemResource()"><i class="fa fa-times"></i></span>' +
            '<a href="javascript:void(0);" data-bind="click:$parent.tagClick">' +
            '<i class="fa" data-bind="css:$parent.Icons[ResourceType()]"></i>&nbsp;<span data-bind="text:Title"></span>' +
            "</a>" +
            "</li>" +
            "</ul>" +
            '<ul class="tag-list">' +
            '<li class="text-success"><a data-bind="click:addClick"><i class="fa fa-plus"></i>&nbsp;&nbsp;添加</a></li>' +
            '<li class="text-danger" data-bind="click:uploadClick,visible:ShowUploadButton"><a data-bind="visible:ShowUploadButton"><i class="fa fa-cloud-upload"></i>&nbsp;&nbsp;上传</a></li>' +
            "</ul>"
    });
})();