using System;
using System.Collections.Generic;
using FrooxEngine;
using Elements.Core;
using HarmonyLib;
using FrooxEngine.ProtoFlux;
using ResoniteModLoader;
using System.Linq;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes;
using System.Reflection;

namespace LogixUtils
{
    class InputNodes : ToggleablePatch
    { // Fix existing input nodes not being defined as input nodes

        public override void Initialize(Harmony harmony, LogixUtils mod, ModConfiguration config)
        {
            addOrRemoveInputnodes(unusedInputs, config.GetValue(LogixUtils.AddInputNodes));
        }
        public override void OnThisConfigurationChanged(ConfigurationChangedEvent configurationChangedEvent)
        {
            if (configurationChangedEvent.Key == LogixUtils.AddInputNodes)
            {
                addOrRemoveInputnodes(unusedInputs, configurationChangedEvent.Config.GetValue(LogixUtils.AddInputNodes));
                return;
            }         
        }

        public static MethodInfo spawnPostfix = typeof(InputNodes).GetMethod(nameof(NodeSpawnPostfix));
        public override void Patch(Harmony harmony, LogixUtils mod)
        {
            harmony.Patch(AccessTools.Method(typeof(ProtoFluxTool), "GenerateSlotNode"), postfix: new(spawnPostfix));
        }
        public override void Unpatch(Harmony harmony, LogixUtils mod)
        {
            harmony.Unpatch(AccessTools.Method(typeof(ProtoFluxTool), "GenerateSlotNode"), spawnPostfix);
        }

        public static bool PostFixNode<T>(Type t, Slot s, T a) where T : class, IWorldElement
        {
            if(typeof(T) != t) return false;

            s.RunInUpdates(0, () =>
            {
                var comp = s.GetComponent<RefObjectInput<T>>();
                if(comp != null)
                {
                    comp.Target.Target = a;
                }
            });
            return true;
        }
        public static void NodeSpawnPostfix(Type type, Slot __result)
        {
            if (!(type.IsGenericType && typeof(RefObjectInput<>) == type.GetGenericTypeDefinition())) return;
            var t = type.GenericTypeArguments[0];
            if (PostFixNode<Slot>(t, __result, __result)) return;
            if (PostFixNode<User>(t, __result, __result.LocalUser)) return;
            if (PostFixNode<UserRoot>(t, __result, __result.LocalUserRoot)) return;
        }

        public static void addOrRemoveInputnodes(HashSet<NodeTypeRecord> nodes, bool addNode)
        {
            var inputNodes = Traverse.Create(typeof(ProtoFluxHelper)).Field("inputNodes").GetValue<List<NodeTypeRecord>>();
            if (addNode)
            {
                inputNodes.InsertRange(0, nodes);
                return;
            }
            inputNodes.RemoveAll(nodes.Contains);
        }

        private static HashSet<NodeTypeRecord> unusedInputs = new() {
              new(typeof(FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.Math.Constants.Bobool3ol), (t)=>t == typeof(bobool3ol), null), // This actually prevents a crash :O
              new(typeof(FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.TimeAndDate.UtcNow), (t)=>t == typeof(DateTime), null),
        };
    }
}