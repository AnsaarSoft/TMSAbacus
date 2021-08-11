using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.Helper;

namespace TMSDeloitte.Controllers
{
    public class EncDecController : BaseController
    {
        // GET: EncDec
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult EncDecString(string encrypt,string decrypt,bool url)
        {
            bool isSuccess = false;
            string message = "";
            try
            {
                Encrypt_Decrypt security = new Encrypt_Decrypt();
                if(url)
                {
                    if (!string.IsNullOrEmpty(encrypt))
                        decrypt = security.DecryptURLString(encrypt);
                    if (!string.IsNullOrEmpty(decrypt))
                        encrypt = security.EncryptURLString(decrypt);
                }
                else
                {
                    if (!string.IsNullOrEmpty(encrypt))
                        decrypt = security.DecryptString(encrypt);
                    if (!string.IsNullOrEmpty(decrypt))
                        encrypt = security.EncryptString(decrypt);
                }

                return Json(new { encrypt = encrypt, decrypt = decrypt }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogFile(ex.Message);
                log.InputOutputDocLog("EncDecController", "Exception occured on EncDecString Controller, " + ex.Message);
                return Json(new { encrypt = encrypt, decrypt = decrypt }, JsonRequestBehavior.AllowGet);
            }



        }
    }
}