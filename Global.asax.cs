using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using urban_archive.Models;

namespace urban_archive
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
               if( HttpContext.Current.User.IsInRole("业务科")){


                }
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    // System.Security.Claims.ClaimsIdentity cid = new System.Security.Claims.ClaimsIdentity();

                    if (HttpContext.Current.User.Identity is System.Security.Claims.ClaimsIdentity)
                    {
                        UrbanUsersEntities db = new UrbanUsersEntities();
                        var user = db.AspNetUsers.Single(c => c.UserName == User.Identity.Name);
                        string rolename = user.RoleName;
                        string departname = user.DepartmentName;

                        var id = HttpContext.Current.User.Identity;
                        HttpContext.Current.User = new GenericPrincipal(id, new string[] { departname, rolename });
                    }
                }
            }
        }
    }
}
