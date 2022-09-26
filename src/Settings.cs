﻿using ModSettings;
using System.IO;
using System;
using System.Text;

namespace HouseLights
{
    internal class HouseLightsSettings : JsonModSettings
    {
        [Name("Intensity Value")]
        [Description("Set the intensity for the lights.")]
        [Slider(0f, 3f, 1)]
        public float intensityValue = 2f;

        [Name("Range Multiplier")]
        [Description("Values above 1 make the lights cast light further. 2 will make them reach double the distance than default, 0 turns the lights off.")]
        [Slider(0f, 5f, 1)]
        public float rangeMultiplier = 1.4f;

        [Name("Turn off aurora light flicker")]
        [Description("If set to yes, aurora powered lights won't flicker and will stay on.")]
        public bool disableAuroraFlicker = false;

        [Name("Cast Shadows")]
        [Description("If set to yes, lights will cast shadows (can show artifacts and might reduce performance)")]
        public bool castShadows = false;

        [Name("Colorless lights")]
        [Description("If set to yes, lights will cast a more white light. If set to no, they will cast light with the default color.")]
        public bool whiteLights = false;

        [Name("Enable Outside lights")]
        [Description("If set to yes, returns the old feature of lights outside being turned on with console cmd: Toggle_Lights (unless you set 'Default light state - ON' to 'True' below) **WARNING PERFORMANCE IMPACT due to all outdoor lights being on at once in scene!**")]
        public bool outsidelights = false;

        [Name("Default light state - ON")]
        [Description("Sets the default state of lights in game to always being ON.")]
        public bool outsidelightsdefault = false;

    }

    internal static class Settings
    {
        public static HouseLightsSettings options;

        public static void OnLoad()
        {
            options = new HouseLightsSettings();
            options.AddToModSettings("House Lights Settings");
        }
    }
}
