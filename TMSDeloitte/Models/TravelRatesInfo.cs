using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TMSDeloitte.DAL;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Models
{

    public class TravelRates
    {
        [IgnorePropertyCompare]
        public  int?  SNO { get; set; }
        [IgnorePropertyCompare]
        public  int?  ID { get; set; }
        //public string TASK { get; set; }
        public bool? ISACTIVE { get; set; }
        public bool? ISDELETED { get; set; }
        [IgnorePropertyCompare]
        public  int?  CREATEDBY { get; set; }
        [IgnorePropertyCompare]
        public DateTime CREATEDATE { get; set; }
        [IgnorePropertyCompare]
        public  int?  UPDATEDBY { get; set; }
        [IgnorePropertyCompare]
        public DateTime UPDATEDATE { get; set; }
        [IgnorePropertyCompare]
        public string KEY { get; set; }
        [IgnorePropertyCompare]
        public string DOCNUM { get; set; } = "";
        [IgnorePropertyCompare]
        public int? BRANCHID { get; set; }
        
        public double FROMKM { get; set; }
        public double TOKM { get; set; }
        public double RATETRIP { get; set; }

        [IgnorePropertyCompare]
        public bool isNewDocument { get; set; }

        [IgnorePropertyCompare]
        public string FROMKM_TOKM { get; set; }

        //public List<Master_Task_Detail> taskList{ get; set; }


        public int Savetbl_award()
        {
            //HanaCommand loCommand = HanaDataContext.OpenConnection();

            //try
            //{
            //    var LoginedUser = HttpContext.Current.Session["UserInfo"];
            //    Models.Security.Sec_User User = (Models.Security.Sec_User)LoginedUser;
            //    loCommand = HanaDataContext.SetStoredProcedure(loCommand, "AddUpdatetbl_award");
            //    loCommand.Parameters.AddWithValue("Id", Id);
            //    loCommand.Parameters.AddWithValue("TenderId", TenderId);
            //    loCommand.Parameters.AddWithValue("AgencyId", AgencyId);
            //    loCommand.Parameters.AddWithValue("UserId", UserId);
            //    loCommand.Parameters.AddWithValue("dateAward", dateAward);
            //    loCommand.Parameters.AddWithValue("AwardDocument", AwardDocument);
            //    loCommand.Parameters.AddWithValue("IsDeleted", 0);
            //    loCommand.Parameters.AddWithValue("DateAdded", DateAdded);

            //    int result = 0;
            //    if (Id == 0)
            //    {
            //        result = HanaDataContext.ExecuteScalar(loCommand);
            //    }
            //    else
            //    {
            //        bool saved = HanaDataContext.ExecuteNonQuery(loCommand);
            //        result = Id;
            //    }

            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    HanaDataContext.CloseConnection(loCommand);
            //}
            return 0;
        }

     
    }
}