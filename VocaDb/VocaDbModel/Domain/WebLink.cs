﻿using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain {

	public class WebLink {

		public static CollectionDiff<T,T> Sync<T>(IList<T> oldLinks, IEnumerable<WebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory) where T : WebLink {

			var diff = CollectionHelper.Diff(oldLinks, newLinks, (n1, n2) => n1.Id == n2.Id);
			var created = new List<T>();

			foreach (var n in diff.Removed) {
				oldLinks.Remove(n);
			}

			foreach (var linkEntry in newLinks) {

				var old = oldLinks.FirstOrDefault(n => n.Id == linkEntry.Id);

				if (old != null) {

					old.Description = linkEntry.Description;
					old.Url = linkEntry.Url;

				} else {

					var n = webLinkFactory.CreateWebLink(linkEntry.Description, linkEntry.Url);
					created.Add(n);

				}

			}

			return new CollectionDiff<T, T>(created, diff.Removed, diff.Unchanged);

		}

		private string description;
		private string url;

		public WebLink() {}

		public WebLink(string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			Description = description;
			Url = url;

		}

		/// <summary>
		/// User-visible link description. Cannot be null.
		/// </summary>
		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		/// <summary>
		/// Link description if the description is not empty. Otherwise URL.
		/// </summary>
		public virtual string DescriptionOrUrl {
			get {
				return !string.IsNullOrEmpty(Description) ? Description : Url;
			}
		}

		public virtual int Id { get; protected set; }

		/// <summary>
		/// Link URL. Cannot be null or empty.
		/// </summary>
		public virtual string Url {
			get { return url; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				url = value;
			}
		}

		public override string ToString() {
			return "web link '" + Url + "'";
		}

	}

}
