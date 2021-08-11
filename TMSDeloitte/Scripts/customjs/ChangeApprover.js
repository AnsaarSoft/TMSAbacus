var ddEmpID = 0;
var EmpCode = "";
var EmpName = "";
var DesignationID = 0;
var DepartmentID = 0;
var PendingList = [];
var AllEmployees = [];
var isPageDataLoaded = false;


$('document').ready(function () {
    loadPanel.show();
    setTimeout(function () { GetHCMFunction(); }, 100);
    stopLoading();
     
    GenerateGrid([]);
    btnSearch();
    btnAdd();
});

function stopLoading() {
    if (isPageDataLoaded == false) {
        setTimeout(function () { stopLoading(); }, 500);
    }
    else {
        loadPanel.hide();
    }
}

function btnSearch() {
   // debugger;
    $('#btnSearch').click(function () {
        var isSuccess = true;
        var message = '';
        var alertSetupObj = {
            EmpID: $("#ddEmpID").val(),
            DesignationID: $("#ddDesignation").val(),
            DepartmentID: $("#ddDepartment").val()
        };

        if (alertSetupObj.EmpID == "" && alertSetupObj.DesignationID == "" && alertSetupObj.DepartmentID == "") {
            isSuccess = false;
            message = 'Please select any one. \n';
        }
        if (isSuccess) {
            var url = "/ChangeApprover/GetChangeApproverForms";
            var EmpID = $("#ddEmpID").val();
            var DesignationID = $("#ddDesignation").val();
            if (DesignationID == "") {
                DesignationID = 0;
            }
            var DepartmentID = $("#ddDepartment").val();
            if (DepartmentID == "") {
                DepartmentID = 0;
            }
            GetChangeApproverForm(EmpID, DesignationID, DepartmentID);
        }
        else {
            AlertToast('error', message);
        }
    });
}

function btnAdd() {
    $('#add_update').click(function () {
        var isSuccess = true;
        var message = '';
        
        var SelectedItems = PendingList.filter(x=>x.Status == true);
       // var SelectedItems = PendingList.filter(x=>x.Status == true)[0];

        if (SelectedItems.length == 0) {
            isSuccess = false;
            message = 'Please select any one from list then change. \n';
        }
        if (isSuccess) {
            var url = "/ChangeApprover/UpdateApprovalSetup";
            $.ajax({
                type: "POST",
                url: url,
                contentType: 'application/json',
                data: JSON.stringify({

                    ChangeApproverChild: SelectedItems
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
    });
}

function GetHCMUser() {
    loadPanel.show();
    var url = "/User/GetHCMUsers";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false,
    }).done(function (data) {
        var JSON_Response = data.response;
         GenerateHCMUserTableDropDown(JSON_Response);
        loadPanel.hide();
    }).fail(function (data) {
        loadPanel.hide();
    });
}

function GenerateHCMUserTableDropDown(jsonData) {
    $("#ddEmpID").dxDropDownBox({
        valueExpr: "ID",
        placeholder: "Select an Employee...",
        displayExpr: "FULLNAME",
        showClearButton: true,
        dataSource: jsonData,
        contentTemplate: function (e) {
            var value = e.component.option("value"),
                $dataGrid = $("<div>").dxDataGrid({
                    dataSource: e.component.getDataSource(),
                    columns: [
                        {
                            dataField: "ID",
                            caption: "ID",
                            allowEditing: false,
                            visible: false
                        },
                        "EMPLOYEECODE", "FULLNAME"],
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
                                    e.component.option("value", keys[0].ID);

                                    ddEmpID = keys[0].ID;
                                    EmpCode = keys[0].EmpID;
                                    EmpName = keys[0].EmpName;
                                    $("#ddEmpID").val(ddEmpID);
                                    
                                    $("#ddEmpID").dxDropDownBox("instance").close();
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

function GetHCMFunction() {
    var url = "/ChangeApprover/GetHCMFunctions";
    $.ajax({
        url: url,
        method: "GET",
        data: {},
        async: false
    }).done(function (data) {
        allDepartmentList = data.DepartmentList;
        allDesignationList = data.DesignationList;
        AllEmployees = data.AllEmployee;
        ChangeTo = data.AllEmployees;
        GenerateSAPFunctionDropDown(allDepartmentList, "#ddDepartment", "Name", "Id", "Select department");
        GenerateSAPFunctionDropDown(allDesignationList, "#ddDesignation", "Name", "Id", "Select designation");
        GenerateHCMUserTableDropDown(AllEmployees);
        isPageDataLoaded = true;
    }).fail(function (data) {
        isPageDataLoaded = true;
        loadPanel.hide();
    });
}

function GenerateGrid(JSON_Response) {
    Table = JSON_Response;
    $(function () {
        $("#gridContainer").dxDataGrid({
            dataSource: JSON_Response,
            keyExpr: "KEY",
            width: "1850px",
            height: "300px",
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
                mode: "cell",
                allowUpdating: true,
                allowDeleting: true,
                useIcons: true
            },
            columns: [
                
                {
                    dataField: "SNO",
                    caption: "S.No",
                    dataType: "Number",
                    allowEditing: false,
                    width: "auto",
                    visible: false
                },
                {
                    dataField: "ID",
                    caption: "ID",
                    width: "auto",
                    allowEditing: false,
                    visible: false
                },
                {
                    dataField: "EmpID",
                    caption: "Emp Code",
                    width: "auto",
                    allowEditing: false,
                    visible: false
                },
                {
                    dataField: "EmpCode",
                    caption: "Emp Code",
                    width: "auto",
                    width: "5%",
                    allowEditing: false,
                    visible: true
                },
                {
                    dataField: "FullName",
                    caption: "Employee Name",
                    width: "11%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                },
                {
                    dataField: "DepartmentName",
                    caption: "Department",
                    width: "11%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                },
                {
                    dataField: "DesignationName",
                    caption: "Designation",
                    width: "11%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                },
                {
                    dataField: "Year",
                    caption: "Year",
                    width: "3%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                },
                {
                    dataField: "Month",
                    caption: "Month",
                    width: "4%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                },
                {
                    dataField: "Weeks",
                    caption: "Week",
                    width: "3%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    }
                },
                {
                    dataField: "PendingAt",
                    caption: "Pending At",
                    width: "11%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    },
                },
                {
                    dataField: "CHANGETOEMPID",
                    caption: "Change To",
                    width: "22%",
                    allowEditing: true,
                    setCellValue: function (rowData, value, currentRowData) {

                        rowData.ID = value;
                        currentRowData.ID = value;

                        var SelectedEmp = ChangeTo.filter(x=>x.ID == value)[0];
                        if (SelectedEmp != undefined) {
                            rowData.CHANGETOEMPID = SelectedEmp.ID;
                            rowData.CHANGETOEMPCODE = SelectedEmp.EMPLOYEECODE;
                            rowData.CHANGETONAME = SelectedEmp.FULLNAME;
                            rowData.Status = true;
                        }
                    },

                    lookup: {
                        dataSource: ChangeTo,
                        valueExpr: "ID",
                        displayExpr: "DETAILNAME"
                    }
                },
                {
                    dataField: "CHANGETOEMPCODE",
                    caption: "Change Code",
                    width: "5%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    },
                },

                {
                    dataField: "CHANGETONAME",
                    caption: "Change Name",
                    width: "11%",
                    allowEditing: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    },
                },
                {
                    dataField: "Status",
                    caption: "Status",
                    dataType: "boolean",
                    //width: "10%",
                    allowEditing: false,
                    visible: false,
                    customizeText: function (rowData) {
                        return rowData.value;
                    },
                },
                
            ],
            onEditorPreparing: function (e) {

            },
            onEditorPrepared: function (e) {
                
            },
            onEditingStart: function (e) {
            },
            //onInitNewRow: function (e) {
            //},
            //onRowInserting: function (e) {
            //},
            //onRowInserted: function (e) {
            //},
            onRowUpdating: function (e) {
            },
            onRowUpdated: function (e) {
                $.each(Table, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                        //val.ISDELETED = e.data.ISDELETED;
                    }

                });
            },
            onRowRemoving: function (e) {
            },
            onRowRemoved: function (e) {
                $.each(Table, function (index, val) {
                    if (val.KEY == e.data.KEY) {
                        //val.ISDELETED = true;
                    }
                });
            }
        });

    });
}

var ChangeTo = [];
function GetChangeApproverForm(EmpID, DesignationID, DepartmentID) {
    var url = "/ChangeApprover/GetChangeApproverForms";
    loadPanel.show();
    $.ajax({
        url: url,
        method: "GET",
        data: {
            EmpID: EmpID,
            DesignationID: DesignationID,
            DepartmentID: DepartmentID
        },
        async: false,
        success: function (data) {
            loadPanel.hide();
            if (data.Success) {
                PendingList = data.response;
                //AllEmployees = data.AllEmployees;
                if (PendingList.length > 0) {
                    var depID = PendingList[0].DepartmentID;
                    console.log(PendingList);
                    ChangeTo = ChangeTo.filter(x => x.DEPARTMENTID == depID);
                    GenerateGrid(PendingList);
                }
                else {
                    AlertToast('error', "no record!");
                }
            }
            else {
                AlertToast('error', "Please select anyone!");
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
