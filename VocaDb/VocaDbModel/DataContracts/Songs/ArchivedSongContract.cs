﻿using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Utils;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedSongContract {

		private static void DoIfExists(ArchivedSongVersion version, SongEditableFields field,
			XmlCache<ArchivedSongContract> xmlCache, Action<ArchivedSongContract> func) {

			var versionWithField = version.GetLatestVersionWithField(field);

			if (versionWithField != null && versionWithField.Data != null) {
				var data = xmlCache.Deserialize(versionWithField.Version, versionWithField.Data);
				func(data);
			}

		}

		private static void DoIfExists(ArchivedSongVersion version, SongEditableFields field, XmlCache<ArchivedSongContract> xmlCache, Action<ArchivedSongContract, XDocument> func) {

			var versionWithField = version.GetLatestVersionWithField(field);

			if (versionWithField != null && versionWithField.Data != null) {
				var data = xmlCache.Deserialize(versionWithField.Version, versionWithField.Data);
				func(data, versionWithField.Data);
			}

		}

		private static void SetArtists(ArchivedSongContract data, ArchivedSongContract serialized, XDocument doc) {

			if (serialized.Artists != null && serialized.Artists.Any())
				data.Artists = serialized.Artists;
			else {

				// For backwards compatibility
				var artistElems = doc.XPathSelectElements("//ArchivedAlbumContract/Artists/ObjectRefContract"); //.Elements("Artists").Elements("ObjectRefContract"));

				data.Artists = artistElems.Select(e => new ArchivedArtistForSongContract {
					Id = int.Parse(e.Element("Id").Value), NameHint = e.Element("NameHint").Value
				}).ToArray();

			}

		}

		public static ArchivedSongContract GetAllProperties(ArchivedSongVersion version) {

			var data = new ArchivedSongContract();
			var xmlCache = new XmlCache<ArchivedSongContract>();
			var thisVersion = xmlCache.Deserialize(version.Version, version.Data);

			data.Id = thisVersion.Id;
			data.LengthSeconds = thisVersion.LengthSeconds;
			data.NicoId = thisVersion.NicoId;
			data.Notes = thisVersion.Notes;
			data.OriginalVersion = thisVersion.OriginalVersion;
			data.SongType = thisVersion.SongType;
			data.TranslatedName = thisVersion.TranslatedName;

			DoIfExists(version, SongEditableFields.Artists, xmlCache, (v, doc) => SetArtists(data, v, doc));
			DoIfExists(version, SongEditableFields.Lyrics, xmlCache, v => data.Lyrics = v.Lyrics);
			DoIfExists(version, SongEditableFields.Names, xmlCache, v => data.Names = v.Names);
			DoIfExists(version, SongEditableFields.PVs, xmlCache, v => data.PVs = v.PVs);
			DoIfExists(version, SongEditableFields.WebLinks, xmlCache, v => data.WebLinks = v.WebLinks);

			return data;

		}

		public ArchivedSongContract() { }

		public ArchivedSongContract(Song song, SongDiff diff) {

			ParamIs.NotNull(() => song);
			ParamIs.NotNull(() => diff);

			Artists = (diff.IncludeArtists ? song.Artists.Select(a => new ArchivedArtistForSongContract(a)).ToArray() : null);
			Id = song.Id;
			LengthSeconds = song.LengthSeconds;
			Lyrics = (diff.IncludeLyrics ? song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray() : null);
			Names = (diff.IncludeNames ? song.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null);
			NicoId = song.NicoId;
			Notes = song.Notes;
			OriginalVersion = (song.OriginalVersion != null ? new ObjectRefContract(song.OriginalVersion) : null);
			PVs = (diff.IncludePVs ? song.PVs.Select(p => new ArchivedPVContract(p)).ToArray() : null);
			SongType = song.SongType;
			TranslatedName = new ArchivedTranslatedStringContract(song.TranslatedName);
			WebLinks = (diff.IncludeWebLinks ? song.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null);
			
		}

		[DataMember]
		public ArchivedArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int LengthSeconds { get; set; }

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public string NicoId { get; set; }

		[DataMember]
		public string Notes { get; set; }

		[DataMember]
		public ObjectRefContract OriginalVersion { get; set; }

		[DataMember]
		public ArchivedPVContract[] PVs { get; set; }

		[DataMember]
		public SongType SongType { get; set; }

		[DataMember]
		public ArchivedTranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }


	}

}
