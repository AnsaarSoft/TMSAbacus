using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace TMSDeloitte {

    public class BundleConfig {


        public static void RegisterBundles(BundleCollection bundles) {

            var scriptBundle = new ScriptBundle("~/Scripts/bundle");
            var styleBundle = new StyleBundle("~/Content/bundle");

            // jQuery
            scriptBundle
                .Include("~/Scripts/jquery-3.4.1.js");

            // Bootstrap
            scriptBundle
                .Include("~/Scripts/bootstrap.js");

            // jquery.toast
            scriptBundle
                .Include("~/Scripts/jquery.toast.js");

            // toastNotifications
            scriptBundle
                .Include("~/Scripts/toastNotifications.js");

            // jquery.cookie.js
            scriptBundle
                .Include("~/Scripts/jquery.cookie.js");

            //Style.css
            styleBundle
              .Include("~/assets/css/style.css");
            styleBundle
           .Include("~/assets/css/customizer.css");

            // Bootstrap
            styleBundle
                .Include("~/Content/bootstrap.css");

            // Custom site styles
            styleBundle
                .Include("~/Content/Site.css");

            //jquery.toast
            styleBundle
              .Include("~/Content/jquery.toast.css");

            bundles.Add(scriptBundle);
            bundles.Add(styleBundle);

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}