var ReverseJE;
var WIPRecordingFormList = [];

function createNew() {
    var abc = [];
    Table = [];

    //Header
    $("#Office").dxSelectBox({ value: "" });
    $("#DebitAccount").val("");
    $("#DebitAccount").dxDropDownBox({ value: "" });
    $("#CreditAccount").val("");
    $("#CreditAccount").dxDropDownBox({ value: "" });
    $("#ddYear").dxSelectBox({ value: "" });
    $("#ddMonth").dxSelectBox({ value: "" });
    $("#Currency").dxSelectBox({ value: "" });
    
    GenerateGrid(abc);
   
}



$('document').ready(function () {
    GetWIPRecordingForm();
    GenerateDateBox();
    GetData();
    GenerateYearMonthDropDown();
    GenerateGrid([]);
    GetDebitAccounts();
    GetCreditAccounts();
    GetSapBranch();
    GetCurrency();
    $('#WIPView').click(function () {
        var isSuccess = true;
        var message = '';
        var alertSetupObj = {
            BranchID: $("#Office").val(),
            AsOnDate: $("#AsOnDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value,
            ddMonth: $("#ddMonth").val(),
            ddYear: $("#ddYear").val(),
            DebitAccount: $("#DebitAccount").val(),
            CreditAccount: $("#CreditAccount").val(),
            Currency: $("#Currency").val(),
        };

        if (alertSetupObj.BranchID == "") {
            isSuccess = false;
            message = 'Please select Office. \n';
        }
        if (alertSetupObj.AsOnDate == "") {
            isSuccess = false;
            message = message + 'Please select as on Date. \n';
        }
        if (alertSetupObj.ddYear == "") {
            isSuccess = false;
            message = message + 'Please select Year. \n';
        }
        if (alertSetupObj.ddMonth == "") {
            isSuccess = false;
            message = message + 'Please select Month. \n';
        }
        if (alertSetupObj.DebitAccount == "") {
            isSuccess = false;
            message = message + 'Please select Debit Account. \n';
        }
        if (alertSetupObj.CreditAccount == "") {
            isSuccess = false;
            message = message + 'Please select Credit Account. \n';
        }
        if (alertSetupObj.Currency == "") {
            isSuccess = false;
            message = message + 'Please select Currency. ';
        }
        if (isSuccess) {
            var url = "/WIPRecordingForm/GetAssignments";
            var AsOnDate = $("#AsOnDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
            var day = AsOnDate.substr(0, 2);
            var month = AsOnDate.substr(3, 2);
            var year = AsOnDate.substr(8, 2);
            AsOnDate = month + '/' + day + '/' + year;
            var BranchID = $("#Office").val();
            if (AsOnDate == null || AsOnDate == undefined) {
                AlertToast('error', "Please select As On Date!");
            }
            else {
                loadPanel.show();
                $.ajax({
                    type: "GET",
                    url: url,
                    contentType: 'application/json',
                    data: { AsOnDate: AsOnDate, BranchID: BranchID },
                    dataType: "json",
                    async: false,
                    success: function (data) {

                        loadPanel.hide();

                        if (data.Success) {
                            var JSON_Response = data.response;
                            GenerateGrid(JSON_Response);
                            totalSum();
                        }
                        else {
                            AlertToast('error', "Please As On date!");
                            GenerateGrid([]);
                        }
                    },
                    error: function (data) {
                        loadPanel.hide();
                        console.log(data);
                    },
                    failure: function (data) {
                        loadPanel.hide();
                        console.log(data);
                    }

                });
            }

        }
        else {
            AlertToast('error', message);
        }
    });

    $('#add_update').click(function () {
        var isSuccess = true;
        var message = '';
        var alertSetupObj = {
            BranchID: $("#Office").val(),
            AsOnDate: $("#AsOnDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value,
            ddMonth: $("#ddMonth").val(),
            ddYear: $("#ddYear").val(),
            DebitAccount: $("#DebitAccount").val(),
            CreditAccount: $("#CreditAccount").val(),
            Currency: $("#Currency").val(),

        };

        if (alertSetupObj.BranchID == "") {
            isSuccess = false;
            message = 'Please select Office. \n';
        }
        if (alertSetupObj.AsOnDate == "") {
            isSuccess = false;
            message = message + 'Please select as on Date. \n';
        }
        if (alertSetupObj.ddYear == "") {
            isSuccess = false;
            message = message + 'Please select Year. \n';
        }
        if (alertSetupObj.ddMonth == "") {
            isSuccess = false;
            message = message + 'Please select Month. \n';
        }
        if (alertSetupObj.DebitAccount == "") {
            isSuccess = false;
            message = message + 'Please select Debit Account. \n';
        }
        if (alertSetupObj.CreditAccount == "") {
            isSuccess = false;
            message = message + 'Please select Credit Account. \n';
        }
        if (alertSetupObj.Currency == "") {
            isSuccess = false;
            message = message + 'Please select Currency. ';
        }
        if (isSuccess) {
            var result = DevExpress.ui.dialog.confirm("Are you sure you want to Post this in SAP B1?", "Confirm changes");
            result.done(function (dialogResult) {
                //alert(dialogResult ? "Confirmed" : "Canceled");
                var msgOccur =false;
                if (dialogResult == true) {
                    for (var i = 0; i < Table.length; i++) {
                        if (Table[i].WIPToBeCharged == 0) {
                            msgOccur = true;
                        }
                    }

                    if (msgOccur == true) {
                        Table = Table.filter(x=>x.WIPToBeCharged != 0);
                        var result2 = DevExpress.ui.dialog.confirm("This WIP From also have some <b>0</b> values which will not be posted in SAP Continue?", "Confirm changes");
                        result2.done(function (dialogResult) {
                            if (dialogResult == true) {
                                //Posting
                                PostWIP();
                            }
                            else {
                                AlertToast('error', "This WIP doesn't post right now.");
                            }
                        });
                    }
                    else {
                        PostWIP();
                    }
                    
                }
            });
            

        }
        else {
            AlertToast('error', message);
        }
    });

});

function PostWIP() {
    var AsOnDate = $("#AsOnDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
    //var day = AsOnDate.toString().substring(0, 2);
    //var month = AsOnDate.toString().substring(3, 2);
    //var year = AsOnDate.toString().substring(6, 4);
    var day = AsOnDate.toString().substring(0, 2);
    var month = AsOnDate.toString().substring(5, 3);
    var year = AsOnDate.toString().substring(14, 6);

    let formatted_date = year + "-" + month + "-" + day;
    var ReversalDate = $("#ReversalDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
    var day2 = ReversalDate.toString().substring(0, 2);
    var month2 = ReversalDate.toString().substring(5, 3);
    var year2 = ReversalDate.toString().substring(14, 6);
    ReversalDate = year2 + "-" + month2 + "-" + day2;

    var DocDate = $("#DocDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
    var day3 = DocDate.toString().substring(0, 2);
    var month3 = DocDate.toString().substring(5, 3);
    var year3 = DocDate.toString().substring(14, 6);
    DocDate = year3 + "-" + month3 + "-" + day3;

    var ReverseJE = $('#ReverseJE').is(":checked");

    var BranchID = $("#Office").val();
    var Header = {
        AsOnDate: formatted_date,
        DebitAccount: $("#DebitAccount").val(),
        CreditAccount: $("#CreditAccount").val(),
        BranchID: $("#Office").val(),
        ReversalDate: ReversalDate,
        ReverseJE: ReverseJE,
        DocDate: DocDate,
        Year: $("#ddYear").val(),
        Month: $("#ddMonth").val(),
        CurrencyID: $("#Currency").val(),
        WipTotal: $("#TotalWIP").val()
    };
    if (AsOnDate == null || AsOnDate == undefined) {
        AlertToast('error', "Please select As On Date!");
    }
    else {
        loadPanel.show();
        var url = "/WIPRecordingForm/PostToSBO";
        $.ajax({
            type: "POST",
            url: url,
            contentType: 'application/json',
            data: JSON.stringify({
                DOCNUM: $("#docNumber").val(),
                WipRecordingForm: Header,
                WipRecordingFormChild: Table,
                ID: 0
            }),
            dataType: "json",
            async: false,
            success: function (response) {
                loadPanel.hide();
                if (response.Success) {
                    AlertToast('success', response.Message);
                    setTimeout(function () {
                        window.location.reload();
                    }, 5000);
                }
                else
                    AlertToast('error', response.Message);
            },
            error: function (response) {
                console.log(response);
                loadPanel.hide();
            },
            failure: function (response) {
                console.log(response);
                loadPanel.hide();
            }

        })
    }
}

function GenerateDateBox() {
    var now = new Date();

    $("#AsOnDate").dxDateBox({
        type: "date",
        pickerType: "rollers",
        value: now,
        showClearButton: true,
        displayFormat: "dd/MM/yyyy",
        onValueChanged: function (data) {
            $("#AsOnDate").val(data.value);
        }
    });
    
    $("#ReversalDate").dxDateBox({
        type: "date",
        pickerType: "rollers",
        value: now,
        showClearButton: true,
        displayFormat: "dd/MM/yyyy",
        onValueChanged: function (data) {
            $("#ReversalDate").val(data.value);
        }
    });

    $("#DocDate").dxDateBox({
        type: "date",
        pickerType: "rollers",
        value: now,
        readonly: true,
        displayFormat: "dd/MM/yyyy",
        onValueChanged: function (data) {
            $("#DocDate").val(data.value);
        }
    
    });

    //.val(now);//.displayFormat("dd/MM/yyyy");
        //showClearButton: true,
      //  displayFormat: "dd/MM/yyyy",
        //sele: false
        //onValueChanged: function (data) {
        //    $("#DocDate").val(data.value);
        //}
   // });
    
    $('.dx-texteditor-input').attr('readonly', true);
}

//function GetAccounts() {
//    var url = "/AssignmentForm/GetAccounts";
//    var id = "#Client";
//    var DisplayExpr = "CLIENTNAME";
//    var ValueExpr = "CLIENTID";
//    var Placeholder = "Select Client...";
//    $.ajax({
//        url: url,
//        method: "GET",
//        data: {},
//    }).done(function (data) {
//        clientFunctions = data.response;
//        GenerateSAPFunctionDropDownClient(clientFunctions, id, DisplayExpr, ValueExpr, Placeholder);
//        GetAssignmentFormSummary();

//    }).fail(function (data) {
//    });
//}

var sapdebAccounts;
function GetDebitAccounts() {
    var url = "/WIPRecordingForm/GetAccounts";
    var id = "#DebitAccount";
    var ValueExpr = "AcctCode";
    var DisplayExpr = "AcctName";
    var Placeholder = "Select Debit Account...";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        sapdebAccounts = data.response;
        GenerateHCMACCDropDown(sapdebAccounts, id, DisplayExpr, ValueExpr, Placeholder);
    }).fail(function (data) {
    });
}

var sapcreAccounts;
function GetCreditAccounts() {
    var url = "/WIPRecordingForm/GetAccounts";
    var id = "#CreditAccount";
    var ValueExpr = "AcctCode";
    var DisplayExpr = "AcctName";
    var Placeholder = "Select Credit Account...";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        sapcreAccounts = data.response;
        GenerateHCMACCDropDown(sapcreAccounts, id, DisplayExpr, ValueExpr, Placeholder);
    }).fail(function (data) {
    });
}

function GenerateHCMACCDropDown(jsonData, id, DisplayExpr, ValueExpr, Placeholder) {
    $(id).dxDropDownBox({
        valueExpr: ValueExpr,
        displayExpr: DisplayExpr,
        placeholder: Placeholder,
        showClearButton: true,
        dataSource: jsonData,
        contentTemplate: function (e) {
            var value = e.component.option("value"),
                $dataGrid = $("<div>").dxDataGrid({
                    dataSource: e.component.getDataSource(),
                    columns: ["AcctCode", "AcctName"],
                    hoverStateEnabled: true,
                    paging: { enabled: true, pageSize: 10 },
                    filterRow: { visible: true },
                    scrolling: { mode: "infinite" },
                    height: 345,
                    selection: { mode: "single" },
                    selectedRowKeys: value,
                    onSelectionChanged: function (selectedItems) {

                        if (selectedItems != null) {
                            if (selectedItems != undefined) {
                                var keys = selectedItems.selectedRowKeys;
                                if (keys.length > 0) {
                                    e.component.option("value", keys[0].AcctCode);

                                    AcctCode = keys[0].AcctCode;
                                    AcctName = keys[0].AcctName;
                                    $(id).val(AcctCode);                                  
                                    $(id).dxDropDownBox("instance").close();
                                }

                            }
                        }


                    }
                });

            dataGrid = $dataGrid.dxDataGrid("instance");

            e.component.on("valueChanged", function (args) {
                var value = args.value;
                dataGrid.selectRows(value, false);
            });

            return $dataGrid;
        }
    });
}

function GetData() {
    monthList = [{
        "ID": 1,
        "Name": "January"
    },
       {
           "ID": 2,
           "Name": "February "
       }
       ,
       {
           "ID": 3,
           "Name": "March"
       }
       ,
       {
           "ID": 4,
           "Name": "April"
       }
       ,
       {
           "ID": 5,
           "Name": "May"
       }
       ,
       {
           "ID": 6,
           "Name": "June"
       }
       ,
       {
           "ID": 7,
           "Name": "July"
       }
       ,
       {
           "ID": 8,
           "Name": "August"
       }
       ,
       {
           "ID": 9,
           "Name": "September"
       }
       ,
       {
           "ID": 10,
           "Name": "October"
       }
       ,
       {
           "ID": 11,
           "Name": "November"
       }
       ,
       {
           "ID": 12,
           "Name": "December"
       }
    ];
}
var yearList = [];
function GenerateYearMonthDropDown() {

    var min = new Date().getFullYear();
    var max = min + 9;
    yearList = [];
    var year = {};
    for (var i = min; i <= max; i++) {
        year = { value: i };
        yearList.push(year);
    }

    $("#ddYear").dxSelectBox({
        //value: yearList[0].value,
        cacheEnabled: false,
        items: yearList,
        displayExpr: "value",
        valueExpr: "value",
        placeholder: "Select Year...",
        cssclass: "form-control frm-cstm",
        //searchEnabled: true,
        readonly: false,
        onValueChanged: function (data) {
            $("#ddYear").val(data.value);
            //var month = $("#ddMonth").dxSelectBox("instance").option('value');
        }
    });

    var date = new Date();

    $("#ddMonth").dxSelectBox({
        //value: date.getMonth() + 1,
        cacheEnabled: false,
        items: monthList,
        displayExpr: "Name",
        valueExpr: "ID",
        placeholder: "Select Month...",
        cssclass: "form-control frm-cstm",
        searchEnabled: true,
        readonly: false,
        onValueChanged: function (data) {
            $("#ddMonth").val(data.value);
        }
    });

}

var currency;
function GetCurrency() {
    var url = "/AssignmentForm/GetCurrencyFunction";
    var id = "#Currency";
    var DisplayExpr = "CurrencyName";
    var ValueExpr = "CurrencyID";
    var Placeholder = "Select Currency...";


    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        currency = data.response;
        GenerateSAPFunctionDropDown(currency, id, DisplayExpr, ValueExpr, Placeholder);

    }).fail(function (data) {
    });
}

var sapBranches;
function GetSapBranch() {
    var url = "/AssignmentForm/GetSapBranches";
    var id = "#Office";
    var DisplayExpr = "BranchName";
    var ValueExpr = "BranchID";
    var Placeholder = "Select Branch...";

    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        sapBranches = data.response;
        GenerateSAPFunctionDropDown(sapBranches, id, DisplayExpr, ValueExpr, Placeholder);

    }).fail(function (data) {
    });
}
var Table;
function GenerateGrid(JSON_Response) {
    Table = JSON_Response;
    $(function () {
        $("#gridContainer").dxDataGrid({
            dataSource: JSON_Response,
            keyExpr: "KEY",
            width: "1800px",
            showBorders: true,
            showScrollbar: 'always',
            filterRow: { visible: true },
            scrolling: {
                mode: "horizontal",
                showScrollbar: 'always'
            },
            paging: {
                enabled: false
            },
            editing: {
                mode: "form",
                //allowUpdating: true,
                //allowDeleting: true,
                //allowAdding: true,
                //confirmDelete: false
                //allowDeleting: true,
                useIcons: true
            },
            columns: [
            {
                dataField: "ID",
                allowEditing: false,
                visible: false,
            },
            //{
            //    dataField: "HeaderID",
            //    allowEditing: false,
            //    visible: false,
            //},
            {
                dataField: "SNO",
                caption: "S.No",
                dataType: "number",
                allowEditing: false,
                width: "45px"
            },
             {
                 dataField: "ClientID",
                 caption: "ClientID",
                 allowEditing: false,
                 visible: false
             },
             {
                 dataField: "ClientName",
                 caption: "Client",
                 allowEditing: false,
                 width: "15%"
             },
             //{
             //    dataField: "AssignmentID",
             //    caption: "AssignmentID",
             //    visible: false,
             //},
             {
                 dataField: "AssignmentCode",
                 caption: "Assignment Code",
                 allowEditing: false,
                 width: "10%"
             },
              {
                  dataField: "AssignmentTitle",
                  caption: "Assignment Title",
                  allowEditing: false,
                  width: "12%"
              },

              {
                  dataField: "PartnerID",
                  caption: "PartnerID",
                  allowEditing: false,
                  visible: false
              },
              {
                  dataField: "PartnerName",
                  caption: "Partner",
                  allowEditing: false,
                  width: "12%"
              },
              {
                  dataField: "FunctionID",
                  caption: "FunctionID",
                  allowEditing: false,
                  visible: false
              },
              {
                  dataField: "FunctionName",
                  caption: "Function",
                  allowEditing: false,
                  width: "10%"
              },
              {
                  dataField: "SubFunctionID",
                  caption: "SubFunctionID",
                  allowEditing: false,
                  visible:false
              },
              {
                  dataField: "SubFunctionName",
                  caption: "Sub Function",
                  allowEditing: false,
                  width: "13%"
              },
              {
                  dataField: "BilledToDate",
                  caption: "Billed To Date",
                  //validationRules: [{ type: "required" }],
                  dataType: "number",
                  allowEditing: false,
                  width: "7%"
              },
              {
                  dataField: "WIPAmountToDate",
                  caption: "WIP Amount To Date",
                  //validationRules: [{ type: "required" }],
                  dataType: "number",
                  allowEditing: false,
                  width: "9%"
              },
              {
                  dataField: "WIPNotCharged",
                  caption: "WIP Not Charged", /// Over Charged to Date
                  dataType: "number",
                  allowEditing: false,
                  width: "8%"
              },
              {
                  dataField: "WIPToBeCharged",
                  caption: "WIP To Be Charged",
                  dataType: "number",
                  allowEditing: false,
                  width: "9%"
              },
              {
                  dataField: "IsDeleted",
                  allowEditing: false,
                  visible: false,
                  dataType: 'boolean',
              },
            ],
            onEditorPreparing: function (e) {

            },
            onEditorPrepared: function (e) {
                //dxSelectBox
           
            },
            onEditingStart: function (e) {
            },
            //onInitNewRow: function (e) {
            //    e.data.ID = 0;
            //    e.data.IsDeleted = false;
            //    serialNo = serialNo + 1;
            //    e.data.SNO = serialNo;
            //},

           // onRowInserting: function (e) {
           ////     serialNo = serialNo + 1;
           //     e.data.SNo = serialNo;
           // },
           // onRowInserted: function (e) {

           //     var Details = {
           //         KEY: e.key,
           //         ID: 0,
           //         SNo: e.data.SNo,

           //         Client: e.data.Client,
           //         //AssignmentID: e.data.AssignmentID,
           //         AssignmentCode: e.data.AssignmentCode,
           //         AssignmentTitle: e.data.AssignmentTitle,
           //         Partner: e.data.Partner,
           //         Function: e.data.Function,
           //         SubFunction: e.data.SubFunction,
           //         BilledToDate: e.data.BilledToDate,
           //         WIPAmountToDate: e.data.WIPAmountToDate,
           //         WIPNotCharged: e.data.WIPNotCharged,
           //         WIPToBeCharged: e.data.WIPToBeCharged,
           //         IsDeleted: false
           //     };

           //     //let IfUserCodeExist = Table.filter(x=>x.USER_CODE == Details.USER_CODE)
           //     //if (IfUserCodeExist.length == 0) {
           //         Table.push(Details);
           //     //}
           // },

            onRowUpdating: function (e) {
                totalSum();
            },
            onRowUpdated: function (e) {
                $.each(Table, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                        
                    }

                });
            },
            onRowRemoving: function (e) {
            },
            onRowRemoved: function (e) {
                $.each(Table, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                        val.IsDeleted = true;
                        totalSum();
                        var serialNum = val.SNO - 1;
                        serialNo = val.SNO;
                        for (var i = serialNum; i < Table.length; i++) {
                            Table[i].SNO = serialNo;
                            serialNo++;
                        }
                    }
                });
            }
        });

    });
}

function totalSum() {
    var totalWIP = 0.00;

    for (var i = 0; i < Table.length; i++) {
        totalWIP += parseFloat(Table[i].WIPToBeCharged);
    }

    if (totalWIP == 'NaN')
        totalWIP = 0.00;

    $("#TotalWIP").val(parseFloat(totalWIP).toFixed(2));
}


function GetWIPRecordingForm() {
    var url = "/WIPRecordingForm/GetWIPRecordingForm";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        WIPRecordingFormList = data.response;
        //var Doc = data.Doc;
        console.log(WIPRecordingFormList);
        //$("#docNumber").val(Doc);
        var abc = [];
    }).fail(function (data) {
    });
}

function GeneratePopupFindGrid() {
    loadPanel.show();
    var ur = "/WIPRecordingForm/GetWIPRecordingForm";
    $.ajax({
        url: ur,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        var JSON_Response1 = [];
        JSON_Response1 = data.response;
        popupFindInstance = $("#FindContainer").dxDataGrid({
            dataSource: JSON_Response1,
            keyExpr: "DocNum",
            paging: { pageSize: 10 },
            columnAutoWidth: true,
            pager: { showInfo: true },
            allowColumnResizing: true,
            allowColumnReordering: true,
            showBorders: true,
            showScrollbar: 'always',
            filterRow: { visible: true },
            scrolling: {
                mode: "horizontal",
                showScrollbar: 'always'
            },
            selection: {
                mode: "single"
            },
            columns: [
                {
                    dataField: "DocNum",
                    caption: "DocNum"
                },
                {
                    dataField: "DocDate",
                    caption: "Doc Date",
                    dataType: "date",
                    format: "dd/MM/yyyy",
                },
                //{
                //    dataField: "DebitAccount",
                //    caption: "Debit Account",
                    
                //},
                //{
                //    dataField: "CreditAccount",
                //    caption: "Credit Account"
                //},
                {
                    dataField: "BranchID",
                    caption: "Branch",
                    lookup: {
                        dataSource: sapBranches,
                        valueExpr: "BranchID",
                        displayExpr: "BranchName",
                    }

                },
                {
                    dataField: "Month",
                    caption: "Month",
                    lookup: {
                        dataSource: monthList,
                        valueExpr: "ID",
                        displayExpr: "Name",
                    }
                    
                },
                {
                    dataField: "Year",
                    caption: "Year",
                    lookup: {
                        dataSource: yearList,
                        valueExpr: "value",
                        displayExpr: "value",
                    }
                    
                },
                {
                    dataField: "WipTotal",
                    caption: "WipTotal"
                }
            ]
        }).dxDataGrid("instance");

        $("#select-all-mode").dxSelectBox({
            dataSource: ["allPages", "page"],
            value: "allPages",
            onValueChanged: function (data) {
                dataGrid.option("selection.selectAllMode", data.value);
            }
        });

        $("#show-checkboxes-mode").dxSelectBox({
            dataSource: ["none", "onClick", "onLongTap", "always"],
            value: "onClick",
            onValueChanged: function (data) {
                dataGrid.option("selection.showCheckBoxesMode", data.value);
                $("#select-all-mode").dxSelectBox("instance").option("disabled", data.value === "none");
            }
        });

        popupFindInstance.clearSelection();

        loadPanel.hide();

    }).fail(function (data) {
        console.log(data);
        loadPanel.hide();
    });

}
$('#btnOk').click(function () {
    var selectedRow = popupFindInstance.getSelectedRowsData();
    if (selectedRow.length > 0) {
        $("#docNumber").val(selectedRow[0].DocNum);
        //$("#DocDate").val(selectedRow[0].DocNum);
        $("#DocDate").dxDateBox({ value: selectedRow[0].DocDate, text: selectedRow[0].DocDate });
        $("#AsOnDate").dxDateBox({ value: selectedRow[0].AsOnDate, text: selectedRow[0].AsOnDate });

        $("#Office").dxSelectBox({ value: selectedRow[0].BranchID });
        $("#ddMonth").dxSelectBox({ value: parseInt(selectedRow[0].Month) });
        $("#ddYear").dxSelectBox({ value: parseInt(selectedRow[0].Year) });
        $("#Currency").dxSelectBox({ value: selectedRow[0].CurrencyID });
        $("#DebitAccount").val(selectedRow[0].DebitAccount);
        $("#DebitAccount").dxDropDownBox({ value: selectedRow[0].DebitAccount });

        $("#CreditAccount").val(selectedRow[0].CreditAccount);
        $("#CreditAccount").dxDropDownBox({ value: selectedRow[0].CreditAccount });
        //$("#ReverseJE").dxCheckBox({
        //    value: selectedRow[0].ReverseJE
        //});
        $("#ReverseJE").prop("checked", selectedRow[0].ReverseJE);

        if (selectedRow[0].ReverseJE == true) {
            $("#ReversalDate").dxDateBox({ value: selectedRow[0].ReversalDate, text: selectedRow[0].ReversalDate });
        }

        //var approveStatus = selectedRow[0].Status;
        //if (approveStatus == 2) {
        //    $("#Submit").hide();
        //    $("#add_update").hide();
        //}
        //else if (approveStatus == 4) {
        //    $("#add_update").hide();
        //    $("#Submit").text("Post to SBO");
        //    if (flgPost == true) {
        //        $("#Submit").hide();
        //    }
        //    else {
        //        $("#Submit").show();
        //    }
        //}
        //else {
        //    $("#Submit").text(" Submit ");
        //    $("#Submit").show();
        //    $("#add_update").show();
        //}
        //$("#docNumber").val(selectedRow[0].DocNum);
        $('#docNumber').prop("disabled", true);
       
        Table = selectedRow[0].Table;
        GenerateGrid(selectedRow[0].Table);
        totalSum();
    }
    $('#myModal_Find').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();

});

function getFilteredData(id, type, Action) {
    idCopy = id;
    var AssignmentFormListCopy = [];
    let DocNum = '';
    if (type == 'DOCNUM') {

        if (Action == 'next') {
            let l1 = WIPRecordingFormList.map(m=> { return m.DocNum });
            l1 = l1.sort();
            var unique = l1.filter((v, i, a) => a.indexOf(v) === i);

            FilteredRecord = WIPRecordingFormList.filter(x=>x.DocNum == unique[0]);
            FilteredRecord.forEach(function (obj) {
                Table = obj.Table;
            })

            if ($("#docNumber").val() == "") {
                DocNum = unique[0];
                id = DocNum;
            }
            else {
                //DocNum = $("#docNumber").val().substr(4, $("#docNumber").val().length);
                DocNum = $("#docNumber").val();
                DocNum = parseInt(DocNum) + 1;
                if (DocNum > unique.length) {
                    return;
                }
                DocNum = DocNum.toString();
                //DocNum = 'ASS-' + DocNum.padStart(6, '0');
                DocNum = DocNum.padStart(7, '0');
                FilteredRecord = WIPRecordingFormList.filter(x=>x.DocNum == DocNum);
                FilteredRecord.forEach(function (obj) {
                    Table = obj.Table;
                })
                id = DocNum;
            }
        }
        else if (Action == 'previous') {

            let l1 = WIPRecordingFormList.map(m=> { return m.DocNum });
            l1 = l1.sort();

            var unique = l1.filter((v, i, a) => a.indexOf(v) === i);

            FilteredRecord = WIPRecordingFormList.filter(x=>x.DocNum == unique[unique.length - 1]);
            FilteredRecord.forEach(function (obj) {
                Table = obj.Table;
            })
            if ($("#docNumber").val() == "") {
                DocNum = unique[unique.length - 1];
                id = DocNum;
                //DocNum = AssignmentFormList[0].DocNum;
                //id = DocNum;
            }
            else {
                //DocNum = $("#docNumber").val().substr(4, $("#docNumber").val().length);
                DocNum = $("#docNumber").val();
                DocNum = parseInt(DocNum) - 1;
                if (DocNum == 0) {
                    return;
                }
                DocNum = DocNum.toString();
                //DocNum = 'ASS-' + DocNum.padStart(6, '0');
                DocNum = DocNum.padStart(7, '0');
                FilteredRecord = WIPRecordingFormList.filter(x=>x.DocNum == DocNum);
                FilteredRecord.forEach(function (obj) {
                    Table = obj.Table;
                })
                id = DocNum;
            }
        }

        else if (Action == 'last') {
            let l1 = WIPRecordingFormList.map(m=> { return m.DocNum });
            l1 = l1.sort();

            var unique = l1.filter((v, i, a) => a.indexOf(v) === i);

            id = WIPRecordingFormList.filter(x=>x.DocNum == unique[unique.length - 1]);
            if (id.length > 0) {
                id = id[0].DocNum;
                FilteredRecord = WIPRecordingFormList.filter(x=>x.DocNum == id);
                FilteredRecord.forEach(function (obj) {
                    Table = obj.Table;
                })
            }
        }

        else if (Action == 'first') {
            id = WIPRecordingFormList[0].DocNum;
            FilteredRecord = WIPRecordingFormList.filter(x=>x.DocNum == id);
            FilteredRecord.forEach(function (obj) {
                Table = obj.Table;
            })
            if (id == "") {
                console.log("null id");
            }
        }
    }

    var BranchID;
    var DebitAccount;
    var CreditAccount;
    var ReverseJE;
    var Status;
    var General;
    var flgPost;
    var ID;
    var AsOnDate;
    var DocDate;
    var ReversalDate;
    var ddMonth;
    var ddYear;
    var Currency;
    FilteredRecord.forEach(function (obj) {
        ID = obj.DocNum;
        DocDate = obj.DocDate;
        AsOnDate = obj.AsOnDate;
        BranchID = obj.BranchID;
        DebitAccount = obj.DebitAccount;
        CreditAccount = obj.CreditAccount;
        ReverseJE = obj.ReverseJE;
        ReversalDate = obj.ReversalDate;
        ddMonth = obj.Month;
        ddYear = obj.Year;
        Currency = obj.CurrencyID;
        GenerateGrid(obj.Table);

        totalSum();
    })

    //if (Table.length > 0) {
    //    DocNum = Table[0].DOCNUM;
    //}

    $("#docNumber").val(ID);
    $("#DocDate").dxDateBox({ value: DocDate, text: DocDate });
    $("#AsOnDate").dxDateBox({ value: AsOnDate, text: AsOnDate });

    $("#Office").dxSelectBox({ value: BranchID });
    $("#ddMonth").dxSelectBox({ value: parseInt(ddMonth) });
    $("#ddYear").dxSelectBox({ value: parseInt(ddYear) });
    $("#Currency").dxSelectBox({ value: Currency });

    $("#DebitAccount").val(DebitAccount);
    $("#DebitAccount").dxDropDownBox({ value: DebitAccount });

    $("#CreditAccount").val(CreditAccount);
    $("#CreditAccount").dxDropDownBox({ value: CreditAccount });
    $("#ReverseJE").prop("checked", ReverseJE);

    if (ReverseJE == true) {
        $("#ReversalDate").dxDateBox({ value: ReversalDate, text: ReversalDate });
    }
    //$("#ReverseJE").dxCheckBox({
    //    value: selectedRow[0].ReverseJE
    // });
    
    //if (Status == 2) {
    //    $("#Submit").hide();
    //    $("#add_update").hide();
    //}
    //else if (Status == 4) {
    //    $("#add_update").hide();
    //    $("#Submit").text("Post to SBO");
    //    if (flgPost == true) {
    //        $("#Submit").hide();
    //    }
    //    else {
    //        $("#Submit").show();
    //    }
    //}
    //else {
    //    $("#Submit").text(" Submit ");
    //    $("#Submit").show();
    //    $("#add_update").show();
    //}

    //$("#docNumber").val(selectedRow[0].DOCNUM);
    $('#docNumber').prop("disabled", true);

    //$("#GeneralID").val(General[0].ID);
    //$("#TypeOfAssignment").dxSelectBox({ value: General[0].TypeOfAssignment });
    //$("#DurationInDays").val(General[0].DurationInDays);
    //$("#ClosureDate").dxDateBox({ value: General[0].ClosureDate, text: General[0].ClosureDate });
   // $("#Status").dxSelectBox({ value: General[0].Status });



    if (id != undefined) {
        $("#docNumber").val(id);
    }
}
