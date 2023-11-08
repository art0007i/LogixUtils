using FrooxEngine;
using FrooxEngine.ProtoFlux;
using HarmonyLib;
using System.Reflection;

namespace LogixUtils
{
    class VrSpawnFix : ToggleablePatch
    {
        public override void Patch(Harmony harmony, LogixUtils mod)
        {
            var positionSpawnedNodeMethod = typeof(ProtoFluxTool).GetMethod("GenerateSlotNode", BindingFlags.NonPublic | BindingFlags.Instance);

            var vrNodeRotationPatchMethod = typeof(VrSpawnFix).GetMethod(nameof(VrNodeRotationPatch));

            harmony.Patch(positionSpawnedNodeMethod, postfix: new HarmonyMethod(vrNodeRotationPatchMethod));
        }

        public override void Unpatch(Harmony harmony, LogixUtils mod)
        {
            var positionSpawnedNodeMethod = typeof(ProtoFluxTool).GetMethod("GenerateSlotNode", BindingFlags.NonPublic | BindingFlags.Instance);

            var vrNodeRotationPatchMethod = typeof(VrSpawnFix).GetMethod(nameof(VrNodeRotationPatch));

            harmony.Unpatch(positionSpawnedNodeMethod, vrNodeRotationPatchMethod);
        }

        public static void VrNodeRotationPatch(ProtoFluxTool __instance, Slot __result)
        {
            if (__instance.InputInterface.VR_Active)
            {
                __result.Up = __instance.Slot.ActiveUserRoot.Slot.Up;
            }
        }
    }
}