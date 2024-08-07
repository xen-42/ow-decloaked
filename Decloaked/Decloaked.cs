using OWML.ModHelper;
using System;
using UnityEngine;

namespace Decloaked
{
    public class Decloaked : ModBehaviour
    {
        public static Decloaked Instance;

        private void Start()
        {
            Instance = this;

            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;

            Patches.Apply();
        }

        private void OnCompleteSceneLoad(OWScene scene, OWScene loadScene)
        {
            if (loadScene != OWScene.SolarSystem) return;

            #region Cloak
            var cloak = GameObject.Find("RingWorld_Body").GetComponentInChildren<CloakFieldController>();

            cloak._playerInsideCloak = true;
            cloak._playerCloakFactor = 1f;
            cloak._worldFadeFactor = 1f;
            cloak._interiorRevealFactor = 1f;
            cloak._rendererFade = 1f;
            cloak._hasTriggeredMusic = true;

            // Idk why but this makes it work
            try
            {
                cloak.LateUpdate();
            }
            catch (Exception) { }

            // Else it messes up reference frames
            cloak._playerInsideCloak = false;
            cloak.gameObject.SetActive(false);

            foreach (var cloakProxy in GameObject.FindObjectsOfType<CloakingFieldProxy>())
            {
                cloakProxy.OnPlayerEnterCloakingField();
            }
            #endregion Cloak

            #region Generic
            // Make it so we can lock on from a distance
            var rfv = GameObject.Find("RingWorld_Body/Volumes_IP/RFVolume").GetComponent<ReferenceFrameVolume>();
            rfv.gameObject.SetActive(true);           

            // Generic improvements
            GameObject.Find("RingWorld_Body/Volumes_IP/SpeedLimitZeroGravityField").SetActive(false);
            GameObject.Find("RingWorld_Body/Volumes_IP/AntiTravelMusicRuleset").SetActive(false);
            GameObject.Find("RingWorld_Body/Volumes_IP/CloakVolume").SetActive(false);

            // Add a map marker with UI
            MapMarker mapMarker = GameObject.Find("RingWorld_Body").AddComponent<MapMarker>();
            mapMarker._labelID = (UITextType)Utility.AddUI(UITextLibrary.GetString(UITextType.LocationIP).ToUpper());
            mapMarker._markerType = MapMarker.MarkerType.Planet;
            mapMarker._maxDisplayDistance = 50000f;

            // Fix reference frame tracker to make it stop caring about the cloak (else after visiting once we can never lock on to anything ever again)
            GameObject.FindObjectOfType<ReferenceFrameTracker>()._cloakController = null;

            // Maybe fix other things like signals? Idk but its worth a shot
            Locator._cloakFieldController = null;
            #endregion region Generic

            #region Assets
            // Load assets
            var streamingGroup = GameObject.Find("RingWorld_Body/StreamingGroup_RW").GetComponent<StreamingGroup>();
            streamingGroup.LoadGeneralAssets();
            streamingGroup.LoadRequiredAssets();
            streamingGroup.LoadRequiredColliders();
            streamingGroup._locked = true;
            #endregion Assets

            #region PersistentCondition
            PlayerData.SetPersistentCondition("MARK_ON_HUD_TUTORIAL_COMPLETE", true);
            PlayerData.SetPersistentCondition("COMPLETED_SHIPLOG_TUTORIAL", true);
            #endregion PersistentCondition

            foreach (var shipLogEntryLocation in GameObject.Find("RingWorld_Body").GetComponentsInChildren<ShipLogEntryLocation>())
            {
                shipLogEntryLocation._isWithinCloakField = false;
            }
        }
    }
}
