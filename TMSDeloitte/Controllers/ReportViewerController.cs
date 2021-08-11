using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMSDeloitte.Controllers
{
    public class ReportViewerController : BaseController
    {
        // GET: ReportViewer
        public ActionResult Index()
        {
            return View();
        }
    }
}