using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using PWCommon2;
using Gaia.Internal;
using Gaia;

namespace ProcedualWorlds.WaterSystem
{
    /// <summary>
    /// Editor for the PWS_WaterReflections
    /// </summary>
    [CustomEditor(typeof(PWS_WaterSystem))]
    public class PWS_WaterReflectionsEditor : PWEditor
    {
        private EditorUtils m_editorUtils;
        private PWS_WaterSystem WaterReflections;

        private void OnEnable()
        {
            WaterReflections = (PWS_WaterSystem)target;

            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }
        }

        #region Inspector Region

        /// <summary>
        /// Custom editor for PWS_WaterReflections
        /// </summary
        public override void OnInspectorGUI()
        {
            //Initialization
            m_editorUtils.Initialize(); // Do not remove this!

            if (WaterReflections == null)
            {
                WaterReflections = (PWS_WaterSystem)target;
            }

            m_editorUtils.Panel("GlobalSettings", GlobalSettings, true);
        }

        #endregion

        #region Panel

        /// <summary>
        /// Global Main Panel
        /// </summary>
        /// <param name="helpEnabled"></param>
        private void GlobalSettings(bool helpEnabled)
        {
            if (m_editorUtils.Button("EditReflectionSettings"))
            {
                GaiaUtils.FocusWaterProfile();
            }

            /*
            EditorGUI.BeginChangeCheck();

            m_editorUtils.Heading("ReflectionSetup");
            EditorGUILayout.BeginHorizontal();
            WaterReflections.m_reflectionLayers = GaiaEditorUtils.LayerMaskField(new GUIContent(m_editorUtils.GetTextValue("ReflectionLayers"), m_editorUtils.GetTooltip("ReflectionLayers")), WaterReflections.m_reflectionLayers);
            EditorGUILayout.EndHorizontal();
            m_editorUtils.InlineHelp("ReflectionLayers", helpEnabled);

            WaterReflections.m_RenderUpdate = (PWS_WaterSystem.RenderUpdateMode)m_editorUtils.EnumPopup("UpdateMode", WaterReflections.m_RenderUpdate, helpEnabled);
            if (WaterReflections.m_RenderUpdate == PWS_WaterSystem.RenderUpdateMode.Interval)
            {
                WaterReflections.m_updateThreshold = m_editorUtils.Slider("UpdateInterval", WaterReflections.m_updateThreshold, 0.01f, 50);
            }
            else
            {
                if (WaterReflections.m_RenderUpdate != PWS_WaterSystem.RenderUpdateMode.OnEnable)
                {
                    WaterReflections.m_useRefreshTime = m_editorUtils.Toggle("ModifyRefreshRate", WaterReflections.m_useRefreshTime);
                    if (WaterReflections.m_useRefreshTime)
                    {
                        WaterReflections.m_refreshRate = m_editorUtils.Slider("RefreshRate", WaterReflections.m_refreshRate, 0.001f, 1);
                    }
                }
            }
            WaterReflections.m_MSAA = m_editorUtils.Toggle("AllowMSAA", WaterReflections.m_MSAA, helpEnabled);
            WaterReflections.m_HDR = m_editorUtils.Toggle("UseHDR", WaterReflections.m_HDR, helpEnabled);
            WaterReflections.m_enableDisabeHeightFeature = m_editorUtils.Toggle("EnableHeightFeatures", WaterReflections.m_enableDisabeHeightFeature, helpEnabled);
            if (WaterReflections.m_enableDisabeHeightFeature)
            {
                EditorGUI.indentLevel++;
                WaterReflections.m_disableHeight = m_editorUtils.FloatField("DisableHeight", WaterReflections.m_disableHeight, helpEnabled);
                EditorGUI.indentLevel--;
            }
            WaterReflections.m_renderTextureSize = m_editorUtils.IntField("RenderTextureSize", WaterReflections.m_renderTextureSize, helpEnabled);
            EditorGUILayout.Space();

            m_editorUtils.Heading("ReflectionRendering");
            WaterReflections.m_skyboxOnly = m_editorUtils.Toggle("RenderOnlySkybox", WaterReflections.m_skyboxOnly, helpEnabled);
            WaterReflections.m_customReflectionDistance = m_editorUtils.Toggle("CustomReflectionDistance", WaterReflections.m_customReflectionDistance, helpEnabled);
            if (WaterReflections.m_customReflectionDistance)
            {
                WaterReflections.m_renderDistance = m_editorUtils.FloatField("ReflectionDistance", WaterReflections.m_renderDistance, helpEnabled);
            }
            WaterReflections.m_clipPlaneOffset = m_editorUtils.FloatField("ClipPlaneOffset", WaterReflections.m_clipPlaneOffset, helpEnabled);

            if (EditorGUI.EndChangeCheck())
            {
                WaterReflections.Generate();
            }

            */
        }

        #endregion
    }
}
