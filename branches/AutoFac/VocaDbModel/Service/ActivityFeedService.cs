﻿using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service {

	public class ActivityFeedService : ServiceBase {

		public ActivityFeedService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public PartialFindResult<ActivityEntryContract> GetActivityEntries(PagingProperties paging) {

			ParamIs.NotNull(() => paging);

			return HandleQuery(session => {

				var entries = session.Query<ActivityEntry>()
					.OrderByDescending(a => a.CreateDate)
					.Skip(paging.Start)
					.Take(paging.MaxEntries)
					.ToArray();

				var contracts = entries
					.Where(e => !e.EntryBase.Deleted)
					.Select(e => new ActivityEntryContract(e, PermissionContext.LanguagePreference))
					.ToArray();

				var count = (paging.GetTotalCount ? session.Query<ActivityEntry>().Count() : 0);

				return new PartialFindResult<ActivityEntryContract>(contracts, count);

			});

		}

		public PartialFindResult<ActivityEntryContract> GetFollowedArtistActivity(int maxEntries) {

			if (!PermissionContext.IsLoggedIn)
				return new PartialFindResult<ActivityEntryContract>();

			return HandleQuery(session => {

				var followedArtists = GetLoggedUser(session).Artists.Select(a => a.Artist).ToArray();

				var albumEntries = session.Query<AlbumActivityEntry>()
					.OrderByDescending(a => a.CreateDate)
					.Where(a => !a.Entry.Deleted && a.EditEvent == EntryEditEvent.Created && a.Entry.AllArtists.Any(r => followedArtists.Contains(r.Artist)))
					.Take(maxEntries)
					.ToArray();

				var songEntries = session.Query<SongActivityEntry>()
					.OrderByDescending(a => a.CreateDate)
					.Where(a => !a.Entry.Deleted && a.EditEvent == EntryEditEvent.Created && a.Entry.AllArtists.Any(r => followedArtists.Contains(r.Artist)))
					.Take(maxEntries)
					.ToArray();

				var contracts = albumEntries.Cast<ActivityEntry>()
					.Concat(songEntries)
					.OrderByDescending(a => a.CreateDate)
					.Take(maxEntries)
					.Select(e => new ActivityEntryContract(e, PermissionContext.LanguagePreference))
					.ToArray();

				return new PartialFindResult<ActivityEntryContract>(contracts, 0);

			});

		}


	}
}
