﻿using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using System.Xml.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Model.Domain.Albums {

	public class Album : IEntryBase, IEntryWithNames, IEquatable<Album>, INameFactory<AlbumName>, IWebLinkFactory<AlbumWebLink> {

		private ArchivedVersionManager<ArchivedAlbumVersion, AlbumEditableFields> archivedVersions 
			= new ArchivedVersionManager<ArchivedAlbumVersion, AlbumEditableFields>();
		private TranslatedString artistString;
		private IList<ArtistForAlbum> artists = new List<ArtistForAlbum>();
		private IList<AlbumComment> comments = new List<AlbumComment>();
		private string description;
		private NameManager<AlbumName> names = new NameManager<AlbumName>();
		private AlbumRelease originalRelease = new AlbumRelease();
		private IList<PVForAlbum> pvs = new List<PVForAlbum>();
		private IList<SongInAlbum> songs = new List<SongInAlbum>();
		private TagManager<AlbumTagUsage> tags = new TagManager<AlbumTagUsage>();
		private IList<AlbumForUser> userCollections = new List<AlbumForUser>();
		private IList<AlbumWebLink> webLinks = new List<AlbumWebLink>();

		protected IEnumerable<Artist> ArtistList {
			get {
				return Artists.Select(a => a.Artist);
			}
		}

		public Album() {
			ArtistString = new TranslatedString(string.Empty, string.Empty, string.Empty);
			CreateDate = DateTime.Now;
			Deleted = false;
			Description = string.Empty;
			DiscType = DiscType.Album;
			OriginalRelease = new AlbumRelease();
			Status = EntryStatus.Draft;
		}

		public Album(string unspecifiedName)
			: this() {

			ParamIs.NotNullOrEmpty(() => unspecifiedName);

			Names.Add(new AlbumName(this, new LocalizedString(unspecifiedName, ContentLanguageSelection.Unspecified)));

		}

		public Album(TranslatedString translatedName)
			: this() {

			ParamIs.NotNull(() => translatedName);

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
			get { return Names.AllValues; }
		}

		public virtual IList<SongInAlbum> AllSongs {
			get { return songs; }
			set {
				ParamIs.NotNull(() => value);
				songs = value;
			}
		}

		public virtual ArchivedVersionManager<ArchivedAlbumVersion, AlbumEditableFields> ArchivedVersionsManager {
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

		public virtual TranslatedString ArtistString {
			get { return artistString; }
			set {
				ParamIs.NotNull(() => value);
				artistString = value;
			}
		}
		public virtual IList<AlbumComment> Comments {
			get { return comments; }
			set {
				ParamIs.NotNull(() => value);
				comments = value; 
			}
		}

		public virtual PictureData CoverPicture { get; set; }

		public virtual DateTime CreateDate { get; set; }

		public virtual string DefaultName {
			get {
				return TranslatedName.Default;
			}
		}

		public virtual bool Deleted { get; set; }

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		public virtual DiscType DiscType { get; set; }

		public virtual EntryType EntryType {
			get {
				return EntryType.Album;
			}
		}

		public virtual int Id { get; set; }

		public virtual int LastDiscNumber {
			get {
				return (Songs.Any() ? Songs.Max(s => s.DiscNumber) : 1);
			}
		}

		public virtual TranslatedString TranslatedName {
			get { return Names.SortNames; }
		}

		public virtual NameManager<AlbumName> Names {
			get { return names; }
			set {
				ParamIs.NotNull(() => value);
				names = value;
			}
		}

		INameManager IEntryWithNames.Names {
			get { return Names; }
		}

		public virtual AlbumRelease OriginalRelease {
			get { return originalRelease; }
			set {
				originalRelease = value;
			}
		}

		/// <summary>
		/// Original release date. Cannot be null.
		/// </summary>
		public virtual OptionalDateTime OriginalReleaseDate {
			get {

				if (OriginalRelease == null)
					OriginalRelease = new AlbumRelease();

				if (OriginalRelease.ReleaseDate == null)
					OriginalRelease.ReleaseDate = new OptionalDateTime();

				return OriginalRelease.ReleaseDate;

			}
		}

		public virtual IList<PVForAlbum> PVs {
			get { return pvs; }
			set {
				ParamIs.NotNull(() => value);
				pvs = value;
			}
		}

		public virtual int RatingCount { get; set; }

		public virtual int RatingTotal { get; set; }

		public virtual IEnumerable<SongInAlbum> Songs {
			get {
				return AllSongs.Where(s => !s.Song.Deleted);
			}
		}

		public virtual EntryStatus Status { get; set; }

		public virtual TagManager<AlbumTagUsage> Tags {
			get { return tags; }
			set {
				ParamIs.NotNull(() => value);
				tags = value;
			}
		}

		public virtual IList<AlbumForUser> UserCollections {
			get { return userCollections; }
			set {
				ParamIs.NotNull(() => value);
				userCollections = value;
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

		public virtual ArtistForAlbum AddArtist(Artist artist) {

			ParamIs.NotNull(() => artist);

			return artist.AddAlbum(this);

		}

		[Obsolete("Replaced by updating properties")]
		public virtual SongInAlbum AddSong(Song song) {
			
			ParamIs.NotNull(() => song);

			var discNum = LastDiscNumber;
			var trackNum = (Songs.Any(s => s.DiscNumber == discNum) 
				? Songs.Where(s => s.DiscNumber == discNum).Max(s => s.TrackNumber) + 1 : 1);

			return AddSong(song, trackNum, discNum);

		}

		public virtual SongInAlbum AddSong(Song song, int trackNum, int discNum) {

			ParamIs.NotNull(() => song);

			var track = new SongInAlbum(song, this, trackNum, discNum);
			AllSongs.Add(track);
			song.AllAlbums.Add(track);

			return track;

		}

		public virtual ArchivedAlbumVersion CreateArchivedVersion(XDocument data, AlbumDiff diff, AgentLoginData author, AlbumArchiveReason reason, string notes) {

			var archived = new ArchivedAlbumVersion(this, data, diff, author, Version, Status, reason, notes);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual AlbumComment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new AlbumComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

		}

		public virtual AlbumName CreateName(string val, ContentLanguageSelection language) {

			ParamIs.NotNullOrEmpty(() => val);

			var name = new AlbumName(this, new LocalizedString(val, language));
			Names.Add(name);

			return name;

		}

		public virtual PVForAlbum CreatePV(PVService service, string pvId, PVType pvType, string name) {

			ParamIs.NotNullOrEmpty(() => pvId);
			ParamIs.NotNull(() => name);

			var pv = new PVForAlbum(this, service, pvId, pvType, name);
			PVs.Add(pv);

			return pv;

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

		public virtual void DeleteArtistForAlbum(ArtistForAlbum artistForAlbum) {

			if (!artistForAlbum.Album.Equals(this))
				throw new ArgumentException("Artist is not attached to album", "artistForAlbum");

			AllArtists.Remove(artistForAlbum);
			artistForAlbum.Artist.AllAlbums.Remove(artistForAlbum);
			UpdateArtistString();

		}

		public virtual bool Equals(Album another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as Album);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		/// <summary>
		/// Checks whether this album has a specific artist.
		/// </summary>
		/// <param name="artist">Artist to be checked. Cannot be null.</param>
		/// <returns>True if the artist has this album. Otherwise false.</returns>
		public virtual bool HasArtist(Artist artist) {

			ParamIs.NotNull(() => artist);

			return Artists.Any(a => a.Artist.Equals(artist));

		}


		public virtual bool HasName(LocalizedString name) {

			ParamIs.NotNull(() => name);

			return Names.HasName(name);

		}

		public virtual bool HasSong(Song song) {

			ParamIs.NotNull(() => song);

			return Songs.Any(a => a.Song.Equals(song));

		}

		public virtual bool HasWebLink(string url) {

			ParamIs.NotNull(() => url);

			return WebLinks.Any(w => w.Url == url);

		}

		public virtual bool IsInUserCollection(User user) {

			ParamIs.NotNull(() => user);

			return UserCollections.Any(w => w.User.Equals(user));

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

		public virtual CollectionDiffWithValue<PVForAlbum, PVForAlbum> SyncPVs(IEnumerable<PVContract> newPVs) {

			ParamIs.NotNull(() => newPVs);

			var diff = CollectionHelper.Diff(PVs, newPVs, (n1, n2) => n1.Id == n2.Id);
			var created = new List<PVForAlbum>();
			var edited = new List<PVForAlbum>();

			foreach (var n in diff.Removed) {
				PVs.Remove(n);
			}

			foreach (var newEntry in diff.Added) {

				var l = CreatePV(newEntry.Service, newEntry.PVId, newEntry.PVType, newEntry.Name);
				created.Add(l);

			}

			foreach (var linkEntry in diff.Unchanged) {

				var entry = linkEntry;
				var newEntry = newPVs.First(e => e.Id == entry.Id);

				if (!entry.ContentEquals(newEntry)) {
					linkEntry.Name = newEntry.Name;
					edited.Add(linkEntry);
				}

			}

			return new CollectionDiffWithValue<PVForAlbum, PVForAlbum>(created, diff.Removed, diff.Unchanged, edited);

		}

		public virtual CollectionDiffWithValue<SongInAlbum, SongInAlbum> SyncSongs(
			IEnumerable<SongInAlbumEditContract> newTracks, Func<SongInAlbumEditContract, Song> songGetter) {

			var diff = CollectionHelper.Diff(Songs, newTracks, (n1, n2) => n1.Id == n2.SongInAlbumId);
			var created = new List<SongInAlbum>();
			var edited = new List<SongInAlbum>();

			foreach (var n in diff.Removed) {
				n.Delete();
			}

			foreach (var newEntry in diff.Added) {

				var song = songGetter(newEntry);

				var link = AddSong(song, newEntry.TrackNumber, newEntry.DiscNumber);
				created.Add(link);

			}

			foreach (var linkEntry in diff.Unchanged) {

				var entry = linkEntry;
				var newEntry = newTracks.First(e => e.SongInAlbumId == entry.Id);

				if (newEntry.TrackNumber != linkEntry.TrackNumber || newEntry.DiscNumber != linkEntry.DiscNumber) {
					linkEntry.DiscNumber = newEntry.DiscNumber;
					linkEntry.TrackNumber = newEntry.TrackNumber;
					edited.Add(linkEntry);
				}

			}

			return new CollectionDiffWithValue<SongInAlbum, SongInAlbum>(created, diff.Removed, diff.Unchanged, edited);

		}

		public override string ToString() {
			return string.Format("album '{0}' [{1}]", DefaultName, Id);
		}

		public virtual void UpdateArtistString() {

			ArtistString = ArtistHelper.GetArtistString(ArtistList);

		}

		public virtual void UpdateRatingTotals() {

			RatingCount = UserCollections.Where(a => a.Rating != AlbumForUser.NotRated).Count();
			RatingTotal = UserCollections.Sum(a => a.Rating);

		}

	}

}
