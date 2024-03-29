﻿using System;
using System.Linq;
using System.Net.Mail;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using System.Collections.Generic;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Users {

	public class User : IEntryWithNames, IUser, IEquatable<User>, IWebLinkFactory<UserWebLink> {

		INameManager IEntryWithNames.Names {
			get {
				return new SingleNameManager(Name);
			}
		}

		private string accessKey;
		private PermissionCollection additionalPermissions = new PermissionCollection();
		private IList<AlbumForUser> albums = new List<AlbumForUser>();
		private IList<ArtistForUser> artists = new List<ArtistForUser>();
		private IList<UserComment> comments = new List<UserComment>();
		private string culture;
		private string email;
		private IList<FavoriteSongForUser> favoriteSongs = new List<FavoriteSongForUser>();
		private string language;
		private string name;
		private string nameLc;
		private UserOptions options;
		private IList<OwnedArtistForUser> ownedArtists = new List<OwnedArtistForUser>(); 
		private string password;
		private IList<UserMessage> receivedMessages = new List<UserMessage>();
		private IList<UserMessage> sentMessages = new List<UserMessage>();
		private IList<SongList> songLists = new List<SongList>();
		private IList<UserWebLink> webLinks = new List<UserWebLink>();

		public User() {

			Active = true;
			AnonymousActivity = false;
			CreateDate = DateTime.Now;
			Culture = string.Empty;
			DefaultLanguageSelection = ContentLanguagePreference.Default;
			Email = string.Empty;
			EmailOptions = UserEmailOptions.PrivateMessagesFromAll;
			GroupId = UserGroupId.Regular;
			Language = string.Empty;
			LastLogin = DateTime.Now;
			Options = new UserOptions(this);
			PreferredVideoService = PVService.Youtube;

		}

		public User(string name, string pass, string email, int salt)
			: this() {

			Name = name;
			NameLC = name.ToLowerInvariant();
			Password = pass;
			Email = email;
			Salt = salt;

			GenerateAccessKey();

		}

		public virtual string AccessKey {
			get { return accessKey; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				accessKey = value; 
			}
		}

		public virtual bool Active { get; set; }

		/// <summary>
		/// Additional user permissions. Base permissions are assigned through the user group.
		/// </summary>
		public virtual PermissionCollection AdditionalPermissions {
			get {
				return additionalPermissions;
			}
			set {
				additionalPermissions = value ?? new PermissionCollection();
			}
		}

		public virtual IEnumerable<AlbumForUser> Albums {
			get {
				return AllAlbums.Where(a => !a.Album.Deleted);
			}
		}

		public virtual IList<AlbumForUser> AllAlbums {
			get { return albums; }
			set {
				ParamIs.NotNull(() => value);
				albums = value;
			}
		}

		/// <summary>
		/// List of artists followed by this user. This list does not include deleted entries. Cannot be null.
		/// </summary>
		public virtual IEnumerable<ArtistForUser> Artists {
			get {
				return AllArtists.Where(a => !a.Artist.Deleted);
			}
		}

		/// <summary>
		/// List of artists followed by this user. Includes deleted artists. Cannot be null.
		/// </summary>
		public virtual IList<ArtistForUser> AllArtists {
			get { return artists; }
			set {
				ParamIs.NotNull(() => value);
				artists = value;
			}
		}

		/// <summary>
		/// List of artists entries for which this user is a verified owner. Includes deleted artists. Cannot be null.
		/// </summary>
		public virtual IList<OwnedArtistForUser> AllOwnedArtists {
			get { return ownedArtists; }
			set {
				ParamIs.NotNull(() => value);
				ownedArtists = value;
			}
		}

		public virtual bool AnonymousActivity { get; set; }

		public virtual bool CanBeDisabled {
			get {

				return !EffectivePermissions.Has(PermissionToken.DisableUsers);

			}
		}

		public virtual IList<UserComment> Comments {
			get { return comments; }
			set {
				ParamIs.NotNull(() => value);
				comments = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual string Culture {
			get { return culture; }
			set { 
				ParamIs.NotNull(() => value);
				culture = value; 
			}
		}

		public virtual ContentLanguagePreference DefaultLanguageSelection { get; set; }

		public virtual string DefaultName {
			get { return Name; }
		}

		public virtual bool Deleted {
			get { return !Active; }
		}

		public virtual PermissionCollection EffectivePermissions {
			get {

				if (!Active)
					return new PermissionCollection();

				return UserGroup.GetPermissions(GroupId).Merge(AdditionalPermissions);

			}
		}

		public virtual string Email {
			get { return email; }
			set {
				ParamIs.NotNull(() => value);
				email = value;
			}
		}

		public virtual UserEmailOptions EmailOptions { get; set; }

		public virtual EntryType EntryType {
			get { return EntryType.User; }
		}

		public virtual IList<FavoriteSongForUser> FavoriteSongs {
			get { return favoriteSongs; }
			set {
				ParamIs.NotNull(() => value);
				favoriteSongs = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual UserGroupId GroupId { get; set; }

		/// <summary>
		/// User's group. Cannot be null.
		/// </summary>
		public virtual UserGroup Group {
			get {
				return UserGroup.GetGroup(GroupId);
			}
		}

		public virtual string Language {
			get { return language; }
			set {
				ParamIs.NotNull(() => value);
				language = value;
			}
		}

		public virtual DateTime LastLogin { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				name = value;
			}
		}

		public virtual string NameLC {
			get { return nameLc; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				nameLc = value;
			}
		}

		public virtual UserOptions Options {
			get { return options; }
			set {
				options = value ?? new UserOptions(this);
			}
		}

		/// <summary>
		/// List of artists entries for which this user is a verified owner. Does not include deleted artists. Cannot be null.
		/// </summary>
		public virtual IEnumerable<OwnedArtistForUser> OwnedArtists {
			get {
				return AllOwnedArtists.Where(a => !a.Artist.Deleted);
			}
		}

		public virtual string Password {
			get { return password; }
			set {
				ParamIs.NotNull(() => value);
				password = value;
			}
		}

		public virtual PVService PreferredVideoService { get; set; }

		public virtual IList<UserMessage> ReceivedMessages {
			get { return receivedMessages; }
			set {
				ParamIs.NotNull(() => value);
				receivedMessages = value;
			}
		}

		public virtual RoleTypes Roles { get; set; }

		public virtual int Salt { get; set; }

		public virtual IList<UserMessage> SentMessages {
			get { return sentMessages; }
			set {
				ParamIs.NotNull(() => value);
				sentMessages = value;
			}
		}

		public virtual IList<SongList> SongLists {
			get { return songLists; }
			set {
				ParamIs.NotNull(() => value);
				songLists = value; 
			}
		}

		public virtual IList<UserWebLink> WebLinks {
			get { return webLinks; }
			set { 
				ParamIs.NotNull(() => value);
				webLinks = value; 
			}
		}

		public virtual AlbumForUser AddAlbum(Album album, PurchaseStatus status, MediaType mediaType, int rating) {

			ParamIs.NotNull(() => album);

			var link = new AlbumForUser(this, album, status, mediaType, rating);
			AllAlbums.Add(link);
			album.UserCollections.Add(link);
			album.UpdateRatingTotals();

			return link;

		}

		public virtual ArtistForUser AddArtist(Artist artist) {

			ParamIs.NotNull(() => artist);

			var link = new ArtistForUser(this, artist);
			AllArtists.Add(link);

			return link;

		}

		public virtual OwnedArtistForUser AddOwnedArtist(Artist artist) {

			ParamIs.NotNull(() => artist);

			var old = ownedArtists.FirstOrDefault(a => a.Artist.Equals(artist));

			if (old != null)
				throw new ArgumentException("Unable to add the same artist again", "artist");

			var link = new OwnedArtistForUser(this, artist);
			AllOwnedArtists.Add(link);
			artist.OwnerUsers.Add(link);

			return link;

		}

		public virtual FavoriteSongForUser AddSongToFavorites(Song song, SongVoteRating rating) {
			
			ParamIs.NotNull(() => song);

			var link = new FavoriteSongForUser(this, song, rating);
			FavoriteSongs.Add(link);
			song.UserFavorites.Add(link);

			if (rating == SongVoteRating.Favorite)
				song.FavoritedTimes++;

			song.RatingScore += FavoriteSongForUser.GetRatingScore(rating);

			return link;

		}

		public virtual UserComment CreateComment(string message, AgentLoginData loginData) {

			ParamIs.NotNullOrEmpty(() => message);
			ParamIs.NotNull(() => loginData);

			var comment = new UserComment(this, message, loginData);
			Comments.Add(comment);

			return comment;

		}

		public virtual UserWebLink CreateWebLink(WebLinkContract contract) {

			ParamIs.NotNull(() => contract);

			var link = new UserWebLink(this, contract);
			WebLinks.Add(link);

			return link;

		}

		public virtual UserWebLink CreateWebLink(string description, string url, WebLinkCategory category) {
			return CreateWebLink(new WebLinkContract(url, description, category));
		}

		public virtual bool Equals(User another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.NameLC == another.NameLC;

		}

		public override bool Equals(object obj) {
			return Equals(obj as User);
		}

		public virtual void GenerateAccessKey() {

			AccessKey = new AlphaPassGenerator(true, true, true).Generate(20);

		}

		public override int GetHashCode() {
			return NameLC.GetHashCode();
		}

		public virtual bool IsTheSameUser(UserContract contract) {

			if (contract == null)
				return false;

			return (Id == contract.Id);

		}

		public virtual UserMessage SendMessage(User to, string subject, string body, bool highPriority) {

			ParamIs.NotNull(() => to);

			var msg = new UserMessage(this, to, subject, body, highPriority);
			SentMessages.Add(msg);
			to.ReceivedMessages.Add(msg);

			return msg;

		}

		public virtual void SetEmail(string newEmail) {
			
			ParamIs.NotNull(() => newEmail);

			if (newEmail != string.Empty)
				new MailAddress(newEmail);

			Email = newEmail;

		}

		public override string ToString() {
			return string.Format("user '{0}' [{1}]", Name, Id);
		}

		public virtual void UpdateLastLogin(string host) {
			LastLogin = DateTime.Now;
			Options.LastLoginAddress = host;
		}

	}

}
