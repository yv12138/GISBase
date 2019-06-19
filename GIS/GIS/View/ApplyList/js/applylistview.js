var id = "";
var fremeName = "";
var YWH = "";
var dtree;
var DemoTree;
$(function () {
    //审核通过
    $("#btnCheck").click(function () {
        check();
    });
    //退件
    $("#btnBack").click(function () {
        back();
    });
    id = getUrlParam("BSM");
    fremeName = getUrlParam("fremeName");
    YWH = getUrlParam("YWH");
    var data = {
        type: "POST",
        url: api_server + "/api/HTMethod/GetApplyListInfo",
        JsonData: JSON.stringify({ BSM: id })
    }
    jqueryRequestData(data, setData);

    //监听tab事件
    layui.use('element', function () {
        var element = layui.element;
        element.on('tab(view)', function () {
            var id = this.getAttribute('lay-id');
            var index = layer.load(0, { shade: false });
            if (id == 'fj') {
                var width = $(".tree_divright").innerWidth() - 30 + "px";
                var height = $(".tree_divright").innerHeight() - 30 + "px";
                $("#fjShow").css({ "max-width": width, "max-height": height });
            }
            layer.close(index);
        });
    })

    //加载扩展模块
    layui.extend({
        dtree: '../../../resources/dtree/dtree'//文件路径
    }).use(['dtree', 'layer', 'jquery'], function () {
        dtree = layui.dtree, layer = layui.layer, $ = layui.jquery;
        var json = {
            YWH: YWH
        }
        DemoTree = dtree.render({
            elem: "#demoTree",
            url: api_server + "/api/HTMethod/GetAccessoryByYWH",
            request: { JsonData: JSON.stringify(json) },
            dataStyle: "layuiStyle",  //使用layui风格的数据格式
            dataFormat: "list",  //配置data的风格为list
            response: { message: "msg", statusCode: 0 },  //修改response中返回数据的定义
            success: function (data, obj) {
                var data = data;
                var obj = obj;
            },
            type: "all",//全量加载
            initLevel: "1",//默认展开
            dot: false,  // 设定小圆点隐藏
            icon: ["0", "0"] // 设定二级图标样式。-1表示非子节点图标不显示，8表示子节点图标   
        });
        //监听树单击
        dtree.on("node('demoTree')", function (obj) {
            if (obj.param.basicData != undefined) {
                //var url = "http://10.15.22.75:8088/File/attatchpath/2019/5/9/d6b7af12542d48bca3e6a75b72830e56/8880832d8b0e4d60833b76d93d54790c.jpg";
                var url = obj.param.basicData;
                    $("#fjShow").attr('src', api_server+ "/api/HTMethod/TransferAsync?url=" + url);
            }
        });
    });

});
var loadIndex;
//审核通过
var check = function () {
    if ($("#txtSHYJ").val() == "") {
        layer.msg("请填写审核意见！", { icon: 5 });
        return false;
    }
    var json = {
        ZT: 1,
        SHYJ: $("#txtSHYJ").val(),
        BSM: id,
        YWH: $("#txtYWH").text(),
    }
    loadIndex = layerLoad("执行中请稍后.....","-18px");
    $.ajax({
        type: "POST",
        url: api_server + "/api/HTMethod/CheckSLSQStatus",
        data: { JsonData: JSON.stringify(json) },
        success: function (data) {
            parent.layer.close(loadIndex);
            var json = {
                ZT: 1,
                SHYJ: $("#txtSHYJ").val(),
                BSM: id,
                YWH: $("#txtYWH").text(),
            }
            var result = data;
            if (result.code == '0') {
                confirmShow(result.msg, function (json) {
                    loadIndex = layerLoad("执行中请稍后.....", "-18px");
                    $.ajax({
                        type: "POST",
                        url: api_server + "/api/HTMethod/CheckSLSQ",
                        data: { JsonData: JSON.stringify(json) },
                        success: function (data) {
                            parent.layer.close(loadIndex)
                            var result = data;
                            if (result.code == '0') {
                                layer.msg(result.msg, { icon: 1 });
                                confirmShow(result.msg, function () {
                                    var id = $(window.frameElement).attr('tab-id');
                                    parent.xadmin.del_tab(id, callback);
                                })
                              
                            } else {
                                layer.msg(result.msg, { icon: 5 });
                            }
                            return false;
                        },
                        error: function (data) {
                            layer.msg(data, { icon: 5 });
                        }
                    });

                }, json)
            } else {
                layer.msg(result.msg, { icon: 5 });
            }
            return false;
        },
        error: function (data) {
            layer.msg(data, { icon: 5 });
        }
    });
}
//退件
var back = function () {
    if ($("#txtSHYJ").val() == "") {
        layer.msg("请填写审核意见！", { icon: 5 });
        return false;
    }
    var json = {
        ZT: 2,
        SHYJ: $("#txtSHYJ").val(),
        BSM: id
    }
    $.ajax({
        type: "POST",
        url: api_server + "/api/HTMethod/CheckSLSQ",
        data: { JsonData: JSON.stringify(json) },
        success: function (data) {
            var result = data;
            if (result.code == '0') {
                confirmShow(result.msg, function () {
                    var id = $(window.frameElement).attr('tab-id');
                    parent.xadmin.del_tab(id, callback);
                })
            } else {
                layer.msg(result.msg, { icon: 5 });
            }
            return false;
        },
        error: function (data) {
            layer.msg(data, { icon: 5 });
        }
    });
}

var callback = function () {
    top.window.frames[fremeName].search();
}
var setData = function (data) {
    var result = data.data;
    setFormValue(result, "div_view");
    if (result.ZT == "审核失败" || result.ZT == "审核成功" || result.ZT == "") {
        $("#btnCheck").attr('disabled', 'true');
        $("#btnCheck").css({ 'background-color': '#6c757d', "cursor": "no-drop" });
        $("#btnBack").attr('disabled', 'true');
        $("#btnBack").css({ 'background-color': '#6c757d', "cursor": "no-drop" });
        $("#txtSHYJ").prop("readonly", true);
        $("#txtSHYJ").val(result.SHYJ);
    } else {
        $("#txtSHYJ").val(result.SHYJ);
    }
    var param = JSON.stringify({ YWH: result.YWH });
    personView(param)
}
var dataView;
//权利人信息
var personView = function (param) {
    var optionPerson = {
        id: 'personList',
        where: { JsonData: param },
        url: api_server + "/api/HTMethod/GetPersonList",
        page: false,
        cols: [[
            { title: '序号', type: 'numbers', align: 'center', width: 70, event: 'click' },
            { field: 'SQRLX', title: '申请人类型', align: 'center', event: 'click' },
            { field: 'SQRMC', title: '申请人名称', align: 'center', event: 'click' },
            { field: 'SQRZJH', title: '证件号', align: 'center', event: 'click' },
            { field: 'LXDH', title: '联系电话', align: 'center', event: 'click' },
            { field: 'SQRTXDZ', title: '通讯地址', align: 'center', event: 'click' },
            {
                field: 'ValveWellCode', title: '操作', align: 'center', event: 'click',
                templet: function (data) {
                    return "<button class=\"layui-btn layui-btn layui-btn-xs\" onclick=\"parent.xadmin.open('查看', '/View/ApplyList/PersonInfo.html?BSM=" + data.BSM + "','70%','50%')\"><i class=\"ui-icon icon-chakan\"></i>查看详情</button>";
                }
            },
        ]],
    }
    dataView = new DataTableView(optionPerson);
}


