﻿using System;

namespace VocaDb.Model.Service.Helpers {

	public static class UrlHelper {

		/// <summary>
		/// Makes a proper URL from a possible URL without a http:// prefix.
		/// </summary>
		/// <param name="partialLink">Partial URL. Can be null.</param>
		/// <param name="assumeWww">Whether to assume the URL should start with www.</param>
		/// <returns>Full URL including http://. Can be null if source was null.</returns>
		public static string MakeLink(string partialLink, bool assumeWww = false) {

			if (string.IsNullOrEmpty(partialLink))
				return partialLink;

			if (partialLink.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || partialLink.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
				return partialLink;

			if (assumeWww && !partialLink.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase))
				return string.Format("http://www.{0}", partialLink);

			return string.Format("http://{0}", partialLink);

		}

	}

}
