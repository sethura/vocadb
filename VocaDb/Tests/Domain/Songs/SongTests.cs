﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.Domain.Songs {

	/// <summary>
	/// Tests for <see cref="Song"/>.
	/// </summary>
	[TestClass]
	public class SongTests {

		private LyricsForSong lyrics;
		private Song song;

		private void CreatePV(PVService service) {
			song.CreatePV(new PVContract { Service = service, PVId = "test", Name = "test" });
		}

		[TestInitialize]
		public void Setup() {

			song = new Song();
			lyrics = song.CreateLyrics(ContentLanguageSelection.Japanese, "Miku!", "miku");

		}

		[TestMethod]
		public void Ctor_LocalizedString() {

			song = new Song(new LocalizedString("song", ContentLanguageSelection.Romaji));

			Assert.AreEqual(1, song.Names.Count(), "Names count");
			Assert.IsTrue(song.Names.HasNameForLanguage(ContentLanguageSelection.Romaji), "Has name for Romaji");
			Assert.IsFalse(song.Names.HasNameForLanguage(ContentLanguageSelection.English), "Does not have name for English");
			Assert.AreEqual("song", song.Names.GetEntryName(ContentLanguagePreference.Romaji).DisplayName, "Display name");

		}

		[TestMethod]
		public void LyricsFromParents_NoLyrics() {

			var result = new Song().LyricsFromParents;

			Assert.AreEqual(0, result.Count, "no lyrics");

		}

		[TestMethod]
		public void LyricsFromParents_NoParent() {

			var result = song.LyricsFromParents;

			Assert.AreEqual(1, result.Count, "one entry");
			Assert.AreSame(lyrics, result.First(), "returned lyrics from entry");

		}

		[TestMethod]
		public void LyricsFromParents_FromParent() {

			var derived = new Song();
			derived.OriginalVersion = song;
			var result = derived.LyricsFromParents;

			Assert.AreEqual(1, result.Count, "one entry");
			Assert.AreSame(lyrics, result.First(), "returned lyrics from entry");

		}

		[TestMethod]
		public void UpdatePVServices_None() {

			song.UpdatePVServices();

			Assert.AreEqual(PVServices.Nothing, song.PVServices);

		}

		[TestMethod]
		public void UpdatePVServices_One() {

			CreatePV(PVService.Youtube);

			song.UpdatePVServices();

			Assert.AreEqual(PVServices.Youtube, song.PVServices);

		}

		[TestMethod]
		public void UpdatePVServices_Multiple() {

			CreatePV(PVService.NicoNicoDouga);
			CreatePV(PVService.SoundCloud);
			CreatePV(PVService.Youtube);

			song.UpdatePVServices();

			Assert.AreEqual(PVServices.NicoNicoDouga | PVServices.SoundCloud | PVServices.Youtube, song.PVServices);

		}

	}

}
