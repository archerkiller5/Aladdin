//按钮组选择组件
ko.components.register("buttonschoices",
{
    viewModel: function(params) {
        var self = this;
        //所选值
        this.SelectValue = ko.observable();
        //text：文本
        //value：值
        //icon：图标
        //des:描述
        this.SelectItem = ko.observable({ text: "", value: "", icon: "", des: "" });
        //选择模型
        this.SelectsModel = ko.observableArray([]);
        if (params && typeof (params.SelectsModel()) != "undefined") {
            self.SelectsModel(params.SelectsModel());
            if (typeof (params.SelectValue()) != "undefined") {
                self.SelectValue(params.SelectValue());
                self.SelectItem($.grep(self
                    .SelectsModel(),
                    function(v, i) { return v.value == self.SelectValue() })[0]);
            }
        }
        this.GetActiveCss = function(item) {
            return item.value == self.SelectValue() ? "active btn-primary" : "";
        };
        this.buttonClick = function(item) {
            self.SelectValue(item.value);
            self.SelectItem(item);
            params.SelectValue(item.value);
        };
    },
    template: '<div class="btn-group" data-bind="foreach: SelectsModel">' +
        '<button class="btn btn-white" data-bind="css:$parent.GetActiveCss($data),click:$parent.buttonClick"><i class="fa" data-bind="css:icon"></i>&nbsp;<span data-bind="text:text"></span></button>' +
        "</div>" +
        '<div class="well" data-bind="with:SelectItem">' +
        '<span data-bind="text:des"></span>' +
        "</div>"
});