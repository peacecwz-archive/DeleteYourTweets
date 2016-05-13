using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LinqToTwitter;

namespace DeleteYourTweets.App_Start
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!new SessionStateCredentialStore().HasAllCredentials())
                return false;
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);

            filterContext.Result = new RedirectResult(urlHelper.Action("Begin", "OAuth"));
        }
    }
}
