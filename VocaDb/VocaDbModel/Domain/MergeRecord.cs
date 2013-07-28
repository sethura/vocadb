﻿namespace VocaDb.Model.Domain {

	/// <summary>
	/// Record of one entry being merged to another.
	/// </summary>
	/// <typeparam name="T">Type of entry being merged.</typeparam>
	/// <remarks>
	/// Merge record is saved separately from the entry itself so it can be made into a weak link
	/// and the original entry deleted.
	/// </remarks>
	public abstract class MergeRecord<T> where T : class, IEntryBase {

		private T source;
		private T target;

		protected MergeRecord() {}

		protected MergeRecord(T source, T target) {
			this.Source = source;
			this.Target = target;
		}

		public virtual int Id { get; set; }

		public virtual T Source {
			get { return source; }
			set {
				ParamIs.NotNull(() => value);
				source = value;
			}
		}

		public virtual T Target {
			get { return target; }
			set {
				ParamIs.NotNull(() => value);
				target = value;
			}
		}

	}
}
