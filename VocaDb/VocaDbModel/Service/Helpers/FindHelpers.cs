﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service.Helpers {

	public static class FindHelpers {

		/// <summary>
		/// Adds a filter for a list of names.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <param name="nameFilter"></param>
		/// <param name="matchMode"></param>
		/// <returns></returns>
		public static IQueryable<T> AddEntryNameFilter<T>(IQueryable<T> query, string nameFilter, NameMatchMode matchMode)
			where T : LocalizedString {

			if (matchMode == NameMatchMode.Exact || (matchMode == NameMatchMode.Auto && nameFilter.Length < 3)) {

				return query.Where(m => m.Value == nameFilter);

			} else {

				return query.Where(m => m.Value.Contains(nameFilter));

			}

		}

		public static IQueryable<T> AddOrder<T>(IQueryable<T> criteria, ContentLanguagePreference languagePreference) where T : IEntryWithNames {

			if (languagePreference == ContentLanguagePreference.Japanese)
				return criteria.OrderBy(e => e.Names.SortNames.Japanese);
			else if (languagePreference == ContentLanguagePreference.English)
				return criteria.OrderBy(e => e.Names.SortNames.English);
			else
				return criteria.OrderBy(e => e.Names.SortNames.Japanese);

		}

		/// <summary>
		/// Adds a filter for entry's SortName.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="criteria"></param>
		/// <param name="name"></param>
		/// <param name="matchMode"></param>
		/// <returns></returns>
		public static IQueryable<T> AddSortNameFilter<T>(IQueryable<T> criteria, string name, NameMatchMode matchMode)
			where T : IEntryWithNames {

			if (matchMode == NameMatchMode.Exact || (matchMode == NameMatchMode.Auto && name.Length < 3)) {

				return criteria.Where(s =>
					s.Names.SortNames.English == name
						|| s.Names.SortNames.Romaji == name
						|| s.Names.SortNames.Japanese == name);

			} else {

				return criteria.Where(s =>
					s.Names.SortNames.English.Contains(name)
						|| s.Names.SortNames.Romaji.Contains(name)
						|| s.Names.SortNames.Japanese.Contains(name));

			}

		}

	}

}
