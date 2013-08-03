﻿using System.Linq;

namespace VocaDb.Model.Service.AlbumImport {

	public class AlbumImporters {

		private readonly IAlbumImporter[] importers = new IAlbumImporter[] { new KarenTAlbumImporter() };

		public AlbumImportResult ImportOne(string url) {

			var importer = importers.FirstOrDefault(i => i.IsValidFor(url));

			if (importer == null)
				return new AlbumImportResult {Message = "URL not recognized"};

			return importer.ImportOne(url);

		}

	}
}
