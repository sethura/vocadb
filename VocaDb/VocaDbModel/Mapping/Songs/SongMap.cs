﻿using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Mapping.Songs {

	public class SongMap : ClassMap<Song> {

		public SongMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.ArtistString).Not.Nullable().Length(500);
			Map(m => m.CreateDate).Not.Nullable();
			Map(m => m.Deleted).Not.Nullable();
			Map(m => m.NicoId).Nullable();
			Map(m => m.PVServices).CustomType(typeof(PVServices)).Not.Nullable();
			Map(m => m.SongType).Not.Nullable();
			Map(m => m.Status).CustomType(typeof(EntryStatus)).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			Component(m => m.Names, c => {
				c.HasMany(m => m.Names).Table("SongNames").KeyColumn("[Song]").Inverse().Cascade.All();
				c.Component(m => m.SortNames, c2 => {
					c2.Map(m => m.DefaultLanguage, "DefaultNameLanguage");
					c2.Map(m => m.Japanese, "JapaneseName");
					c2.Map(m => m.English, "EnglishName");
					c2.Map(m => m.Romaji, "RomajiName");
					//c.Map(m => m.Other, "OtherName");
				});
			});

			Component(m => m.ArtistString, c => {
				c.Map(m => m.Japanese, "ArtistString").Length(500).Not.Nullable();
				c.Map(m => m.Romaji, "ArtistStringRomaji").Length(500).Not.Nullable();
				c.Map(m => m.English, "ArtistStringEnglish").Length(500).Not.Nullable();
			});

			HasMany(m => m.AllAlbums).Table("SongsInAlbums").Inverse().Cascade.All();
			HasMany(m => m.AllArtists).Table("ArtistsForSongs").Inverse().Cascade.All();
			HasMany(m => m.ArchivedVersions).Inverse().Cascade.All();
			HasMany(m => m.Lyrics).Inverse().Cascade.All();
			HasMany(m => m.PVs).Inverse().Cascade.All();
			HasMany(m => m.UserFavorites).Inverse();
			HasMany(m => m.WebLinks).Table("SongWebLinks").Inverse().Cascade.All();

		}

	}

	public class ArchivedSongVersionMap : ClassMap<ArchivedSongVersion> {

		public ArchivedSongVersionMap() {

			Id(m => m.Id);

			Map(m => m.AgentName).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Author);
			References(m => m.Song);

		}

	}

	public class SongNameMap : ClassMap<SongName> {

		public SongNameMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Language).Not.Nullable();
			Map(m => m.Value).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

	public class ArtistForSongMap : ClassMap<ArtistForSong> {

		public ArtistForSongMap() {

			Schema("dbo");
			Table("ArtistsForSongs");
			Cache.ReadWrite();
			Id(m => m.Id);
			References(m => m.Artist).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

	public class SongInAlbumMap : ClassMap<SongInAlbum> {

		public SongInAlbumMap() {

			Schema("dbo");
			Table("SongsInAlbums");
			Cache.ReadWrite();
			Id(m => m.Id);
			Map(m => m.TrackNumber).Not.Nullable();
			References(m => m.Album).Not.Nullable();
			References(m => m.Song).Not.Nullable();

		}

	}

	public class SongWebLinkMap : ClassMap<SongWebLink> {

		public SongWebLinkMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Description).Not.Nullable();
			Map(m => m.Url).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

	public class PVForSongMap : ClassMap<PVForSong> {
		
		public PVForSongMap() {
			
			Table("PVsForSongs");
			Id(m => m.Id);

			Map(m => m.Name).Not.Nullable();
			Map(m => m.PVId).Not.Nullable();
			Map(m => m.PVType).Not.Nullable();
			Map(m => m.Service).Not.Nullable();

			References(m => m.Song).Not.Nullable();

		}

	}

}
