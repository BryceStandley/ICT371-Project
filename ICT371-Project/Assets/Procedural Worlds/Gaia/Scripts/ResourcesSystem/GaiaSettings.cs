﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Gaia.Pipeline;

namespace Gaia
{
    /// <summary>
    /// This object stores Gaia settings. It remembers what you have been working on, and resets these when you start up the Gaia Manager window.
    /// </summary>
    public class GaiaSettings : ScriptableObject
    {
        [Header("Current Settings")]
        public GaiaConstants.EnvironmentControllerType m_currentController = GaiaConstants.EnvironmentControllerType.FirstPerson;
        [Tooltip("Target size.")]
        public GaiaConstants.EnvironmentSize m_currentSize = GaiaConstants.EnvironmentSize.Is2048MetersSq;
        [Tooltip("Current target environment.")]
        public GaiaConstants.EnvironmentTarget m_currentEnvironment = GaiaConstants.EnvironmentTarget.Desktop;
        [Tooltip("Current target renderer.")]
        public GaiaConstants.EnvironmentRenderer m_currentRenderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
        [Tooltip("Current defaults object.")]
        public GaiaDefaults m_currentDefaults;
        [Range(1, 20), Tooltip("Number of tiles in X direction for a new terrain.")]
        public int m_tilesX = 1;
        [Range(1, 20), Tooltip("Number of tiles in Z direction for a new terrain.")]
        public int m_tilesZ = 1;
        [Tooltip("If terrains should be created in new scenes per default.")]
        public bool m_createTerrainScenes = false;
        [Tooltip("If scenes should be unloaded after creation per default.")]
        public bool m_unloadTerrainScenes = false;
        [Tooltip("If scenes should be unloaded after creation per default.")]
        public bool m_floatingPointFix = false;
        public GaiaConstants.EnvironmentSizePreset m_targeSizePreset = GaiaConstants.EnvironmentSizePreset.Large;
        [Tooltip("Default Random Terrain Generation stamp spawn settings.")]
        public SpawnerSettings m_defaultStampSpawnSettings;
        [Tooltip("Default World Biome Mask settings.")]
        public SpawnerSettings m_defaultBiomeMaskSettings;
        public ComputeShader m_spawnSimulateComputeShader;
        //[Tooltip("Current terrain resources object.")]
        //public GaiaResource m_currentResources;
        //[Tooltip("Current game object resources object.")]
        //public GaiaResource m_currentGameObjectResources;
        [Tooltip("Current size divisor.")]
        public float m_currentSizeDivisor = 1f;
        [Tooltip("Current prefab name for the player object.")]
        public string m_currentPlayerPrefabName = "FPSController";
        [Tooltip("This prefab is what is sapwnned and attached to the Fly camera if the fly camera is being used in Gaia.")]
        public GameObject m_flyCamUI;
        [Tooltip("Current prefab name for the water object.")]
        public string m_currentWaterPrefabName = "Water4Advanced";
        [Tooltip("This setting will write baked collision data as image files to the disk when saving the scene. This allows you to close a scene and continue working on it later without having to re-bake collision data.")]
        public bool m_saveCollisionCacheWhenSaving = false;
        //[Tooltip("The selected sky to set up from the Gaia Manager Standard Tab")]
        //public GaiaConstants.GaiaLightingProfileType m_currentSkies;
        [Tooltip("When using dynamically loaded terrains, Gaia will keep terrain scenes in memory until this memory usage is reached before it starts to unload them again.")]
        public long m_terrainUnloadMemoryTreshold = 4294967296;
        [Tooltip("The default Terrain Layer that is used on the world map")]
        public TerrainLayer m_worldmapLayer;
        [Tooltip("Sets if ambient audio should be spawned or not into the scene")]
        public bool m_enableAmbientAudio = true;
        [Tooltip("Sets if post processing should be added to the scene")]
        public bool m_enablePostProcessing = true;
        [Tooltip("The selected Post FX to set up from the Gaia Manager Standard Tab")]
        public GaiaConstants.PostFX m_currentPostFX;
        [Tooltip("The selected Water to set up from the Gaia Manager Standard Tab")]
        public GaiaConstants.Water m_currentWater;
        [Tooltip("The selected Water to set up from the Gaia Manager Standard Tab")]
        public GaiaConstants.GaiaWaterProfileType m_currentWaterPro;
        [Tooltip("Enables underwater features to be setup by Gaia.")]
        public bool m_enableUnderwaterEffects = true;
        [Tooltip("Whether a wind zone will be set up when creating a new terrain")]
        public bool m_createWind;
        public GaiaConstants.GaiaGlobalWindType m_windType = GaiaConstants.GaiaGlobalWindType.Calm;
        [Tooltip("Whether a screen shotter will be set up when creating a new terrain")]
        public bool m_createScreenShotter;
        [Tooltip("If enabled this will focus the scene view camera at the created player spawned in the scene")]
        public bool m_focusPlayerOnSetup = false;
        [Tooltip("Default threshold for floating point fix")]
        public float m_FPFDefaultThreshold = 5000;

        [Header("User Directory Settings")]
        [Tooltip("Path for biome settings files. Must include 'Assets\' at the beginning.")]
        public string m_biomesDirectory = "Assets/Gaia User Data/Biomes";
        //[Tooltip("Path for exported files. Must include 'Assets\' at the beginning.")]
        //public string m_exportsDirectory = "Assets/Gaia User Data/Exports";
        [Tooltip("Path for screenshots. Must include 'Assets\' at the beginning.")]
        public string m_screenshotsDirectory = "Assets/Gaia User Data/Screenshots";
        [Tooltip("Path for session storage. Must include 'Assets\' at the beginning.")]
        public string m_sessionsDirectory = "Assets/Gaia User Data/Sessions";
        //[Tooltip("Path for terrain Texture layer storage. Must include 'Assets\' at the beginning.")]
        //public string m_terrainLayersDirectory = "Assets/Gaia User Data/Terrain Layers";
        [Tooltip("Path for general settings files (e.g Stamper Settings, Mask export settings, etc.) Must include 'Assets\' at the beginning.")]
        public string m_userSettingsDirectory = "Assets/Gaia User Data/Settings";



        [Header("Stamper Settings")]
        [Tooltip("The color in which positive height changes are displayed in the stamp preview")]
        public Color m_stamperPositiveHeightChangeColor = new Color(0.5611f, 0.9716f, 0.5362f, 1f);
        [Tooltip("The color in which negative height changes are displayed in the stamp preview.")]
        public Color m_stamperNegativeHeightChangeColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        [Tooltip("The color for the sea level tinting for the stamp preview.")]
        public Color m_stamperSeaLevelTintColor = new Color(0f, 0.2151f, 0.7843f, 0.5f);
        [Tooltip("The normal map influence for the stamp preview. (Can make the stamp shape easier to read)")]
        [Range(0.0f, 1.0f)]
        public float m_stamperNormalMapColorPower = 0.3f;
        [Tooltip("Time in milliseconds to hide the stamp preview after stamping. Set to 0 for keeping it off after stamping, Set to 1 to immediately reactivate after stamping.")]
        public long m_stamperAutoHidePreviewMilliseconds = 2000;
        [Tooltip("Time in milliseconds to wait until triggering the automatic texture spawning in the stamper.")]
        public long m_autoTextureTreshold = 250;
        [Tooltip("Default Stamp that is loaded in when creating a new stamper.")]
        public Texture2D m_defaultStamp;



        [Header("Spawner Settings")]
        [Tooltip("If enabled will hide all objects in the 'Gaia Game Object Spawns'")]
        public bool m_hideObjectsInHierachy = false;
        [Tooltip("The color spectrum from which the spawner chooses its preset visualisation colors.")]
        public Gradient m_spawnerColorGradient = new Gradient() {
                                                                    colorKeys = new GradientColorKey[] {
                                                                                                        new GradientColorKey(new Color(0.7686275f,0.3960784f,0.3921569f),0),
                                                                                                        new GradientColorKey(new Color(0.9411765f,0.9137255f,0.6f),0.25f),
                                                                                                        new GradientColorKey(new Color(0.7215686f,0.7882353f,0.6156863f),0.5f),
                                                                                                        new GradientColorKey(new Color(0.6078432f,0.4470588f,0.4352941f),0.75f),
                                                                                                        new GradientColorKey(new Color(0.9333333f,0.6941177f,0.3568628f),1.0f),
                                                                                                        }
                                                                 };
        [Tooltip("Time in milliseconds to hide the spawner preview after spawning. Set to 0 for keeping it off after spawning, Set to 1 to immediately reactivate after spawning.")]
        public long m_spawnerAutoHidePreviewMilliseconds = 2000;

        [Tooltip("The range that is used for spawners in world spawn mode to iterate over the World. Higher = faster spawns, but also more RAM consumption during spawning.")]
        public int m_spawnerWorldSpawnRange = 1024;

        [Header("Publisher Settings")]
        [Tooltip("Publisher name for exported extensions.")]
        public string m_publisherName = "";
        [Tooltip("Default prefab name for the first person player object.")]
        public string m_fpsPlayerPrefabName = "FPSController";
        [Tooltip("Default prefab name for the third person player object.")]
        public string m_3pPlayerPrefabName = "ThirdPersonController";
        [Tooltip("Default prefab name for the roller ball player object.")]
        public string m_rbPlayerPrefabName = "RollerBall";
        [Tooltip("Default prefab name for the light weight water object.")]
        public string m_waterMobilePrefabName = "WaterBasicDaytime";
        [Tooltip("Default prefab name for the water object.")]
        public string m_waterPrefabName = "Water4Advanced";
        [Tooltip("Show or hide tooltips in all custom editors.")]
        public bool m_showTooltips = true;

        [Header("Alternative Configurations")]
        [Tooltip("Ultra light defaults object.")]
        public GaiaDefaults m_ultraLightDefaults;
        [Tooltip("Ultra light resources object.")]
        public GaiaResource m_ultraLightResources;
        [Tooltip("Ultra light gameobject resources object.")]
        public GaiaResource m_ultraLightGameObjectResources;
        [Tooltip("Mobile defaults object.")]
        public GaiaDefaults m_mobileDefaults;
        [Tooltip("Mobile resources object.")]
        public GaiaResource m_mobileResources;
        [Tooltip("Mobile game object resources object.")]
        public GaiaResource m_mobileGameObjectResources;
        [Tooltip("Desktop defaults object.")]
        public GaiaDefaults m_desktopDefaults;
        [Tooltip("Desktop resources object.")]
        public GaiaResource m_desktopResources;
        [Tooltip("Desktop game object resources object.")]
        public GaiaResource m_desktopGameObjectResources;
        [Tooltip("Powerful desktop defaults object.")]
        public GaiaDefaults m_powerDesktopDefaults;
        [Tooltip("Powerful desktop resources object.")]
        public GaiaResource m_powerDesktopResources;
        [Tooltip("Powerful desktop resources object.")]
        public GaiaResource m_powerDesktopGameObjectResources;

        [Header("Skies and Water Profiles")]
        public GaiaLightingProfile m_gaiaLightingProfile;
        public GaiaWaterProfile m_gaiaWaterProfile;
        public int m_selectedWaterProfile;

        [Header("Pipeline Profile")]
        public string m_gaiaProURL = "GaiaProURL";
        public UnityPipelineProfile m_pipelineProfile;

        [Header("Scene GUI Config")]
        public HeightMaskType m_terrainOpListPositionType = HeightMaskType.Absolute;
        public Vector2 m_terrainOpListPosition;
        public HeightMaskType m_terrainOpListSizeType = HeightMaskType.Absolute;
        public Vector2 m_terrainOpListSize;

        public HeightMaskType m_gaiaPanelPositionType = HeightMaskType.Absolute;
        public Vector2 m_gaiaPanelPosition;
        public HeightMaskType m_gaiaPanelSizeType = HeightMaskType.Absolute;
        public Vector2 m_gaiaPanelSize;

        [Header("News")]
        public long m_lastWebUpdate = 0;
        public bool m_hideHeroMessage = false;
        public string m_latestNewsTitle = "Latest News";
        public string m_latestNewsBody = "Here is the news";
        public string m_latestNewsUrl = "http://www.procedural-worlds.com/blog/";
        public Texture2D m_latestNewsImage;

        [Header("Misc UI Textures")]
        public Texture2D m_originUIProBackgroundPro;
        public Texture2D m_originUIBackground;
        public Texture2D m_originUIUnfoldUp;
        public Texture2D m_originUIUnfoldDown;
        public Texture2D m_originUIProUnfoldUp;
        public Texture2D m_originUIProUnfoldDown;


        [Header("Icons")]
        public Texture2D m_IconCancel;
        public Texture2D m_IconCopy;
        public Texture2D m_IconDown;
        public Texture2D m_IconUp;
        public Texture2D m_IconDuplicate;
        public Texture2D m_IconPaste;
        public Texture2D m_IconRemove;
        public Texture2D m_IconVisible;

        [Header("Pro Skin Icons")]
        public Texture2D m_IconProCancel;
        public Texture2D m_IconProCopy;
        public Texture2D m_IconProDown;
        public Texture2D m_IconProUp;
        public Texture2D m_IconProDuplicate;
        public Texture2D m_IconProPaste;
        public Texture2D m_IconProRemove;
        public Texture2D m_IconProVisible;

    }
}