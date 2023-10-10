using Elements.Core;
using FrooxEngine;
using HarmonyLib;
using OpenGlovesLib;
using ResoniteModLoader;
using System.Collections.Generic;

namespace OpenGlovesHaptics;

/*
	* [____CURSOR PARKING LOT_______]
	* [                             ]
	* [_____________________________]
	*  EDIT: this was important when we were in live share
	* 	Users present at one point: dfgHiatus, eia485
	*/

public class OpenGlovesHaptics : ResoniteMod
{
    public override string Name => "OpenGlovesHaptics";
    public override string Author => "dfgHiatus, eia485";
    public override string Version => "0.0.1";
    public override string Link => "https://github.com/dfgHiatus/ResoniteOpenGlovesHaptics/";

    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool> useIsGrabbing =
        new("useIsGrabbing", "Lock fingers if something is grabbed", () => true);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<bool> useRayCasts = 
        new("useRayCasts", "Use raycasts to decide if a finger should be locked", () => true);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<float> rayCastDistance = 
        new("RayCastDistance", "Distance to raycast from finger segments", () => 0.1f);
    [AutoRegisterConfigKey] private static readonly ModConfigurationKey<float> debugTime = 
        new ("DebugTime", "Debug visuals duration", () => 0.1f);

    private static ModConfiguration config;

    private static ForceFeedbackLink feedbackLinkLeft;
    private static ForceFeedbackLink feedbackLinkRight;
    private static Dictionary<HandPoser, Grabber> HandPoserToGrabber = new();

    public override void OnEngineInit()
    {
        config = GetConfiguration();
        new Harmony("net.dfgHiatus.OpenGlovesHaptics").PatchAll();
        feedbackLinkLeft = new ForceFeedbackLink(Handness.Left);
        feedbackLinkRight = new ForceFeedbackLink(Handness.Right);
        OpenGlovesLogger.Init((s) => Msg(s)); // Log stuff from OpenGlovesLogger
    }

    [HarmonyPatch(typeof(HandPoser), "OnCommonUpdate")]
    class OpenGlovesHapticsPatch
    {
        static void Postfix(HandPoser __instance)
        {
            bool useIsGrabbing = config.GetValue(OpenGlovesHaptics.useIsGrabbing);
            bool useRayCasts = config.GetValue(OpenGlovesHaptics.useRayCasts);
            if (__instance.LocalUser == __instance.Slot.ActiveUser && (useIsGrabbing || useRayCasts))
            {
                ForceFeedbackLink feedbackLink = __instance.Side.Value == Chirality.Left ? feedbackLinkLeft : feedbackLinkRight;

                if (useIsGrabbing)
                {
                    if (!HandPoserToGrabber.ContainsKey(__instance))
                    {
                        if(__instance.Slot.FindInteractionHandler()?.Grabber != null)
                            HandPoserToGrabber[__instance] = __instance.Slot.FindInteractionHandler()?.Grabber;
                    }
                    else if (HandPoserToGrabber[__instance].IsHoldingObjects)
                    {
                        Msg("constriting");
                        feedbackLink.Constrict();
                        return;
                    }
                }

                VRFFBInput hand = new();
                if (useRayCasts)
                {
                    hand.thumbCurl = (short)(processFinger(__instance.Thumb, __instance) ? 1000 : 0);
                    hand.indexCurl = (short)(processFinger(__instance.Index, __instance) ? 1000 : 0);
                    hand.middleCurl = (short)(processFinger(__instance.Middle, __instance) ? 1000 : 0);
                    hand.ringCurl = (short)(processFinger(__instance.Ring, __instance) ? 1000 : 0);
                    hand.pinkyCurl = (short)(processFinger(__instance.Pinky, __instance) ? 1000 : 0);
                }
                feedbackLinkLeft.Write(hand);
            }
        }
    }

    static bool processFinger(HandPoser.Finger finger, HandPoser instance)
    {
        return processFingerSegment(finger.Proximal, instance)
        || processFingerSegment(finger.Intermediate, instance)
        || processFingerSegment(finger.Distal, instance);
    }

    static bool processFingerSegment(HandPoser.FingerSegment fingerSegment, HandPoser instance)
    {
        float3 globalPoint = instance.Slot.LocalPointToGlobal(float3.Zero);
        float globalDistance = instance.Slot.LocalScaleToGlobal(config.GetValue(rayCastDistance));
        var hit = instance.Physics.RaycastOne(globalPoint, instance.Slot.Down, globalDistance, debugDuration : config.GetValue(debugTime));
        return hit.HasValue;
    }
}