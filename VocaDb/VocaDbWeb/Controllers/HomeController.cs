﻿using System.Web.Mvc;
using VocaDb.Model.Domain;
using VocaDb.Web.Models;
using VocaDb.Web.Models.Home;

namespace VocaDb.Web.Controllers
{
    public class HomeController : ControllerBase
    {

		public ActionResult FindNames(string term) {

			var result = Services.Other.FindNames(term);

			return Json(result);

		}

        //
        // GET: /Home/

        public ActionResult Index() {

			var contract = MvcApplication.Services.Other.GetFrontPageContent();

            return View(contract);

        }

		[HttpPost]
		public ActionResult GlobalSearch(GlobalSearchBoxModel model) {

			string action, controller;
			if (model.ObjectType != EntryType.Undefined) {
				action = "Index";
				controller = model.ObjectType.ToString();
			} else {
				action = "Search";
				controller = "Home";
			}

			return RedirectToAction(action, controller, new {filter = model.GlobalSearchTerm});

		}

		public ActionResult Search(string filter) {

			filter = filter ?? string.Empty;
			var result = Services.Other.Find(filter, 15, true);

			if (result.OnlyOneItem) {

				if (result.Albums.Items.Length == 1)
					return RedirectToAction("Details", "Album", new { id = result.Albums.Items[0].Id });

				if (result.Artists.Items.Length == 1)
					return RedirectToAction("Details", "Artist", new { id = result.Artists.Items[0].Id });

				if (result.Songs.Items.Length == 1)
					return RedirectToAction("Details", "Song", new { id = result.Songs.Items[0].Id });

			}

			var model = new SearchEntries(filter, result.Albums, result.Artists, result.Songs);

			return View(model);

		}

    }
}
