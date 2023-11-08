using FrooxEngine;
using FrooxEngine.ProtoFlux;
using HarmonyLib;
using ResoniteModLoader;
using System;
using System.Collections.Generic;

namespace LogixUtils;

public class LogixUtils : ResoniteMod
{
    public override string Name => "LogixUtils";
    public override string Author => "badhaloninja"; // ported by art0007i
    public override string Version => "1.7.0";
    public override string Link => "https://github.com/badhaloninja/LogixUtils";

    internal static ModConfiguration config;
    private static Harmony harmony;

    // Fix Scales
    [AutoRegisterConfigKey]
    private static readonly ModConfigurationKey<bool> NodeScaleFixesOption = new ModConfigurationKey<bool>("nodeScales", "Fix relay nodes not scaling correctly", () => true);
    // Clamp
    // NOTE: Not porting because they are shared assets, and modifying shared assets can be sketchy
    //[AutoRegisterConfigKey]
    //private static readonly ModConfigurationKey<bool> ClampNodeTextures = new ModConfigurationKey<bool>("clampNodeTextures", "Clamp various node textures", () => true);


    // Input Nodes
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> AddInputNodes = new ModConfigurationKey<bool>("addInputNodes", "Add extra input nodes to input node list", () => true);
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> TweakInputNodes = new ModConfigurationKey<bool>("tweakInputNodes", "Change some nodes to contain some default data (Slot input contains the node slot, User input contains spawning user, UserRoot input contains spawning user root)", () => true);

    // UI Align
    [AutoRegisterConfigKey]
    private static readonly ModConfigurationKey<bool> UIAlignItemsBackwards = new ModConfigurationKey<bool>("uiAlignItemsBackwards", "Enable UI Align tweaks", () => true);
    
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> SnapToAngleOnAlign = new ModConfigurationKey<bool>("snapToAngleOnAlign", "Snap to angle when aligning, if disabled only flips forward and backward", () => true);
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<float> SnapAngle = new ModConfigurationKey<float>("alignSnapAngle", "Angle to snap to when aligning", () => 90f);

    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<Key> AlignScaleModifierKey = new ModConfigurationKey<Key>("alignScaleModifierKey", "Key to be held to allow scaling on align", () => Key.Space);
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> ModifiedScaleToUserScale = new ModConfigurationKey<bool>("modifiedScaleToUserScale", "Scale to user scale when aligning scale instead of global 1", () => false);
    
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> AlignNodeRotationToUserVr = new ModConfigurationKey<bool>("alignNodeRotationToUserVr", "Aligns the spawned nodes to the user's up", () => true);


    public static Dictionary<ModConfigurationKey<bool>, ToggleablePatch> TogglePatchList = new Dictionary<ModConfigurationKey<bool>, ToggleablePatch>() {
        { NodeScaleFixesOption, new NodeScales() },
        { UIAlignItemsBackwards, new UIAlignItemTweaks() },
        { AddInputNodes, new InputNodes() },
        { AlignNodeRotationToUserVr, new VrSpawnFix() },
    };

    public override void OnEngineInit()
    {
        config = GetConfiguration();

        harmony = new Harmony("me.badhaloninja.LogixUtils");
        harmony.PatchAll(); //For reverse patches


        foreach(var patch in TogglePatchList)
        {
            patch.Value.Initialize(harmony, this, config);

            config.OnThisConfigurationChanged += patch.Value.OnThisConfigurationChanged;

            if (!config.GetValue(patch.Key)) continue;
            patch.Value.Patch(harmony, this);
        }

        config.OnThisConfigurationChanged += HandleConfigChanged;
    }




    private void HandleConfigChanged(ConfigurationChangedEvent configurationChangedEvent)
    {
        var BoolKey = configurationChangedEvent.Key as ModConfigurationKey<bool>;

        if (!config.TryGetValue(BoolKey, out bool patchEnabled)) return;

        if (TogglePatchList.TryGetValue(BoolKey, out ToggleablePatch patch)) {
            if (patchEnabled)
            {
                Msg($"Patch \"{patch.GetType().Name}\" enabled");
                patch.Patch(harmony, this);
                return;
            }
            Msg($"Patch \"{patch.GetType().Name}\" disabled");
            patch.Unpatch(harmony, this);
        }
    }
}