﻿using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using Gaia.Internal;
using PWCommon2;
using System.Collections.Generic;

namespace Gaia
{
    [CustomEditor(typeof(GaiaGlobal))]
    public class GaiaGlobalEditor : PWEditor
    {
        private EditorUtils m_editorUtils;
        private string m_version;
        private GaiaSettings m_gaiaSettings;
        private GaiaGlobal m_profile;
        private GaiaLightingProfileValues m_profileValues;
        private List<string> LightingList = new List<string>();
        private List<string> WaterList = new List<string>();
        private GaiaLightingProfile m_lightingProfile;
        private GaiaWaterProfile m_waterProfile;
        private GUIStyle dropdownGUIStyle;
        private GaiaConstants.TimeOfDayStartingMode m_currentSelectedTOD;

        private void OnEnable()
        {
            //Get Gaia Lighting Profile object
            m_profile = (GaiaGlobal)target;

            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }

            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
            }

            if (m_gaiaSettings != null)
            {
                m_lightingProfile = m_gaiaSettings.m_gaiaLightingProfile;
                m_waterProfile = m_gaiaSettings.m_gaiaWaterProfile;
            }

            #if GAIA_PRO_PRESENT
            if (ProceduralWorldsGlobalWeather.Instance != null)
            {
                LoadSettings();
            }
#endif

            CreateArrayLists();

            if (dropdownGUIStyle == null)
            {
                dropdownGUIStyle = new GUIStyle(EditorStyles.popup);
                dropdownGUIStyle.fixedHeight = 16f;
                dropdownGUIStyle.margin = new RectOffset(0, 0, 4, 0);
            }

            m_version = PWApp.CONF.Version;
        }

        public override void OnInspectorGUI()
        {
            if (dropdownGUIStyle == null)
            {
                dropdownGUIStyle = new GUIStyle(EditorStyles.popup)
                {
                    fixedHeight = 16f, margin = new RectOffset(0, 0, 4, 0)
                };
            }

            if (m_profile != null)
            {
                Transform transform = m_profile.gameObject.transform;
                transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
            }

            //Initialization
            m_editorUtils.Initialize(); // Do not remove this!
            if (m_gaiaSettings == null)
            {
                m_gaiaSettings = GaiaUtils.GetGaiaSettings();
                m_lightingProfile = m_gaiaSettings.m_gaiaLightingProfile;
                m_waterProfile = m_gaiaSettings.m_gaiaWaterProfile;
            }

            EditorGUILayout.LabelField("Profile Version: " + m_version);
            m_editorUtils.Panel("GlobalSettings", GlobalSettingsPanel, true);
        }

        private void GlobalSettingsPanel(bool helpEnabled)
        {
            bool updateLighting = false;
            int currentLightProfile = m_lightingProfile.m_selectedLightingProfileValuesIndex;
            bool updateWater = false;
            int currentWaterProfile = m_gaiaSettings.m_selectedWaterProfile;
            if (Application.isPlaying)
            {
                LoadFromApplicationPlaying();
            }

            //Monitor for changes
            EditorGUI.BeginChangeCheck();

            if (GaiaProUtils.IsGaiaPro())
            {
                m_editorUtils.Heading("ProfileSelection");
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                currentLightProfile = EditorGUILayout.Popup("Lighting Profiles", currentLightProfile, LightingList.ToArray(), dropdownGUIStyle);
                if (m_editorUtils.Button("Edit", GUILayout.Width(50f)))
                {
                    GaiaLighting.FocusGaiaLightingProfile();
                }
                EditorGUILayout.EndHorizontal();
                if (currentLightProfile != m_lightingProfile.m_selectedLightingProfileValuesIndex)
                {
                    updateLighting = true;
                    m_lightingProfile.m_selectedLightingProfileValuesIndex = currentLightProfile;
                }

                EditorGUILayout.BeginHorizontal();
                currentWaterProfile = EditorGUILayout.Popup("Water Profiles", currentWaterProfile, WaterList.ToArray(), dropdownGUIStyle);
                if (m_editorUtils.Button("Edit", GUILayout.Width(50f)))
                {
                    GaiaUtils.FocusWaterProfile();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                if (currentWaterProfile != m_gaiaSettings.m_selectedWaterProfile)
                {
                    updateWater = true;
                    m_gaiaSettings.m_selectedWaterProfile = currentWaterProfile;
                }
                EditorGUILayout.Space();
#if GAIA_PRO_PRESENT
                if (ProceduralWorldsGlobalWeather.Instance != null)
                {
                    m_editorUtils.Heading("TimeOfDay");
                    EditorGUI.indentLevel++;
                    m_profile.GaiaTimeOfDayValue.m_todStartingType = (GaiaConstants.TimeOfDayStartingMode)m_editorUtils.EnumPopup("StartingTOD", m_profile.GaiaTimeOfDayValue.m_todStartingType, helpEnabled);
                    if (m_profile.GaiaTimeOfDayValue.m_todStartingType != m_currentSelectedTOD)
                    {
                        m_currentSelectedTOD = m_profile.GaiaTimeOfDayValue.m_todStartingType;
                        m_profile.UpdateTimeOfDayMode(m_currentSelectedTOD, false);
                        ProceduralWorldsGlobalWeather.Instance.UpdateAllSystems(false);
                        GaiaGlobal.Instance.UpdateGaiaTimeOfDay(false);
                    }
                    m_profile.GaiaTimeOfDayValue.m_todEnabled = m_editorUtils.Toggle("TODEnable", m_profile.GaiaTimeOfDayValue.m_todEnabled, helpEnabled);
                    if (m_profile.GaiaTimeOfDayValue.m_todEnabled)
                    {
                        EditorGUI.indentLevel++;
                        m_profile.GaiaTimeOfDayValue.m_todDayTimeScale = m_editorUtils.Slider("TODScale", m_profile.GaiaTimeOfDayValue.m_todDayTimeScale, 0f, 500f, helpEnabled);
                        EditorGUI.indentLevel--;
                    }
                    m_profile.GaiaTimeOfDayValue.m_todHour = m_editorUtils.IntSlider("TODHour", m_profile.GaiaTimeOfDayValue.m_todHour, 0, 23, helpEnabled);
                    if (m_profile.GaiaTimeOfDayValue.m_todHour > 23)
                    {
                        m_profile.GaiaTimeOfDayValue.m_todHour = 0;
                    }
                    m_profile.GaiaTimeOfDayValue.m_todMinutes = m_editorUtils.Slider("TODMinutes", m_profile.GaiaTimeOfDayValue.m_todMinutes, 0f, 59.9f, helpEnabled);
                    if (m_profile.GaiaTimeOfDayValue.m_todMinutes > 59.9f)
                    {
                        m_profile.GaiaTimeOfDayValue.m_todMinutes = 0f;
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();

                    m_editorUtils.Heading("Weather");
                    EditorGUI.indentLevel++;
                    if (ProceduralWorldsGlobalWeather.Instance.EnableSnow)
                    {
                        EditorGUILayout.LabelField("Snow Chance " + Mathf.RoundToInt(ProceduralWorldsGlobalWeather.Instance.m_snowSampleStrength * 100) + "%");
                        EditorGUI.indentLevel++;
                        ProceduralWorldsGlobalWeather.Instance.SnowHeight = m_editorUtils.FloatField("SnowHeight", ProceduralWorldsGlobalWeather.Instance.SnowHeight, helpEnabled);
                        EditorGUI.indentLevel--;
                    }
                    if (ProceduralWorldsGlobalWeather.Instance.EnableRain)
                    {
                        EditorGUILayout.LabelField("Rain Chance " + Mathf.RoundToInt(ProceduralWorldsGlobalWeather.Instance.m_rainSampleStrength * 100) + "%");
                    }
                    m_profile.GaiaWeather.m_season = m_editorUtils.Slider("SeasonAmount", m_profile.GaiaWeather.m_season, 0f, 3.9999f, helpEnabled);
                    EditorGUI.indentLevel++;
                    if (m_profile.GaiaWeather.m_season < 1f)
                    {
                        EditorGUILayout.LabelField(string.Format("{0:0}% Winter {1:0}% Spring", (1f - m_profile.GaiaWeather.m_season) * 100f, m_profile.GaiaWeather.m_season * 100f));
                    }
                    else if (m_profile.GaiaWeather.m_season < 2f)
                    {
                        EditorGUILayout.LabelField(string.Format("{0:0}% Spring {1:0}% Summer", (2f - m_profile.GaiaWeather.m_season) * 100f, (m_profile.GaiaWeather.m_season - 1f) * 100f));
                    }
                    else if (m_profile.GaiaWeather.m_season < 3f)
                    {
                        EditorGUILayout.LabelField(string.Format("{0:0}% Summer {1:0}% Autumn", (3f - m_profile.GaiaWeather.m_season) * 100f, (m_profile.GaiaWeather.m_season - 2f) * 100f));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(string.Format("{0:0}% Autumn {1:0}% Winter", (4f - m_profile.GaiaWeather.m_season) * 100f, (m_profile.GaiaWeather.m_season - 3f) * 100f));
                    }
                    EditorGUI.indentLevel--;

                    EditorGUILayout.BeginHorizontal();
                    m_profile.GaiaWeather.m_windDirection = m_editorUtils.Slider("WindDirection", m_profile.GaiaWeather.m_windDirection, 0f, 1f);
                    if (m_profile.GaiaWeather.m_windDirection < 0.25f || m_profile.GaiaWeather.m_windDirection == 1)
                    {
                        EditorGUILayout.LabelField("N ", GUILayout.Width(30f));
                    }
                    else if (m_profile.GaiaWeather.m_windDirection > 0.25f && m_profile.GaiaWeather.m_windDirection < 0.5f)
                    {
                        EditorGUILayout.LabelField("E ", GUILayout.Width(30f));
                    }
                    else if (m_profile.GaiaWeather.m_windDirection > 0.5 && m_profile.GaiaWeather.m_windDirection < 0.75f)
                    {
                        EditorGUILayout.LabelField("S ", GUILayout.Width(30f));
                    }
                    else
                    {
                        EditorGUILayout.LabelField("W ", GUILayout.Width(30f));
                    }
                    EditorGUILayout.EndHorizontal();
                    m_editorUtils.InlineHelp("WindDirection", helpEnabled);
                    EditorGUI.indentLevel--;

                    if (!Application.isPlaying)
                    {
                        GUI.enabled = false;
                    }
                    if (ProceduralWorldsGlobalWeather.Instance.m_renderPipeline == GaiaConstants.EnvironmentRenderer.HighDefinition)
                    {
                        GUI.enabled = false;
                    }

                    EditorGUILayout.BeginHorizontal();
                    if (ProceduralWorldsGlobalWeather.Instance.IsRaining)
                    {
                        if (GUILayout.Button("Stop Rain"))
                        {
                            ProceduralWorldsGlobalWeather.Instance.StopRain();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Start Rain"))
                        {
                            ProceduralWorldsGlobalWeather.Instance.PlayRain();
                        }
                    }

                    if (ProceduralWorldsGlobalWeather.Instance.IsSnowing)
                    {
                        if (GUILayout.Button("Stop Snow"))
                        {
                            ProceduralWorldsGlobalWeather.Instance.StopSnow();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Start Snow"))
                        {
                            ProceduralWorldsGlobalWeather.Instance.PlaySnow();
                        }
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();

                    if (!Application.isPlaying)
                    {
                        if (ProceduralWorldsGlobalWeather.Instance.m_renderPipeline != GaiaConstants.EnvironmentRenderer.HighDefinition)
                        {
                            EditorGUILayout.HelpBox("To use 'Start and Stop' buttons please press play. These buttons are only avaliable when the application is running.", MessageType.Info);
                        }
                    }
                    else
                    {
                        //EditorGUILayout.LabelField("Snow active height is " + Mathf.Round(ProceduralWorldsGlobalWeather.Instance.SnowHeight) + " your current height is " + Mathf.Round(ProceduralWorldsGlobalWeather.Instance.m_player.transform.position.y), EditorStyles.boldLabel);
                    }

                    if (ProceduralWorldsGlobalWeather.Instance.m_renderPipeline == GaiaConstants.EnvironmentRenderer.HighDefinition)
                    {
                        EditorGUILayout.HelpBox("Rain and Snow is not avaliable for HDRP, this feature will be available soon.", MessageType.Info);
                    }
                }
#endif
            }
            else
            {
                m_editorUtils.Heading("ProfileSelection");
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                currentLightProfile = EditorGUILayout.Popup("Lighting Profiles", currentLightProfile, LightingList.ToArray(), dropdownGUIStyle);
                if (m_editorUtils.Button("Edit", GUILayout.Width(50f)))
                {
                    GaiaLighting.FocusGaiaLightingProfile();
                }
                EditorGUILayout.EndHorizontal();
                if (currentLightProfile != m_lightingProfile.m_selectedLightingProfileValuesIndex)
                {
                    if (m_lightingProfile.m_lightingProfiles[currentLightProfile].m_typeOfLighting != "Procedural Worlds Sky")
                    {
                        updateLighting = true;
                    }
                    m_lightingProfile.m_selectedLightingProfileValuesIndex = currentLightProfile;
                }

                EditorGUILayout.BeginHorizontal();
                currentWaterProfile = EditorGUILayout.Popup("Water Profiles", currentWaterProfile, WaterList.ToArray(), dropdownGUIStyle);
                if (m_editorUtils.Button("Edit", GUILayout.Width(50f)))
                {
                    GaiaUtils.FocusWaterProfile();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                if (currentWaterProfile != m_gaiaSettings.m_selectedWaterProfile)
                {
                    updateWater = true;
                    m_gaiaSettings.m_selectedWaterProfile = currentWaterProfile;
                }

                if (m_lightingProfile.m_lightingProfiles[m_lightingProfile.m_selectedLightingProfileValuesIndex].m_profileType == GaiaConstants.GaiaLightingProfileType.ProceduralWorldsSky)
                {
                    EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("GaiaProInfo"), MessageType.Info);

                    bool currentGUIState = GUI.enabled;
                    GUI.enabled = false;
                    m_editorUtils.Heading("TimeOfDay");
                    EditorGUI.indentLevel++;
                    var fake = (GaiaConstants.TimeOfDayStartingMode)m_editorUtils.EnumPopup("StartingTOD", m_profile.GaiaTimeOfDayValue.m_todStartingType, helpEnabled);
                    m_editorUtils.Toggle("TODEnable", true, helpEnabled);
                    EditorGUI.indentLevel++;
                    m_profile.GaiaTimeOfDayValue.m_todDayTimeScale = m_editorUtils.Slider("TODScale", 25f, 0f, 500f, helpEnabled);
                    EditorGUI.indentLevel--;
                    var fake2 = m_editorUtils.IntSlider("TODHour", 12, 0, 23, helpEnabled);
                    var fake3 = m_editorUtils.Slider("TODMinutes", 30, 0f, 59.9f, helpEnabled);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();

                    m_editorUtils.Heading("Weather");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Snow Chance " + Mathf.RoundToInt(0.6f * 100) + "%");
                    EditorGUI.indentLevel++;
                    var fake4 = m_editorUtils.FloatField("SnowHeight", 150, helpEnabled);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.LabelField("Rain Chance " + Mathf.RoundToInt(0.3f * 100) + "%");
                    var fake5 = m_editorUtils.Slider("SeasonAmount", 2.5f, 0f, 3.9999f, helpEnabled);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(string.Format("{0:0}% Spring {1:0}% Summer", 0.25 * 100f, 0.75f * 100f));
                    EditorGUI.indentLevel--;

                    EditorGUILayout.BeginHorizontal();
                    var fake6 = m_editorUtils.Slider("WindDirection", 0.3f, 0f, 1f);
                    if (m_profile.GaiaWeather.m_windDirection < 0.25f || m_profile.GaiaWeather.m_windDirection == 1)
                    {
                        EditorGUILayout.LabelField("N ", GUILayout.Width(30f));
                    }
                    else if (m_profile.GaiaWeather.m_windDirection > 0.25f && m_profile.GaiaWeather.m_windDirection < 0.5f)
                    {
                        EditorGUILayout.LabelField("E ", GUILayout.Width(30f));
                    }
                    else if (m_profile.GaiaWeather.m_windDirection > 0.5 && m_profile.GaiaWeather.m_windDirection < 0.75f)
                    {
                        EditorGUILayout.LabelField("S ", GUILayout.Width(30f));
                    }
                    else
                    {
                        EditorGUILayout.LabelField("W ", GUILayout.Width(30f));
                    }
                    EditorGUILayout.EndHorizontal();
                    m_editorUtils.InlineHelp("WindDirection", helpEnabled);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Start Rain"))
                    {

                    }
                    if (GUILayout.Button("Start Snow"))
                    {
                    }
                    EditorGUILayout.EndHorizontal();

                    GUI.enabled = currentGUIState;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_profile);
                Undo.RecordObject(m_profile, "Changes Made");

                m_profile.UpdateGaiaTimeOfDay(false);
                m_profile.UpdateGaiaWeather();

                //Update Profiles
                if (updateLighting)
                {
                    GaiaLighting.GetProfile(m_lightingProfile, m_gaiaSettings.m_pipelineProfile, m_gaiaSettings.m_pipelineProfile.m_activePipelineInstalled);
                }

                if (updateWater)
                {
                    if (m_profile.WaterMaterial == null)
                    {
                        m_profile.WaterMaterial = GaiaWater.GetGaiaOceanMaterial();
                    }
                    GaiaWater.GetProfile(m_gaiaSettings.m_selectedWaterProfile, m_profile.WaterMaterial, m_waterProfile, m_gaiaSettings.m_pipelineProfile.m_activePipelineInstalled, true, false);
                }
            }
        }

        private void LoadSettings()
        {
#if GAIA_PRO_PRESENT
            if (ProceduralWorldsGlobalWeather.Instance != null)
            {
                //Time Of Day
                m_currentSelectedTOD = m_profile.GaiaTimeOfDayValue.m_todStartingType;

                //Weather
                m_profile.GaiaWeather.m_season = ProceduralWorldsGlobalWeather.Instance.Season;
                m_profile.GaiaWeather.m_windDirection = ProceduralWorldsGlobalWeather.Instance.WindDirection;

                if (m_profile.GaiaTimeOfDayValue.m_todHour >= 6 && m_profile.GaiaTimeOfDayValue.m_todHour < 10)
                {
                    m_currentSelectedTOD = GaiaConstants.TimeOfDayStartingMode.Morning;
                    m_profile.GaiaTimeOfDayValue.m_todStartingType = GaiaConstants.TimeOfDayStartingMode.Morning;
                }
                else if (m_profile.GaiaTimeOfDayValue.m_todHour >= 11 && m_profile.GaiaTimeOfDayValue.m_todHour < 17)
                {
                    m_currentSelectedTOD = GaiaConstants.TimeOfDayStartingMode.Day;
                    m_profile.GaiaTimeOfDayValue.m_todStartingType = GaiaConstants.TimeOfDayStartingMode.Day;
                }
                else if (m_profile.GaiaTimeOfDayValue.m_todHour >= 17 && m_profile.GaiaTimeOfDayValue.m_todHour < 20)
                {
                    m_currentSelectedTOD = GaiaConstants.TimeOfDayStartingMode.Evening;
                    m_profile.GaiaTimeOfDayValue.m_todStartingType = GaiaConstants.TimeOfDayStartingMode.Evening;
                }
                else if (m_profile.GaiaTimeOfDayValue.m_todHour >= 20 && m_profile.GaiaTimeOfDayValue.m_todHour != 6)
                {
                    m_currentSelectedTOD = GaiaConstants.TimeOfDayStartingMode.Night;
                    m_profile.GaiaTimeOfDayValue.m_todStartingType = GaiaConstants.TimeOfDayStartingMode.Night;
                }
            }
#endif
        }

        private void LoadFromApplicationPlaying()
        {
#if GAIA_PRO_PRESENT
            if (ProceduralWorldsGlobalWeather.Instance != null)
            {
                //Weather
                m_profile.GaiaWeather.m_season = ProceduralWorldsGlobalWeather.Instance.Season;
                m_profile.GaiaWeather.m_windDirection = ProceduralWorldsGlobalWeather.Instance.WindDirection;
            }
#endif
        }

        private void CreateArrayLists()
        {
            LightingList.Clear();
            foreach (GaiaLightingProfileValues gaiaLighting in m_lightingProfile.m_lightingProfiles)
            {
                LightingList.Add(gaiaLighting.m_typeOfLighting);
            }

            WaterList.Clear();
            foreach (GaiaWaterProfileValues gaiaWater in m_waterProfile.m_waterProfiles)
            {
                WaterList.Add(gaiaWater.m_typeOfWater);
            }
        }
    }
}