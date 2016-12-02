
!(function(window) {

    var _pageIndex = 1, //当前页
        _totalCount = 20, //数据总数
        _pageSize = 10; //页大小

    var Index = function() {};
    Index.prototype = {
        init: function() {
            var _this = this;
            _this.iniEvent();
            //_this.setPaging();

            _this.searchOrderList(); //搜索订单
        },
        iniEvent: function() {
            var _this = this;

            //订单状态选择筛选
            $("#header_state_search")
                .find("li")
                .on("click",
                    function() {
                        var _el_this = $(this);
                        if (_el_this.hasClass("active")) return;
                        _el_this.addClass("active").siblings("li").removeClass("active");

                        _pageIndex = 1;
                        _this.searchOrderList(); //搜索订单
                    });
            //搜索
            $("#btn_search")
                .on("click",
                    function() {
                        var _el_this = $(this);

                        _pageIndex = 1;
                        _this.searchOrderList(); //搜索订单
                    });
        },
        //搜索订单
        searchOrderList: function() {
            var _this = this;
            var data = {
                OrderState: $("#header_state_search").find(".active:eq(0)").attr("data-state"),
                OrderNo: $("#txt_order_no").val(),
                ShippingCode: $("#txt_shipping_code").val(),
                Mobile: $("#txt_mobile").val(),
                Consignee: $("#txt_consignee").val(),
                FreighType: $("#freighType").val(),
                PayType: $("#payType").val(),
                //StartDate: $('#txt_start_dete').val(),
                //EndDate: $('#txt_end_dete').val(),
                pageIndex: _pageIndex,
                pageSize: _pageSize
            };

            mwc.restApi.post({
                url: "../../Order_Info/SearchOrderList",
                data: data,
                success: function(data) {
                    var listPanel = $("#list_panel");
                    if (!data.Success || data.Result == null || data.Result.length <= 0) {
                        listPanel.html('<div id="no_data" class="no_data">暂无订单信息！</div>');
                        _totalCount = 0; //当前获取到数据的总数量
                        _this.setPaging();
                        return;
                    }

                    if (!$("#order_rowlist").length) {
                        listPanel.html('<ul id="order_rowlist"></ul>');
                    }

                    var rowList = $("#order_rowlist"),
                        html = "";

                    data.Result.OrderList.forEach(function(order) {
                        html += "<li>" +
                            '    <div class="item_header">' +
                            '        <label class="chk_order"><input type="checkbox" /></label>' +
                            '        <label><span>订单编号：</span><span class="order_no">' +
                            order.Code +
                            "</span></label>" +
                            '        <label><span>物流单号：</span><span class="order_no">' +
                            order.OrderShipping.ShippingCode +
                            "</span></label>" +
                            "        <label><span>下单时间：" +
                            order.CreateTime +
                            "</span></label>" +
                            "        <label><span>买家：" +
                            (null == order.User[0] ? "" : order.User[0].NickName) +
                            "</span></label>" +
                            '        <label><span class="order_state">' +
                            order.State +
                            "</span></label>" +
                            "    </div>";
                        var shippID = "";
                        if (order.OrderDetail.length > 0) {
                            order.OrderDetail.forEach(function(detail, index) {
                                var Shipping = "",
                                    consigneeInfo = "",
                                    priceInfo = "",
                                    leaveInfo = "",
                                    className = "";

                                if (parseFloat(order.Shipping) > 0) {
                                    Shipping = '<p class="freight">(含运费<span>¥' + order.Shipping + "</span>)</p>";
                                }
                                if (index == 0) {
                                    consigneeInfo = '<div class="order_consignee">' +
                                        '  <p class="name">' +
                                        order.OrderShipping.Consignee +
                                        "</p>" +
                                        '  <p class="mobile">' +
                                        order.OrderShipping.Mobile +
                                        " </p>" +
                                        "</div>";
                                    priceInfo = '<div class="order_price">' +
                                        '  <p class="price">¥' +
                                        order.TotalPrice +
                                        "</p>" +
                                        Shipping +
                                        '   <p class="logistics">' +
                                        order.OrderShipping.Logistics +
                                        "</p>" +
                                        "</div>";
                                    leaveInfo = '<div class="order_message">' + order.Leave + "</div>";
                                    shippID = order.OrderShipping.Id;
                                }
                                if (index > 0) {
                                    className = 'class="clear_b"';
                                }

                                html += '<ul class="goods_item">' +
                                    "     <li>" +
                                    '          <div class="goods_info">' +
                                    '              <div class="goods_img">' +
                                    '                  <img src="' +
                                    detail.ProductImage +
                                    '" />' +
                                    "              </div>" +
                                    '              <div class="goods_detail">' +
                                    '                  <p class="title">' +
                                    detail.ProductName +
                                    "</p>" +
                                    '                  <p class="price">¥ <span>' +
                                    detail.Price +
                                    "</span></p>" +
                                    '                  <p class="attribute">' +
                                    (detail.Rule1 + "  " + detail.Rule2) +
                                    "</p>" +
                                    '                  <p class="number">数量：<span>' +
                                    detail.Quantity +
                                    "</span></p>" +
                                    "              </div>" +
                                    "          </div>" +
                                    "      </li>" +
                                    "      <li " +
                                    className +
                                    ">" +
                                    consigneeInfo +
                                    "</li>" +
                                    "      <li " +
                                    className +
                                    ">" +
                                    priceInfo +
                                    "</li>" +
                                    "      <li " +
                                    className +
                                    ">" +
                                    leaveInfo +
                                    "</li>" +
                                    "  </ul>";
                            });
                        }

                        html += '    <div class="order_oper">' +
                            '        <a href="/Order_Info/Details/' +
                            order.Id +
                            '">查看详情</a>';
                        if (order.State == "待发货") {
                            html += '        <a href="javascript:SendGoods(\'' + shippID + '\');">发货</a>';
                        }
                        //if (order.State == '待发货' || order.State == '退货/退款') {
                        //    html += '        <a href="javascript:RemakSave(\'' + order.Id + '\');">备注</a>'
                        //}
                        //html += '        <a href="javascript:OpOrder(\'' + order.Id + '\',\'D\');">删除</a>'
                        if (order.State == "待付款") {
                            html += '        <a href="javascript:OpOrder(\'' + order.Id + '\',\'C\');">关闭</a>';
                        }
                        html += "    </div>" + "</li>";
                    });

                    rowList.html(html);

                    mwc.bs.init();

                    //_pageIndex = 1;
                    _totalCount = data.Result.TotalCount; //当前获取到数据的总数量
                    _this.setPaging();
                }
            });

        },
        //设置分页
        setPaging: function() {

            if (Math.ceil(_totalCount / _pageSize) <= 1) {
                $("#fy_list").hide();
                return;
            }
            var _this = this;
            //重置分页控件
            $("#fy_list").show().html('<div class="col-lg-12 animated fadeInLeft"><ul id="pagination"></ul></div>');
            $("#pagination")
                .twbsPagination({
                    totalPages: Math.ceil(_totalCount / _pageSize), //总页数
                    startPage: _pageIndex, //当前页
                    visiblePages: 10, //最大可见页面
                    first: "首页",
                    prev: '<i class="fa fa-chevron-left"></i>',
                    next: '<i class="fa fa-chevron-right"></i>',
                    last: "尾页",
                    paginationClass: "pagination",
                    onPageClick: function(event, curpage) {
                        console.log(curpage);
                        _pageIndex = curpage; //更新当前页数
                        _this.searchOrderList(); //搜索订单
                    }
                });
        }
    };
    window.Index = new Index();

})(window);