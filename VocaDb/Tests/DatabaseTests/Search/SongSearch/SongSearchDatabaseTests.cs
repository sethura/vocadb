﻿using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.SongSearch;

namespace VocaDb.Tests.DatabaseTests.Search.SongSearch {

	[TestClass]
	public class SongSearchDatabaseTests {

		private DatabaseTestContext<IQuerySource> context;
		private SongQueryParams queryParams;

		private TestDatabase Db {
			get { return TestContainerManager.TestDatabase; }
		}

		[TestInitialize]
		public void SetUp() {
			
			queryParams = new SongQueryParams { SortRule = SongSortRule.Name };
			context = new DatabaseTestContext<IQuerySource>();

		}

		private PartialFindResult<Song> CallFind(ContentLanguagePreference languagePreference = ContentLanguagePreference.Default) {
			
			return context.RunTest(querySource => {
				
				var search = new Model.Service.Search.SongSearch.SongSearch(querySource, languagePreference);

				var watch = new Stopwatch();
				watch.Start();

				var result = search.Find(queryParams);

				Console.WriteLine("Test finished in {0}ms", watch.ElapsedMilliseconds);

				return result;

			});

		}

		/// <summary>
		/// List all (no filters).
		/// </summary>
		[TestMethod]
		public void ListAll() {

			var result = CallFind();

			Assert.AreEqual(3, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual(Db.SongWithArtist, result.Items[0]);
			Assert.AreEqual(Db.Song.DefaultName, result.Items[1].DefaultName);

		}

		/// <summary>
		/// Listing, skip first result.
		/// </summary>
		[TestMethod]
		public void ListSkip() {

			queryParams.Paging.Start = 1;

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual(Db.Song, result.Items[0]);

		}

		/// <summary>
		/// List with sort by name.
		/// </summary>
		[TestMethod]
		public void ListSortName() {

			queryParams.SortRule = SongSortRule.Name;

			var result = CallFind();

			Assert.AreEqual(3, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);
			Assert.AreEqual("Nebula", result.Items[1].DefaultName);

		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		public void ListSortFavorites() {

			queryParams.SortRule = SongSortRule.FavoritedTimes;

			var result = CallFind();

			Assert.AreEqual(3, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);
			Assert.AreEqual("Nebula", result.Items[1].DefaultName);

		}

		/// <summary>
		/// List with sort by favorites.
		/// </summary>
		[TestMethod]
		public void ListSortAdditionDate() {

			queryParams.SortRule = SongSortRule.AdditionDate;

			var result = CallFind();

			Assert.AreEqual(3, result.Items.Length, "Number of results");
			Assert.AreEqual(3, result.TotalCount, "Total result count");
			Assert.AreEqual("Nebula", result.Items[0].DefaultName);
			Assert.AreEqual("Tears of Palm", result.Items[1].DefaultName);
			Assert.AreEqual("Crystal Tears", result.Items[2].DefaultName);

		}

		/// <summary>
		/// Query by name.
		/// </summary>
		[TestMethod]
		public void QueryName() {

			queryParams.Common.Query = "Crystal Tears";

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);

		}

		/// <summary>
		/// Query by name as words.
		/// </summary>
		[TestMethod]
		public void QueryNameWords() {

			queryParams.Common.NameMatchMode = NameMatchMode.Words;
			queryParams.Common.Query = "Tears Crystal";

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "total result count");
			Assert.AreEqual("Crystal Tears", result.Items[0].DefaultName);

		}

		/// <summary>
		/// Query by name, move exact result to top.
		/// 
		/// 2 songs match, Tears of Palm and Crystal Tears.
		/// Tears of Palm is later in the results when sorted by title, 
		/// but matches from the beginning so it should be moved to first.
		/// </summary>
		[TestMethod]
		public void QueryNameMoveExactToTop() {

			queryParams.Common.Query = "Tears";
			queryParams.Common.MoveExactToTop = true;
			queryParams.Paging.MaxEntries = 1;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(2, result.TotalCount, "2 total count");
			Assert.AreEqual(Db.Song2, result.Items[0], "result is as expected");

		}

		/// <summary>
		/// Query by tag
		/// </summary>
		[TestMethod]
		public void QueryTag() {

			queryParams.Common.Query = "tag:Electronic";

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "1 result");
			Assert.AreEqual(1, result.TotalCount, "1 total count");
			Assert.AreEqual(Db.Song, result.Items[0], "result is as expected");

		}

		/// <summary>
		/// Query by type.
		/// </summary>
		[TestMethod]
		public void QueryType() {

			queryParams.SongTypes = new[] { SongType.Original };

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "Number of results");
			Assert.AreEqual(2, result.TotalCount, "Total result count");
			Assert.AreEqual(Db.Song, result.Items[0]);

		}

		/// <summary>
		/// Query by artist.
		/// </summary>
		[TestMethod]
		public void QueryArtist() {

			queryParams.ArtistId = Db.Producer.Id;

			var result = CallFind();

			Assert.AreEqual(1, result.Items.Length, "Number of results");
			Assert.AreEqual(1, result.TotalCount, "Total result count");
			Assert.AreEqual(Db.SongWithArtist, result.Items[0], "songs are equal");

		}

		/// <summary>
		/// Query songs with only PVs.
		/// </summary>
		[TestMethod]
		public void QueryOnlyWithPVs() {

			queryParams.OnlyWithPVs = true;

			var result = CallFind();

			Assert.AreEqual(2, result.Items.Length, "Number of results");
			Assert.AreEqual(2, result.TotalCount, "Total result count");
			Assert.AreEqual(Db.Song, result.Items[0], "songs are equal");

		}

	}

}
