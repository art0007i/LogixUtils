using FrooxEngine;
using FrooxEngine.LogiX;
using FrooxEngine.LogiX.Data;
using HarmonyLib;

namespace LogixUtils
{
    internal class OtherInputNodes : ToggleablePatch
    {
        public override void Patch(Harmony harmony, LogixUtils mod)
        {
            var method = AccessTools.Method(typeof(LogixTip), "AttachInput");
            harmony.Patch(method, postfix: new HarmonyMethod(typeof(OtherInputNodes).GetMethod("AttachInputPatch")));
        }

        public override void Unpatch(Harmony harmony, LogixUtils mod)
        {
            var method = AccessTools.Method(typeof(LogixTip), "AttachInput");
            harmony.Unpatch(method, typeof(OtherInputNodes).GetMethod("AttachInputPatch"));
        }

        public static void AttachInputPatch(LogixTip __instance, ref LogixNode __result, IInputElement input)
        {
            if (__result == null)
            {
                if (input.InputType == typeof(Slot))
                {
                    __result = LogixUtils.ReversePatches.CreateNewNodeSlot(__instance, LogixHelper.GetNodeName(typeof(SlotRegister))).AttachComponent<SlotRegister>();
                    (__result as SlotRegister).Target.Target = __result.Slot;
                    return;
                }
                if (input.InputType == typeof(User))
                {
                    __result = LogixUtils.ReversePatches.CreateNewNodeSlot(__instance, LogixHelper.GetNodeName(typeof(UserRegister))).AttachComponent<UserRegister>();
                    (__result as UserRegister).User.Target = __instance.LocalUser;
                    return;
                }
            }
        }

    }
}
