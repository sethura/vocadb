﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Tags {

	public class Tag : IEquatable<Tag>, IEntryWithNames, IEntryWithStatus {

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		bool IDeletableEntry.Deleted {
			get { return false; }
		}

		INameManager IEntryWithNames.Names {
			get {
				return new SingleNameManager(TagName);
			}
		}

		int IEntryBase.Version {
			get { return 0; }
		}

		public const int MaxDisplayedTags = 4;
		public static readonly Regex TagNameRegex = new Regex(@"^[a-zA-Z0-9_-]+$");

		private Iesi.Collections.Generic.ISet<AlbumTagUsage> albumTagUsages = new Iesi.Collections.Generic.HashedSet<AlbumTagUsage>();
		private Iesi.Collections.Generic.ISet<Tag> aliases = new Iesi.Collections.Generic.HashedSet<Tag>();
		private ArchivedVersionManager<ArchivedTagVersion, TagEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedTagVersion, TagEditableFields>();		
		private Iesi.Collections.Generic.ISet<ArtistTagUsage> artistTagUsages = new Iesi.Collections.Generic.HashedSet<ArtistTagUsage>();
		private string categoryName;
		private string description;
		private Iesi.Collections.Generic.ISet<SongTagUsage> songTagUsages = new Iesi.Collections.Generic.HashedSet<SongTagUsage>();

		public Tag() {
			CategoryName = string.Empty;
			Description = string.Empty;
			Status = EntryStatus.Finished;
		}

		public Tag(string name)
			: this() {

			if (!TagNameRegex.IsMatch(name))
				throw new ArgumentException("Tag name must contain only word characters", "name");

			Name = name;

		}

		/// <summary>
		/// Actual tag to be used for this tag name.
		/// If this tag has been aliased to some other tag, that tag name will be used.
		/// </summary>
		public virtual Tag ActualTag {
			get { return AliasedTo ?? this; }
		}

		public virtual Tag AliasedTo { get; set; }

		public virtual Iesi.Collections.Generic.ISet<Tag> Aliases {
			get { return aliases; }
			set {
				ParamIs.NotNull(() => value);
				aliases = value;
			}
		}

		public virtual Iesi.Collections.Generic.ISet<AlbumTagUsage> AllAlbumTagUsages {
			get { return albumTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				albumTagUsages = value;
			}
		}

		public virtual IEnumerable<AlbumTagUsage> AlbumTagUsages {
			get {
				return AllAlbumTagUsages.Where(a => !a.Album.Deleted);
			}
		}

		public virtual Iesi.Collections.Generic.ISet<ArtistTagUsage> AllArtistTagUsages {
			get { return artistTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				artistTagUsages = value;
			}
		}

		public virtual ArchivedVersionManager<ArchivedTagVersion, TagEditableFields> ArchivedVersionsManager {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual IEnumerable<ArtistTagUsage> ArtistTagUsages {
			get {
				return AllArtistTagUsages.Where(a => !a.Artist.Deleted);
			}
		}

		public virtual string CategoryName {
			get { return categoryName; }
			set {
				ParamIs.NotNull(() => value);
				categoryName = value; 
			}
		}

		/// <summary>
		/// Tag description, may contain Markdown formatting.
		/// </summary>
		public virtual string Description {
			get { return description; }
			set { description = value; }
		}

		public virtual EntryType EntryType {
			get { return EntryType.Tag; }
		}

		/// <summary>
		/// Database identity column. Generated by an identity.
		/// Provides an unique integer Id, to make tags compatible with other entries.
		/// Note that this is not the primary key of the table, <see cref="Name"/> is the primary key.
		/// </summary>
		public virtual int Id { get; set; }

		/// <summary>
		/// Tag name. Primary key. Guaranteed to be unique (case insensitive).
		/// </summary>
		/// <remarks>
		/// Unlike other entry types, tags use the string name field as the primary key.
		/// 
		/// Accessing this field does not guarantee the tag even exists in the database because NHibernate won't
		/// try to load the object if only the Id is accessed.
		/// </remarks>
		public virtual string Name { get; set; }

		/// <summary>
		/// Tag name mapped as a regular column. Value of this property is the same as <see cref="Name"/>.
		/// Forces the tag to be loaded from the database, unlike the Name field.
		/// </summary>
		public virtual string TagName { get; set; }

		/// <summary>
		/// Tag name with underscores replaced with spaces.
		/// Cannot be null or empty.
		/// </summary>
		public virtual string NameWithSpaces {
			get {
				return Name.Replace('_', ' ');
			}
		}

		/// <summary>
		/// Entry thumbnail picture. Can be null.
		/// </summary>
		public virtual EntryThumb Thumb { get; set; }

		public virtual ArchivedTagVersion CreateArchivedVersion(TagDiff diff, AgentLoginData author, EntryEditEvent reason) {

			var archived = new ArchivedTagVersion(this, diff, author, reason);
			ArchivedVersionsManager.Add(archived);

			return archived;

		}

		public virtual void Delete() {

			while (AllAlbumTagUsages.Any())
				AllAlbumTagUsages.First().Delete();

			while (AllArtistTagUsages.Any())
				AllArtistTagUsages.First().Delete();

			while (AllSongTagUsages.Any())
				AllSongTagUsages.First().Delete();

		}

		public virtual bool Equals(Tag tag) {

			if (tag == null)
				return false;

			return tag.Name.Equals(Name, StringComparison.InvariantCultureIgnoreCase);

		}

		public override bool Equals(object obj) {
			return Equals(obj as Tag);
		}

		public override int GetHashCode() {
			return Name.ToLowerInvariant().GetHashCode();
		}

		public virtual Iesi.Collections.Generic.ISet<SongTagUsage> AllSongTagUsages {
			get { return songTagUsages; }
			set {
				ParamIs.NotNull(() => value);
				songTagUsages = value;
			}
		}

		public virtual IEnumerable<SongTagUsage> SongTagUsages {
			get {
				return AllSongTagUsages.Where(a => !a.Song.Deleted);
			}
		}

		public virtual EntryStatus Status { get; set; }

		public override string ToString() {
			return string.Format("tag '{0}'", Name);
		}

	}

}
