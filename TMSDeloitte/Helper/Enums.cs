using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Helper
{
    public static class Enums
    {

        public enum FormsOperations
        {
            Add = 1,
            Update = 2,
            Delete = 3
        }
        public enum FormsName
        {
            UserManagement=101,
            UserAuthorization=102,
            UserDataAccess=103,
            AssignmentCostSetup = 10041,
            TaskMasterSetup = 10042,
            AssignmentNatureSetup=10043,
            NonChargeableSetup=10044,
            TypeOfClaimSetup=10045,
            ResourceBillingRateSetup=10046,
            TravelRatesSetup=10047,
            TravelLocationSetup=10048,
            TimeSheetPeriodSetup=10049,
            AlertSetup= 100410,
            GroupSetup= 100411,
            HoliDaySetup = 100412,
            ApprovalStages = 10051,
            ApprovalSetup=10052,
            ChangeApprover=10053,
            ApprovalDecision=10054,
            AssignmentForm=10061,
            NCTaskAssignmentForm = 10062,
            WIPRecordingForm=10063,
            TimeSheetForm=301,
            TimeSheetView=302,
            MonthlyTravelManagement=401,
            ClaimForm=402,
            ReportUploader=501,
            ReportViewer=502,
            UserReport=503,
            HigherManagementReport=504
        }
        public enum UserAuthorization
        {
            Full_Authorization = 0,
            No_Authorization = 1,
            View_Only=2
        }

        public enum General
        {
            Nonchargeable_Task = 100001,
            Absence_Management_Internal = 200001
        }

    }
}