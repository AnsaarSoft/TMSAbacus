function RunReport() {
    var PrintType = "ApprovalReport.rpt";

    //var jsonParams = document.getElementById('DocNum').value;
    window.open('http://localhost:52836/ReportViewer.aspx?ReportName=' + PrintType, '_newTab');
    
}

var KEYMain;
var Table = [];
var Table2 = [];
var Table3 = [];
var AssignmentFormList = [];
var AssignmentFormListOnLoad = [];

var TaskList = [];

var serialNo = 0;
var serialNoCost = 0;
var serialNoSum = 0;
index = 0;
var DocNum = "";
var CLIENTID = 0;
var DEPARTMENTNAME;
var StdBillingRateHr;
var TotalCost;
var TravelCost;
var RevenueRateHr;
var Revenue;

var productsDataSource = null;

var FULLNAME;
var DESIGNATIONNAME;
var AllEmployees;
var TravelLocationList;
//var ResourceBilling;
var AllTypeOfCost;
var AllTask;
var SaveEditedData = -1;
var NonChargeable;
var Doc;
$('#docNumber').prop("disabled", true);

var clientFunctions;
var sapFunctions;
var sapSubFunctions;
var sapPartners;
var hcmDirector;
var DirectorID = "0";
var DirectorName = "";
var Status = 0;
var flgPost = false;
var isValidateAssignmentTitle = false;
var isPageDataLoaded = false;

function stopLoading() {
    if (isPageDataLoaded == false) {
        setTimeout(function () { stopLoading(); }, 500);
    }
    else {
        loadPanel.hide();
    }
}
function createNew() {
   // debugger;
    var abc = [];
    Table = [];
    Table2 = [];
    Table3 = [];
    //Header
    $("#Office").dxSelectBox({ value: "" });
    //$("#AssignmentTitle").val("");
    $("#AssignmentTitle").dxTextBox({ value: "" });
    $("#Client").dxSelectBox({ value: "" });
    $("#Function").dxSelectBox({ value: "" });
    $("#SubFunction").dxSelectBox({ value: "" });
    $("#Partner").dxSelectBox({ value: "" });
    $("#Director").val("");
    $("#Director").dxDropDownBox({ value: "" });

    
    $("#NonChargeable").prop("checked", false);
    

    //General Tab
    $("#TypeOfAssignment").dxSelectBox({ value: "" });
    $("#NatureOfAssignment").dxSelectBox({ value: "" });
    $("#TypeOfBilling").dxSelectBox({ value: "" });
    $("#Currency").dxSelectBox({ value: "" });
    $("#AssignmentValue").val("");
    $("#StartDate").dxDateBox({ value: "", text: "" });
    $("#EndDate").dxDateBox({ value: "", text: "" });
    $("#DurationInDays").val("");
    $("#ClosureDate").dxDateBox({ value: "" });
    $("#Status").dxSelectBox({ value: "" });
    $("#Updateable").hide();

    

    GetTripRatePolicy('', 0);
    //Resource Tab
    GenerateGrid(abc);
    //Cost Tab
    GenerateGridCost(abc);
    //Summary Tab
    GenerateGridSummary(abc);
}

function applyValidation() {
   // debugger;
    usernameInstance = $("#AssignmentTitle").dxTextBox({
        placeholder:"Assignment Title",
        onValueChanged: function (e) {
            const previousValue = e.previousValue;
            const newValue = e.value;
            $("#AssignmentTitle").val(newValue);
            if (previousValue != "") {
                //if (newValue == $("#USERNAME").val()) {
                //    isValidateUserName = false;
                //}
                //else {
                if (previousValue != newValue){
                    isValidateAssignmentTitle = true;
                    
                }
                        
                    else
                        isValidateAssignmentTitle = false;
               // }
            }
        }
    })
    .dxValidator({
        validationRules: [{
            type: "required",
            message: "Assignment Title is required"
        },
        {
            type: "async",
            message: "Assignment Title is already registered",
            validationCallback: function (params) {
                return validateTitle(params.value);
            }
        }]
    }).dxTextBox('instance');

}


//$("#AssignmentTitle").onValueChanged(function (e) {

//    var AssignmemtTitle = document.getElementById('AssignmentTitle').value;
//    if (AssignmemtTitle !== '') {
//        $('#registry').formValidation({
//});


var validateTitle = function (value) {
    var d = $.Deferred();

    if (value != "") {
        if (isValidateAssignmentTitle == false && $("#ID").val() != 0) {
            setTimeout(function () {
                d.resolve(true);
            }, 1000);
        }
        else {
            var url = "/AssignmentForm/validateAssignmentTitle";
            $.ajax({
                type: "POST",
                url: url,
                data: {
                    assignmentTitle: value
                },
                async: false,
                success: function (response) {
                    setTimeout(function () {
                        d.resolve(response.Msg == "");
                    }, 1000);
                },
                error: function (response) {
                    console.log(response);
                },
                failure: function (response) {
                    console.log(response);
                }
            })
        }
    }
    return d.promise();
}

function GetFormData()
{
   // debugger;
    GetAssignmentForm();
    GetAssignmentFormResource();
    GetSapBranch();
    GetSAPFunction();
    GetSAPSubFunction();
    GetSAPPartner();
    GetDirector();
    GetAssginmentType();
    GetClientFunction();
    GetBillingType();
    GetStatusFunction();
    GetDocStatusFunction();
    GetAssignmentNature();
    GetCurrency();
    GenerateDateBox();

    // GetDocFunction();
    GetTripRatePolicy('', 0);
    GenerateGrid([]);
    GenerateGridCost([]);
    GenerateGridSummary([]);
    GetAssignmentCost();
    
    //GetAssignmentFormSummary();
    isPageDataLoaded = true;
}

function GetFormDataOnLoad(DocId) {
    //debugger;
    var res=GetAssignmentFormOnLoad(DocId);
    //GetAssignmentFormResourceOnLoad(DocId);
    GetSapBranch();
    GetSAPFunction();
    GetSAPSubFunction();
    GetSAPPartner();
    GetDirector();
    GetAssginmentType();
    GetClientFunction();
    GetBillingType();
    GetStatusFunction();
    GetDocStatusFunction();
    GetAssignmentNature();
    GetCurrency();
    GenerateDateBox();

    // GetDocFunction();
    GetTripRatePolicy('', 0);
    GenerateGrid([]);
    GenerateGridCost([]);
    GenerateGridSummary([]);
    GetAssignmentCost();

    //GetAssignmentFormSummary();
    isPageDataLoaded = true;

    return res;
}


function refresh()
{
    //debugger;
    $("#AssignmentTitle").dxTextBox({
        icon: "check"    
    });
}


$('document').ready(function () {
    //debugger;
    loadPanel.show();
    setTimeout(function () { GetFormData() }, 100);
    stopLoading();

    //applyValidation();
    ValidateAuthorization();
    $("#AssignmentTitle").dxTextBox({
        placeholder: "Assignment Title"
    })
    if ($('#isView').val() == "True") {
        $("#topButton").remove();
        GeneratePopupFindGrid();
        $('#myModal_Find').modal('show');
    }
    $("#Updateable").hide();
    $('#Updateable').click(function () {
        var Updateable = $('#Updateable').is(":checked");

        if (Updateable == true) {
            
            $("#Submit").text(" Submit ");
            $("#Submit").show();
            $("#add_update").show();
        }
        else {
            if($("#docStatus").val() == 4){
                $("#Submit").text("Post to SBO");
                if (flgPost == true) {
                    $("#Submit").hide();
                }
                else {
                    $("#Submit").show();                 
                }
                $("#Updateable").show();
                $("#add_update").hide();
            }
            else if ($("#docStatus").val() == 5) {
                $("#Updateable").show();
                $("#Submit").show();
                $("#add_update").show();
            }
            else {
                $("#Updateable").show();
                $("#Submit").hide();
                $("#add_update").hide();
            }
        }
    });

    $('#add_update').click(function () {
        Status = 1;
        flgPost = false;
        var NonChargeable = $('#NonChargeable').is(":checked");
        GetAssignmentCreationCheck();

        //if (NonChargeable == true) {
        //    AddUpdateNC();
        //}
        //else {
        //    AddUpdateStat();
        //}
              
    });

    $('#summary-tab').click(function () {
        var Task = [];
        var tsk = [];
        for (var i = 0; i < Table.length; i++) {
            Task[i] = Table[i].TaskID;

        }
        Task = $.distinct(Task);

        var filteredTask = [];
        for (var i = 0; i < Task.length; i++) {
            // tsk[i] = TaskList.filter(x=>x.TaskID == Task[i]);
            var tsk = TaskList.filter(x=>x.TaskID == Task[i]);
            if (tsk.length > 0) {
                filteredTask.push(tsk[0]);
            }

        }
        $("#gridSummaryContainer").dxDataGrid('instance').beginUpdate();
        $("#gridSummaryContainer").dxDataGrid('instance').columnOption('TaskID', 'lookup.dataSource', filteredTask);
        $("#gridSummaryContainer").dxDataGrid('instance').endUpdate();

    });
    $('#resource-tab').click(function () {
        TaskUpdateResource();
        UpdateLocation();

    });
    
    $('#Submit').click(function () {
        var text = $('#Submit').text();
        if (text == ' Submit ') {
            Status = 2;
            flgPost = false;
        }
        else {
            Status = 4;
            flgPost = true;
        }
        var NonChargeable = $('#NonChargeable').is(":checked");
        GetAssignmentCreationCheck();

        //if (NonChargeable == true) {
        //    AddUpdateNC();
        //}
        //else {
        //    AddUpdateStat();
        //}
    });
});


function AddUpdateNC() {
    var isSuccess = true;
    var message = '';
    var alertSetupObj = {
        AssignmentTitle: $("#AssignmentTitle").val(),
        NonChargeable: $('#NonChargeable').is(":checked")
   
    };
    var assignmntTtl = $("#AssignmentTitle");
    if (alertSetupObj.AssignmentTitle == "") {
        isSuccess = false;
        message = message + 'Please enter Assignment Title. \n';
    }
    if (alertSetupObj.NonChargeable == false) {
        isSuccess = false;
        message = message + 'Please check to Non-Chargeable if you want to make this type of document.. \n';
    }
    if (isSuccess) {
        var ID = 0;
        FilteredRecordForID = AssignmentFormList.filter(x=>x.DOCNUM == $("#docNumber").val());
        if (FilteredRecordForID.length > 0) {
            ID = FilteredRecordForID[0].ID;
        }
        if (ID == undefined) {
            ID = 0;
        }
        var NonChargeable = $('#NonChargeable').is(":checked");
        var textBoxInstance = $('#AssignmentTitle').dxTextBox({
        }).dxTextBox('instance');
        var assignTitle = textBoxInstance.option('value'); //Get the current value
        var Header = {
            //AssignmentTitle: $("#AssignmentTitle").val(),
            //AssignmentTitle: $("#AssignmentTitle").dxTextBox({ value: AssignmentTitle }),
            AssignmentTitle: assignTitle,
            Status: Status,
            flgPost:flgPost              
        }
        
        loadPanel.show();
        //var url = "/AssignmentForm/AddUpdateNCAssignmentForm";
        var url = "/AssignmentForm/AddUpdateAssignmentForm";
        $.ajax({
            type: "POST",
            url: url,
            contentType: 'application/json',
            data: JSON.stringify({
                // AssignmentCode: $("#AssignmentCode").val(),
                DOCNUM: $("#docNumber").val(),
                NonChargeable: NonChargeable,
                AssignmentForm: Header,
                AssignmentFormGeneral: [],
                AssignmentFormChild: Table,
                AssignmentFormCost: Table2,
                AssignmentFormSummary: Table3,
                ID: ID
                //DocNum: $("#docNumber").val(),
                //NonChargeable: NonChargeable,
                //AssignmentForm: Header,
                //AssignmentFormChild: Table,
                //ID: ID
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
    else {
        AlertToast('error', message);
    }
}

function AddUpdateStat() {
    var isSuccess = true;
    var message = '';
    //var func = $("#Function").val();
    var textBoxInstance = $('#AssignmentTitle').dxTextBox({
    }).dxTextBox('instance');
    var assignTitle = textBoxInstance.option('value'); //Get the current value

    if (Table.length > 0) {
        var alertSetupObj = {
            Office: $("#Office").val(),
            // AssignmentTitle: $("#AssignmentTitle").val(),
            AssignmentTitle: assignTitle,
            Client: $("#Client").dxSelectBox("instance").option('value'),
            Function: $("#Function").dxSelectBox("instance").option('value'),
            SubFunction: $("#SubFunction").dxSelectBox("instance").option('value'),
            Partner: $("#Partner").dxSelectBox("instance").option('value'),
            //Client: $("#Client").val(),
            //Function: $("#Function").val(),
            //SubFunction: $("#SubFunction").val(),
            //Partner: $("#Partner").val(),
            Director: $("#Director").val(),
            //ClosureDate: $("#ClosureDate").val()
        };

        if (alertSetupObj.Office == "") {
            isSuccess = false;
            message = 'Please select Office. \n';
        }
        if (alertSetupObj.AssignmentTitle == "") {
            isSuccess = false;
            message = message + 'Please enter Assignment Title. \n';
        }
        if (alertSetupObj.Client == "") {
            isSuccess = false;
            message = message + 'Please select Client. \n';
        }
        if (alertSetupObj.Function == "") {
            isSuccess = false;
            message = message + 'Please select Function. \n';
        }
        if (alertSetupObj.SubFunction == "") {
            isSuccess = false;
            message = message + 'Please select Sub Function. \n';
        }
        if (alertSetupObj.Partner == "") {
            isSuccess = false;
            message = message + 'Please select Partner. \n';
        }
        if (alertSetupObj.Director == "") {
            isSuccess = false;
            message = message + 'Please select Director. \n';
        }
        //if (alertSetupObj.ClosureDate == "") {
        //    isSuccess = false;
        //    message = message + 'Please select Closure Date. \n';
        //}
    }
    
    if (isSuccess) {
        if (Status == 4 && flgPost == true)
        {
            var result = DevExpress.ui.dialog.confirm("Are you sure you want to Post this in SAP B1?", "Confirm changes");
            result.done(function (dialogResult) {
                //alert(dialogResult ? "Confirmed" : "Canceled");
                if (dialogResult == true) {
                    AddUpdatePost();
                }
            });
        }
        else {
            AddUpdatePost();
        }
    }
    else {
        AlertToast('error', message);
    }
}

function AddUpdatePost() {
    if (Table.length > 0) {
        var gridResourceContainer = $("#gridResourceContainer").dxDataGrid("instance");
        loadPanel.show();
        gridResourceContainer.getController('validating').validate(true).done(function (result) {
            if (result) {
                SaveEditedData = gridResourceContainer.saveEditData();
                setTimeout(function () { AddUpdate(); }, 2000);
            }
            else {
                loadPanel.hide();
                AlertToast('error', "Please fill required fields!");
            }
        });
    }
    else {
        AddUpdate();
    }
}
var assginmentType;
function GetAssginmentType() {
    var url = "/AssignmentForm/GetAssignmentType";
    var id = "#TypeOfAssignment";
    var DisplayExpr = "AssignmentType";
    var ValueExpr = "AssignmentType";
    var Placeholder = "Select Assignment Type...";

    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        assginmentType = data.response;
        GenerateSAPFunctionDropDown(assginmentType, id, DisplayExpr, ValueExpr, Placeholder);

    }).fail(function (data) {
    });
}

var billingType;
function GetBillingType() {
    var url = "/AssignmentForm/GetBillingType";
    var id = "#TypeOfBilling";
    var DisplayExpr = "BillingType";
    var ValueExpr = "BillingType";
    var Placeholder = "Select Billing Type...";

    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        billingType = data.response;
        GenerateSAPFunctionDropDown(billingType, id, DisplayExpr, ValueExpr, Placeholder);

    }).fail(function (data) {
    });
}

var statusFunction;
function GetStatusFunction() {
    var url = "/AssignmentForm/GetStatusFunction";
    var id = "#Status";
    var DisplayExpr = "StatusType";
    var ValueExpr = "StatusType";
    var Placeholder = "Select Status...";

    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        statusFunction = data.response;
        GenerateSAPFunctionDropDown(statusFunction, id, DisplayExpr, ValueExpr, Placeholder);

    }).fail(function (data) {
    });
}

var docstatusFunction;
function GetDocStatusFunction() {
    var url = "/AssignmentForm/GetDocStatusFunction";
    var id = "#docStatus";
    var DisplayExpr = "Name";
    var ValueExpr = "ID";
    var Placeholder = "Select Status...";

    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        docstatusFunction = data.response;
        GenerateSAPFunctionDropDown(docstatusFunction, id, DisplayExpr, ValueExpr, Placeholder);
        $("#docStatus").dxSelectBox({ disabled: "disabled" });

    }).fail(function (data) {
    });
}

var assignmentNature;
function GetAssignmentNature() {
    var url = "/AssignmentForm/GetAssignmentNature";
    var id = "#NatureOfAssignment";
    var DisplayExpr = "AssignmentNature";
    var ValueExpr = "ID";
    var Placeholder = "Select Nature Of Assignment...";

    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        assignmentNature = data.response;
        GenerateSAPFunctionDropDown(assignmentNature, id, DisplayExpr, ValueExpr, Placeholder);

    }).fail(function (data) {
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
    var Placeholder = "Select Office...";

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

function GetClientFunction() {
    var url = "/AssignmentForm/GetClientFunction";
    var id = "#Client";
    var DisplayExpr = "CLIENTNAME";
    var ValueExpr = "CLIENTID";
    var Placeholder = "Select Client...";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        clientFunctions = data.response;
        GenerateSAPFunctionDropDownClient(clientFunctions, id, DisplayExpr, ValueExpr, Placeholder);
        GetAssignmentFormSummary();

    }).fail(function (data) {
    });
}

function GenerateSAPFunctionDropDownClient(jsonResponse, id, DisplayExpr, ValueExpr, Placeholder) {
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
            UpdateLocation();
        }
    });
}

function UpdateLocation() {
    var ClientID = $("#Client").val();
    var BranchID = $("#Office").val();
    //GetTripRatePolicy(ClientID, BranchID);
    var Task = TravelLocationList.filter(x=>x.ClientID == ClientID && x.BranchID == BranchID);

    $("#gridResourceContainer").dxDataGrid('instance').beginUpdate();
    $("#gridResourceContainer").dxDataGrid('instance').columnOption('TravelRateID', 'lookup.dataSource', Task);
    $("#gridResourceContainer").dxDataGrid('instance').endUpdate();

}
function GetTripRatePolicy(ClientID, BranchID) {
    var url = "/AssignmentForm/GetTripRatePolicy";
    $.ajax({
        url: url,
        method: "GET",
        data: { ClientID: ClientID, BranchID: BranchID },
        async: false,
    }).done(function (data) {
        TravelLocationList = data.TravelLocationList;
        console.log(TravelLocationList);
    }).fail(function (data) {
    });
}

function GetSAPFunction() {
    var url = "/AssignmentForm/GetSapFunctions";
    var id = "#Function";
    var DisplayExpr = "FunctionName";
    var ValueExpr = "FunctionID";
    var Placeholder = "Select Function...";

    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        sapFunctions = data.response;
        GenerateSAPFunctionDropDownCustom(sapFunctions, id, DisplayExpr, ValueExpr, Placeholder);


    }).fail(function (data) {
    });
}

function GenerateSAPFunctionDropDownCustom(jsonResponse, id, DisplayExpr, ValueExpr, Placeholder) {
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
            var FunctionID = $("#Function").val();
            Tasklst = TaskList.filter(x=>x.FUNCTIONNAME == FunctionID);
            TaskUpdateResource();
            $("#gridSummaryContainer").dxDataGrid('instance').beginUpdate();
            $("#gridSummaryContainer").dxDataGrid('instance').columnOption('TaskID', 'lookup.dataSource', Tasklst);
            $("#gridSummaryContainer").dxDataGrid('instance').endUpdate();


            //GenerateGrid(abc);
            //GenerateGridSummary(abc);

        }
    });
}

var Tasklst = [];

function TaskUpdateResource() {
    //debugger;
    var FunctionID = $("#Function").val();
    Tasklst = TaskList.filter(x=>x.FUNCTIONNAME == FunctionID);
    var abc = [];

    //$("#gridResourceContainer").dxDataGrid("instance").refresh();
    //GetAssignmentFormResource();
    $("#gridResourceContainer").dxDataGrid('instance').beginUpdate();
    $("#gridResourceContainer").dxDataGrid('instance').columnOption('TaskID', 'lookup.dataSource', Tasklst);
    $("#gridResourceContainer").dxDataGrid('instance').endUpdate();

}

function GetSAPSubFunction() {
    var url = "/AssignmentForm/GetSapSubFunctions";
    var id = "#SubFunction";
    var DisplayExpr = "SubFunctionName";
    var ValueExpr = "SubFunctionID";
    var Placeholder = "Select Sub-Function...";

    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        sapSubFunctions = data.response;
        GenerateSAPFunctionDropDown(sapSubFunctions, id, DisplayExpr, ValueExpr, Placeholder);

    }).fail(function (data) {
    });
}

function GetSAPPartner() {
    var url = "/AssignmentForm/GetSapPartners";
    var id = "#Partner";
    var DisplayExpr = "PartnerName";
    var ValueExpr = "PartnerID";
    var Placeholder = "Select Practice Area Lead...";

    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        sapPartners = data.response;
        GenerateSAPFunctionDropDown(sapPartners, id, DisplayExpr, ValueExpr, Placeholder);

    }).fail(function (data) {
    });
}

function GetDirector() {
    var url = "/AssignmentForm/GetDirectors";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        hcmDirector = data.response;
        //GenerateSAPFunctionDropDown(hcmDirector, id, DisplayExpr, ValueExpr, Placeholder);
        GenerateHCMDirectorDropDown(hcmDirector);
    }).fail(function (data) {
    });
}

function GenerateHCMDirectorDropDown(jsonData) {
    $("#Director").dxDropDownBox({
        //value: [3],
        valueExpr: "DirectorID",
        placeholder: "Select Project Manager...",
        displayExpr: "PmName",
        showClearButton: true,
        dataSource: jsonData,
        contentTemplate: function (e) {
            var value = e.component.option("value"),
                $dataGrid = $("<div>").dxDataGrid({
                    dataSource: e.component.getDataSource(),
                    columns: [
		            {
		                dataField: "DirectorID",
		                caption: "DirectorID",
		                allowEditing: false,
		                visible: false
		            },
 
                    "PmId", "PmName", "DepartmentName"],
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
                                    e.component.option("value", keys[0].DirectorID);

                                    DirectorID = keys[0].DirectorID;
                                    PmName = keys[0].DirectorName;
                                    $("#Director").val(DirectorID);
                                    //GetUserInfoByEmpCode(EmpCode);

                                    $("#Director").dxDropDownBox("instance").close();
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

var BillingRate;
function GetBillingRate(FunctionID, DesignationID, FromDate, ToDate) {
    var url = "/AssignmentForm/GetBillingRates";
    $.ajax({
        url: url,
        method: "GET",
        data: {
            FunctionID: FunctionID, DesignationID: DesignationID, FromDate: FromDate, ToDate: ToDate
        },
        async: false,
    }).done(function (data) {

        BillingRate = data.response;
        if (BillingRate.length == 0) {
            BillingRate = 0;
            AlertToast('error', "Please add billing rate of this user in resource billing rate setup.");
        }
        else {
            BillingRate = BillingRate[0].StdBillingRateHr;
        }
        

    }).fail(function (data) {
    });
}


function GenerateDateBox() {
    var now = new Date();
    $("#DocDate").dxDateBox({
        type: "date",
        //pickerType: "calendar",
        pickerType: "rollers",
        value: now,
        showClearButton: true,
        displayFormat: "dd/MM/yyyy",
        disabled: true
    });

    $("#StartDate").dxDateBox({
        type: "date",
        //pickerType: "calendar",
        pickerType: "rollers",
        value: now,
        showClearButton: true,
        displayFormat: "dd/MM/yyyy",
        onValueChanged: function (data) {
            $("#EndDate").dxDateBox({ min: data.value });
            CalculateDays();
        }
    });

    $("#EndDate").dxDateBox({
        type: "date",
        pickerType: "rollers",
        value: now,
        showClearButton: true,
        displayFormat: "dd/MM/yyyy",
        onValueChanged: function (data) {
            $("#StartDate").dxDateBox({ max: data.value });
            CalculateDays();
        }
    });
    $("#ClosureDate").dxDateBox({
        type: "date",
        //value: now,
        pickerType: "rollers",
        placeholder: "Select Date",
        showClearButton: true,
        displayFormat: "dd/MM/yyyy",
        onValueChanged: function (data) {
            $("#ClosureDate").val(data.value);
        }
    });

    //$('.dx-texteditor-input').attr('readonly', true);
}

function dateDiff(date, date2) {
    var diffInDay = Math.floor(Math.abs((date2 - date) / (24 * 60 * 60 * 1000)));
    return $("#DurationInDays").val(diffInDay);
}

function CalculateDays() {
    var EndDate = $("#EndDate > .dx-dropdowneditor-input-wrapper ").find("input").val();
    EndDate = new Date(EndDate);
    var StartDate = $("#StartDate > .dx-dropdowneditor-input-wrapper ").find("input").val();
    StartDate = new Date(StartDate);
    // $("#StartDate").val(data.value);                 
    dateDiff(StartDate, EndDate);
}

function daysBetween(startDate, endDate) {
    var millisecondsPerDay = 24 * 60 * 60 * 1000;
    return (treatAsUTC(endDate) - treatAsUTC(startDate)) / millisecondsPerDay;
}

function treatAsUTC(date) {
    var result = new Date(date);
    result.setMinutes(result.getMinutes() - result.getTimezoneOffset());
    return result;
}

function deleteRow(r) {
    var rowIndex = parseInt($(r).closest('tr').index() + 1);
    
    document.getElementById("gridResourceContainer").deleteRow(rowIndex);

    //line = parseInt(line);
    //Del(line);
    //for (var i = 0, row; row = table.rows[i + 1]; i++) {
    //    var colmn = row.cells[1].innerHTML;
    //    if (colmn == i + 1) {

    //    }
    //    else {
    //        RowDeleteAfter(i, row);
    //    }
    //}
}

function GenerateGrid(JSON_Response) {
    //debugger;
    $(function () {
        $("#gridResourceContainer").dxDataGrid({
            dataSource: JSON_Response,
            keyExpr: "KEY",
            width: "2000px",
            height: "300px",
            showBorders: true,
            //showScrollbar: 'always',
            filterRow: { visible: true },
            scrolling: {
                mode: "vertical",
                showScrollbar: 'always'
            },
            paging: {
                enabled: false
            },
            editing: {
                mode: "cell",
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                useIcons: true
            },
            columns: [
                 {
                     dataField: "RowID",
                     caption: "RowID",
                     dataType: "Number",
                     allowEditing: false,
                     visible: false,
                     width: "auto"
                 },
                 {
                     dataField: "SNO",
                     caption: "S.No",
                     dataType: "Number",
                     allowEditing: false,
                     width: "auto"
                 },
                 {
                     dataField: "ID",
                     caption: "Employee",
                     width: "8%",
                     allowEditing: true,
                     validationRules: [{ type: "required" }],
                     setCellValue: function (rowData, value, currentRowData) {

                         rowData.ID = value;
                         currentRowData.ID = value;

                         rowData.ID = value;

                         var SelectedEmp = AllEmployees.filter(x=>x.ID == value)[0];
                         if (SelectedEmp != undefined) {
                             rowData.UserID = SelectedEmp.ID;

                             rowData.DepartmentID = SelectedEmp.DEPARTMENTID;
                             rowData.DEPARTMENTNAME = SelectedEmp.DEPARTMENTNAME;
                             rowData.USER_CODE = SelectedEmp.EMPLOYEECODE;
                             rowData.FULLNAME = SelectedEmp.FULLNAME;
                             rowData.DesignationID = SelectedEmp.DESIGNATIONID;
                             rowData.DESIGNATIONNAME = SelectedEmp.DESIGNATIONNAME;

                            var FunctionID = $("#Function").val();
                            var DesignationID = rowData.DesignationID;
                            var FromDate = $("#StartDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
                            var ToDate = $("#EndDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
                            var now = new Date();
                            var datec = now.getDate();
                            var monthc = now.getMonth();
                            monthc += 1;
                            if (monthc != 10 || monthc != 11 || monthc != 12) {
                                monthc = 0 + '' + monthc;
                            }
                            var yearc = now.getFullYear();
                            yearc = yearc.toString().substring(2, 4);
                            now = yearc + '' + monthc + '' + datec;

                            GetBillingRate(FunctionID, DesignationID, now, now);

                            if (BillingRate.length == 0 || BillingRate == 0) {
                                var dataGrid = $("#gridResourceContainer").dxDataGrid("instance");
                                $("#gridResourceContainer").parent("tr").remove();
                                AlertToast('error', "Please add billing rate of this user or select relevant function & Dates.");
                            }
                            else {
                                rowData.StdBillingRateHr = BillingRate;
                                currentRowData.StdBillingRateHr = BillingRate;
                            }
                         }
                     },

                     lookup: {
                         dataSource: AllEmployees,
                         valueExpr: "ID",
                         displayExpr: "FULLNAME",
                     }
                 },
                {
                    dataField: "FULLNAME",
                    caption: "Originator",
                    validationRules: [{ type: "required" }],
                    width: "8%",
                    allowEditing: false,
                    visible: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }

                },
                {
                    dataField: "DESIGNATIONNAME",
                    caption: "Designation",
                    width: "8%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                },
                {
                    dataField: "DEPARTMENTNAME",
                    caption: "Department",
                    width: "8%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                },
                {
                    dataField: "TaskID",
                    caption: "Task",
                    width: "8%",
                    allowEditing: true,
                    setCellValue: function (rowData, value, currentRowData) {

                        rowData.TaskID = value;
                        currentRowData.TaskID = value;
                        rowData.TASK = value;

                    },
                    //customizeText: function (rowData) {
                    //    return rowData.value;
                    //},
                    lookup: {
                        dataSource: Tasklst,
                        valueExpr: "TaskID",
                        displayExpr: "TASK",

                    }
                },
                {
                    dataField: "TotalHours",
                    caption: "Total Hours",
                    width: "5%",
                    allowEditing: true,
                    dataType: "number",
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    },
                    //customizeText: function (rowData) {
                    //    return rowData.value;
                    //},
                    setCellValue: function (rowData, value, currentRowData) {

                        rowData.TotalHours = value;
                        currentRowData.TotalHours = value;

                        rowData.TotalHours = value;

                        var hours = currentRowData.TotalHours * currentRowData.StdBillingRateHr;

                        rowData.ResourceCost = hours;
                        currentRowData.ResourceCost = hours;
                        var TotalOtherCost = $("#TotalOtherCost").val();
                        if (TotalOtherCost == "") {
                            TotalOtherCost = 0.00;
                        }
                        if (currentRowData.TravelCost == "" || currentRowData.TravelCost == undefined) {
                            currentRowData.TravelCost = 0;
                        }

                        //var TotalCost = currentRowData.ResourceCost + TotalOtherCost;
                        //rowData.TotalCost = TotalCost;
                        var TotalCost = parseFloat(currentRowData.ResourceCost) + parseFloat(currentRowData.TravelCost) + parseFloat(TotalOtherCost);
                        rowData.TotalCost = parseFloat(TotalCost).toFixed(2);

                        //TskList = [];
                        //TskList = taskList.filter(x=>x.FunctionID == DesignationID);
                    }

                },
                {
                    dataField: "StdBillingRateHr",
                    //dataType: "double",
                    caption: "Std Billing Rate/Hr",
                    width: "7%",
                    allowEditing: false,
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    }
                    //customizeText: function (rowData) {
                    //    return rowData.value;
                    //}
                },
                {
                    dataField: "ResourceCost",
                    caption: "Resource Cost",
                    width: "6%",
                    allowEditing: false,
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    dataField: "TravelRateID",
                    caption: "Travel Location",
                    width: "8%",
                    allowEditing: true,
                    //customizeText: function (rowData) {
                    //    return rowData.value;
                    //},
                    setCellValue: function (rowData, value, currentRowData) {

                        rowData.TravelRateID = value;
                        currentRowData.TravelRateID = value;


                        var SelectedRate = TravelLocationList.filter(x=>x.TravelRateID == value)[0];
                        if (SelectedRate != undefined) {


                            //rowData.LOCATION = value;
                            //currentRowData.LOCATION = value;

                            //rowData.LOCATION.text = SelectedRate.LOCATION;
                            //currentRowData.LOCATION.text = SelectedRate.LOCATION;

                            var tripRate = SelectedRate.RATETRIP;
                            var onSiteDays = parseFloat(currentRowData.TotalHours) / 8;
                            var travelCost = onSiteDays * tripRate;

                            rowData.TravelCost = travelCost;
                            currentRowData.TravelCost = travelCost;
                            var TotalOtherCost = $("#TotalOtherCost").val();
                            if (TotalOtherCost == "") {
                                TotalOtherCost = 0.00;
                            }
                            var TotalCost = parseFloat(currentRowData.ResourceCost) + parseFloat(travelCost) + parseFloat(TotalOtherCost);
                            rowData.TotalCost = TotalCost;
                        }

                    },

                    lookup: {
                        dataSource: TravelLocationList,
                        valueExpr: "TravelRateID",
                        displayExpr: "LOCATION",

                    }

                },
                {
                    dataField: "TravelCost",
                    caption: "Travel Cost",
                    width: "5%",
                    allowEditing: false,
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    dataField: "TotalCost",
                    caption: "Total Cost",
                    width: "5%",
                    allowEditing: false,
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    dataField: "RevenueRateHr",
                    caption: "Revenue Rate/Hr",
                    width: "6%",
                    allowEditing: true,
                    dataType: "number",
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    },
                    setCellValue: function (rowData, value, currentRowData) {

                        rowData.RevenueRateHr = value;
                        currentRowData.RevenueRateHr = value;

                        var Revenue = currentRowData.TotalHours * currentRowData.RevenueRateHr;

                        rowData.Revenue = Revenue;
                        currentRowData.Revenue = Revenue;
                    }

                },
                {
                    dataField: "Revenue",
                    caption: "Revenue",
                    width: "8%",
                    allowEditing: false,
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    }
                    //customizeText: function (rowData) {
                    //    return rowData.value;
                    //}
                },
                {
                    dataField: "IsChargeable",
                    caption: "Chargeable",
                    dataType: "boolean",
                    width: "5%",
                    allowEditing: true,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                },
                {
                    dataField: "InActive",
                    caption: "InActive",
                    dataType: "boolean",
                    width: "5%",
                    allowEditing: true,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                }
            ],
            onEditorPreparing: function (e) {
                if (e.parentType == "filterRow" && e.editorName == 'dxSelectBox')
                    e.editorOptions.onOpened = function (e) { e.component._popup.option('width', 400); };
                
            },
            onEditorPrepared: function (e) {
                //dxSelectBox
                if (e.dataField == "USER_CODE") {
                    $(e.editorElement).dxSelectBox("instance").on("valueChanged", function (args) {
                        var grid = $("#gridResourceContainer").dxDataGrid("instance");
                        var index = e.row.rowIndex;
                        //var result = "new description " + args.value;
                        //grid.cellValue(index, "DEPARTMENTNAME", result);

                        var SelectedEmp = AllEmployees.filter(x=>x.ID == args.value)[0];
                        console.log(SelectedEmp);
                        if (SelectedEmp != null) {
                            grid.cellValue(index, "USER_CODE", SelectedEmp.EMPLOYEECODE);
                            //grid.cellValue(index, "DepartmentID", SelectedEmp.DepartmentID);
                            grid.cellValue(index, "DEPARTMENTNAME", SelectedEmp.DEPARTMENTNAME);
                            grid.cellValue(index, "FULLNAME", SelectedEmp.USERNAME);
                            //grid.cellValue(index, "DesignationID", SelectedEmp.DesignationID);
                            grid.cellValue(index, "DESIGNATIONNAME", SelectedEmp.DESIGNATIONNAME);
                            grid.cellValue(index, "BillingRatesPerHour", SelectedEmp.BillingRatesPerHour);
                        }
                    });
                }

            },
            onEditingStart: function (e) {
            },
            onInitNewRow: function (e) {
                e.data.ID = 0;
                e.data.ISDELETED = false;
                //e.data.INACTIVE = true;
                serialNo = serialNo + 1;
                e.data.SNO = serialNo
                e.data.IsChargeable = true;
                e.data.InActive = false;
                window.setTimeout(function () { e.component.cancelEditData(); }, 0)
                GeneratePopupUserGrid();
            },
            onRowInserting: function (e) {
            },
            onRowInserted: function (e) {

                var dataExist = Table.filter(x=>x.KEY == e.data.KEY);

                if (dataExist.length == 0) {
                    var Details = {
                        //KEYMain: e.key + 1,
                        KEY: e.key,
                        ROWID: 0,
                        ID: e.data.ID,
                        SNO: e.data.SNO,

                        DepartmentID: e.data.DepartmentID,
                        DEPARTMENTNAME: e.data.DEPARTMENTNAME,

                        DesignationID: e.data.DesignationID,
                        DESIGNATIONNAME: e.data.DESIGNATIONNAME,

                        EmpID: e.data.ID,
                        USER_CODE: e.data.USER_CODE,
                        FULLNAME: e.data.FULLNAME,
                        TaskID: e.data.TaskID,
                        TASK: e.data.TASK,
                        TotalHours: e.data.TotalHours,
                        StdBillingRateHr: e.data.StdBillingRateHr,

                        ResourceCost: e.data.ResourceCost,
                        TravelRateID: e.data.TravelRateID,
                        TravelCost: e.data.TravelCost,
                        TotalCost: e.data.TotalCost,
                        RevenueRateHr: e.data.RevenueRateHr,
                        Revenue: e.data.Revenue,
                        IsChargeable: e.data.IsChargeable,
                        InActive: e.data.InActive,
                        IsDeleted: false
                    };
                    Table.push(Details);
                    
                }
                else
                {
                    $.each(Table, function (index, val) {
                        if (val.KEY == e.data.KEY) {
                            val.ROWID = e.data.ROWID;
                                
                            val.DepartmentID= e.data.DepartmentID;
                            val.DEPARTMENTNAME= e.data.DEPARTMENTNAME;

                            val.DesignationID= e.data.DesignationID;
                            val.DESIGNATIONNAME= e.data.DESIGNATIONNAME;

                            val.EmpID= e.data.ID;
                            val.USER_CODE= e.data.USER_CODE;
                            val.FULLNAME= e.data.FULLNAME;
                            val.TaskID= e.data.TaskID;
                            val.TASK= e.data.TASK;
                            val.TotalHours= e.data.TotalHours;
                            val.StdBillingRateHr= e.data.StdBillingRateHr;

                            val.ResourceCost= e.data.ResourceCost;
                            val.TravelRateID= e.data.TravelRateID;
                            val.TravelCost= e.data.TravelCost;
                            val.TotalCost= e.data.TotalCost;
                           val.RevenueRateHr= e.data.RevenueRateHr;
                            val.Revenue= e.data.Revenue;
                            val.IsChargeable= e.data.IsChargeable;
                            val.InActive = e.data.InActive;
                            val.ISDELETED = false;
                        }

                    });
                }
                totalSum();

                //let IfUserCodeExist = Table.filter(x=>x.USER_CODE == Details.USER_CODE)
                //if (IfUserCodeExist.length == 0) {
                //    Table.push(Details);
                //}
                
                

            },
            onRowUpdating: function (e) {
                //totalSum();

            },
            onRowUpdated: function (e) {
                $.each(Table, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                        val.ISDELETED = e.data.ISDELETED;                    
                    }
                    totalSum();
                });
            },
            onRowRemoving: function (e) {
                totalSum();
            },
            onRowRemoved: function (e) {
                $.each(Table, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                        val.ISDELETED = true;
                    }
                    var serialNum = val.SNO - 1;
                    serialNo = val.SNO;
                    for (var i = serialNum; i < Table.length; i++) {
                        Table[i].SNO = serialNo;
                        serialNo++;
                    }
                    totalSum();

                });
            }
        });

    });
}


function GenerateGridCost(JSON_Response) {
    $(function () {
        $("#gridCostContainer").dxDataGrid({
            dataSource: JSON_Response,
            keyExpr: "KEY",
            showBorders: true,
            showScrollbar: 'always',
            filterRow: { visible: true },
            scrolling: {
                mode: "horizontal",
                //showScrollbar: 'always'
            },
            paging: {
                enabled: false
            },
            editing: {
                mode: "row",
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                useIcons: true
            },
            columns: [
                {
                    dataField: "SNO",
                    caption: "S.No",
                    dataType: "Number",
                    allowEditing: false,
                    width: "10%"
                },
                {
                    dataField: "RowID",
                    caption: "RowID",
                    dataType: "Number",
                    allowEditing: false,
                    width: "10%",
                    visible: false
                },
                //{
                //    dataField: "ID",
                //    caption: "ID",
                //    allowEditing: false,
                //    width: "10%",
                //},
                 {
                     dataField: "ID",
                     caption: "Type of Cost",
                     allowEditing: true,
                     width: "40%",
                     setCellValue: function (rowData, value, currentRowData) {

                         rowData.ID = value;
                         currentRowData.ID = value;

                         var SelectedEmp = AllTypeOfCost.filter(x=>x.ID == value)[0];
                         if (SelectedEmp != undefined) {

                             rowData.ID = SelectedEmp.ID;
                             rowData.TYPEOFCOST = SelectedEmp.TYPEOFCOST;
                         }
                     },

                     lookup: {
                         dataSource: AllTypeOfCost,
                         valueExpr: "ID",
                         displayExpr: "TYPEOFCOST",
                     }
                 },

                {
                    dataField: "Amount",
                    caption: "Amount",
                    dataType: "number",
                    width: "30%",
                    allowEditing: true,
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    },
                    validationRules: [{ type: "required" }],
                    //{
                    //    type: "pattern",
                    //    message: 'Your Revenue Distribution must have percentage format!',
                    //    pattern: /^\d+([.]\d+)?$/
                    //}],
                    //customizeText: function (rowData) {
                    //    return rowData.value;
                    //}
                }
            ],
            summary: {
                totalItems: [
                    {
                        column: "Amount",
                        displayFormat: "Total Other Cost: {0}",
                        summaryType: "sum",
                        alignment: "right",
                        valueFormat: "#0.00"
                    }
                ]
            },
            onEditorPreparing: function (e) {

            },
            onEditorPrepared: function (e) {
                //dxSelectBox
                if (e.dataField == "ID") {
                    $(e.editorElement).dxSelectBox("instance").on("valueChanged", function (args) {
                        var grid = $("#gridCostContainer").dxDataGrid("instance");
                        var index = e.row.rowIndex;
                        //var result = "new description " + args.value;
                        //grid.cellValue(index, "DEPARTMENTNAME", result);

                        var SelectedEmp = AllTypeOfCost.filter(x=>x.ID == args.value)[0];
                        console.log(SelectedEmp);
                        if (SelectedEmp != null) {
                            grid.cellValue(index, "ID", SelectedEmp.ID);
                            grid.cellValue(index, "TYPEOFCOST", SelectedEmp.TYPEOFCOST);
                        }
                    });
                }
            },
            onEditingStart: function (e) {
            },
            onInitNewRow: function (e) {
                e.data.RowID = 0;
                e.data.ISDELETED = false;
                serialNoCost = serialNoCost + 1;
                e.data.SNO = serialNoCost;
            },
            onRowInserting: function (e) {
                e.data.SNO = serialNoCost;
            },
            onRowInserted: function (e) {

                //if (Table2.length == 0) {
                var dataExist = Table2.filter(x=>x.KEY == e.data.KEY);


                if (dataExist.length == 0) {

                    var Details = {
                        //KEYMain: e.key + 1,
                        KEY: e.key,
                        RowID: e.data.RowID,
                        SNO: e.data.SNO,

                        ID: e.data.ID,
                        TYPEOFCOST: e.data.TYPEOFCOST,
                        Amount: e.data.Amount,
                        //ISACTIVE: e.data.ISACTIVE,
                        ISDELETED: false
                    };

                    //let IfUserCodeExist = Table2.filter(x=>x.USER_CODE == Details.USER_CODE)
                    //if (IfUserCodeExist.length == 0) {
                    //    Table2.push(Details);
                    //}
                    Table2.push(Details);
                    totalSumCost();
                    serialNoCost = Table2.length;
                }
                else {
                    $.each(Table2, function (index, val) {
                        if (val.KEY == e.data.KEY) {
                            val.ID = e.data.ID;
                            val.TYPEOFCOST = e.data.TYPEOFCOST;
                            val.Amount = e.data.Amount;
                            val.ISDELETED = e.data.ISDELETED;
                        }

                    });
                }
            },
            onRowUpdating: function (e) {
                totalSumCost();
            },
            onRowUpdated: function (e) {
                $.each(Table2, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                        //val.KM = e.data.KM;
                        //val.LOCATION = e.data.LOCATION;
                        //val.CLIENTID = e.data.CLIENTID;
                        //val.ISACTIVE = e.data.ISACTIVE;
                    }

                });
            },
            onRowRemoving: function (e) {
            },
            onRowRemoved: function (e) {
                $.each(Table2, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                        val.ISDELETED = true;
                        totalSumCost();
                        var serialNum = val.SNO - 1;
                        serialNoCost = val.SNO;
                        for (var i = serialNum; i < Table2.length; i++) {
                            Table2[i].SNO = serialNoCost;
                            serialNoCost++;
                        }
                    }
                });
            }
        });

    });
}

function GenerateGridSummary(JSON_Response) {
    $(function () {
        $("#gridSummaryContainer").dxDataGrid({
            dataSource: JSON_Response,
            keyExpr: "KEY",
            showBorders: true,
            showScrollbar: 'always',
            filterRow: { visible: true },
            scrolling: {
                mode: "horizontal",
                //showScrollbar: 'always'
            },
            paging: {
                enabled: false
            },
            editing: {
                mode: "row",
                allowUpdating: true,
                allowDeleting: true,
                allowAdding: true,
                useIcons: true
            },
            columns: [
                 {
                     dataField: "ID",
                     caption: "ID",
                     width: "auto",
                     allowEditing: false,
                     visible: false
                 },
                {
                    dataField: "SNO",
                    caption: "S.No",
                    dataType: "Number",
                    allowEditing: false,
                    width: "4%"
                },
                 {
                     dataField: "TaskID",
                     caption: "TASK",
                     allowEditing: true,
                     width: "18%",
                     sortOrder: "asc",
                     setCellValue: function (rowData, value, currentRowData) {

                         rowData.TaskID = value;
                         currentRowData.TaskID = value;

                         //var SelectedEmp = TaskList.filter(x=>x.ID == value)[0];
                         //if (SelectedEmp != undefined) {
                         //    rowData.TASK = SelectedEmp.TASK;
                         //    rowData.TotalBudgetedHour = SelectedEmp.TotalBudgetedHour;
                         //    rowData.EstimatedResourceCost = SelectedEmp.EstimatedResourceCost;
                         //    rowData.RevenueDistribution = SelectedEmp.RevenueDistribution;
                         //    rowData.EstimatedRevenue = SelectedEmp.EstimatedRevenue;
                         //}
                         var tble3 = Table3.filter(x=>x.TaskID == value);
                         if(tble3.length == 0){
                         
                             var SelectedRate = Table.filter(x=>x.TaskID == value);
                             if (SelectedRate != undefined) {

                                 var totalBudgetedHour = 0.00;
                                 var estimatedResourceCost = 0.00;
                                 // var revenueDistribution = 0.00;
                                 var estimatedRevenue = 0.00;
                                 for (var i = 0; i < SelectedRate.length; i++) {
                                     totalBudgetedHour += parseFloat(SelectedRate[i].TotalHours);
                                     estimatedResourceCost += parseFloat(SelectedRate[i].ResourceCost);
                                     //revenueDistribution += parseFloat(SelectedRate[i].RevenueRateHr);
                                     estimatedRevenue += parseFloat(SelectedRate[i].Revenue);
                                 }
                                 //rowData.LOCATION = value;
                                 //currentRowData.LOCATION = value;

                                 //rowData.LOCATION.text = SelectedRate.LOCATION;
                                 //currentRowData.LOCATION.text = SelectedRate.LOCATION;
                                 if (totalBudgetedHour == NaN) {
                                     totalBudgetedHour = 0.00;
                                 }
                                 if (estimatedResourceCost == NaN) {
                                     estimatedResourceCost = 0.00;
                                 }
                                 if (estimatedRevenue == NaN) {
                                     estimatedRevenue = 0.00;
                                 }
                                 rowData.TASK = SelectedRate[0].TASK;

                                 rowData.TotalBudgetedHour = totalBudgetedHour;
                                 currentRowData.TotalBudgetedHour = totalBudgetedHour;
                                 rowData.EstimatedResourceCost = estimatedResourceCost;
                                 currentRowData.EstimatedResourceCost = estimatedResourceCost;
                                 var TypeOfBilling = $("#TypeOfBilling").val();
                                 if (TypeOfBilling == "Hourly") {
                                     rowData.RevenueDistribution = 0.00;
                                     currentRowData.RevenueDistribution = 0.00;
                                     rowData.EstimatedRevenue = estimatedRevenue;
                                     currentRowData.EstimatedRevenue = estimatedRevenue;
                                 }
                                 else {
                                     rowData.RevenueDistribution = "";
                                     currentRowData.RevenueDistribution = "";
                                     rowData.EstimatedRevenue = 0;
                                     currentRowData.EstimatedRevenue = 0;
                                 }
                             }
                         }
                         else {
                             rowData.TaskID = null;
                             currentRowData.TaskID = null;
                         }
                     },

                     lookup: {
                         dataSource: Tasklst,
                         valueExpr: "TaskID",
                         displayExpr: "TASK",
                     }
                 },
                {
                    dataField: "TotalBudgetedHour",
                    caption: "Total Budgeted Hours",
                    width: "15%",
                    allowEditing: false,
                    dataType: "number",
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    },
                },
                {
                    dataField: "EstimatedResourceCost",
                    caption: "Total Estimated Resource Cost",
                    width: "24%",
                    allowEditing: false,
                    dataType: "number",
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    },
                    //customizeText: function (rowData) {
                    //    return rowData.value;
                    //}
                },
                {
                    dataField: "RevenueDistribution",
                    caption: "Revenue Distribution %",
                    alignment: "center",
                    width: "13%",
                    allowEditing: true,
                    dataType: "number",
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    },
                    validationRules: [{ type: "required" }],
                    //{
                    //    type: "pattern",
                    //    message: 'Your Revenue Distribution must have percentage format!',
                    //    pattern: /^\d+([.]\d+)?$/
                    //}],
                    setCellValue: function (rowData, value, currentRowData) {

                        var TypeOfBilling = $("#TypeOfBilling").val();
                        if (TypeOfBilling == "Fixed") {
                            rowData.RevenueDistribution = value;
                            currentRowData.RevenueDistribution = value;
                            var AssignmentValue = $("#AssignmentValue").val();
                            var RevenueDistribution = currentRowData.RevenueDistribution;
                            var total = RevenueDistribution * AssignmentValue;
                            currentRowData.EstimatedRevenue = total;
                            rowData.EstimatedRevenue = total;
                        }
                    }
                },
                {
                    dataField: "EstimatedRevenue",
                    caption: "Total Estimated Revenue",
                    width: "32%",
                    allowEditing: false,
                    format: {
                        type: "fixedPoint",
                        precision: 2
                    }
                }
            ],
            summary: {
                totalItems: [
                    {
                        column: "EstimatedRevenue",
                        displayFormat: "Total Estimated Revenue: {0}",
                        summaryType: "sum",
                        alignment: "right",
                        valueFormat: "#0.00"
                    },
                    {
                        //name: "SelectedRowsSummary",
                        column: "EstimatedResourceCost",
                        displayFormat: "Total Estimate Resorce Cost: {0}",
                        //valueFormat: "currency",
                        summaryType: "sum",
                        alignment: "right",
                        valueFormat: "#0.00"
                    }
                // ...
                ]
            },
            onEditorPreparing: function (e) {
                //if (e.parentType == 'dataRow' && e.dataField == 'RevenueDistribution') {
                //    e.editorOptions.onKeyPress = numbervalidation;
                //}
                var TypeOfBilling = $("#TypeOfBilling").val();
                if (TypeOfBilling == "Hourly") {
                    e.editorOptions.disabled = e.parentType == "dataRow" && e.dataField == "RevenueDistribution" && !e.row.inserted;
                }
            },
            onEditorPrepared: function (e) {
                //dxSelectBox
                //if (e.dataField == "ID") {
                //    $(e.editorElement).dxSelectBox("instance").on("valueChanged", function (args) {
                //        var grid = $("#gridSummaryContainer").dxDataGrid("instance");
                //        var index = e.row.rowIndex;
                //        //var result = "new description " + args.value;
                //        //grid.cellValue(index, "DEPARTMENTNAME", result);

                //        var SelectedEmp = AllTask.filter(x=>x.ID == args.value)[0];
                //        console.log(SelectedEmp);
                //        if (SelectedEmp != null) {
                //            var TypeOfBilling = $("#TypeOfBilling").val();

                //            grid.cellValue(index, "ID", SelectedEmp.ID);
                //            grid.cellValue(index, "TASK", SelectedEmp.TASK);
                //            grid.cellValue(index, "TotalBudgetedHour", SelectedEmp.TotalBudgetedHour);
                //            grid.cellValue(index, "EstimatedResourceCost", SelectedEmp.EstimatedResourceCost);
                //            grid.cellValue(index, "RevenueDistribution", SelectedEmp.RevenueDistribution);
                //            grid.cellValue(index, "EstimatedRevenue", SelectedEmp.EstimatedRevenue);

                //            //if (TypeOfBilling == "Fixed") {
                //            //    grid.RevenueDistribution.allowEditing = true;

                //            //}
                //            //else {
                //            //    grid.RevenueDistribution.allowEditing = true;
                //            //    //rowData.RevenueDistribution.allowEditing = true;

                //            //}
                //        }
                //    });
                //}

            },
            onEditingStart: function (e) {
            },
            onInitNewRow: function (e) {
                var assignmentValue = $("#AssignmentValue").val();
                var totalResourceCost = $("#TotalResourceCost").val();
                var totalEstimatedResourceCost = 0.00;
                var totalBudgetedHour = 0.00;
                var totalEstimatedRevenue = 0.00;
                var totalHours = 0.00;
                for (var i = 0; i < Table3.length; i++) {
                    totalEstimatedResourceCost += parseFloat(Table3[i].EstimatedResourceCost);
                    totalBudgetedHour += parseFloat(Table3[i].TotalBudgetedHour);
                    //totalOtherCost += Table[i].Revenue;
                    totalEstimatedRevenue += parseFloat(Table3[i].EstimatedRevenue);
                }
                for (var i = 0; i < Table.length; i++) {
                    totalHours += parseFloat(Table[i].TotalHours);
                }
                if (isNaN(e.data.EstimatedRevenue)) {
                    e.data.EstimatedRevenue = 0;
                }
                if (isNaN(e.data.EstimatedResourceCost)) {
                    e.data.EstimatedResourceCost = 0;
                }
                if (isNaN(e.data.TotalBudgetedHour)) {
                    e.data.TotalBudgetedHour = 0;
                }
                totalEstimatedRevenue = totalEstimatedRevenue + parseFloat(e.data.EstimatedRevenue);
                totalEstimatedResourceCost = totalEstimatedResourceCost + parseFloat(e.data.EstimatedResourceCost);
                totalBudgetedHour = totalBudgetedHour + parseFloat(e.data.TotalBudgetedHour);
                
                if (isNaN(totalEstimatedRevenue)) {
                    totalEstimatedRevenue = 0;
                }
                if (isNaN(totalEstimatedResourceCost)) {
                    totalEstimatedResourceCost = 0;
                }
                if (isNaN(totalBudgetedHour)) {
                    totalBudgetedHour = 0;
                }
                if (isNaN(totalHours)) {
                    totalHours = 0;
                }

                if (parseFloat(assignmentValue) > totalEstimatedRevenue && parseFloat(totalResourceCost) > totalEstimatedResourceCost && totalHours > totalBudgetedHour) {

                var Task = [];
                var tsk = [];
                for (var i = 0; i < Table.length; i++) {
                    Task[i] = Table[i].TaskID;

                }
                Task = $.distinct(Task);
                
                var filteredTask = [];
                for (var i = 0; i < Task.length; i++) {
                    // tsk[i] = TaskList.filter(x=>x.TaskID == Task[i]);
                    var tsk = TaskList.filter(x=>x.TaskID == Task[i]);
                    if (tsk.length > 0) {
                        filteredTask.push(tsk[0]);
                    }

                }
                $("#gridSummaryContainer").dxDataGrid('instance').beginUpdate();
                $("#gridSummaryContainer").dxDataGrid('instance').columnOption('TaskID', 'lookup.dataSource', filteredTask);
                $("#gridSummaryContainer").dxDataGrid('instance').endUpdate();

                e.data.ID = 0;
                e.data.ISDELETED = false;
                //e.data.INACTIVE = true;
                serialNoSum = serialNoSum + 1;
                e.data.SNO = serialNoSum;
                }
                else {
                    AlertToast('error', "Please donot add row all are tasks are selected of resource allocation.");
                    window.setTimeout(function () { e.component.cancelEditData(); }, 0)
                }
            },
            onRowInserting: function (e) {
                //serialNoSum = serialNoSum + 1;
                e.data.SNO = serialNoSum;
            },
            onRowInserted: function (e) {
                
                    var dataExist = Table3.filter(x=>x.KEY == e.data.KEY);


                    if (dataExist.length == 0) {
                        var taskExist = Table3.filter(x=>x.TaskID == e.data.TaskID);

                        if (taskExist.length == 0) {

                            var Details = {
                                //KEYMain: e.key + 1,
                                KEY: e.key,
                                ID: e.data.ID,
                                SNO: e.data.SNO,


                                TaskID: e.data.TaskID,
                                TASK: e.data.TASK,
                                TotalBudgetedHour: e.data.TotalBudgetedHour,
                                EstimatedResourceCost: e.data.EstimatedResourceCost,

                                RevenueDistribution: e.data.RevenueDistribution,
                                EstimatedRevenue: e.data.EstimatedRevenue,
                                //ISACTIVE: e.data.ISACTIVE,

                                ISDELETED: false
                            };

                            Table3.push(Details);
                            //CalculateHours();
                            serialNoSum = Table3.length;
                        }
                        else {
                            AlertToast('error', "Please donot add row all are selected.");
                        }
                    }
                    else {
                        $.each(Table3, function (index, val) {
                            if (val.KEY == e.data.KEY) {
                                val.TaskID = e.data.TaskID;
                                val.TotalBudgetedHour = e.data.TotalBudgetedHour;
                                val.EstimatedResourceCost = e.data.EstimatedResourceCost;
                                val.RevenueDistribution = e.data.RevenueDistribution;
                                val.EstimatedRevenue = e.data.EstimatedRevenue;
                                val.ISDELETED = e.data.ISDELETED;
                            }

                        });
                    }
            },           
            onRowUpdating: function (e) {
            },
            onRowUpdated: function (e) {
                $.each(Table3, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                    }

                });
            },
            onRowRemoving: function (e) {
            },
            onRowRemoved: function (e) {
                $.each(Table3, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                        val.ISDELETED = true;
                        var serialNum = val.SNO - 1;
                        serialNoSum = val.SNO;
                        for (var i = serialNum; i < Table3.length; i++) {
                            Table3[i].SNO = serialNoSum;
                            serialNoSum++;
                        }
                    }
                });
            }
        });

    });
}
$.extend({
    distinct: function (anArray) {
        var result = [];
        $.each(anArray, function (i, v) {
            if ($.inArray(v, result) == -1) result.push(v);
        });
        return result;
    }
});


function totalSum() {
    var totalTravelCost = 0.00;
    var totalResourceCost = 0.00;
    var totalOtherCost = 0.00;
    var totalCost = 0.00;
    var totalRevenue = 0.00;
    for (var i = 0; i < Table.length; i++) {
        if (isNaN(Table[i].TravelCost))
            Table[i].TravelCost = 0;
        if (isNaN(Table[i].Revenue))
            Table[i].Revenue = 0;

        totalTravelCost += parseFloat(Table[i].TravelCost);
        totalResourceCost += parseFloat(Table[i].ResourceCost);
        totalCost += parseFloat(Table[i].TotalCost);
        totalRevenue += parseFloat(Table[i].Revenue);
    }
    if (Table2 != null) {
        for (var i = 0; i < Table2.length; i++) {
            totalOtherCost += Table2[i].Amount;
        }
    }
    
    if (isNaN(totalTravelCost))
        totalTravelCost = 0;

    if (isNaN(totalResourceCost))
        totalResourceCost = 0;

    if (isNaN(totalOtherCost))
        totalOtherCost = 0;

    if (isNaN(totalCost))
        totalCost = 0;

    if (isNaN(totalRevenue))
        totalRevenue = 0;
    $("#TotalTravelCost").val(parseFloat(totalTravelCost).toFixed(2));
    $("#TotalResourceCost").val(parseFloat(totalResourceCost).toFixed(2));
    $("#TotalOtherCost").val(parseFloat(totalOtherCost).toFixed(2));
    $("#TotalCost").val(parseFloat(totalCost).toFixed(2));
    $("#TotalRevenue").val(parseFloat(totalRevenue).toFixed(2));
}

function totalSumCost() {
    var totalOtherCost = 0.00;

    if (Table2.length != 0) {
        for (var i = 0; i < Table2.length; i++) {
            totalOtherCost += parseFloat(Table2[i].Amount);
        }
    }
    $("#TotalOtherCost").val(parseFloat(totalOtherCost).toFixed(2));
}

var numbervalidation = function (e) {
    var event = e.jQueryEvent,
    str = String.fromCharCode(event.keyCode);
    if (!/^[0-9]+$/.test(str))
        event.preventDefault();
};

function GetAssignmentCreationCheck() {
    var url = "/AssignmentForm/GetAssignmentCreationCheck";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
        success: function (response) {
            loadPanel.hide();
            if (response.Success) {
                var NonChargeable = $('#NonChargeable').is(":checked");
                if (NonChargeable == true) {
                    //AddUpdateNC();
                    AddUpdate();
                }
                else {
                    AddUpdateStat();
                }
            }
            else {
                AlertToast('error', response.Message);     
            }              
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


function GetAssignmentFormOnLoad(DocId) {
    var url = "/AssignmentForm/GetAssignmentForm";
    $.ajax({
        url: url,
        //type:'json'
        method: "GET",
        data: { ID: DocId },
        async: false,
    }).done(function (data) {
        //debugger;
        AssignmentFormListOnLoad = data.response;
        //Doc = data.Doc;
        console.log(AssignmentFormListOnLoad);
        //console.log(Doc);
        //$("#docNumber").val(Doc);
        //var abc = [];
    }).fail(function (data) {
        console.log(Doc);
    });
    //Doc;
    return AssignmentFormListOnLoad;
}

function GetAssignmentForm() {
    var url = "/AssignmentForm/GetAssignmentFormInit";
    $.ajax({
        url: url,
        //type:'json'
        method: "GET",
        data: {},
        //async: false,
    }).done(function (data) {
        //debugger;
        AssignmentFormList = data.response;
         Doc = data.Doc;
        console.log(AssignmentFormList);
        console.log(Doc);
        $("#docNumber").val(Doc);
        var abc = [];
    }).fail(function (data) {
        console.log(Doc);
    });
    Doc;
}


function GetAssignmentFormResourceOnLoad(DocId) {
    var url = "/AssignmentForm/GetAssignmentFormResourceOnLoad";
    $.ajax({
        url: url,
        method: "GET",
        data: { ID: DocId },
        //async: false,
    }).done(function (data) {
        AllEmployees = data.response;
        TaskList = data.TaskList;
        Tasklst = data.TaskList;
        //TravelLocationList = data.TravelLocationList;
        //ResourceBilling = data.ResourceBilling;
        console.log(TaskList);
        console.log(AllEmployees);
        //console.log(TravelLocationList);
        //console.log(ResourceBilling);
        var abc = []
        GenerateGrid(abc);
    }).fail(function (data) {
    });
}

function GetAssignmentFormResource() {
    var url = "/AssignmentForm/GetAssignmentFormResource";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        //async: false,
    }).done(function (data) {
        AllEmployees = data.response;
        TaskList = data.TaskList;
        Tasklst = data.TaskList;
        //TravelLocationList = data.TravelLocationList;
        //ResourceBilling = data.ResourceBilling;
        console.log(TaskList);
        console.log(AllEmployees);
        //console.log(TravelLocationList);
        //console.log(ResourceBilling);
        var abc = []
        GenerateGrid(abc);
    }).fail(function (data) {
    });
}

function GetAssignmentCost() {
    var url = "/AssignmentForm/GetAssignmentCostSetup";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        AllTypeOfCost = data.response;
        console.log(AllTypeOfCost);
        var abc = []
        GenerateGridCost(abc);
    }).fail(function (data) {
    });
}


function GetAssignmentFormSummary() {
    var url = "/AssignmentForm/GetAssignmentFormSummary";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        TaskList = data.response;
        console.log(TaskList);
        var abc = [];
        GenerateGridSummary(abc);

        //GenerateGridSummary(fgh);
    }).fail(function (data) {
    });
}

function ValidateAuthorization() {
    var id = $("#AuthID").val();
    if (id != -1) {
        if (id == 2) {
            $("#add_update").remove();
            $("#gridResourceContainer").dxDataGrid({ disabled: true });

        }
    }
}

function AddUpdate() {
    var ID = 0;
    FilteredRecordForID = AssignmentFormList.filter(x=>x.DOCNUM == $("#docNumber").val());
    if (FilteredRecordForID.length > 0) {
        ID = FilteredRecordForID[0].ID;
    }
    if (ID == undefined) {
        ID = 0;
    }
    var NonChargeable = $('#NonChargeable').is(":checked");

    var DocDate = $("#DocDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
    var FromDate = $("#StartDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
    var ToDate = $("#EndDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
    var day = FromDate.substr(0, 2);
    var month = FromDate.substr(3, 2);
    var year = FromDate.substr(8, 2);
    FromDate = month + '/' + day + '/' + year;

    var day = ToDate.substr(0, 2);
    var month = ToDate.substr(3, 2);
    var year = ToDate.substr(8, 2);
    ToDate = month + '/' + day + '/' + year;

    var day = DocDate.substr(0, 2);
    var month = DocDate.substr(3, 2);
    var year = DocDate.substr(8, 2);
    DocDate = month + '/' + day + '/' + year;
    
    var brnch = $("#Office > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
    var clientNme = $("#Client > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
    var textBoxInstance = $('#AssignmentTitle').dxTextBox({
    }).dxTextBox('instance');
    var assignTitle = textBoxInstance.option('value'); //Get the current value
    var Header = {
        BranchID: $("#Office").val(),
        BranchName: brnch,
        FunctionID: $("#Function").val(),
        SubFunctionID: $("#SubFunction").val(),
        //AssignmentTitle: $("#AssignmentTitle").val(),
        AssignmentTitle: assignTitle,
        PartnerID: $("#Partner").val(),
        ClientID: $("#Client").val(),
        ClientName: clientNme,
        DirectorID: $("#Director").val(),
        DocDate: DocDate,
        //TypeOfAssignment: $("#TypeOfAssignment").val(),
        //StartDate: FromDate,
        //EndDate: ToDate,
        //AssignmentNatureID: $("#NatureOfAssignment").val(),
        //TypeOfBilling: $("#TypeOfBilling").val(),
        //DurationInDays: $("#DurationInDays").val(),
        //CurrencyID: $("#Currency").val(),
        //ClosureDate: $("#ClosureDate").val(),
        //AssignmentValue: $("#AssignmentValue").val(),
        Status: Status,
        flgPost: flgPost
    };
    var General = {
        ID:$("#GeneralID").val(),
        TypeOfAssignment: $("#TypeOfAssignment").val(),
        StartDate: FromDate,
        EndDate: ToDate,
        AssignmentNatureID: $("#NatureOfAssignment").val(),
        TypeOfBilling: $("#TypeOfBilling").val(),
        DurationInDays: $("#DurationInDays").val(),
        CurrencyID: $("#Currency").val(),
        
        ClosureDate: $("#ClosureDate").val(),
        AssignmentValue: $("#AssignmentValue").val(),
        Status: $("#Status").val(),
    };

    loadPanel.show();
    var url = "/AssignmentForm/AddUpdateAssignmentForm";
    $.ajax({
        type: "POST",
        url: url,
        contentType: 'application/json',
        data: JSON.stringify({
            // AssignmentCode: $("#AssignmentCode").val(),
            DOCNUM: $("#docNumber").val(),
            NonChargeable: NonChargeable,
            AssignmentForm: Header,
            AssignmentFormGeneral: General,
            AssignmentFormChild: Table,
            AssignmentFormCost: Table2,
            AssignmentFormSummary: Table3,
            ID: ID
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
    //}
    //else {
    //    loadPanel.hide();
    //    AlertToast('error', "Please add atleast one new record");
    //}

}


$("#NonChargeable").dxCheckBox({
    text: "Non Chargeable",
    value: true,
    onValueChanged: function (e) {
        console.log(e.value);
        NonChargeable = e.value;
    }
});

$("#Updateable").dxCheckBox({
    text: "Updateable",
    value: true,
    onValueChanged: function (e) {
        console.log(e.value);
        NonChargeable = e.value;
    }
});

$("#docNumber").blur(function () {
    var DN = $("#docNumber").val()
    if (DN) {
        Table = AssignmentFormList.filter(x=>x.DOCNUM == DN);

    } else {
        Table = AssignmentFormList;
    }

    // GenerateGrid(Table);
    //GetMaster_TaskByDocNum($("#docNumber").val());
});





function GeneratePopupFindGrid() {
    //debugger;
    loadPanel.show();
    var ur = "/AssignmentForm/GetAssignmentFormHeader";
    $.ajax({
        url: ur,
        method: "GET",
        data: {},
        async:false,
    }).done(function (data) {
        var JSON_Response1 = [];
        JSON_Response1 = data.AssignmentForm;
        popupFindInstance = $("#FindContainer").dxDataGrid({
            dataSource: JSON_Response1,
            keyExpr: "DOCNUM",
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
                    dataField: "DOCNUM",
                    caption: "Assignment Code"
                },
                {
                    dataField: "AssignmentTitle",
                    caption: "Assignment Title"
                },
                {
                    dataField: "DocDate",
                    caption: "Doc Date",
                    dataType: "date",
                    format: "dd/MM/yyyy",

                },
                {
                    dataField: "NonChargeable",
                    caption: "Non Chargeable"
                },
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
                    dataField: "ClientID",
                    caption: "Client",
                    lookup: {
                        dataSource: clientFunctions,
                        valueExpr: "CLIENTID",
                        displayExpr: "CLIENTNAME",
                    }
                },
                {
                    dataField: "FunctionID",
                    caption: "Function",
                    lookup: {
                        dataSource: sapFunctions,
                        valueExpr: "FunctionID",
                        displayExpr: "FunctionName",
                    }
                },

                {
                    dataField: "SubFunctionID",
                    caption: "Sub Function",
                    lookup: {
                        dataSource: sapSubFunctions,
                        valueExpr: "SubFunctionID",
                        displayExpr: "SubFunctionName",
                    }
                },
                {
                    dataField: "PartnerID",
                    caption: "Partner",
                    lookup: {
                        dataSource: sapPartners,
                        valueExpr: "PartnerID",
                        displayExpr: "PartnerName",
                    }
                },
                {
                    dataField: "DirectorID",
                    caption: "Director",
                    lookup: {
                        dataSource: hcmDirector,
                        valueExpr: "DirectorID",
                        displayExpr: "PmName",
                    }
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

        if ($('#isView').val() == "True") {
            setTimeout(function () {
                $("#myModal_Find").css("display", "none");
            }, 100);

            loadPanel.show();

            popupFindInstance.selectRows([$('#viewDocNum').val()], true);
           // $("#btnOk").click();
            //window.setTimeout(function () { popupFindInstance.selectRows([$('#viewDocNum').val()], true); }, 0);
            //window.setTimeout(function () { $("#btnOk").click(); }, 0);
            //setTimeout(function () {
            //    $("#btnOk").click();
            //    loadPanel.hide();
            //}, 5000);
        }
        else
            $("#myModal_Find").css("display", "block");

    }).fail(function (data) {
        console.log(data);
        loadPanel.hide();
    });

}
$('#btnOk').click(function () {
    //System.Diagnostics.Debugger.Break();
    //debugger
    //refresh();
    //createNew();
    
    var selectedRow = popupFindInstance.getSelectedRowsData();
    var selectedRowOnLoad;
    if (selectedRow.length > 0) {
        DocId = selectedRow[0].ID;
        //selectedRowOnLoad =GetFormDataOnLoad(DocId);
        selectedRowOnLoad = GetAssignmentFormOnLoad(DocId);
        //selectedRowOnLoad = GetFormDataOnLoad();
        $("#docNumber").val(selectedRow[0].DOCNUM);
        $('#docNumber').prop("disabled", true);
        //$("#docNumber").dxTextBox({ value: selectedRow[0].DOCNUM });
        //$("#AssignmentTitle").val(selectedRow[0].AssignmentTitle);
        $("#AssignmentTitle").dxTextBox({ value: selectedRow[0].AssignmentTitle });
        $("#DocDate").dxDateBox({ value: selectedRow[0].DocDate, text: selectedRow[0].DocDate });
        $("#docStatus").dxSelectBox({ value: selectedRow[0].Status });


        $("#Updateable").hide();
        var approveStatus = selectedRow[0].Status;
        if (approveStatus == 2) {
            $("#Submit").hide();
            $("#add_update").hide();
            $("#Updateable").show();
        }
        else if (approveStatus == 4) {
            $("#add_update").hide();
            $("#Updateable").show();
            $("#Submit").text("Post to SBO");
            if (flgPost == true) {
                $("#Submit").hide();
                $("#Updateable").show();
            }
            else {
                $("#Submit").show();
            }
        }
        else {
            $("#Submit").text(" Submit ");
            $("#Submit").show();
            $("#add_update").show();
        }
        var NonChargeable = selectedRow[0].NonChargeable;
        if (NonChargeable == true) {
            $("#NonChargeable").prop("checked", NonChargeable);
        }
        else {
                $("#NonChargeable").prop("checked",false);
             }
            //AssignmentFormHeader = selectedRow[0].Table;
        $("#Office").dxSelectBox({ value: selectedRow[0].BranchID });
        $("#Client").dxSelectBox({ value: selectedRow[0].ClientID });
        $("#Function").dxSelectBox({ value: selectedRow[0].FunctionID });
        $("#SubFunction").dxSelectBox({ value: selectedRow[0].SubFunctionID });
        $("#Partner").dxSelectBox({ value: selectedRow[0].PartnerID });
            //$("#Director").dxSelectBox({ value: selectedRow[0].DirectorID });
        $("#Director").val(selectedRow[0].DirectorID);
        $("#Director").dxDropDownBox({ value: selectedRow[0].DirectorID });
            TaskUpdateResource();
            UpdateLocation();
            
        // var General = selectedRow[0].General;
        //if (selectedRowOnLoad.length > 0) {
            var General = selectedRowOnLoad[0].General;
        //}

        if (selectedRowOnLoad[0].General.length > 0) {
            $("#GeneralID").val(General[0].ID);
            $("#TypeOfAssignment").dxSelectBox({ value: General[0].TypeOfAssignment });
            $("#NatureOfAssignment").dxSelectBox({ value: General[0].AssignmentNatureID });
            $("#TypeOfBilling").dxSelectBox({ value: General[0].TypeOfBilling });
            $("#Currency").dxSelectBox({ value: General[0].CurrencyID });
            $("#AssignmentValue").val(General[0].AssignmentValue);
            $("#StartDate").dxDateBox({ value: General[0].StartDate, text: General[0].StartDate });
            $("#EndDate").dxDateBox({ value: General[0].EndDate, text: General[0].EndDate });
            $("#DurationInDays").val(General[0].DurationInDays);
            $("#ClosureDate").dxDateBox({ value: General[0].ClosureDate, text: General[0].ClosureDate });
            $("#Status").dxSelectBox({ value: General[0].Status });

        }
        else {
            var dt = new Date();
            $("#GeneralID").val("");
            $("#TypeOfAssignment").dxSelectBox({ value: ""  });
            $("#NatureOfAssignment").dxSelectBox({ value: "" });
            $("#TypeOfBilling").dxSelectBox({ value: "" });
            $("#Currency").dxSelectBox({ value: "" });
            $("#AssignmentValue").val("");
            $("#StartDate").dxDateBox({ value: dt, text: dt });
            $("#EndDate").dxDateBox({ value: dt, text: dt });
            $("#DurationInDays").val("");
            $("#ClosureDate").dxDateBox({ value: dt, text: dt });
            $("#Status").dxSelectBox({ value: "" });
        }
            //Table = selectedRow[0].Table;
            //Table2 = selectedRow[0].Table2
            //Table3 = selectedRow[0].Table3
            //GenerateGrid(selectedRow[0].Table);
            //GenerateGridCost(selectedRow[0].Table2);
            //GenerateGridSummary(selectedRow[0].Table3);
            //totalSum();

        Table = selectedRowOnLoad[0].Table;
        Table2 = selectedRowOnLoad[0].Table2
        Table3 = selectedRowOnLoad[0].Table3
        GenerateGrid(selectedRowOnLoad[0].Table);
        GenerateGridCost(selectedRowOnLoad[0].Table2);
        GenerateGridSummary(selectedRowOnLoad[0].Table3);
            totalSum();

        
            serialNo = Table.length;
        //refresh();
       
    }
    $('#myModal_Find').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
    loadPanel.hide();
});


function getFilteredData(id, type, Action) {
    idCopy = id;
    var AssignmentFormListCopy = [];
    let DocNum = '';
    if (type == 'DOCNUM') {
        //debugger;

        if (Action == 'next') {
            let l1 = AssignmentFormList.map(m=> { return m.DOCNUM });
            l1 = l1.sort();
            var unique = l1.filter((v, i, a) => a.indexOf(v) === i);

            FilteredRecord = AssignmentFormList.filter(x=>x.DOCNUM == unique[0]);
            FilteredRecord.forEach(function (obj) {
                Table = obj.Table;
                Table2 = obj.Table2;
                Table3 = obj.Table3;
            })

            if ($("#docNumber").val() == "") {
                DocNum = unique[0];
                id = DocNum;
            }
            else {
                DocNum = $("#docNumber").val().substr(4, $("#docNumber").val().length);
                DocNum = parseInt(DocNum) + 1;
                if (DocNum > unique.length) {
                    return;
                }
                DocNum = DocNum.toString();
                DocNum = 'ASS-' + DocNum.padStart(6, '0');
                FilteredRecord = AssignmentFormList.filter(x=>x.DOCNUM == DocNum);
                FilteredRecord.forEach(function (obj) {
                    Table = obj.Table;
                    Table2 = obj.Table2;
                    Table3 = obj.Table3;
                })
                id = DocNum;
            }
        }
        
        else if (Action == 'previous') {

            let l1 = AssignmentFormList.map(m=> { return m.DOCNUM });
            l1 = l1.sort();

            var unique = l1.filter((v, i, a) => a.indexOf(v) === i);

            FilteredRecord = AssignmentFormList.filter(x=>x.DOCNUM == unique[unique.length - 1]);
            FilteredRecord.forEach(function (obj) {
                Table = obj.Table;
                Table2 = obj.Table2;
                Table3 = obj.Table3;
            })
            if ($("#docNumber").val() == "") {
                DocNum = unique[unique.length - 1];
                id = DocNum;
                //DocNum = AssignmentFormList[0].DOCNUM;
                //id = DocNum;
            }
            else {
                DocNum = $("#docNumber").val().substr(4, $("#docNumber").val().length);
                DocNum = parseInt(DocNum) - 1;
                if (DocNum == 0) {
                    return;
                }
                DocNum = DocNum.toString();
                DocNum = 'ASS-' + DocNum.padStart(6, '0');
                FilteredRecord = AssignmentFormList.filter(x=>x.DOCNUM == DocNum);
                FilteredRecord.forEach(function (obj) {
                    Table = obj.Table;
                    Table2 = obj.Table2;
                    Table3 = obj.Table3;
                })
                id = DocNum;
            }
        }

        else if (Action == 'last') {
            let l1 = AssignmentFormList.map(m=> { return m.DOCNUM });
            l1 = l1.sort();

            var unique = l1.filter((v, i, a) => a.indexOf(v) === i);

            id = AssignmentFormList.filter(x=>x.DOCNUM == unique[unique.length - 1]);
            if (id.length > 0) {
                id = id[0].DOCNUM;
                FilteredRecord = AssignmentFormList.filter(x=>x.DOCNUM == id);
                FilteredRecord.forEach(function (obj) {
                    Table = obj.Table;
                    Table2 = obj.Table2;
                    Table3 = obj.Table3;
                })
            }
        }

        else if (Action == 'first') {
            id = AssignmentFormList[0].DOCNUM;
            FilteredRecord = AssignmentFormList.filter(x=>x.DOCNUM == id);
            FilteredRecord.forEach(function (obj) {
                Table = obj.Table;
                Table2 = obj.Table2;
                Table3 = obj.Table3;
            })
            if (id == "") {
                console.log("null id");
            }
        }

    }

    var AssignmentTitle;
    var Office;
    var Client;
    var Function;
    var SubFunction;
    var Partner;
    var Director;
    var NonChargeable;
    var Status;
    var General;
    var flgPost = false;
    var docDate;
    FilteredRecord.forEach(function (obj) {
        createNew();
        DocId = obj.ID;
        AssignmentCode = obj.DOCNUM;
        AssignmentTitle = obj.AssignmentTitle;
        Status = obj.Status;
        flgPost = obj.flgPost;
        docDate = obj.DocDate;

        NonChargeable = obj.NonChargeable;
        Office = obj.BranchID;
        Client = obj.ClientID;
        Function = obj.FunctionID;
        SubFunction = obj.SubFunctionID;
        Partner = obj.PartnerID;
        Director = obj.DirectorID;
        General = obj.General;
        Table = obj.Table;
        Table2 = obj.Table2;
        Table3 = obj.Table3;
    })
    //if (Table.length > 0) {
    //    DocNum = Table[0].DOCNUM;
    //}
    $('#docNumber').prop("disabled", true);
    $("#docNumber").val(AssignmentCode);
   // $("#AssignmentTitle").val(AssignmentTitle);
    $("#AssignmentTitle").dxTextBox({ value: AssignmentTitle });
    $("#DocDate").dxDateBox({ value: docDate, text: docDate });
    $("#docStatus").dxSelectBox({ value: Status });
    $("#Updateable").hide();
    if (Status == 2) {
        $("#Submit").hide();
        $("#add_update").hide();
        $("#Updateable").show();
    }
    else if (Status == 4) {
        $("#Updateable").show();
        $("#add_update").hide();
        $("#Submit").text("Post to SBO");
        if (flgPost == true) {
            $("#Submit").hide();
        }
        else {
            $("#Submit").show();
        }
    }
    //else if (Status == 5) {
    //    $("#Updateable").show();
    //    $("#Submit").show();
    //    $("#add_update").show();
    //}
    else {
        $("#Submit").text(" Submit ");
        $("#Submit").show();
        $("#add_update").show();
    }

    $("#NonChargeable").prop("checked", NonChargeable);
    $("#Office").dxSelectBox({ value: Office });
    $("#Client").dxSelectBox({ value: Client });
    $("#Function").dxSelectBox({ value: Function });
    $("#SubFunction").dxSelectBox({ value: SubFunction });
    $("#Partner").dxSelectBox({ value: Partner });
    $("#Director").val(Director);
    $("#Director").dxDropDownBox({ value: Director });
    // UpdateLocation();

    $("#GeneralID").val(General[0].ID);
    $("#TypeOfAssignment").dxSelectBox({ value: General[0].TypeOfAssignment });
    $("#NatureOfAssignment").dxSelectBox({ value: General[0].AssignmentNatureID });
    $("#TypeOfBilling").dxSelectBox({ value: General[0].TypeOfBilling });
    $("#Currency").dxSelectBox({ value: General[0].CurrencyID });
    $("#AssignmentValue").val(General[0].AssignmentValue);
    $("#StartDate").dxDateBox({ value: General[0].StartDate, text: General[0].StartDate });
    $("#EndDate").dxDateBox({ value: General[0].EndDate, text: General[0].EndDate });
    $("#DurationInDays").val(General[0].DurationInDays);
    $("#ClosureDate").dxDateBox({ value: General[0].ClosureDate, text: General[0].ClosureDate });
    $("#Status").dxSelectBox({ value: General[0].Status });
    GenerateGrid(Table);
    GenerateGridCost(Table2);
    GenerateGridSummary(Table3);

    totalSum();

    //if (id != undefined) {
    //    $("#docNumber").val(id);
    //}
}
var DocId = 0;

function loadAuthLogData(url) {
    if (DocId != 0) {
        url = url + "?docId=" + DocId;

        $.ajax({
            url: url,
            method: "GET",
            data: {},
            async: false,
        }).done(function (data) {
            data = JSON.parse(data.response.Data);
            console.log(data);
            $("#HeaderLogContainer").dxDataGrid({
                dataSource: data.Table,
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
            });
            $("#GeneralLogContainer").dxDataGrid({
                dataSource: data.Table1,
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
            });
            $("#ResourceLogContainer").dxDataGrid({
                dataSource: data.Table2,
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
            });
            $("#CostLogContainer").dxDataGrid({
                dataSource: data.Table3,
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
            });
            $("#SummaryLogContainer").dxDataGrid({
                dataSource: data.Table4,
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
            });

        }).fail(function (data) {
            console.log(data);
        });
    }



}

$('#btnOkUser').click(function () {
    //Table = [];
    var popupUsers = popupUserInstance.getSelectedRowsData();
    var sno = Table.length;

    $.each(popupUsers, function (index, val) {

        var dataExist = Table.filter(x=>x.UserID == val.ID);
        //if (dataExist.length == 0) {
            sno = sno + 1;

            var FunctionID = $("#Function").val();
            var DesignationID = val.DESIGNATIONID;
            var FromDate = $("#StartDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
            var ToDate = $("#EndDate > .dx-dropdowneditor-input-wrapper ").find("input")[1].value;
            var now = new Date();
            var datec = now.getDate();
            var monthc = now.getMonth();
            monthc += 1;
            if (monthc != 10 || monthc != 11 || monthc != 12) {
                monthc = 0 + '' + monthc;
            }
            if (datec < 10) {
                datec = "0" + datec;
            }
            var yearc = now.getFullYear();
            yearc = yearc.toString().substring(2, 4)
            now = yearc + '' + monthc + '' + datec;

            GetBillingRate(FunctionID, DesignationID, now, now);
            
            if (BillingRate.length == 0 || BillingRate == 0) {
                var dataGrid = $("#gridResourceContainer").dxDataGrid("instance");
                $("#gridResourceContainer").parent("tr").remove();
                AlertToast('error', "Please add billing rate of this user or select relevant function & Dates.");
            }
            else {
                var Details = {
                    //KEYMain: e.key + 1,
                    KEY: uuidv4(),
                    ID: 0,
                    SNO: sno,

                    FULLNAME: val.FULLNAME,
                    //BRANCHNAME: val.BRANCHNAME,
                                      
                    DesignationID: val.DesignationID,
                    DESIGNATIONNAME: val.DESIGNATIONNAME,
                    DepartmentID: val.DepartmentID,
                    DEPARTMENTNAME: val.DEPARTMENTNAME,
                    StdBillingRateHr: BillingRate,
                    ID: val.ID,
                    //USER_CODE: val.USER_CODE,
                    USER_CODE : val.EMPLOYEECODE,
                             
                    IsChargeable : true,
                    InActive : false,
                    ISDELETED: false
                };

                Table.push(Details);
            }
            TaskUpdateResource();
            UpdateLocation();



       // }

    });

    $('#myModal_Users').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();

    GenerateGrid(Table);
});
function GeneratePopupUserGrid() {
    loadPanel.show();

    popupUserInstance = $("#userContainer").dxDataGrid({
        dataSource: AllEmployees,
        keyExpr: "ID",
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
            mode: "multiple",
            showCheckBoxesMode: "always",
            allowSelectAll: true,
            selectAllMode: 'page'
        },
        columns: [
            {
                dataField: "EMPLOYEECODE",
                caption: "User Code"
            },
            {
                dataField: "FULLNAME",
                caption: "User Name"
            },
            {
                dataField: "DESIGNATIONNAME",
                caption: "Designation"
            },
            {
                dataField: "DEPARTMENTNAME",
                caption: "Department"
            },
            {
                dataField: "BRANCHNAME",
                caption: "Branch"
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
            $("#select-all-mode").dxSelect("instance").option("disabled", data.value === "none");
        }
    });

    //var popupUser_Table = [];
    //var popupUsersList = Table;
    popupUserInstance.clearSelection();
    //$.each(popupUsersList, function (index, val) {
    //    popupUser_Table.push(val.UserID);
    //});
    //popupUserInstance.selectRows(popupUser_Table, true);

    loadPanel.hide();

    $('#myModal_Users').modal('show');
}

function uuidv4() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}
