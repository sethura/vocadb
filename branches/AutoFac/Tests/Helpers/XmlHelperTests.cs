﻿using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	/// <summary>
	/// Tests for <see cref="XmlHelper"/>.
	/// </summary>
	[TestClass]
	public class XmlHelperTests {

		private static T SerializeToObjectAndBack<T>(T obj) {

			var xml = XmlHelper.SerializeToXml(obj);

			Assert.IsNotNull(xml, "result is not null");

			return XmlHelper.DeserializeFromXml<T>(xml);

		}

		[TestMethod]
		public void SerializeToXml_ValidObject() {

			var album = new ArchivedAlbumContract { Description = "Miku Miku!" };

			var res = SerializeToObjectAndBack(album);

			Assert.AreEqual(album.Description, res.Description, "string is intact");

		}

		[TestMethod]
		public void SerializeToXml_Unicode() {

			var album = new ArchivedAlbumContract { Description = "初音ミク" };

			var res = SerializeToObjectAndBack(album);

			Assert.AreEqual(album.Description, res.Description, "string is intact");

		}

		[TestMethod]
		public void SerializeToXml_ForbiddenChars() {

			var name = "Miku Miku!";
			var album = new ArchivedAlbumContract { Description = name + '\x02'};

			Assert.IsTrue(album.Description.IsNormalized());

			var res = SerializeToObjectAndBack(album);

			Assert.AreEqual(name, res.Description, "string is intact");

		}

	}

}
