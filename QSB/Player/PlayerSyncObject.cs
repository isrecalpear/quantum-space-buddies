﻿using QuantumUNET;

namespace QSB.Player
{
	public abstract class PlayerSyncObject : QSBNetworkBehaviour
	{
		public uint AttachedNetId => NetIdentity?.NetId.Value ?? uint.MaxValue;
		public uint PlayerId => NetIdentity.RootIdentity?.NetId.Value ?? NetIdentity.NetId.Value;
		public PlayerInfo Player => QSBPlayerManager.GetPlayer(PlayerId);

		protected virtual void Awake() => QSBPlayerManager.PlayerSyncObjects.Add(this);

		protected virtual void OnDestroy() => QSBPlayerManager.PlayerSyncObjects.Remove(this);
	}
}