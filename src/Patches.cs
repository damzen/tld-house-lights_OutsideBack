﻿using System;
using HarmonyLib;
using UnityEngine;

namespace HouseLights
{
    class Patches
    {
        [HarmonyPatch(typeof(GameManager), "InstantiatePlayerObject")]
        internal class GameManager_InstantiatePlayerObject
        {
            public static void Prefix()
            {
                /*if (!InterfaceManager.IsMainMenuActive())
                {
                    HouseLights.Init();
                    HouseLights.GetSwitches();
                }*/

                // if (!InterfaceManager.IsMainMenuEnabled() && (!GameManager.IsOutDoorsScene(GameManager.m_ActiveScene) || HouseLights.notReallyOutdoors.Contains(GameManager.m_ActiveScene)))
                if (!InterfaceManager.IsMainMenuEnabled())
                 {
                    HouseLights.Init();
                    HouseLights.GetSwitches();
                }
            }
        }

        [HarmonyPatch(typeof(AuroraElectrolizer), "Initialize")]
        internal class AuroraElectrolizer_Initialize
        {
            private static void Postfix(AuroraElectrolizer __instance)
            {
                //if (InterfaceManager.IsMainMenuEnabled() || (GameManager.IsOutDoorsScene(GameManager.m_ActiveScene) && !HouseLights.notReallyOutdoors.Contains(GameManager.m_ActiveScene)))
                if (InterfaceManager.IsMainMenuEnabled() || (!Settings.options.outsidelights && GameManager.IsOutDoorsScene(GameManager.m_ActiveScene)))
                {
                    return;
                }

                AuroraActivatedToggle[] radios = __instance.gameObject.GetComponentsInParent<AuroraActivatedToggle>();
                AuroraScreenDisplay[] screens = __instance.gameObject.GetComponentsInChildren<AuroraScreenDisplay>();

                if (radios.Length == 0 && screens.Length == 0)
                {
                    HouseLights.AddElectrolizer(__instance);
                }

            }
        }

        [HarmonyPatch(typeof(AuroraManager), "RegisterAuroraLightSimple", new Type[] { typeof(AuroraLightingSimple) })]
        internal class AuroraManager_RegisterLightSimple
        {
            private static void Postfix(AuroraManager __instance, AuroraLightingSimple auroraLightSimple)
            {
                // if (InterfaceManager.IsMainMenuEnabled() || (GameManager.IsOutDoorsScene(GameManager.m_ActiveScene) && !HouseLights.notReallyOutdoors.Contains(GameManager.m_ActiveScene)))
                if (InterfaceManager.IsMainMenuEnabled() || (!Settings.options.outsidelights && GameManager.IsOutDoorsScene(GameManager.m_ActiveScene)))
                {
                    return;
                }

              

                HouseLights.AddElectrolizerLight(auroraLightSimple);
            }
        }


        //[HarmonyPatch(typeof(AuroraManager), "Update")]
        [HarmonyPatch(typeof(AuroraManager), "UpdateForceAurora")]
        internal class AuroraManager_UpdateForceAurora
        {
            private static void Postfix(AuroraManager __instance)
            {
                //  if (InterfaceManager.IsMainMenuEnabled() || (GameManager.IsOutDoorsScene(GameManager.m_ActiveScene) && !HouseLights.notReallyOutdoors.Contains(GameManager.m_ActiveScene)))
                if (InterfaceManager.IsMainMenuEnabled() || (!Settings.options.outsidelights && GameManager.IsOutDoorsScene(GameManager.m_ActiveScene)))
                {
                    return;
                }

                if (HouseLights.electroSources.Count > 0)
                {
                    HouseLights.UpdateElectroLights(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerManager), "GetInteractiveObjectDisplayText", new Type[] {typeof(GameObject)})]
        internal class PlayerManager_GetObjText
        {
            private static void Postfix(PlayerManager __instance, ref string __result, GameObject interactiveObject)
            {
                if (interactiveObject.name == "XPZ_Switch")
                {
                    if (HouseLights.lightsOn)
                    {
                        __result = "Turn Lights Off";
                    }
                    else
                    {
                        __result = "Turn Lights On";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction")]
        internal class PlayerManager_InteractiveObjectsProcessInteraction
        {
            private static void Postfix(PlayerManager __instance, ref bool __result)
            {
                if (__instance.m_InteractiveObjectUnderCrosshair != null && __instance.m_InteractiveObjectUnderCrosshair.name == "XPZ_Switch")
                {
                    HouseLights.ToggleLightsState();
                    GameAudioManager.PlaySound("Stop_RadioAurora", __instance.gameObject);

                    Vector3 scale = __instance.m_InteractiveObjectUnderCrosshair.transform.localScale;
                    __instance.m_InteractiveObjectUnderCrosshair.transform.localScale = new Vector3(scale.x, scale.y * -1, scale.z);

                    //Play Sound

                    __result = true;
                }
            }
        }

        [HarmonyPatch(typeof(AuroraElectrolizer), "UpdateIntensity")]
        internal class AuroraElectrolizer_UpdateIntensity
        {
            private static bool Prefix(AuroraElectrolizer __instance)
            {
                if (Settings.options.disableAuroraFlicker)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(Weather), "IsTooDarkForAction", new Type[] { typeof(ActionsToBlock) })]
        internal class Weather_IsTooDarkForAction
        {
            private static void Postfix(Weather __instance, ref bool __result)
            {
                if (__result && GameManager.GetWeatherComponent().IsIndoorScene() && HouseLights.lightsOn)
                {
                    __result = false;
                }
            }
        }
    }
}
