using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LinqToTwitter;

namespace DeleteYourTweets.Controllers
{
    public class OAuthController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Begin()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"]
                }
            };
            string twitterCallbackUrl = Request.Url.ToString().Replace("Begin", "Complate");
            return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl));
        }

        public async Task<ActionResult> Complate()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore()
            };

            await auth.CompleteAuthorizeAsync(Request.Url);
            
            return RedirectToAction("Index", "Home");
        }
    }
}