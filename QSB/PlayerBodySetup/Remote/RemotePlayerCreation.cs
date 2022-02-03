﻿using QSB.Audio;
using QSB.Player;
using QSB.RoastingSync;
using QSB.Tools;
using QSB.Utility;
using System.Linq;
using UnityEngine;

namespace QSB.PlayerBodySetup.Remote
{
	public static class RemotePlayerCreation
	{
		public static Transform CreatePlayer(
			PlayerInfo player,
			out Transform visibleCameraRoot,
			out Transform visibleRoastingSystem,
			out Transform visibleStickPivot,
			out Transform visibleStickTip)
		{
			DebugLog.DebugWrite($"CREATE PLAYER");

			/*
			 * CREATE PLAYER STRUCTURE
			 */

			DebugLog.DebugWrite($"CREATE PLAYER STRUCTURE");

			// Variable naming convention is broken here to reflect OW unity project (with REMOTE_ prefixed) for readability

			var REMOTE_Player_Body = UnityEngine.Object.Instantiate(QSBCore.NetworkAssetBundle.LoadAsset<GameObject>("Assets/Prefabs/REMOTE_Player_Body.prefab"));
			REMOTE_Player_Body.transform.localPosition = Vector3.zero;
			REMOTE_Player_Body.transform.localScale = Vector3.one;
			REMOTE_Player_Body.transform.localRotation = Quaternion.identity;
			var REMOTE_PlayerCamera = REMOTE_Player_Body.transform.Find("REMOTE_PlayerCamera").gameObject;
			var REMOTE_RoastingSystem = REMOTE_Player_Body.transform.Find("REMOTE_RoastingSystem").gameObject;
			var REMOTE_Stick_Root = REMOTE_RoastingSystem.transform.Find("REMOTE_Stick_Root").gameObject;
			var REMOTE_Traveller_HEA_Player_v2 = REMOTE_Player_Body.transform.Find("REMOTE_Traveller_HEA_Player_v2").gameObject;

			/*
			 * SET UP PLAYER BODY
			 */

			DebugLog.DebugWrite($"SET UP PLAYER BODY");

			player.Body = REMOTE_Player_Body;

			FixMaterialsInAllChildren.ReplaceMaterials(REMOTE_Player_Body.transform);

			player.AnimationSync.InitRemote(REMOTE_Traveller_HEA_Player_v2.transform);
			player.InstrumentsManager.InitRemote(REMOTE_Player_Body.transform);

			REMOTE_Player_Body.GetComponent<PlayerHUDMarker>().Init(player);
			REMOTE_Player_Body.GetComponent<PlayerMapMarker>().PlayerName = player.Name;
			player._ditheringAnimator = REMOTE_Player_Body.GetComponent<DitheringAnimator>();
			// get inactive renderers too
			Delay.RunNextFrame(() =>
				player._ditheringAnimator._renderers = player._ditheringAnimator
					.GetComponentsInChildren<Renderer>(true)
					.Select(x => x.gameObject.GetAddComponent<OWRenderer>())
					.ToArray());
			player.AudioController = PlayerAudioManager.InitRemote(REMOTE_Player_Body.transform);

			/*
			 * SET UP PLAYER CAMERA
			 */

			DebugLog.DebugWrite($"SET UP PLAYER CAMERA");

			REMOTE_PlayerCamera.GetComponent<Camera>().enabled = false;
			var owcamera = REMOTE_PlayerCamera.GetComponent<OWCamera>();
			player.Camera = owcamera;
			player.CameraBody = REMOTE_PlayerCamera;
			visibleCameraRoot = REMOTE_PlayerCamera.transform;

			PlayerToolsManager.InitRemote(player);

			/*
			 * SET UP ROASTING STICK
			 */

			DebugLog.DebugWrite($"SET UP ROASTING STICK");

			var REMOTE_Stick_Pivot = REMOTE_Stick_Root.transform.GetChild(0);
			REMOTE_Stick_Pivot.gameObject.SetActive(false);
			var mallowRoot = REMOTE_Stick_Pivot.Find("REMOTE_Stick_Tip/Mallow_Root");
			var newSystem = mallowRoot.Find("MallowSmoke").gameObject.GetComponent<CustomRelativisticParticleSystem>();
			newSystem.Init(player);
			player.RoastingStick = REMOTE_Stick_Pivot.gameObject;
			var marshmallow = mallowRoot.GetComponent<QSBMarshmallow>();
			player.Marshmallow = marshmallow;

			visibleRoastingSystem = REMOTE_RoastingSystem.transform;
			visibleStickPivot = REMOTE_Stick_Pivot;
			visibleStickTip = REMOTE_Stick_Pivot.Find("REMOTE_Stick_Tip");

			return REMOTE_Player_Body.transform;
		}
	}
}
