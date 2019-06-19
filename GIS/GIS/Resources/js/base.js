var MiniSite = new Object();
/**
 * 判断浏览器
 */
MiniSite.Browser = {
    ie: /msie/.test(window.navigator.userAgent.toLowerCase()),
    moz: /gecko/.test(window.navigator.userAgent.toLowerCase()),
    opera: /opera/.test(window.navigator.userAgent.toLowerCase()),
    safari: /safari/.test(window.navigator.userAgent.toLowerCase())
};
/**
 * JsLoader对象用来加载外部的js文件
 */
MiniSite.JsLoader = {
    /**
     * 加载外部的js文件
     * @param sUrl 要加载的js的url地址
     * @fCallback js加载完成之后的处理函数
     */
    load: function (sUrl, fCallback) {
        var _script = document.createElement('script');
        _script.setAttribute('charset', 'gbk');
        _script.setAttribute('type', 'text/javascript');
        _script.setAttribute('src', sUrl);
        document.getElementsByTagName('head')[0].appendChild(_script);
        if (MiniSite.Browser.ie) {
            _script.onreadystatechange = function () {
                if (this.readyState == 'loaded' || this.readyStaate == 'complete') {
                    //fCallback();
                    if (fCallback != undefined) {
                        fCallback();
                    }

                }
            };
        } else if (MiniSite.Browser.moz) {
            _script.onload = function () {
                //fCallback(); 
                if (fCallback != undefined) {
                    fCallback();
                }
            };
        } else {
            //fCallback();
            if (fCallback != undefined) {
                fCallback();
            }
        }
    }
};

/**
    * 动态加载CSS
    * @param {string} url 样式地址
    */
function dynamicLoadCss(url) {
    var head = document.getElementsByTagName('head')[0];
    var link = document.createElement('link');
    link.type = 'text/css';
    link.rel = 'stylesheet';
    link.href = url;
    head.appendChild(link);
}

document.write('<script src="../../Resources/js/jquery-2.0.3.min.js"></script>');
document.write('<link href="../../Resources/lib/layui/css/layui.css" rel="stylesheet" />');
document.write('<link href="../../Resources/css/base.css" rel="stylesheet" />');
//document.write('<link href="../../Resources/iconfont/iconfont.css" rel="stylesheet" />');
document.write('<link href="../../Resources/css/cover.css" rel="stylesheet" />');
document.write('<script src="../../Resources/js/jquery.cookie.js"></script>');
document.write('<script src="../../Resources/lib/layui/layui.js"></script>');
document.write('<script src="../../Resources/lib/layui/layui.all.js"></script>');
document.write('<script src="../../config.js"></script>');


var loadJs = function (url) {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = url;
    head.appendChild(script);

    return new Promise(function (resolve) {
        script.onload = script.onreadystatechange = function () {
            if (!this.readyState || this.readyState === "loaded" || this.readyState === "complete") {
                script.onload = script.onreadystatechange = null;
                resolve();
            }
        };
    });
};

var require = function (urls) {
    return Promise.all(urls.map(function (url) {
        return loadJs(url);
    }));
};

// 搜索框的显示隐藏
var showSearch = function (className) {
    if (className) {
        $('.custom-searchWrap.' + className).toggleClass("isshow");
        $('.custom-searchWrap.' + className).animate({ height: 'toggle' }, 100);
    } else {
        $('.custom-searchWrap').toggleClass("isshow");
        $('.custom-searchWrap').animate({ height: 'toggle' }, 100);
    }

}

// 执行搜索功能
var cancelSearch = function () {
    $('.custom-searchWrap').css('display', 'none');
}
// 单击双击事件标识
var clickFlag = null;
//初始化列表数据
//elemid:tableid
//url:路径
//查询条件
//cols:列
//function DataView(elemid, url, where, cols) {
function DataTableView(parameter) {
    //layer.msg('数据请求中', { icon: 16, width: '180px', shade: 0.001, time: 600 });
    var _this = this;
    var options = parameter;
    this.selectRow = false;
    this.selectRowNumbers = 0;
    this.where = options.where;
    var tableInfo;
    var tableuse;
    this.table;
    this._checkbox = 0;// 当前数据表格是否有复选框

    this.init = function () {
        tableuse = layui.use('table', function () {
            _this.table = layui.table;
            tableInfo = _this.table.render({
                method: options.method ? options.method : 'post',
                id: options.id ? options.id : 'list',
                elem: '#' + (options.id ? options.id : 'list'),
                where: options.where,
                url: options.url,
                data: options.data,
                page: options.page == undefined ? true : options.page, //开启分页(参数page=false,只能判断undefined)
                cols: options.cols,
                limit: options.limit || 10,
                loading: options.loading ? options.loading : true,
                done: options.done ? options.done : function (res, curr, count) {

                }, request: {
                    pageName: 'page',
                    limitName: 'rows'
                }
            });
            //// 注册table，并将当前点击行的数据转为字符串并保存到selectRow
            //table.on('tool(' + options.id + ')', function (obj) {
            //    _this.selectRow = obj.data; //获得当前行数据
            //});
        });
    }
    this.init();

    this.AllRows = function () {//获取所有行 返回数组对象
        return tableuse.table.cache[options.id];
    }

    //加载数据
    this.load = function (data) {//通过数组对象，重新刷新列表  会清空table的url避免刷新table导致修改的数据还原为url数据
        //通过数组对象，重新刷新列表  会清空table的url避免刷新table导致修改的数据还原为url数据
        var tmpurl = options.url;
        options.elem = options.elem ? options.elem : ("#" + options.id);
        options.url = null;
        options.data = data;
        options.limit = data.length + 1;
        options.where = {};
        _this.table.render(options);
    }


    //刷新
    this.reload = function () {
        tableInfo.reload({
            where: _this.where,
            page: _this.getOptions().page == false ? false : { curr: 1 }
        });
        this.selectRow = false;
    }

    this.getOptions = function () {//获取载入参数
        return options;
    }
    this.getTableContent = function () {//获取table上下文
        return tableInfo;
    }

    this.updateRow = function (index, data) {//更新索引所在的行
        if (index != undefined && index != 'undefined' && data) {
            var allData = _this.AllRows();
            var loadData = [];
            $.each(allData, function () {
                if (this.LAY_TABLE_INDEX == index) {
                    loadData.push(data);
                } else {
                    loadData.push(this);
                }
            });
            _this.load(loadData);
        }
    }

    this.appendAfterRow = function (data) {//在最后面插入行
        if (data) {
            var allData = _this.AllRows();
            if (allData) {
                for (var i = 0; i < data.length; i++) {
                    var flag = false;
                    for (var j = 0; j < allData.length; j++) {
                        if (allData[j].ID == data[i].ID) {
                            flag = true;
                            continue;
                        }
                    }
                    if (!flag) {
                        if (data[i].LAY_CHECKED) {
                            data[i].LAY_CHECKED = false;
                        }
                        allData.push(data[i]);
                    }

                }
            } else {
                allData = new Array();
                for (var i = 0; i < data.length; i++) {
                    allData.push(data[i]);
                }
            }
            _this.load(allData);
        }
    }
    this.appendAfterNewRow = function (data) {//在最后面插入新行
        if (data) {
            var allData = _this.AllRows();
            if (allData) {
                for (var i = 0; i < data.length; i++) {
                    allData.push(data[i]);
                }
            } else {
                allData = new Array();
                for (var i = 0; i < data.length; i++) {
                    allData.push(data[i]);
                }
            }
            _this.load(allData);
        }
    }
    this.removeAfterRow = function (data) {//选中的数据
        if (data) {
            if (!Array.isArray(data)) {
                data = Array(data);
            }
            var allData = _this.AllRows();
            var newData = [];
            $.each(allData, function (index, item) {
                var isContain = false;
                for (var i = 0; i < data.length; i++) {
                    if (item.ID == data[i].ID) {
                        isContain = true;
                        break;
                    }
                }
                if (!isContain) {
                    newData.push(item);
                }
            })
            _this.load(newData);
        }
    }
    this.appendBeforeRow = function (data) {//在最前面插入一行
        if (data) {
            var allData = _this.AllRows();
            var loadData = [];
            loadData.push(data);
            $.each(allData, function () {
                loadData.push(this);
            });
            _this.load(loadData);
        }
    }
    this.insertRow = function (index, data) {//在索引后插入一行
        if (index != undefined && index != 'undefined' && data) {
            var allData = _this.AllRows();
            var loadData = [];
            $.each(allData, function () {
                loadData.push(this);
                if (this.LAY_TABLE_INDEX == index) {
                    loadData.push(data);
                }
            });
            _this.load(loadData);
        }
    }
    this.deleteRow = function (index) {//通过索引删除一行
        if (index != undefined && index != 'undefined') {
            var allData = _this.AllRows();
            var loadData = [];
            $.each(allData, function () {
                if (this.LAY_TABLE_INDEX != index) {
                    loadData.push(this);
                }
            });
            _this.load(loadData);
        }
    }

    // layui原生table选中行 checkbox勾选并且给行添加选中class
    // idname是table的ID
    this.getTableSelectData = function () {
        $(document).on("click", "#" + options.id + " + div .layui-table-body table.layui-table tbody tr", function () {
            if (clickFlag) {//取消上次延时未执行的方法
                clickFlag = clearTimeout(clickFlag);
            }
            var _row = this;
            clickFlag = setTimeout(function () {

                // 判断table是否有checkbox
                _row._checkbox = $('.layui-form-checkbox', _row).length;
                if (_row._checkbox > 0) {  // 存在checkbox，table可以选择多行数据进行操作
                    var index = $(_row).attr('data-index');
                    var tableBox = $(_row).parents('.layui-table-box');
                    //存在固定列
                    if (tableBox.find(".layui-table-fixed.layui-table-fixed-l").length > 0) {
                        tableDiv = tableBox.find(".layui-table-fixed.layui-table-fixed-l");
                    } else {
                        tableDiv = tableBox.find(".layui-table-body.layui-table-main");
                    }
                    var checkCell = tableDiv.find("tr[data-index=" + index + "]").find("td div.laytable-cell-checkbox div.layui-form-checkbox");
                    if (checkCell.length > 0) {
                        checkCell.click();
                    }
                    var checkStatus = _this.table.checkStatus(options.id);
                    _this.selectRow = checkStatus.data;
                    _this.selectRowNumbers = checkStatus.data.length;
                    if (_this.selectRowNumbers == 0) {
                        _this.selectRow = false;
                    }
                } else {
                    _this.selectRow = _this.AllRows()[$(_row).attr('data-index')];

                    // 否则table只能选则一行进行操作
                    $(_row).parent().find('tr').removeClass('data-selected');
                    $(_row).toggleClass('data-selected');
                }
            }, 50);
        });

        $(document).on("click", "td div.laytable-cell-checkbox div.layui-form-checkbox", function (e) {
            e.stopPropagation();
        });
        _this.table.on('checkbox(' + options.id + ')', function (obj) {
            var one = obj.data;
            if (obj.type == "all") {
                //$("#" + option.id + " + div .layui-table-body tr").toggleClass('data-selected');
                if (obj.checked) {
                    $("#" + options.id + " + div .layui-table-body tr").addClass('data-selected');
                    _this.selectRow = _this.table.cache.list ? _this.table.cache.list : _this.table.cache[options.id];
                    _this.selectRowNumbers = _this.selectRow ? _this.selectRow.length : 0;
                  
                } else {
                    $("#" + options.id + " + div .layui-table-body tr").removeClass('data-selected');
                    _this.selectRow = false;
                    _this.selectRowNumbers = 0;
                    if (_this.sealUp) {
                        $("#btnSealUp").attr('disabled', false);
                        $("#btnEnable").attr('disabled', false);
                    }
                    if (_this.AreaStop) {
                        if (parameter.child) {
                            $("#btnChildStop").attr('disabled', false);
                            $("#btnChildStart").attr('disabled', false);
                        } else {
                            $("#btnStop").attr('disabled', false);
                            $("#btnStart").attr('disabled', false);
                        }

                    }
                }
            }
            if (obj.type == "one") {
                $("#" + options.id + " + div .layui-table-body tr[data-index='" + obj.data.LAY_TABLE_INDEX + "']").toggleClass('data-selected');
                var checkStatus = _this.table.checkStatus(options.id);
                _this.selectRow = checkStatus.data;
                _this.selectRowNumbers = checkStatus.data.length;
            }
        });
    }
    this.getTableSelectData();

    // 返回选中行的数据
    this.isSelectRow = function () {
        if (this.selectRow) {//&& this.selectRow.length > 0
            return this.selectRow;
        } else {
            layer.msg('请选择数据！');
            return false;
        }
    }
}


//重置搜索条件
var resetSearch = function (form) {
    var id = form ? ('#' + form) : 'form';
    $(id)[0].reset();
    $(id + ' input[data-id]').each(function () {
        $(this).attr("data-id", '0');
    });
}

//获取表单值
var getFormValue = function (form) {
    var id = form ? ('#' + form) : 'form';
    var value = {};
    var t = $(id).serializeArray();

    $.each(t, function () {
        var checkBox = "";
        var mm = $(id).find('[name=' + this.name + ']');
        if (mm.length > 2) {
            $.each(mm, function (index, item) {
                if (item.checked) {
                    checkBox += (item.value) + ",";
                }
            });
            value[this.name] = checkBox;
        } else {
            var val = $(id).find('[name=' + this.name + ']').attr('data-id');
            value[this.name] = (val == undefined ? this.value : val);
        }

    });
    return value;
}
//设置表单值（data-id属性的多写一次） container 可以是div
var setFormValue = function (data, container) {
    var id = container ? ('#' + container) : 'form';
    var keys = Object.keys(data);
    $.each(keys, function (index, item) {
        var ht = $(id).find('[name=' + item + ']');
        if (ht.length > 0) {
            if (ht.is('label') || ht.is('span')) {
                var val = data[item] == null ? "" : data[item] + "";
                if (val != "") {
                    if (val == "true") {
                        val = "是";
                    } else if (val == "false") {
                        val = "否";
                    }
                }

                ht.text(val);
            } else {
                if (ht.attr('data-id')) {
                    ht.attr('data-id', data[item])
                } else {
                    var val = data[item] == null ? "" : "" + data[item];
                    //单选框设置选中
                    if (ht.length > 1) {
                        $.each(ht, function (index1, item1) {
                            if (val == item1.value + "") {
                                $(item1).attr("checked", true);
                            } else {
                                $(item1).attr("checked", false);
                            }
                        });
                    } else {
                        ht.val(val);
                    }
                }
            }
        }
    })
}
//注册日期事件
// bool ：false 默认单选时间，true可以选择时间开始结束的范围
// time类型：字符串，'year'，'month'，'time'，'datetime'
var registerDate = function (time, bool) {
    var laydate = layui.laydate;
    $('.time-input').each(function (i, item) {
        laydate.render({
            elem: '#' + item.id,
            type: time ? time : 'date',
            range: bool ? '~' : false
        });
    });
}

//注册开始日期和结束时间约束
// starID  开始控件ID
// endID  结束控件ID
// time类型：字符串，'year'，'month'，'time'，'datetime'
var registerStarAndEndDate = function (starID, endID, time) {
    var laydate = layui.laydate;

    cartimeDate = laydate.render({
        elem: '#' + starID //
        , type: time ? time : 'date'
        , format: 'yyyy-MM-dd'
        , done: function (value, date) {
            if (value == "") {
                returntimeDate.config.min.year = Number.MIN_VALUE;
                returntimeDate.config.min.month = Number.MIN_VALUE;
                returntimeDate.config.min.date = Number.MIN_VALUE;
                returntimeDate.config.min.hours = Number.MIN_VALUE;
                returntimeDate.config.min.minutes = Number.MIN_VALUE;
                returntimeDate.config.min.seconds = Number.MIN_VALUE;
            } else {
                returntimeDate.config.min.year = date.year;
                returntimeDate.config.min.month = date.month - 1;
                returntimeDate.config.min.date = date.date;
                returntimeDate.config.min.hours = date.hours;
                returntimeDate.config.min.minutes = date.minutes;
                returntimeDate.config.min.seconds = date.seconds;
            }
        }
    });

    returntimeDate = laydate.render({
        elem: '#' + endID //
        , type: time ? time : 'date'
        , format: 'yyyy-MM-dd'
        , done: function (value, date) {
            if (value == "") {
                cartimeDate.config.max.year = Number.MAX_VALUE;
                cartimeDate.config.max.month = Number.MAX_VALUE;
                cartimeDate.config.max.date = Number.MAX_VALUE;
                cartimeDate.config.max.hours = Number.MAX_VALUE;
                cartimeDate.config.max.minutes = Number.MAX_VALUE;
                cartimeDate.config.max.seconds = Number.MAX_VALUE;
            } else {
                cartimeDate.config.max.year = date.year;
                cartimeDate.config.max.month = date.month - 1;
                cartimeDate.config.max.date = date.date;
                cartimeDate.config.max.hours = date.hours;
                cartimeDate.config.max.minutes = date.minutes;
                cartimeDate.config.max.seconds = date.seconds;
            }
        }
    });
}

function getDateArray(date) {//获取时间数组

    var darray = {};
    darray.year = date.year;
    darray.month = date.month - 1;
    var day = date.date;
    if (date.hours == 23 && date.minutes == 59 && date.seconds == 59) {
        day = day + 1;
    } else {
        darray.hours = date.hours;
        darray.minutes = date.minutes;
        darray.seconds = date.seconds;
    }
    darray.date = day;
    return darray;
}
//修改的时候，重新设置时间控件范围
function registerDateByEdit(startDate, endDate) {
    if (returntimeDate) {
        var sdt = new Date(startDate);
        returntimeDate.config.min = getDateArray({ year: sdt.getFullYear(), month: sdt.getMonth() + 1, date: sdt.getDate(), hours: sdt.getHours(), minutes: sdt.getMinutes(), seconds: 0 });
    }
    if (cartimeDate && endDate) {
        var edt = new Date(endDate);
        cartimeDate.config.max = getDateArray({ year: edt.getFullYear(), month: edt.getMonth() + 1, date: edt.getDate(), hours: edt.getDate(), minutes: edt.getMinutes(), seconds: 0 });
    }
}


//弹窗
// param对象包含的属性：
// type：弹出层样式，2表示iframe层
// title：弹出层标题，在上级以判断并获取
// width height：弹出层的宽高比例
// url：弹出层iframe的url地址
var PopupPage = function (param, callback) {
    parent.layer.open({
        type: 2,
        scrollbar: true,
        title: param.title,
        maxmin: param.maxmin == undefined ? false : param.maxmin,
        area: [param.width == undefined ? '80%' : param.width, param.height == undefined ? '80%' : param.height],
        content: param.iframName ? param.url.indexOf('?') > 0 ? param.url += '&iframName=' + param.iframName : param.url += '?iframName=' + param.iframName : param.url,
        end: callback,
        cancel: function (index, layero) {
            layer.close(index);//关闭当前页
        }
    });
}
//当前页弹窗
var PopupCurrentPage = function (param, callback) {
    layer.open({
        type: 2,
        scrollbar: true,
        maxmin: true,
        title: param.title,
        maxmin: param.maxmin == undefined ? false : param.maxmin,
        area: [param.width == undefined ? '80%' : param.width, param.height == undefined ? '80%' : param.height],
        content: param.iframName ? param.url.indexOf('?') > 0 ? param.url += '&iframName=' + param.iframName : param.url += '?iframName=' + param.iframName : param.url,
        end: callback,
        cancel: function (index, layero) {
            layer.close(index);//关闭当前页
        }
    });
}
//弹窗2(默认带按钮)
var PopupControl = function (param, callback1, callback2) {
    parent.layer.open({
        type: 2,
        title: param.title,
        scrollbar: true,
        maxmin: param.maxmin == undefined ? false : param.maxmin,
        area: [param.width == undefined ? '80%' : param.width, param.height == undefined ? '80%' : param.height],
        content: param.url,
        btn: ['确定', '清空', '关闭'],
        yes: function (index) {
            callback1(index);
            layer.close(index);
        },
        btn2: function (index) {
            callback2(index)
        }
    });
}

//弹窗3(默认带按钮)
var PopupControl2 = function (param, callback1) {
    layer.open({
        type: 2,
        title: param.title,
        scrollbar: true,
        maxmin: param.maxmin == undefined ? false : param.maxmin,
        area: [param.width == undefined ? '80%' : param.width, param.height == undefined ? '80%' : param.height],
        content: param.url,
        btn: ['确定', '关闭'],
        yes: function (index) {
            if (callback1(index) == 0) { } else
                layer.close(index);
        }
    });
}

//弹窗3(默认带按钮)
var PopupControl3 = function (param, callback1) {
    layer.open({
        type: 2,
        title: param.title,
        scrollbar: true,
        maxmin: param.maxmin == undefined ? false : param.maxmin,
        area: [param.width == undefined ? '80%' : param.width, param.height == undefined ? '80%' : param.height],
        content: param.url,
        btn: '关闭',
        yes: function (index) {
            if (callback1(index) == 0) { } else
                layer.close(index);
        }
    });
}
//关闭弹窗
var closePopupPage = function (index) {
    if (index) {
        //setTimeout(function () {
        layer.close(index);
        //}, 100); 
    }
    if (parent.layer) {
        //setTimeout(function () {
        //当你在iframe页面关闭自身时
        index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
        parent.layer.close(index); //再执行关闭  
        //}, 100);

    } else {
        //setTimeout(function () {
        //如果你想关闭最新弹出的层，直接获取layer.index即可
        layer.close(layer.index); //它获取的始终是最新弹出的某个层，值是由layer内部动态递增计算的
        //}, 100);
    }
}

//弹窗提示
//使用之前调用/View/js/baseframe/js/layer/layer.js
//content:提示内容
var showInfo = function (type, title, content) {
    layer.msg(content, {
        icon: (type.toLowerCase() == 'success' ? 1 : 2)
    });
}

//询问弹窗
//使用之前调用/View/js/baseframe/js/layer/layer.js
//content:提示内容
//callback:回调方法
//jsondata:需要参数 格式{key:value}
var confirmShow = function (content, callback, jsondata) {
    //询问框
    layer.confirm(content, {
        btn: ['取消', '确定'] //按钮
    }, function () {
        layer.close(layer.index);
    }, function () {
        if (jsondata) {
            callback(jsondata);
        } else {
            callback();
        }
    });
}

//获取url中的参数
var getUrlParam = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return decodeURI(r[2]); return null; //返回参数值
}

//jquery请求
//data:请求需要参数
//callback：回调函数
var jqueryRequest = function (data, callback) {
    $.ajax({
        type: "POST",
        url: data.url,
        data: { cmd: data.cmd, JsonData: data.JsonData },
        success: function (data) {
            var result = JSON.parse(data);
            showPrompt(result, callback);
            //if (result.errNum == "0") {
            //    layer.msg(result.retMsg, { icon: 1 });
            //    if (callback) { callback(); }
            //} else {
            //    layer.msg(result.retMsg, { icon: 5 });
            //}
            //return false;
        },
        error: function (data) {
            layer.msg(data, { icon: 5 });
        }
    });
}

//jquery请求
//data:请求需要参数
//callback：回调函数（返回数据）
var jqueryRequestData = function (data, callback) {
    $.ajax({
        type: "POST",
        url: data.url,
        data: { cmd: data.cmd, JsonData: data.JsonData },
        success: function (data) {
            //var result = JSON.parse(data);
            var result = data;
            if (result.code == '0' || result.errNum == "0") {
                if (callback) { callback(result.Data || result.data); }
            } else {
                layer.msg(result.retMsg || result.msg, { icon: 5 });
            }
            return false;
        },
        error: function (data) {
            layer.msg(data, { icon: 5 });
        }
    });
}
//页面请求
//url:控件页面地址
//target:控件页面所需要选择器
//control:页面所要存的选择器
var pageRequest = function (param) {
    $.ajax({
        url: param.url, //这里是静态页的地址
        type: "GET", //静态页用get方法，否则服务器会抛出405错误
        success: function (data) {
            var result = $(data).find(param.target);
            $(param.control).after(result);
        }
    });

}



///----------------获取当前日期-------------------
var getDate = function () {
    var mydate = new Date();
    var str = "" + mydate.getFullYear() + "-";
    str += (mydate.getMonth() + 1) + "-";
    var day = mydate.getDate();
    str += day.toString().length == 1 ? "0" + day.toString() : day.toString();
    return str;
}

//局部打印案例
function doPrint() {
    new print().doPrint();
}
//过度数据
var over_data = {};
///过度存储数据
var setExcessive = function (data) {
    over_data = data;
}

var getExcessive = function () {
    return over_data;
}

// param对象包含的属性：
// type：弹出层样式，2表示iframe层()
// title：弹出层标题，在上级以判断并获取
// width：弹出层的宽 默认80%
// height：弹出层的宽高 默认80%
// url：弹出层iframe的url地址,或者要执行的后台方法地址，比如删除操作
// maxmin: 弹出层能否全屏，默认true
// cmd:要执行的后台方法名，比如删除操作：cmd：del

//添加按钮
var add = function (param, callback) {
    if (param.title == undefined) {
        param.title = "添加";
    }
    if (param.url == undefined || param.url == "") {
        alert("请填写添加页面地址！");
    } else {
        //是否父级弹窗,默认为true
        var isParent = param.isParent != undefined ? param.isParent : true;
        if (isParent) {
            PopupPage(param, callback);
        } else {
            PopupCurrentPage(param, callback);
        }
    }
}

//查看
var view = function (param, callback) {
    if (!param.url) {
        layer.msg('请填写查看页面Url！');
        return false;
    }
    if (!param.ID) {
        if (param.Data.selectRowNumbers > 1) {
            layer.msg('请选择一行数据进行操作！')
            return false;
        } else if (param.Data.isSelectRow()) {
            var _Key = param.Key == undefined ? 'ID' : param.Key;
            if (Array.isArray(param.Data.selectRow)) {//多选
                param.ID = param.Data.selectRow[0][_Key];
            } else {
                param.ID = param.Data.selectRow[_Key];
            }
        } else {
            return false;
        }
    }
    param.title = param.title == undefined ? "查看" : param.title;
    param.url = (param.url + '?' + (param.var == undefined ? "ID" : param.var) + '=' + param.ID);
    //是否父级弹窗,默认为true
    var isParent = param.isParent != undefined ? param.isParent : true;
    if (callback) {
        if (isParent) {
            PopupPage(param, callback);
        } else {
            PopupCurrentPage(param, callback);
        }
    } else {
        if (isParent) {
            PopupPage(param);
        } else {
            PopupCurrentPage(param);
        }
    }
}

//修改
var editData = function (param, callback) {
    if (param.Data.selectRowNumbers > 1) {
        layer.msg('请选择一行数据进行操作！')
        return false;
    } else if (param.Data.isSelectRow()) {
        var _Key = param.Key == undefined ? 'ID' : param.Key;
        if (Array.isArray(param.Data.selectRow)) {//多选
            param.ID = param.Data.selectRow[0][_Key];
        } else {
            param.ID = param.Data.selectRow[_Key];
        }

        // //增加传传参
        if (param.url == undefined) {
            layer.msg('请填写添加页面');
        } else {
            if (param.url.indexOf("?") != -1) {
                param.url = (param.url + '&ID=' + param.ID);
            } else {
                param.url = (param.url + '?ID=' + param.ID);
            }
        }

        // param.url = param.url == undefined ? function () { layer.msg('请填写添加页面'); } : (param.url + '?ID=' + param.ID);
        param.title = param.title == undefined ? "修改" : param.title;
        //是否父级弹窗,默认为true
        var isParent = param.isParent != undefined ? param.isParent : true;
        if (isParent) {
            PopupPage(param, callback);
        } else {
            PopupCurrentPage(param, callback);
        }

    }

}

//自定义删除
// callback 数据刷新的回掉函数
var confirmDel = function (param, callback) {
    if (param.Data.isSelectRow()) {
        var _Key = param.Key == undefined ? 'ID' : param.Key;
        var id = "";
        if (Array.isArray(param.Data.selectRow)) {//多选
            $.each(param.Data.selectRow, function (index, item) {
                id += item[_Key] + ',';
            })
            id = id.substring(0, id.length - 1);
        } else {
            id = param.Data.selectRow[_Key];
        }
        var data = { ID: id };
        param.data = data;
        param.callback = callback;
        confirmShow('确定要删除吗？', delItem, param);
    }
}

//删除
var delItem = function (param) {
    $.ajax({
        type: "POST",
        url: param.url,
        data: { cmd: param.cmd, JsonData: JSON.stringify(param.data) },
        async: false,
        success: function (data) {
            var result = JSON.parse(data);
            showPrompt(result, param.callback);
        },
        error: function (data) {
            showInfo('error', '提示', result.retMsg);
        }
    });
}
//加载等待层
//msg：提示信息
//leftpx：根据提示信息长短调试显示位置
var layerLoad = function (msg, leftPX) {
    var index = parent.layer.load(0, {
        shade: 0.3,
        success: function (layero) {
            layero.find('.layui-layer-content').css({ 'padding-top': '40px', 'width': '300px' });
            layero.find('.layui-layer-content').append(" <div style='margin-left:" + leftPX + ";font-size:16px;'>" + msg + "</div>");
        }
    })
    return index
}
var showPrompt = function (result, callback) {
    if (result.code != undefined) {
        if (result.code == "0") {

            if (result.msg) {
                showInfo('success', '提示', result.msg);
            }
            if (callback) {
                callback(result.data);
            }

        } else {
            showInfo('warning', '提示', result.msg);
        }
    } else {
        if (result.errNum == "0") {
            if (result.retMsg) {
                showInfo('success', '提示', result.retMsg);
            }
            if (callback) {
                callback(result.Data);
            }

        } else {
            showInfo('warning', '提示', result.retMsg);
        }
    }
    return false;
}
