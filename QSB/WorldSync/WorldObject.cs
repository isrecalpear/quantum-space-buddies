﻿using UnityEngine;

namespace QSB.WorldSync
{
	public abstract class WorldObject<T> : IWorldObject
		where T : MonoBehaviour
	{
		public int ObjectId { get; init; }
		public T AttachedObject { get; init; }
		public string Name => AttachedObject ? AttachedObject.name : "<NullObject!>";
		public string LogName => $"{ObjectId}:{GetType().Name} ({Name})";

		public virtual void Init() { }
		public virtual void OnRemoval() { }
		public MonoBehaviour ReturnObject() => AttachedObject;
		public virtual bool ShouldDisplayDebug() => QSBWorldSync.AllObjectsReady && AttachedObject && AttachedObject.gameObject.activeInHierarchy;
		public virtual string ReturnLabel() => LogName;
		public virtual void DisplayLines() { }

		/// indicates that this won't become ready immediately
		protected void StartDelayedReady() => QSBWorldSync._numObjectsReadying++;

		/// indicates that this is now ready
		protected void FinishDelayedReady() => QSBWorldSync._numObjectsReadying--;
	}
}
