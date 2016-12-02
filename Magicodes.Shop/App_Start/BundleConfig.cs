// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : BundleConfig.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Optimization;

namespace Magicodes.Shop
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif

            #region 微信JSSDK封装

            bundles.Add(new ScriptBundle("~/Scripts/wc.weichat").Include(
                "~/Scripts/weui/wc_weichat.js"
            ));

            #endregion

            #region 微信前端UI脚本

            // AUI
            bundles.Add(new StyleBundle("~/Content/aui/css/styles").Include(
                "~/Content/aui/css/aui.css"
                , "~/Content/aui/css/api.css"
                , "~/Content/aui/css/common.css"
                , "~/Content/aui/css/aui-indexed-list.css"
            ));

            bundles.Add(new ScriptBundle("~/Content/weui/scripts").Include(
                "~/Content/aui/script/api.js"
                , "~/Content/aui/script/aui-alert.js"
                , "~/Content/aui/script/aui-waterfall.js"
                //, "~/Content/aui/script/aui-indexed-list.js"
                , "~/Scripts/weui/zepto/zepto.js"
                , "~/Scripts/weui/zepto/ajax.js"
                , "~/Scripts/weui/zepto/callbacks.js"
                , "~/Scripts/weui/zepto/deferred.js"
                , "~/Scripts/weui/zepto/event.js"
                , "~/Scripts/weui/zepto/selector.js"
                , "~/Scripts/weui/zepto/fx.js"
                , "~/Scripts/weui/zepto/fx_methods.js"
                , "~/Scripts/weui/wc.js"
                , "~/Scripts/knockout-3.3.0.js"
            ));

            #region 全屏滑动

            bundles.Add(new ScriptBundle("~/Content/weui/slider-full/scripts").Include(
                "~/Content/aui/script/aui-slider-full.js"
            ));

            bundles.Add(new StyleBundle("~/Content/weui/slides-full/styles").Include(
                "~/Content/aui/css/aui-slider-full.css"
                , "~/Content/aui/css/aui-flex.css"
            ));

            #endregion

            bundles.Add(new StyleBundle("~/Content/weui/slides/styles").Include(
                "~/Content/aui/css/aui-slide.css"
            ));

            bundles.Add(new StyleBundle("~/Content/raty").Include(
              "~/Content/raty/css/raty.css"
             , "~/Content/raty/css/pygments.css"
             , "~/Content/raty/css/normalize.css"
             , "~/Content/aui/css/font-awesome.css"
             , "~/Content/aui/css/demo.css"
             , "~/Content/aui/css/common.css"
            ));

            bundles.Add(new ScriptBundle("~/Content/weui/slides/scripts").Include(
                "~/Content/aui/script/aui-slide.js"
            ));

            //图片延迟加载
            bundles.Add(new ScriptBundle("~/weui/plugins/zepto.unveil").Include(
                "~/scripts/weui/plugins/zepto.unveil.js"
            ));

            //无限滚动和下拉刷新
            bundles.Add(new StyleBundle("~/weui/plugins/droploadStyles").Include(
                "~/Content/plugins/dropload/dropload.css"
            ));
            bundles.Add(new ScriptBundle("~/weui/plugins/dropload").Include(
                "~/scripts/weui/dropload/dropload.js",
                "~/scripts/weui/components/infinitescroll.js"
            ));

            #endregion


            //msui
            bundles.Add(new StyleBundle("~/Content/msui/styles").Include(
                "~/Content/msui/css/sm.min.css",
                "~/Content/msui/css/sm-extend.min.css"));

            bundles.Add(new ScriptBundle("~/Content/msui/scripts").Include(
                "~/Content/msui/js/sm.min.js",
                "~/Content/msui/js/sm-extend.min.js",
                "~/Content/msui/js/sm-city-picker.min.js"));

            // mwc:Magicodes.WeiChat JS框架
            bundles.Add(new ScriptBundle("~/bundles/nwc").Include(
                "~/Scripts/app/mwc.js",
                "~/Scripts/app/mwc_elements.js",
                "~/Scripts/app/mwc_business.js"));

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/animate.css",
                "~/Content/style.css",
                "~/Content/plugins/awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css",
                "~/Content/inspinia_modify/inspinia_modify.css"
            ));

            // Font Awesome icons
            bundles.Add(new StyleBundle("~/font-awesome/css").Include(
                "~/fonts/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));

            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-2.1.1.js"));

            // jquery.validate
            bundles.Add(new ScriptBundle("~/bundles/jquery.validate").Include(
                "~/Scripts/jquery.validate.js"));

            // validate 
            bundles.Add(new ScriptBundle("~/plugins/validate").Include(
                      "~/Scripts/plugins/validate/jquery.validate.min.js"));

            // jquery.blockUI
            bundles.Add(new ScriptBundle("~/bundles/jquery.blockUI").Include(
                "~/Scripts/jquery.blockUI.js"));


            // knockout-3.3.0
            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/knockout.mapping-latest.js"));

            // jquery.validate
            bundles.Add(new ScriptBundle("~/bundles/jquery.validate").Include(
                "~/Scripts/jquery.validate.js"));

            // jquery.signalR
            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                "~/Scripts/jquery.signalR-{version}.js"));

            // jQueryUI CSS
            bundles.Add(new ScriptBundle("~/Scripts/plugins/jquery-ui/jqueryuiStyles").Include(
                "~/Scripts/plugins/jquery-ui/jquery-ui.css"));

            // jQueryUI 
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/plugins/jquery-ui/jquery-ui.min.js"));

            // select2 
            bundles.Add(new StyleBundle("~/Content/plugins/select2/select2Styles").Include(
                "~/Content/plugins/select2/select2.css",
                "~/Content/plugins/select2/select2-bootstrap.css"));

            // select2 
            bundles.Add(new ScriptBundle("~/plugins/select2").Include(
                    "~/Scripts/plugins/select2/select2.js"
                    , "~/Scripts/plugins/select2/i18n/zh-CN.js"
                )
            );
            //echarts
            bundles.Add(new ScriptBundle("~/plugins/echarts").Include(
                "~/Scripts/plugins/echart/echarts.js",
                "~/Scripts/plugins/echart/theme/macarons.js",
                "~/Scripts/app/components/echart.js"));

            // Bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.min.js"));

            // Inspinia script
            bundles.Add(new ScriptBundle("~/bundles/inspinia").Include(
                "~/Scripts/plugins/metisMenu/metisMenu.min.js",
                "~/Scripts/plugins/pace/pace.min.js",
                "~/Scripts/app/inspinia.min.js"));

            // Inspinia skin config script
            bundles.Add(new ScriptBundle("~/bundles/skinConfig").Include(
                "~/Scripts/app/skin.config.min.js"));

            // SlimScroll
            bundles.Add(new ScriptBundle("~/plugins/slimScroll").Include(
                "~/Scripts/plugins/slimscroll/jquery.slimscroll.min.js"));

            // Peity
            bundles.Add(new ScriptBundle("~/plugins/peity").Include(
                "~/Scripts/plugins/peity/jquery.peity.min.js"));

            // Video responsible
            bundles.Add(new ScriptBundle("~/plugins/videoResponsible").Include(
                "~/Scripts/plugins/video/responsible-video.js"));

            // Lightbox gallery css styles
            bundles.Add(new StyleBundle("~/Content/plugins/blueimp/css/").Include(
                "~/Content/plugins/blueimp/css/blueimp-gallery.min.css"));

            // Lightbox gallery
            bundles.Add(new ScriptBundle("~/plugins/lightboxGallery").Include(
                "~/Scripts/plugins/blueimp/jquery.blueimp-gallery.min.js"));

            // Sparkline
            bundles.Add(new ScriptBundle("~/plugins/sparkline").Include(
                "~/Scripts/plugins/sparkline/jquery.sparkline.min.js"));

            // Morriss chart css styles
            bundles.Add(new StyleBundle("~/plugins/morrisStyles").Include(
                "~/Content/plugins/morris/morris-0.4.3.min.css"));

            // Morriss chart
            bundles.Add(new ScriptBundle("~/plugins/morris").Include(
                "~/Scripts/plugins/morris/raphael-2.1.0.min.js",
                "~/Scripts/plugins/morris/morris.js"));

            // Morriss chart css styles
            bundles.Add(new StyleBundle("~/plugins/awesome-bootstrap-checkboxStyles").Include(
                "~/Content/plugins/awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css"));

            // Flot chart
            bundles.Add(new ScriptBundle("~/plugins/flot").Include(
                "~/Scripts/plugins/flot/jquery.flot.js",
                "~/Scripts/plugins/flot/jquery.flot.tooltip.min.js",
                "~/Scripts/plugins/flot/jquery.flot.resize.js",
                "~/Scripts/plugins/flot/jquery.flot.pie.js",
                "~/Scripts/plugins/flot/jquery.flot.time.js",
                "~/Scripts/plugins/flot/jquery.flot.spline.js"));

            // Rickshaw chart
            bundles.Add(new ScriptBundle("~/plugins/rickshaw").Include(
                "~/Scripts/plugins/rickshaw/vendor/d3.v3.js",
                "~/Scripts/plugins/rickshaw/rickshaw.min.js"));

            // ChartJS chart
            bundles.Add(new ScriptBundle("~/plugins/chartJs").Include(
                "~/Scripts/plugins/chartjs/Chart.min.js"));

            // iCheck css styles
            bundles.Add(new StyleBundle("~/Content/plugins/iCheck/iCheckStyles").Include(
                "~/Content/plugins/iCheck/custom.css"));

            // iCheck
            bundles.Add(new ScriptBundle("~/plugins/iCheck").Include(
                "~/Scripts/plugins/iCheck/icheck.min.js"));

            // dataTables css styles
            bundles.Add(new StyleBundle("~/Content/plugins/dataTables/dataTablesStyles").Include(
                "~/Content/plugins/dataTables/dataTables.bootstrap.css",
                "~/Content/plugins/dataTables/dataTables.responsive.css",
                "~/Content/plugins/dataTables/dataTables.tableTools.min.css"));

            // dataTables 
            bundles.Add(new ScriptBundle("~/plugins/dataTables").Include(
                "~/Scripts/plugins/dataTables/jquery.dataTables.js",
                "~/Scripts/plugins/dataTables/dataTables.bootstrap.js",
                "~/Scripts/plugins/dataTables/dataTables.responsive.js",
                "~/Scripts/plugins/dataTables/dataTables.tableTools.min.js"));

            // jeditable 
            bundles.Add(new ScriptBundle("~/plugins/jeditable").Include(
                "~/Scripts/plugins/jeditable/jquery.jeditable.js"));

            // jqGrid styles
            bundles.Add(new StyleBundle("~/Content/plugins/jqGrid/jqGridStyles").Include(
                "~/Content/plugins/jqGrid/ui.jqgrid.css"));

            // jqGrid 
            bundles.Add(new ScriptBundle("~/plugins/jqGrid").Include(
                "~/Scripts/plugins/jqGrid/i18n/grid.locale-en.js",
                "~/Scripts/plugins/jqGrid/jquery.jqGrid.min.js"));

            // codeEditor styles
            bundles.Add(new StyleBundle("~/plugins/codeEditorStyles").Include(
                "~/Content/plugins/codemirror/codemirror.css",
                "~/Content/plugins/codemirror/ambiance.css"));

            // codeEditor 
            bundles.Add(new ScriptBundle("~/plugins/codeEditor").Include(
                "~/Scripts/plugins/codemirror/codemirror.js",
                "~/Scripts/plugins/codemirror/mode/javascript/javascript.js"));

            // codeEditor 
            bundles.Add(new ScriptBundle("~/plugins/nestable").Include(
                "~/Scripts/plugins/nestable/jquery.nestable.js"));

            // validate 
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js"
            ));

            // fullCalendar styles
            bundles.Add(new StyleBundle("~/plugins/fullCalendarStyles").Include(
                "~/Content/plugins/fullcalendar/fullcalendar.css"));

            // fullCalendar 
            bundles.Add(new ScriptBundle("~/plugins/fullCalendar").Include(
                "~/Scripts/plugins/fullcalendar/moment.min.js",
                "~/Scripts/plugins/fullcalendar/fullcalendar.min.js"));

            // vectorMap 
            bundles.Add(new ScriptBundle("~/plugins/vectorMap").Include(
                "~/Scripts/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                "~/Scripts/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"));

            // ionRange styles
            bundles.Add(new StyleBundle("~/Content/plugins/ionRangeSlider/ionRangeStyles").Include(
                "~/Content/plugins/ionRangeSlider/ion.rangeSlider.css",
                "~/Content/plugins/ionRangeSlider/ion.rangeSlider.skinFlat.css"));

            // ionRange 
            bundles.Add(new ScriptBundle("~/plugins/ionRange").Include(
                "~/Scripts/plugins/ionRangeSlider/ion.rangeSlider.min.js"));

            // dataPicker styles
            bundles.Add(new StyleBundle("~/plugins/dataPickerStyles").Include(
                "~/Content/plugins/datapicker/datepicker3.css"));

            // dataPicker 
            bundles.Add(new ScriptBundle("~/plugins/dataPicker").Include(
                "~/Scripts/plugins/datapicker/bootstrap-datepicker.js"));

            // nouiSlider styles
            bundles.Add(new StyleBundle("~/plugins/nouiSliderStyles").Include(
                "~/Content/plugins/nouslider/jquery.nouislider.css"));

            // nouiSlider 
            bundles.Add(new ScriptBundle("~/plugins/nouiSlider").Include(
                "~/Scripts/plugins/nouslider/jquery.nouislider.min.js"));

            // jasnyBootstrap styles
            bundles.Add(new StyleBundle("~/plugins/jasnyBootstrapStyles").Include(
                "~/Content/plugins/jasny/jasny-bootstrap.min.css"));

            // jasnyBootstrap 
            bundles.Add(new ScriptBundle("~/plugins/jasnyBootstrap").Include(
                "~/Scripts/plugins/jasny/jasny-bootstrap.min.js"));

            // switchery styles
            bundles.Add(new StyleBundle("~/plugins/switcheryStyles").Include(
                "~/Content/plugins/switchery/switchery.css"));

            // switchery 
            bundles.Add(new ScriptBundle("~/plugins/switchery").Include(
                "~/Scripts/plugins/switchery/switchery.js"));

            // chosen styles
            bundles.Add(new StyleBundle("~/Content/plugins/chosen/chosenStyles").Include(
                "~/Content/plugins/chosen/chosen.css"));

            // chosen 
            bundles.Add(new ScriptBundle("~/plugins/chosen").Include(
                "~/Scripts/plugins/chosen/chosen.jquery.js"));

            // knob 
            bundles.Add(new ScriptBundle("~/plugins/knob").Include(
                "~/Scripts/plugins/jsKnob/jquery.knob.js"));

            // wizardSteps styles
            bundles.Add(new StyleBundle("~/plugins/wizardStepsStyles").Include(
                "~/Content/plugins/steps/jquery.steps.css"));

            // wizardSteps 
            bundles.Add(new ScriptBundle("~/plugins/wizardSteps").Include(
                "~/Scripts/plugins/steps/jquery.steps.min.js"));

            // dropZone styles
            bundles.Add(new StyleBundle("~/Content/plugins/dropzone/dropZoneStyles").Include(
                "~/Content/plugins/dropzone/basic.css",
                "~/Content/plugins/dropzone/dropzone.css"));

            // dropZone 
            bundles.Add(new ScriptBundle("~/plugins/dropZone").Include(
                "~/Scripts/plugins/dropzone/dropzone.js"));

            // summernote styles
            bundles.Add(new StyleBundle("~/plugins/summernoteStyles").Include(
                "~/Content/plugins/summernote/summernote.css",
                "~/Content/plugins/summernote/summernote-bs3.css"));

            // summernote 
            bundles.Add(new ScriptBundle("~/plugins/summernote").Include(
                "~/Scripts/plugins/summernote/summernote.min.js"));

            // toastr notification 
            bundles.Add(new ScriptBundle("~/plugins/toastr").Include(
                "~/Scripts/plugins/toastr/toastr.min.js"));

            // toastr notification styles
            bundles.Add(new StyleBundle("~/plugins/toastrStyles").Include(
                "~/Content/plugins/toastr/toastr.min.css"));

            // color picker
            bundles.Add(new ScriptBundle("~/plugins/colorpicker").Include(
                "~/Scripts/plugins/colorpicker/bootstrap-colorpicker.min.js"));

            // color picker styles
            bundles.Add(new StyleBundle("~/Content/plugins/colorpicker/colorpickerStyles").Include(
                "~/Content/plugins/colorpicker/bootstrap-colorpicker.min.css"));

            // image cropper
            bundles.Add(new ScriptBundle("~/plugins/imagecropper").Include(
                "~/Scripts/plugins/cropper/cropper.min.js"));

            // image cropper styles
            bundles.Add(new StyleBundle("~/plugins/imagecropperStyles").Include(
                "~/Content/plugins/cropper/cropper.min.css"));

            // jsTree
            bundles.Add(new ScriptBundle("~/plugins/jsTree").Include(
                "~/Scripts/plugins/jsTree/jstree.min.js"));

            // jsTree styles
            bundles.Add(new StyleBundle("~/Content/plugins/jsTree/styles").Include(
                "~/Content/plugins/jsTree/style.css"));

            // Diff
            bundles.Add(new ScriptBundle("~/plugins/diff").Include(
                "~/Scripts/plugins/diff_match_patch/javascript/diff_match_patch.js",
                "~/Scripts/plugins/preetyTextDiff/jquery.pretty-text-diff.min.js"));

            // Idle timer
            bundles.Add(new ScriptBundle("~/plugins/idletimer").Include(
                "~/Scripts/plugins/idle-timer/idle-timer.min.js"));

            // Tinycon
            bundles.Add(new ScriptBundle("~/plugins/tinycon").Include(
                "~/Scripts/plugins/tinycon/tinycon.min.js"));

            //sweetalert:http://t4t5.github.io/sweetalert/
            bundles.Add(new StyleBundle("~/plugins/sweetalertStyles").Include(
                "~/Content/plugins/sweetalert/sweetalert.css"));

            bundles.Add(new ScriptBundle("~/plugins/sweetalert").Include(
                "~/Scripts/plugins/sweetalert/sweetalert.min.js"));

            //spin:http://fgnass.github.io/spin.js/
            bundles.Add(new ScriptBundle("~/plugins/spin").Include(
                "~/Scripts/plugins/spin/spin.js"));

            // Ladda buttons Styless
            bundles.Add(new StyleBundle("~/plugins/laddaStyles").Include(
                "~/Content/plugins/ladda/ladda-themeless.min.css"));

            // Ladda buttons
            bundles.Add(new ScriptBundle("~/plugins/ladda").Include(
                "~/Scripts/plugins/ladda/spin.min.js",
                "~/Scripts/plugins/ladda/ladda.min.js",
                "~/Scripts/plugins/ladda/ladda.jquery.min.js"));

            // Touch Spin Styless
            bundles.Add(new StyleBundle("~/plugins/touchSpinStyles").Include(
                "~/Content/plugins/touchspin/jquery.bootstrap-touchspin.min.css"));

            // Touch Spin
            bundles.Add(new ScriptBundle("~/plugins/touchSpin").Include(
                "~/Scripts/plugins/touchspin/jquery.bootstrap-touchspin.min.js"));

            //raty
            bundles.Add(new ScriptBundle("~/plugins/raty").Include(
                "~/Scripts/plugins/raty/jquery.raty.js",
                "~/Scripts/plugins/raty/jquery.raty.min.js"));
        }
    }
}