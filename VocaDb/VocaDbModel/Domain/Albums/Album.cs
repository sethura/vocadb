﻿using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Linq.Utilities;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using System.Xml.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Albums {

	public class Album {

		private IList<ArchivedAlbumVersion> archivedVersions = new List<ArchivedAlbumVersion>();
		private IList<ArtistForAlbum> artists = new List<ArtistForAlbum>();
		private string description;
		private IList<AlbumName> names = new List<AlbumName>();
		private IList<SongInAlbum> songs = new List<SongInAlbum>();
		private IList<AlbumWebLink> webLinks = new List<AlbumWebLink>();

		public Album() {
			Deleted = false;
			Description = string.Empty;
			DiscType = DiscType.Album;
			ProductCode = string.Empty;
			ReleaseDate = null;
			TranslatedName = new TranslatedString();
		}

		public Album(TranslatedString translatedName)
			: this() {

			TranslatedName = translatedName;
			
			foreach (var name in translatedName.AllLocalized)
				Names.Add(new AlbumName(this, name));

		}

		public virtual IList<ArtistForAlbum> AllArtists {
			get { return artists; }
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		public virtual IEnumerable<string> AllNames {
			get {
				return TranslatedName.All
					.Concat(Names.Select(n => n.Value))
					.Distinct();
			}
		}

		public virtual IList<SongInAlbum> AllSongs {
			get { return songs; }
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual IList<ArchivedAlbumVersion> ArchivedVersions {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual IEnumerable<ArtistForAlbum> Artists {
			get {
				return AllArtists.Where(a => !a.Artist.Deleted);
			}
		}

		public virtual PictureData CoverPicture { get; set; }

		public virtual bool Deleted { get; set; }

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual DiscType DiscType { get; set; }

		public virtual int Id { get; set; }

		public virtual TranslatedString TranslatedName { get; set; }

		public virtual string Name {
			get {
				return TranslatedName.Default;
			}
			set {
				TranslatedName.Default = value;
			}
		}

		public virtual IList<AlbumName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		public virtual string ProductCode { get; set; }

		public virtual DateTime? ReleaseDate { get; set; }

		public virtual IEnumerable<SongInAlbum> Songs {
			get {
				return AllSongs.Where(s => !s.Song.Deleted);
			}
		}

		public virtual int Version { get; set; }

		public virtual IList<AlbumWebLink> WebLinks {
			get { return webLinks; }
			set {
				ParamIs.NotNull(() => value);
				webLinks = value;
			}
		}

		public virtual SongInAlbum AddSong(Song song) {
			
			ParamIs.NotNull(() => song);

			var trackNum = (Songs.Any() ? Songs.Max(s => s.TrackNumber) + 1 : 1);
			var track = new SongInAlbum(song, this, trackNum);
			AllSongs.Add(track);
			song.AllAlbums.Add(track);

			return track;

		}

		public virtual ArchivedAlbumVersion CreateArchivedVersion(XDocument data, AgentLoginData author) {

			var archived = new ArchivedAlbumVersion(this, data, author, Version);
			ArchivedVersions.Add(archived);
			Version++;

			return archived;

		}

		public virtual AlbumName CreateName(string val, ContentLanguageSelection language) {

			ParamIs.NotNullOrEmpty(() => val);

			var name = new AlbumName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;

		}

		public virtual AlbumWebLink CreateWebLink(string description, string url) {

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrEmpty(() => url);

			var link = new AlbumWebLink(this, description, url);
			WebLinks.Add(link);

			return link;

		}

		public virtual void Delete() {

			Deleted = true;

		}

		public virtual bool Equals(Album another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as Album);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void MoveSongDown(SongInAlbum songInAlbum) {

			ParamIs.NotNull(() => songInAlbum);

			if (!songInAlbum.Album.Equals(this))
				throw new ArgumentException("Song is not in album");

			var next = Songs.FirstOrDefault(s => s.TrackNumber == songInAlbum.TrackNumber + 1);

			if (next != null) {
				next.TrackNumber--;
				songInAlbum.TrackNumber++;				
			}

		}

		public virtual void MoveSongUp(SongInAlbum songInAlbum) {

			ParamIs.NotNull(() => songInAlbum);

			if (!songInAlbum.Album.Equals(this))
				throw new ArgumentException("Song is not in album");

			var prev = Songs.FirstOrDefault(s => s.TrackNumber == songInAlbum.TrackNumber - 1);

			if (prev != null) {
				prev.TrackNumber++;
				songInAlbum.TrackNumber--;				
			}

		}

		public virtual void OnSongDeleting(SongInAlbum songInAlbum) {
			
			ParamIs.NotNull(() => songInAlbum);

			if (!songInAlbum.Album.Equals(this))
				throw new ArgumentException("Song is not in album");

			foreach (var song in Songs.Where(song => song.TrackNumber > songInAlbum.TrackNumber)) {
				song.TrackNumber--;
			}

			AllSongs.Remove(songInAlbum);

		}

	}

}
