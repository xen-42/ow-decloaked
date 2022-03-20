using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decloaked
{
    public static class Patches
    {
        public static void Apply()
        {
            var harmony = Decloaked.Instance.ModHelper.HarmonyHelper;

            harmony.AddPostfix<MapController>(nameof(MapController.OnTargetReferenceFrame), typeof(Patches), nameof(Patches.OnTargetReferenceFrame));
            harmony.AddPostfix<ReferenceFrame>(nameof(ReferenceFrame.GetHUDDisplayName), typeof(Patches), nameof(Patches.GetHUDDisplayName));

            // CloakingFieldProxy needs a lot of work
            //harmony.AddPrefix<CloakingFieldProxy>(nameof(CloakingFieldProxy.OnPlayerEnterCloakingField), typeof(Patches), nameof(Patches.OnPlayerEnterCloakingField));
            harmony.AddPrefix<CloakingFieldProxy>(nameof(CloakingFieldProxy.OnPlayerExitCloakingField), typeof(Patches), nameof(Patches.OnPlayerExitCloakingField));
            harmony.AddPrefix<CloakingFieldProxy>(nameof(CloakingFieldProxy.OnEnterDreamWorld), typeof(Patches), nameof(Patches.OnEnterDreamWorld));
            harmony.AddPrefix<CloakingFieldProxy>(nameof(CloakingFieldProxy.OnExitDreamWorld), typeof(Patches), nameof(Patches.OnExitDreamWorld));
            harmony.AddPrefix<CloakingFieldProxy>(nameof(CloakingFieldProxy.ShouldBeActive), typeof(Patches), nameof(Patches.ShouldBeActive));
            harmony.AddPrefix<CloakingFieldProxy>(nameof(CloakingFieldProxy.ShouldBeHidden), typeof(Patches), nameof(Patches.ShouldBeHidden));
        }

        public static void OnTargetReferenceFrame(MapController __instance, ReferenceFrame __0)
        {
            // So that it'll follow it up
            if (__0.GetAstroObject().GetAstroObjectName() == AstroObject.Name.RingWorld)
            {
                __instance._isLockedOntoMapSatellite = true;
            }
        }

        public static void GetHUDDisplayName(ReferenceFrame __instance, ref string __result)
        {
            if (__instance.GetAstroObject().GetAstroObjectName() == AstroObject.Name.RingWorld)
            {
                __result = UITextLibrary.GetString(UITextType.LocationIP);
            }
        }

        #region CloakingFieldProxy
        public static bool OnPlayerEnterCloakingField()
        {
            return false;
        }

        public static bool OnPlayerExitCloakingField()
        {
            return false;
        }

        public static bool OnEnterDreamWorld()
        {
            return false;
        }

        public static bool OnExitDreamWorld()
        {
            return false;
        }

        public static void ShouldBeActive(CloakingFieldProxy __instance)
        {
            __instance._playerInCloakingField = true;
            __instance._playerInDreamWorld = false;
        }

        public static void ShouldBeHidden(CloakingFieldProxy __instance)
        {
            __instance._playerInCloakingField = true;
            __instance._playerInDreamWorld = false;
        }
        #endregion CloakingFieldProxy
    }
}
