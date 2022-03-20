using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Decloaked
{
    public class Decloaked : ModBehaviour
    {
        private List<string> _assetBundles = null;
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

            // Collect all the asset bundles (only have to do it once)
            //if (_assetBundles == null) CollectAssetBundles();

            StopCloakingField();

            // Some generic improvements
            var rfv = GameObject.Find("RingWorld_Body/Volumes_IP/RFVolume").GetComponent<ReferenceFrameVolume>();
            rfv.gameObject.SetActive(true);           

            GameObject.Find("RingWorld_Body/Volumes_IP/SpeedLimitZeroGravityField").SetActive(false);
            GameObject.Find("RingWorld_Body/Volumes_IP/AntiTravelMusicRuleset").SetActive(false);
            GameObject.Find("RingWorld_Body/Volumes_IP/CloakVolume").SetActive(false);

            MapMarker mapMarker = GameObject.Find("RingWorld_Body").AddComponent<MapMarker>();
            mapMarker._labelID = (UITextType)AddUI(UITextLibrary.GetString(UITextType.LocationIP).ToUpper());
            mapMarker._markerType = MapMarker.MarkerType.Planet;
            mapMarker._maxDisplayDistance = 50000f;

            var streamingGroup = GameObject.Find("RingWorld_Body/StreamingGroup_RW").GetComponent<StreamingGroup>();
            streamingGroup.LoadGeneralAssets();
            streamingGroup.LoadRequiredAssets();
            streamingGroup.LoadRequiredColliders();
            streamingGroup._locked = true;

            //LoadRingWorldAssets();

            // When leaving the stuff is unloaded so just load it back in yolo
            //Locator.GetAstroObject(AstroObject.Name.RingWorld).GetRootSector().OnOccupantExitSector += OnOccupantExitSector;
        }

        private void StopCloakingField()
        {
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
        }

        private void CollectAssetBundles()
        {
            var ringWorld = GameObject.Find("RingWorld_Body");
            var ring = GameObject.Find("StaticRing_Body");

            _assetBundles = new List<string>();
            // Fine to just have the low LOD materials because we're gonna be far away
            var handles = ringWorld.GetComponentsInChildren<StreamingMeshHandle>().Concat(ring.GetComponentsInChildren<StreamingMeshHandle>());
            foreach (var handle in handles)
            {
                _assetBundles.SafeAdd(handle.assetBundle);
            }
        }

        private void OnOccupantExitSector(SectorDetector sd)
        {
            LoadRingWorldAssets();
        }

        private void LoadRingWorldAssets()
        {
            foreach (var assetBundle in _assetBundles)
            {
                StreamingManager.LoadStreamingAssets(assetBundle);
            }
        }

        public int AddUI(string text)
        {
            var uiTable = TextTranslation.Get().m_table.theUITable;

            var key = uiTable.Keys.Max() + 1;
            try
            {
                // Ensure it doesn't already contain our UI entry
                KeyValuePair<int, string> pair = uiTable.First(x => x.Value.Equals(text));
                if (pair.Equals(default(KeyValuePair<int, string>))) key = pair.Key;
            }
            catch (Exception) { }

            TextTranslation.Get().m_table.Insert_UI(key, text);

            return key;
        }
    }
}
