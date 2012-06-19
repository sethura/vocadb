﻿using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Helpers {

	public static class ArtistHelper {

		private static TranslatedString GetTranslatedName(IArtistWithSupport link) {

			return (link.Artist != null ? link.Artist.TranslatedName : TranslatedString.Create(link.Name));

		}

		private static bool IsProducerRole(IArtistWithSupport link, bool isAnimation) {

			return IsProducerRole(GetCategories(link), isAnimation);

		}

		private static bool IsProducerRole(ArtistCategories categories, bool isAnimation) {

			return (categories.HasFlag(ArtistCategories.Producer) 
				|| categories.HasFlag(ArtistCategories.Circle) 
				|| (isAnimation && categories.HasFlag(ArtistCategories.Animator)));

		}

		private static bool IsValidCreditableArtist(IArtistWithSupport artist) {

			if (artist.IsSupport)
				return false;

			var cat = GetCategories(artist);

			return (cat != ArtistCategories.Nothing && cat != ArtistCategories.Label);

		}

		/// <summary>
		/// Artist types able to work as groups/circles for an artist.
		/// </summary>
		public static readonly ArtistType[] ArtistGroupTypes = new[] {
			ArtistType.Circle, ArtistType.Label, ArtistType.OtherGroup
		};

		public static readonly Dictionary<ArtistType, ArtistCategories> CategoriesForTypes = new Dictionary<ArtistType, ArtistCategories> {
			{ ArtistType.Animator, ArtistCategories.Animator },
			{ ArtistType.Circle, ArtistCategories.Circle },
			{ ArtistType.Label, ArtistCategories.Label },
			{ ArtistType.OtherGroup, ArtistCategories.Circle },
			{ ArtistType.OtherIndividual, ArtistCategories.Other },
			{ ArtistType.OtherVocalist, ArtistCategories.Vocalist },
			{ ArtistType.Producer, ArtistCategories.Producer },
			{ ArtistType.Unknown, ArtistCategories.Other },
			{ ArtistType.UTAU, ArtistCategories.Vocalist },
			{ ArtistType.Vocaloid, ArtistCategories.Vocalist },
		};

		/// <summary>
		/// The roles of these artists can be customized
		/// </summary>
		public static readonly ArtistType[] CustomizableTypes = new[] {
			ArtistType.Animator, ArtistType.OtherGroup, ArtistType.OtherIndividual, 
			ArtistType.OtherVocalist, ArtistType.Producer, ArtistType.Unknown
		};

		public static readonly ArtistType[] ProducerTypes = new[] {
			ArtistType.Producer, ArtistType.Circle, ArtistType.OtherGroup, ArtistType.Animator
		};

		/// <summary>
		/// Artists allowed for a song.
		/// </summary>
		public static readonly ArtistType[] SongArtistTypes = new[] {
			ArtistType.Unknown, ArtistType.OtherGroup, ArtistType.OtherVocalist,
			ArtistType.Producer, ArtistType.UTAU, ArtistType.Vocaloid, ArtistType.Animator, ArtistType.OtherIndividual
		};

		public static readonly ArtistType[] VocalistTypes = new[] {
			ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.OtherVocalist
		};

		public static TranslatedStringWithDefault GetArtistString(IEnumerable<IArtistWithSupport> artists, bool isAnimation) {

			ParamIs.NotNull(() => artists);

			var matched = artists.Where(IsValidCreditableArtist).ToArray();
			var producers = matched.Where(a => IsProducerRole(a, isAnimation)).ToArray();
			var performers = matched.Where(a => GetCategories(a).HasFlag(ArtistCategories.Vocalist) 
				&& !producers.Contains(a)).ToArray();

			const string various = "Various artists";

			if (producers.Count() >= 4 || (!producers.Any() && performers.Count() >= 4))
				return new TranslatedStringWithDefault(various, various, various, various);

			var performerNames = performers.Select(GetTranslatedName);
			var producerNames =	producers.Select(GetTranslatedName);

			if (producers.Any() && performers.Any() && producers.Count() + performers.Count() >= 5) {

				return TranslatedStringWithDefault.Create(lang => string.Format("{0} feat. various",
					string.Join(", ", producerNames.Select(p => p[lang]))));

			} else if (producers.Any() && performers.Any()) {

				return TranslatedStringWithDefault.Create(lang => string.Format("{0} feat. {1}",
					string.Join(", ", producerNames.Select(p => p[lang])),
					string.Join(", ", performerNames.Select(p => p[lang]))));

			} else {

				return TranslatedStringWithDefault.Create(lang => string.Join(", ", matched.Select(a => GetTranslatedName(a)[lang])));

			}

		}

		public static ArtistCategories GetCategories(IArtistWithSupport artist) {

			ParamIs.NotNull(() => artist);

			return GetCategories(artist.Artist != null ? artist.Artist.ArtistType : ArtistType.Unknown, artist.Roles);

		}

		public static ArtistCategories GetCategories(ArtistType type, ArtistRoles roles) {

			if (roles == ArtistRoles.Default || !IsCustomizable(type)) {

				return CategoriesForTypes[type];

			} else {

				var cat = ArtistCategories.Nothing;

				if (roles.HasFlag(ArtistRoles.Vocalist))
					cat |= ArtistCategories.Vocalist;

				if (roles.HasFlag(ArtistRoles.Arranger) || roles.HasFlag(ArtistRoles.Composer) || roles.HasFlag(ArtistRoles.VoiceManipulator))
					cat |= ArtistCategories.Producer;

				if (roles.HasFlag(ArtistRoles.Distributor) || roles.HasFlag(ArtistRoles.Publisher))
					cat |= ArtistCategories.Circle;

				if (roles.HasFlag(ArtistRoles.Animator))
					cat |= ArtistCategories.Animator;

				if (cat == ArtistCategories.Nothing)
					cat = ArtistCategories.Other;

				return cat;

			}


		}

		public static bool IsCustomizable(ArtistType at) {

			return CustomizableTypes.Contains(at);

		}

	}

}
