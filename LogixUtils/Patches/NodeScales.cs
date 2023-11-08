using Elements.Core;
using FrooxEngine;
using FrooxEngine.ProtoFlux;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;



namespace LogixUtils
{
    class NodeScales : ToggleablePatch
    {
        public override void Patch(Harmony harmony, LogixUtils mod) 
        {
            var relays = typeof(ProtoFluxTool).GetMethod(nameof(ProtoFluxTool.OnGrabbing));

            var relayTranspiler = typeof(NodeScales).GetMethod(nameof(RelayTranspiler));
                        
            harmony.Patch(relays, transpiler: new HarmonyMethod(relayTranspiler));
        }
        public override void Unpatch(Harmony harmony, LogixUtils mod)
        {
            var relays = typeof(ProtoFluxTool).GetMethod(nameof(ProtoFluxTool.OnGrabbing));

            var relayTranspiler = typeof(NodeScales).GetMethod(nameof(RelayTranspiler));

            harmony.Unpatch(relays, relayTranspiler);
        }

        public static float3 SetScale(float3 scale)
        {
            return scale * 1.25f;
        }

        public static IEnumerable<CodeInstruction> RelayTranspiler(IEnumerable<CodeInstruction> codes)
        { // Pretty good tutorial, https://gist.github.com/JavidPack/454477b67db8b017cb101371a8c49a1c

            var lookFor = typeof(Slot).GetMethod("set_" + nameof(Slot.GlobalScale));

            foreach (var code in codes)
            {
                if (code.Calls(lookFor))
                {
                    yield return new(OpCodes.Call, typeof(NodeScales).GetMethod(nameof(SetScale)));
                }
                yield return code;
            }
        }
    }
}
