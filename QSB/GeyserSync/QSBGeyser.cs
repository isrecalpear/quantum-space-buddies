﻿using QSB.EventsCore;
using QSB.QuantumUNET;
using QSB.WorldSync;

namespace QSB.GeyserSync
{
    public class QSBGeyser : WorldObject
    {
        private GeyserController _geyserController;

        public void Init(GeyserController geyserController, int id)
        {
            ObjectId = id;
            _geyserController = geyserController;

            geyserController.OnGeyserActivateEvent += () => HandleEvent(true);
            geyserController.OnGeyserDeactivateEvent += () => HandleEvent(false);
        }

        private void HandleEvent(bool state)
        {
            if (QSBNetworkServer.active)
            {
                GlobalMessenger<int, bool>.FireEvent(EventNames.QSBGeyserState, ObjectId, state);
            }
        }

        public void SetState(bool state)
        {
            if (state)
            {
                _geyserController?.ActivateGeyser();
            }
            else
            {
                _geyserController?.DeactivateGeyser();
            }
        }
    }
}
