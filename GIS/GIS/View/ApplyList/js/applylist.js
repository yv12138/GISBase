var dataView;
var form;
$(function () {
    registerDate('datetime', true);
    layui.use(['form'], function () {
        form = layui.form;
        form.render();
    });
    loadTable(); 
})
$(document).on("dblclick", "#list + div .layui-table-body table.layui-table tbody tr", function () {
    //取消上次延时未执行的方法
    if (clickFlag) {
        clickFlag = clearTimeout(clickFlag);
    }
    //var data = dataView.AllRows()[$(this).attr('data-index')];
    //parent.xadmin.add_tab('受理申请详情', '/View/ApplyList/ApplyListView.html?BSM=' + data.BSM, true)
})


//刷新
var refresh = function () {
    dataView.where = dataWhere();
    dataView.reload();
}
//搜索
var search = function () {
    refresh();
}

var getParam = function () {
    var param = getFormValue();
    return JSON.stringify(param);
}
var dataWhere = function () { return { JsonData: getParam() } };

var loadTable = function () {
    var option = {
        id: 'list',
        where: dataWhere(),
        url: api_server+"/api/HTMethod/GetApplyListByPage",
        cols: [[
            { title: '序号', type: 'numbers', align: 'left',  event: 'click' },
            {
                field: 'YWH', title: '业务号', align: 'left', event: 'click', sort: 'true',
                templet: function (data) {
                    return "<a onclick=\"parent.xadmin.add_tab('受理申请详情', '/View/ApplyList/ApplyListView.html?BSM=" + data.BSM + "&fremeName=" + window.name + "&YWH=" + data.YWH + "', true)\" style=\"color: blue; cursor:pointer\">" + data.YWH + "</a>";
                }
            },
            { field: 'DJLX', title: '登记类型', align: 'left', event: 'click', sort: 'true' },
            { field: 'SFDL', title: '是否代理', align: 'left', event: 'click', sort: 'true' },
            { field: 'SFFBCZ', title: '是否分别持证', align: 'left', event: 'click', sort: 'true'},
            { field: 'YSHTH', title: '预售合同号', align: 'left', event: 'click', sort: 'true' },
            { field: 'BDCQZH', title: '不动产权证号', align: 'left', event: 'click', sort: 'true'},
            { field: 'QXDMCN', title: '区县代码', align: 'left', event: 'click', sort: 'true', width: 90 },
            { field: 'SLSJ', title: '受理时间', align: 'left', event: 'click', sort: 'true'},
            {
                field: 'ZT', title: '当前状态', align: 'left', event: 'click', sort: 'true',
                templet: function (data) {
                    if (data.ZT == "在线受理成功") {
                        return "<span style=\"color: blue;\">" + data.ZT + "</span>";
                    } else if (data.ZT == "审核成功") {
                        return "<span style=\"color: green;\">" + data.ZT + "</span>";
                    } else if (data.ZT == "审核失败") {
                        return "<span style=\"color: red;\">" + data.ZT + "</span>";
                    } else {
                        return "";
                    }
                   
                }
            },
            { field: 'DJYWH', title: '登记系统业务号', align: 'left', event: 'click', sort: 'true' },


        ]],
    }
    dataView = new DataTableView(option);
}