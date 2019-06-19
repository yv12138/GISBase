
$(function () {
    getUserInfo();
})

var logout = function () {
    layer.confirm('是否注销?', {
        icon: 3, title: '提示', btn: ['取消', '注销'], btn2: function (index, layero) {
            Logout_Click();
        }
    });
}
var changeUser = function () {
    layer.confirm('是否切换账号?', {
        icon: 3, title: '提示', btn: ['取消', '切换'], btn2: function (index, layero) {
            Logout_Click();
        }
    });
}
// 获取当前登录用户信息
var getUserInfo = function () {
    $.ajax({
        type: "POST",
        url: "/Controllers/LoginHandler.ashx",
        data: { cmd: "getuserinfo" },
        async: false,
        success: function (result) {
            var data = JSON.parse(result);
            if (data.code == 0) {
                if (data.data.PersName == null) {
                    alert("登录超时，请重新登录！");
                    window.location = '/View/Login.html';
                    return;
                }
                $("#userName").html(data.data.PersName)
            }
            else {
                toastr.error(data.msg + "可点此处请重新登录！",
                    '错误消息',
                    {
                        closeButton: true,
                        timeOut: 0,
                        positionClass: 'toast-top-center',
                        onclick: function () {
                            window.location = '/View/Login.html';
                        }
                    });
                return false;
            }
        },
        error: function (data) { }
    });
}
// 注销
var Logout_Click = function () {
    $.ajax
        ({
            type: "POST",
            url: "/Controllers/LoginHandler.ashx",
            data: { cmd: "logout" },
            async: false,
            success: function (data) {
                var result = JSON.parse(data);
                if (result.code == 0) {
                    $.cookie('.ASPAUTH', "", { path: '/', expires: -7 });
                    window.location = '/View/Login.html';
                } else {
                    showInfo("error", "错误", result.msg);
                    return;
                }
            },
            error: function (data) {

            }
        });
}