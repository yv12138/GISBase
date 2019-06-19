var id = "";
$(function () {
    id = getUrlParam("BSM");
    var data = {
        type: "POST",
        url: api_server + "/api/HTMethod/GetPersonInfo",
        JsonData: JSON.stringify({ BSM: id })
    }
    jqueryRequestData(data, setData);

});

var setData = function (result) {
    setFormValue(result.data, "div_view");
}



