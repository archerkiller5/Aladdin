/**
 * Created by u002 on 2016/11/11.
 */
$(
    function(){
        tabChange("VIPdSmg");
        $("#page-wrapper").css("minHeight", $("#wrapper").height());
        $(".card_excl").css("display", "none");
    }
)
function init(){
    $(".directView>div").css("display","none");
    $(".directTitle>div").css({
        color:"#ffffff",
        background:"#707070"
    });
}
function tabChange(id){
    init();
    $("#"+id).css("display","block");
    $("."+id).css({
        color:"#000000",
        background:"#ffffff"
    });
}
$(".VIPdSmg")
    .bind("click",
    function(){
        tabChange("VIPdSmg")
    }
);
$(".VIPdEsc")
    .bind("click",
    function(){
        tabChange("VIPdEsc")
    }
);

$("#select_option").bind("change", function () {
    var oHeight = $("#select_option").find("option:selected").text();
    if (oHeight == "请选择卡券类型") {
        $(".card_excl").css("display", "none");
      } else {
        $(".card_excl").css("display", "block");
      }
})


$(window).bind("scroll", function () {
    if ($(document).scrollTop() >= 160) {
        $(".card_excl").addClass("viewPos");
    } else {
        $(".card_excl").removeClass("viewPos");
    }

})
