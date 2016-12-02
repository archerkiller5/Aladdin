ko.components.register("news-imageshow",
{
    viewModel: function(params) {
        var self = this;
        this.Name = ko.observable();
        this.Url = ko.observable("/Content/patterns/congruent_pentagon.png");
        this.Id = ko.observable();
        if (params && typeof (params.value()) == "string" && params.value().length > 0) {
            this.Id(params.value());
            window.CurrentModel.Api.request("GET",
            {
                url: "/api/News/" + params.value(),
                func: function(data) {
                    self.Name(data.Title);
                    self.Url("/MediaFiles/thumb/" + data.ThumbMediaId + ".jpg");
                }
            });
        }
    },
    template: '<span class="corner"></span>' +
        '<div class="image">' +
        '<img alt="image" class="img-responsive" data-bind="attr:{src:Url}">' +
        "</div>" +
        '<div class="file-name" data-bind="text:Name">' +
        "</div>"
});