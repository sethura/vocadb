﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="SongQueries"/>.
	/// </summary>
	[TestClass]
	public class SongQueriesTests {

		private CreateSongContract newSongContract;
		private FakePermissionContext permissionContext;
		private Artist producer;
		private FakePVParser pvParser;
		private FakeSongRepository repository;
		private SongQueries queries;
		private Song song;
		private User user;
		private User user2;
		private Artist vocalist;

		private SongContract CallCreate() {
			return queries.Create(newSongContract);
		}

		[TestInitialize]
		public void SetUp() {

			producer = CreateEntry.Producer(id: 1, name: "Tripshots");
			vocalist = CreateEntry.Vocalist(id: 39, name: "Hatsune Miku");

			song = CreateEntry.Song(id: 1, name: "Nebula");
			song.LengthSeconds = 39;
			repository = new FakeSongRepository(song);
			repository.Save(song.AddArtist(producer));
			repository.Save(song.AddArtist(vocalist));

			user = CreateEntry.User(id: 1, name: "Miku");
			user2 = CreateEntry.User(id: 2, name: "Rin");
			repository.Add(user, user2);
			repository.Add(producer, vocalist);

			permissionContext = new FakePermissionContext(user);
			var entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");

			newSongContract = new CreateSongContract {
				SongType = SongType.Original,
				Names = new[] {
					new LocalizedStringContract("Resistance", ContentLanguageSelection.English)
				},
				Artists = new[] {
					new ArtistContract(producer, ContentLanguagePreference.Default),
					new ArtistContract(vocalist, ContentLanguagePreference.Default), 
				},
				PVUrl = "http://test.vocadb.net/"
			};

			pvParser = new FakePVParser();
			pvParser.ResultFunc = url => 
				VideoUrlParseResult.CreateOk(url, PVService.NicoNicoDouga, "sm393939", 
				VideoTitleParseResult.CreateSuccess("Resistance", "Tripshots", "testimg.jpg", 39));

			queries = new SongQueries(repository, permissionContext, entryLinkFactory, pvParser);

		}

		[TestMethod]
		public void Create() {

			var result = CallCreate();

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Resistance", result.Name, "Name");

			song = repository.HandleQuery(q => q.Query().FirstOrDefault(a => a.DefaultName == "Resistance"));

			Assert.IsNotNull(song, "Song was saved to repository");
			Assert.AreEqual("Resistance", song.DefaultName, "Name");
			Assert.AreEqual(ContentLanguageSelection.English, song.Names.SortNames.DefaultLanguage, "Default language should be English");
			Assert.AreEqual(2, song.AllArtists.Count, "Artists count");
			VocaDbAssert.ContainsArtists(song.AllArtists, "Tripshots", "Hatsune Miku");
			Assert.AreEqual("Tripshots feat. Hatsune Miku", song.ArtistString.Default, "ArtistString");
			Assert.AreEqual(39, song.LengthSeconds, "Length");	// From PV
			Assert.AreEqual(PVServices.NicoNicoDouga, song.PVServices, "PVServices");

			var archivedVersion = repository.List<ArchivedSongVersion>().FirstOrDefault();

			Assert.IsNotNull(archivedVersion, "Archived version was created");
			Assert.AreEqual(song, archivedVersion.Song, "Archived version song");
			Assert.AreEqual(SongArchiveReason.Created, archivedVersion.Reason, "Archived version reason");

			var activityEntry = repository.List<ActivityEntry>().FirstOrDefault();

			Assert.IsNotNull(activityEntry, "Activity entry was created");
			Assert.AreEqual(song, activityEntry.EntryBase, "Activity entry's entry");
			Assert.AreEqual(EntryEditEvent.Created, activityEntry.EditEvent, "Activity entry event type");

			var pv = repository.List<PVForSong>().FirstOrDefault();

			Assert.IsNotNull(pv, "PV was created");
			Assert.AreEqual(song, pv.Song, "pv.Song");
			Assert.AreEqual("Resistance", pv.Name, "pv.Name");

		}

		[TestMethod]
		public void Create_Notification() {

			user2.AddArtist(producer);

			CallCreate();

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user2, notification.Receiver, "Receiver");

		}

		[TestMethod]
		public void Create_NoNotificationForSelf() {

			user.AddArtist(producer);

			CallCreate();

			Assert.IsFalse(repository.List<UserMessage>().Any(), "No notification was created");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Create_NoPermission() {

			user.GroupId = UserGroupId.Limited;
			permissionContext.RefreshLoggedUser(repository);

			CallCreate();

		}

		[TestMethod]
		public void Merge_ToEmpty() {

			user.GroupId = UserGroupId.Trusted;
			permissionContext.RefreshLoggedUser(repository);

			var song2 = new Song();
			repository.Save(song2);

			queries.Merge(song.Id, song2.Id);

			Assert.AreEqual("Nebula", song2.Names.AllValues.FirstOrDefault(), "Name");
			Assert.AreEqual(2, song2.AllArtists.Count, "Artists");
			Assert.IsTrue(song2.Artists.Any(a => a.Artist.Equals(producer)), "Producer");
			Assert.IsTrue(song2.Artists.Any(a => a.Artist.Equals(vocalist)), "Vocalist");
			Assert.AreEqual(song.LengthSeconds, song2.LengthSeconds, "LengthSeconds");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Merge_NoPermissions() {

			var song2 = new Song();
			repository.Save(song2);

			queries.Merge(song.Id, song2.Id);

		}

	}
}
