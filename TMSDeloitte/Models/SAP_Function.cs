using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMSDeloitte.Models
{
    public class SAP_Function
    {   public string FunctionID { get; set; }
        public string FunctionCode { get; set; }
        public string FunctionName { get; set; }

        public string SubFunctionID { get; set; }
        public string SubFunctionName { get; set; }
        public string PartnerID { get; set; }
        public string PartnerName { get; set; }

        public string DirectorID { get; set; }
        public string DirectorName { get; set; }

        public int BranchID { get; set; }
        public string BranchName { get; set; }

        public string CLIENTID { get; set; }

        public string CLIENTNAME { get; set; }

        public string CurrencyID { get; set; }

        public string CurrencyName { get; set; }
        public string AcctCode { get; set; }
        public string AcctName { get; set; }


    }
}