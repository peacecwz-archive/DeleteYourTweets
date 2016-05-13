using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DeleteYourTweets.App_Start;
using DeleteYourTweets.Helpers;
using LinqToTwitter;

namespace DeleteYourTweets.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        // GET: Home
        public ActionResult Index()
        {
            if (TwitterHelper.IsLogged)
                ViewBag.Name = TwitterHelper.GetName();
            return View();
        }

        [CustomAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Models.Options model)
        {
            if (ModelState.IsValid)
            {
                if (!TwitterHelper.IsLogged) return View(model);
                if (model.Tweets)
                    await TwitterHelper.DeleteAllTweets();
                if (model.Favourites)
                    await TwitterHelper.DeleteAllFavourites();
                if (model.Followings)
                    await TwitterHelper.UnfollowAllFollowings();
                if (model.Messages)
                {
                    await TwitterHelper.DeleteAllSendByMessages();
                    await TwitterHelper.DeleteAllSendToMessages();
                }
                await TwitterHelper.SendTweet("Delete all my tweets. I'm starting new life. Do you want it? Visit here! app.barisceviz.com/Twitter/ Thanks @PeacecCwz");
                ModelState.AddModelError("", "İşletiminiz Tamamlanmıştır");
            }
            return View();
        }
    }
}