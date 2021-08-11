//Nav Controls
$('#new').click(function () {
    $("#docNumber").val("");
    $('#docNumber').prop("disabled", true);
    createNew();
});

$('#find').click(function () {
    $("#docNumber").val("");
    $('#docNumber').prop("disabled", false);

});
$('#first').click(function () {
    $('#docNumber').prop("disabled", true);
    getFilteredData($("#docNumber").val(), 'DOCNUM', 'first');
});
$('#last').click(function () {

    $('#docNumber').prop("disabled", true);
    getFilteredData($("#docNumber").val(), 'DOCNUM', 'last');
});
$('#previous').click(function () {
    $('#docNumber').prop("disabled", true);
    getFilteredData($("#docNumber").val(), 'DOCNUM', 'previous');
});
$('#next').click(function () {
    $('#docNumber').prop("disabled", true);
    getFilteredData($("#docNumber").val(), 'DOCNUM', 'next');

});

//End Nav Controls

//DropDown Initialization
function GenerateSAPFunctionDropDown(jsonResponse, id, DisplayExpr, ValueExpr, Placeholder) {
    $(id).dxSelectBox({
        cacheEnabled: false,
        items: jsonResponse,
        displayExpr: DisplayExpr,
        valueExpr: ValueExpr,
        placeholder: Placeholder,
        cssclass: "form-control frm-cstm",
        searchEnabled: true,
        readonly: false,
        onValueChanged: function (data) {
            var val = data.value;
            $(id).val(val);
        }
    });
}
//End DropDown

//Input only Numbers
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    //var approvaleList = Table.length;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        AlertToast('error', "Please enter numbers only!");
        return false;
    }
    return true;
}