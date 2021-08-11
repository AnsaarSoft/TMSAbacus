using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMSDeloitte.Models;

namespace TMSDeloitte.Controllers
{
    public class BaseController : Controller
    {
        UserSession _user;
        
        public UserSession CurrentUser
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["TMSUserSession"] != null)
                {
                    _user = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];
                }
                return _user;
            }
            set
            {
                _user = value;
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {


            var urlHelper = new UrlHelper(filterContext.RequestContext);
            if (System.Web.HttpContext.Current.Session["TMSUserSession"] != null)
            {

                UserSession sess = (UserSession)System.Web.HttpContext.Current.Session["TMSUserSession"];
               
                if (!Convert.ToBoolean(sess.SessionUser.ISSUPER))
                {

                    var rd = System.Web.HttpContext.Current.Request.RequestContext.RouteData;
                    string currentController = rd.GetRequiredString("controller");
                    string currentAction = rd.GetRequiredString("action");
                    string currentURL = "/"+currentController+"/"+currentAction;
                    bool isAllowed = true;
                    string currentMethod = System.Web.HttpContext.Current.Request.HttpMethod;

                    //For view only Authorization
                    var menu = sess.pagelist.Where(x => x.PageURL.ToLower().Contains("/"+currentController.ToLower()+"/")).FirstOrDefault();
                    if (menu == null)
                        isAllowed = false;
                    else
                    {
                        if (menu.Role == 2)
                            if (currentMethod!= "GET")
                                isAllowed = false;    
                    }
                    //End

                    if (!isAllowed)
                    {
                        if (!filterContext.HttpContext.Request.IsAjaxRequest())
                            filterContext.Result = new RedirectResult("~/Home");
                        else
                        {
                            filterContext.HttpContext.Response.StatusCode = 403;
                            filterContext.Result = new JsonResult
                            {
                                Data = new
                                {
                                    Error = "NotAuthorized",
                                    LogOnUrl = urlHelper.Action("Index", "Home")
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                    }

                }

            }

            else
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                    filterContext.Result = new RedirectResult("~/Home");
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 403;
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            Error = "SessionExpire",
                            LogOnUrl = urlHelper.Action("Index", "Home")
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            base.OnActionExecuting(filterContext);
        }

    }
}