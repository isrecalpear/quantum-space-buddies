﻿using QSB.Utility;
using QSB.WorldSync;
using UnityEngine;

namespace QSB.Animation.NPC.WorldObjects
{
	internal abstract class NpcAnimController<T> : WorldObject<T>, INpcAnimController
		where T : MonoBehaviour
	{
		public abstract CharacterDialogueTree GetDialogueTree();

		public virtual void StartConversation()
			=> GetDialogueTree().RaiseEvent("OnStartConversation");

		public virtual void EndConversation()
			=> GetDialogueTree().RaiseEvent("OnEndConversation");

		public abstract bool InConversation();
	}
}
