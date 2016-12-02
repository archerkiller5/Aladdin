//按钮组选择组件
ko.components.register("tenanturl-input",
{
    viewModel: function(params) {
        var self = this;
        self.Url = params.url;
        self.TenantId = params.tenantId;
        var url = self.Url();

        function setTenantUrl() {
            if (url == "" || url == null)
                return;
            if (url.indexOf("tenantId=") == -1) {
                url += (url.indexOf("?") == -1 ? "?" : "&");
                url += "tenantId=" + self.TenantId;
                self.Url(url);
            }
        }

        setTenantUrl();
        self.Url.subscribe(function(newValue) {
            url = newValue;
            setTenantUrl();
        });
    },
    template: '<input type="url" class="form-control" data-bind="value:Url" />'
});