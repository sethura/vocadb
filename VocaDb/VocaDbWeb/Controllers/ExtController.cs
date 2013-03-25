﻿using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Utils;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Ext;

namespace VocaDb.Web.Controllers
{
    public class ExtController : ControllerBase
    {
        //
        // GET: /Ext/

		[OutputCache(Duration = 600, VaryByParam = "songId;pvId;lang")]
        public ActionResult EmbedSong(int songId = invalidId, int pvId = invalidId) {

			if (songId == invalidId)
				return NoId();

			if (string.IsNullOrEmpty(Request.Params[Model.Service.Security.LoginManager.LangParamName]))
				LoginManager.OverrideLanguage(ContentLanguagePreference.Default);

			var song = Services.Songs.GetSongWithPVAndVote(songId);

			return PartialView(song);

		}

		public ActionResult EntryToolTip(string url, string callback) {

			var route = new RouteInfo(new Uri(url), AppConfig.HostAddress).RouteData;
			var controller = route.Values["controller"].ToString();
			var id = int.Parse(route.Values["id"].ToString());
			string data = string.Empty;

			switch (controller) {
				case "Album":
					data = RenderPartialViewToString("AlbumWithCoverPopupContent", Services.Albums.GetAlbum(id));
					break;
				case "Artist":
					data = RenderPartialViewToString("ArtistPopupContent", Services.Artists.GetArtist(id));
					break;
			}

			return Json(data, callback);
		}

		public ActionResult OEmbed(string url, DataFormat format = DataFormat.Json) {

			var route = new RouteInfo(new Uri(url), AppConfig.HostAddress).RouteData;
			var controller = route.Values["controller"].ToString();

			if (controller != "Song") {
				return HttpStatusCodeResult(HttpStatusCode.BadRequest, "Only song embeds are supported");
			}

			var id = int.Parse(route.Values["id"].ToString());

			return Object(new SongOEmbedResponse {Html = string.Format("<iframe src='{0}'></iframe>", AppConfig.HostAddress + Url.Action("EmbedSong", new {id}))}, format);

		}

    }
}
