﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using ProcedualWorlds.HierachySystem;
using static Gaia.GaiaConstants;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Rendering;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
#if UPPipeline
using UnityEngine.Rendering.Universal;
#endif
#if HDPipeline
using UnityEngine.Rendering.HighDefinition;
#endif

namespace Gaia
{
    public static class GaiaPrefabUtility
    {
        public static readonly string m_vfxLayerName = "PW_VFX";
        public static readonly int m_vfxLayerIndex = 8;

        public static readonly string m_objectSmallLayerName = "PW_Object_Small";
        public static readonly int m_objectSmallLayerIndex = 9;

        public static readonly string m_objectMediumLayerName = "PW_Object_Medium";
        public static readonly int m_objectMediumLayerIndex = 10;

        public static readonly string m_objectLargeLayerName = "PW_Object_Large";
        public static readonly int m_objectLargeLayerIndex = 11;
    }

    public class GaiaUtils : MonoBehaviour
    {
        #region Asset directory helpers
        /// <summary>
        /// Get raw gaia asset directory
        /// </summary>
        /// <returns>Base gaia directory</returns>
        //public static string GetGaiaAssetDirectory()
        //{
        //    string path = Path.Combine(Application.dataPath, Gaia.GaiaConstants.AssetDir);
        //    return path.Replace('\\', '/');
        //}

        /// <summary>
        /// Get the asset directory for a particular featiure type
        /// </summary>
        /// <param name="featureType"></param>
        /// <returns>Path of feature type</returns>
        //public static string GetGaiaAssetDirectory(Gaia.GaiaConstants.FeatureType featureType)
        //{
        //    string path = Path.Combine(Application.dataPath, Gaia.GaiaConstants.AssetDir);
        //    path = Path.Combine(path, featureType.ToString());
        //    return path.Replace('\\', '/');
        //}

        /// <summary>
        /// Get a list of the Gaia stamps for the feature type provided
        /// </summary>
        /// <param name="featureType"></param>
        /// <returns></returns>
        public static List<string> GetGaiaStampsList(Gaia.GaiaConstants.FeatureType featureType)
        {
            return new List<string>(System.IO.Directory.GetFiles(GaiaDirectories.GetStampFeatureDirectory(featureType), "*.exr"));
        }



        /// <summary>
        /// Get the full asset path for a specific asset type and name
        /// </summary>
        /// <param name="featureType">The type of feature this asset is</param>
        /// <param name="assetName">The file name of the asset</param>
        /// <returns>Fully qualified path of the asset</returns>
        //public static string GetGaiaAssetPath(Gaia.GaiaConstants.FeatureType featureType, string assetName)
        //{
        //    string path = GetGaiaAssetDirectory(featureType);
        //    path = Path.Combine(GetGaiaAssetDirectory(featureType), assetName);
        //    return path.Replace('\\','/');
        //}

        /// <summary>
        /// Get the full asset path for a specific asset type and name
        /// </summary>
        /// <param name="featureType">The type of feature this asset is</param>
        /// <param name="assetName">The file name of the asset</param>
        /// <returns>Fully qualified path of the asset</returns>
        //public static string GetGaiaStampAssetPath(Gaia.GaiaConstants.FeatureType featureType, string assetName)
        //{
        //    string path = GetGaiaAssetDirectory(featureType);
        //    path = Path.Combine(GetGaiaAssetDirectory(featureType), "Data");
        //    path = Path.Combine(path, assetName);
        //    return path.Replace('\\', '/');
        //}


        /// <summary>
        /// Parse a stamp preview texture to work out where the stamp lives
        /// </summary>
        /// <param name="source">Source texture</param>
        /// <returns></returns>
        public static string GetGaiaStampPath(Texture2D source)
        {
            string path = "";
#if UNITY_EDITOR
            path = UnityEditor.AssetDatabase.GetAssetPath(source);
#endif

            string fileName = Path.GetFileName(path);
            path = Path.Combine(Path.GetDirectoryName(path), "Data");
            path = Path.Combine(path, fileName);
            path = Path.ChangeExtension(path, ".bytes");
            path = path.Replace('\\', '/');
            return path;
        }


        /// <summary>
        /// Helper to load in all terrains managed by placeholders, call a function on them, and then unload them again. Helpful to process changes across the entire game world.
        /// </summary>
        /// <param name="terrainAction">A function that accepts a terrain as 1st parameter</param>
        /// <param name="dirtyScenes">Should scenes be marked as 'dirty' so they will be saved during the process?</param>
        public static void CallFunctionOnDynamicLoadedTerrains(Action<Terrain> terrainAction, bool dirtyScenes, List<string> terrainNames = null)
        {
#if UNITY_EDITOR

            GaiaSessionManager gsm = GaiaSessionManager.GetSessionManager();

            TerrainScene[] allTerrainScenes;
            if (terrainNames == null)
            {
                //no list of names given - go over all terrains
                allTerrainScenes = GaiaTerrainLoaderManager.TerrainScenes.ToArray();
            }
            else
            {
                //filter by list of terrain names
                allTerrainScenes = GaiaTerrainLoaderManager.TerrainScenes.Where(x => terrainNames.Contains(x.GetTerrainName())).ToArray();
            }


            if (GaiaTerrainLoaderManager.TerrainScenes.Count > 0)
            {
                try
                {
                    int count = 1;
                    foreach (TerrainScene terrainScene in allTerrainScenes)
                    {
                        GaiaUtils.DisplayProgressBarNoEditor("Processing Terrains", "Terrain " + count.ToString() + " of " + GaiaTerrainLoaderManager.TerrainScenes.Count.ToString(), (float)count / (float)GaiaTerrainLoaderManager.TerrainScenes.Count);
                        terrainScene.AddReference(gsm.gameObject);
                        terrainScene.UpdateLoadState();
                        Scene scene = EditorSceneManager.GetSceneByPath(terrainScene.m_scenePath);
                        foreach (GameObject go in scene.GetRootGameObjects())
                        {
                            Terrain terrain = go.GetComponent<Terrain>();
                            if (terrain != null)
                            {
                                terrainAction(terrain);
                            }
                        }
                        if (dirtyScenes)
                        {
                            EditorSceneManager.MarkSceneDirty(scene);
                        }
                        terrainScene.RemoveAllReferences();
                        terrainScene.UpdateLoadState();
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error while processing multiple dynamic loaded Terrains, Exception: " + ex.Message + "  Stack Trace: " + ex.StackTrace);
                }
                finally
                {
                    GaiaUtils.ClearProgressBarNoEditor();
                }
            }
#endif
        }




        /// <summary>
        /// Finds a gameobject by name even when it is deactivated.
        /// </summary>
        /// <param name="searchFor">The name of the GO to look for</param>
        /// <param name="fullNameMatch">Whether the name needs to be a full match or if it is sufficient if the name of the GO just contains "searchFor".</param>
        /// <returns></returns>
        public static GameObject FindObjectDeactivated(string searchFor, bool fullNameMatch = true)
        {
            GameObject[] allGOs = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject go in allGOs)
            {
                if (go.name == searchFor || (!fullNameMatch && go.name.Contains(searchFor)))
                {
                    return go;
                }
            }
            return null;
        }

        /// <summary>
        /// Check to see if this actually a valid stamp - needs a .jpg and a .bytes file
        /// </summary>
        /// <param name="source">Source texture</param>
        /// <returns></returns>
        public static bool CheckValidGaiaStampPath(Texture2D source)
        {
            string path = "";
#if UNITY_EDITOR
            path = UnityEditor.AssetDatabase.GetAssetPath(source);
#endif

            //path = GetGaiaAssetDirectory() + path.Replace(Gaia.GaiaConstants.AssetDirFromAssetDB, "");

            // Check to see if we have a jpg file
            if (Path.GetExtension(path).ToLower() != ".jpg")
            {
                return false;
            }

            //Check to see if we have asset file
            string fileName = Path.GetFileName(path);
            path = Path.Combine(Path.GetDirectoryName(path), "Data");
            path = Path.Combine(path, fileName);
            path = Path.ChangeExtension(path, ".bytes");
            path = path.Replace('\\', '/');

            if (System.IO.File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// allows non-editor classes to display a progress bar which will not create a conflict on build
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="progress"></param>
        public static void DisplayProgressBarNoEditor(string title, string text, float progress)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar(title, text, progress);
#endif
        }

        public static float GetTreeRadius(GameObject treePrefab)
        {
            //TreePrototype protoType = terrain.terrainData.treePrototypes[m_treePrototypeId];
            //if (protoType != null)
            //{
            //    GameObject treeGO = protoType.prefab;
            if (treePrefab != null)
            {
                Bounds bounds = GetBounds(treePrefab);
                return (float)Math.Round(Mathf.Max(bounds.extents.x, bounds.extents.z), 2);
            }
            return 5;
            //}
            //else
            //{
            //    return 5;
            //}
        }

        /// <summary>
        /// Create all the Gaia stamp directories for scans to go into
        /// </summary>
        public static void CreateGaiaStampDirectories()
        {
#if UNITY_EDITOR
            string path = GaiaDirectories.GetStampDirectory();
            try
            {
                bool addedDir = false;
                foreach (Gaia.GaiaConstants.FeatureType feature in Enum.GetValues(typeof(Gaia.GaiaConstants.FeatureType)))
                {
                    path = GaiaDirectories.GetStampFeatureDirectory(feature);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        path = Path.Combine(path, "Data");
                        Directory.CreateDirectory(path);
                        addedDir = true;
                    }
                }

                if (addedDir)
                {
                    AssetDatabase.Refresh();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Failed to create directory {0} : {1}", path, e.Message));
            }
#endif
        }

        public static float GetBoundsForTaggedObject(string tag)
        {
            try
            {
                var allGOsWithTag = GameObject.FindGameObjectsWithTag(tag);
                Bounds bounds = GetBounds(allGOsWithTag[0]);
                return (float)Math.Round(Mathf.Max(bounds.extents.x, bounds.extents.z), 2);
            }
            catch { }

            return 5f;
        }

        /// <summary>
        /// Clears a progress bar from a class outside the editor namespace
        /// </summary>
        public static void ClearProgressBarNoEditor()
        {
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
        }

        /// <summary>
        /// Get all objects of the given type at the location in the path. Only works in the editor.
        /// </summary>
        /// <typeparam name="T">Type of object to load</typeparam>
        /// <param name="path">The path to look in</param>
        /// <returns>List of those objects</returns>
        public static T[] GetAtPath<T>(string path)
        {

            ArrayList al = new ArrayList();

#if UNITY_EDITOR

            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + path);
            foreach (string fileName in fileEntries)
            {
                int index = fileName.LastIndexOf("/");
                string localPath = "Assets/" + path;

                if (index > 0)
                    localPath += fileName.Substring(index);

                UnityEngine.Object t = UnityEditor.AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

                if (t != null)
                    al.Add(t);
            }

#endif

            T[] result = new T[al.Count];
            for (int i = 0; i < al.Count; i++)
                result[i] = (T)al[i];

            return result;
        }



        #endregion

        #region Asset, Scriptable Object, GameObject helpers

        /// <summary>
        /// Gets and returns the particle renderer material
        /// </summary>
        /// <param name="checkSelf"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static Material GetParticleMaterial(GameObject selfObject)
        {
            Material material = null;
            if (selfObject != null)
            {
                ParticleSystemRenderer systemRenderer = selfObject.GetComponent<ParticleSystemRenderer>();
                if (systemRenderer != null)
                {
                    material = systemRenderer.sharedMaterial;
                }
            }

            return material;
        }

        /// <summary>
        /// Gets and returns the particle renderer material
        /// </summary>
        /// <param name="checkSelf"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static Material GetParticleMaterial(string objectName)
        {
            Material material = null;
            if (objectName.Length > 0)
            {
                GameObject particleObject = GameObject.Find(objectName);
                if (particleObject != null)
                {
                    ParticleSystemRenderer systemRenderer = particleObject.GetComponent<ParticleSystemRenderer>();
                    if (systemRenderer != null)
                    {
                        material = systemRenderer.sharedMaterial;
                    }
                }
            }

            return material;
        }

        /// <summary>
        /// Gets the cloud materials from the scene
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static List<Material> GetCloudLayerMaterials(string objectName, string ignoreContainName)
        {
            List<Material> materials = new List<Material>();
            if (objectName.Length > 0)
            {
                GameObject cloudObject = GameObject.Find(objectName);
                if (cloudObject != null)
                {
                    MeshRenderer[] meshRenderers = cloudObject.GetComponentsInChildren<MeshRenderer>();
                    if (meshRenderers.Length > 0)
                    {
                        foreach (MeshRenderer renderer in meshRenderers)
                        {
                            if (ignoreContainName.Length > 0)
                            {
                                if (!renderer.name.Contains(ignoreContainName))
                                {
                                    materials.Add(renderer.sharedMaterial);
                                }
                            }
                            else
                            {
                                materials.Add(renderer.sharedMaterial);
                            }
                        }
                    }
                }
            }
            return materials;
        }

        /// <summary>
        /// Gets the current installed SRP
        /// </summary>
        /// <returns></returns>
        public static GaiaConstants.EnvironmentRenderer GetActivePipeline()
        {
            GaiaConstants.EnvironmentRenderer renderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
            //Sets up the render to the correct pipeline
            if (GraphicsSettings.renderPipelineAsset == null)
            {
                renderer = GaiaConstants.EnvironmentRenderer.BuiltIn;
            }
            else if (GraphicsSettings.renderPipelineAsset.GetType().ToString().Contains("HDRenderPipelineAsset"))
            {
                renderer = GaiaConstants.EnvironmentRenderer.HighDefinition;
            }
            else if (GraphicsSettings.renderPipelineAsset.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
            {
                renderer = GaiaConstants.EnvironmentRenderer.Universal;
            }
            else
            {
                renderer = GaiaConstants.EnvironmentRenderer.Lightweight;
            }

            return renderer;
        }

        /// <summary>
        /// Gets the global volume profile
        /// </summary>
        /// <returns></returns>
#if HDPipeline || UPPipeline
        public static VolumeProfile GetVolumeProfile(bool isPlaying, string deepSearchName, string doesNotContain)
        {
            VolumeProfile volumeProfile = null;
            Volume[] volumes = FindObjectsOfType<Volume>();
            if (Application.isPlaying)
            {
                isPlaying = true;
            }
            else
            {
                isPlaying = false;
            }

            if (isPlaying)
            {
                if (volumes.Length > 0)
                {
                    foreach (Volume volume in volumes)
                    {
                        if (volume.isGlobal)
                        {
                            if (volume.name.Contains(deepSearchName) && !volume.name.Contains(doesNotContain))
                            {
                                VolumeProfile processProfileInstance = ScriptableObject.CreateInstance<VolumeProfile>();
                                List<VolumeComponent> settings = volume.sharedProfile.components;
                                processProfileInstance.components = settings;
                                volumeProfile = processProfileInstance;
                                volume.sharedProfile = processProfileInstance;
                                volumes = new Volume[0];
                            }
                        }
                    }
                }
            }
            else
            {
                if (volumes.Length > 0)
                {
                    foreach (Volume volume in volumes)
                    {
                        if (volume.isGlobal)
                        {
                            if (volume.name.Contains(deepSearchName) && !volume.name.Contains(doesNotContain))
                            {
                                volumeProfile = volume.sharedProfile;
                                break;
                            }
                        }
                    }
                }
            }

            return volumeProfile;
        }

#if UNITY_POST_PROCESSING_STACK_V2 && UNITY_EDITOR
        public static void CreateURPOrHDRPPostProcessing(GaiaConstants.EnvironmentRenderer renderPipeline, PostProcessProfile profile, string saveLocation)
        {
            if (profile == null)
            {
                Debug.LogError("No Post Process Profile to copy from has been assigned, please assign a profile you wish to copy from");
                return;
            }

            string extentionName = "";

            //V2
#if HDPipeline
            UnityEngine.Rendering.PostProcessing.AmbientOcclusion ambientOcclusion;
#endif
            UnityEngine.Rendering.PostProcessing.Bloom bloom;
            UnityEngine.Rendering.PostProcessing.ChromaticAberration chromaticAberration;
            UnityEngine.Rendering.PostProcessing.ColorGrading colorGrading;
            UnityEngine.Rendering.PostProcessing.DepthOfField depthOfField;
            UnityEngine.Rendering.PostProcessing.Grain grain;
            UnityEngine.Rendering.PostProcessing.LensDistortion lensDistortion;
            UnityEngine.Rendering.PostProcessing.MotionBlur motionBlur;
            UnityEngine.Rendering.PostProcessing.Vignette vignette;
            //URP
#if UPPipeline
            UnityEngine.Rendering.Universal.Bloom URPBloom;
            UnityEngine.Rendering.Universal.ChromaticAberration URPChromaticAberration;
            UnityEngine.Rendering.Universal.ColorAdjustments URPColorAdjustments;
            UnityEngine.Rendering.Universal.WhiteBalance URPWhiteBalance;
            UnityEngine.Rendering.Universal.ChannelMixer URPChannelMixer;
            UnityEngine.Rendering.Universal.Tonemapping URPTonemapping;
            UnityEngine.Rendering.Universal.DepthOfField URPDepthOfField;
            UnityEngine.Rendering.Universal.FilmGrain URPFilmGrain;
            UnityEngine.Rendering.Universal.LensDistortion URPLensDistortion;
            UnityEngine.Rendering.Universal.MotionBlur URPMotionBlur;
            UnityEngine.Rendering.Universal.Vignette URPVignette;
            UnityEngine.Rendering.Universal.SplitToning URPSplitToning;
#endif
            //HDRP
#if HDPipeline
            UnityEngine.Rendering.HighDefinition.AmbientOcclusion HDRPAmbientOcclusion;
            UnityEngine.Rendering.HighDefinition.Bloom HDRPBloom;
            UnityEngine.Rendering.HighDefinition.ChromaticAberration HDRPChromaticAberration;
            UnityEngine.Rendering.HighDefinition.ColorAdjustments HDRPColorAdjustments;
            UnityEngine.Rendering.HighDefinition.WhiteBalance HDRPWhiteBalance;
            UnityEngine.Rendering.HighDefinition.ChannelMixer HDRPChannelMixer;
            UnityEngine.Rendering.HighDefinition.Tonemapping HDRPTonemapping;
            UnityEngine.Rendering.HighDefinition.DepthOfField HDRPDepthOfField;
            UnityEngine.Rendering.HighDefinition.FilmGrain HDRPFilmGrain;
            UnityEngine.Rendering.HighDefinition.LensDistortion HDRPLensDistortion;
            UnityEngine.Rendering.HighDefinition.MotionBlur HDRPMotionBlur;
            UnityEngine.Rendering.HighDefinition.Vignette HDRPVignette;
            UnityEngine.Rendering.HighDefinition.SplitToning HDRPSplitToning;
#endif

            //Configure file
            VolumeProfile volumeProfile = ScriptableObject.CreateInstance<VolumeProfile>();
            if (renderPipeline == EnvironmentRenderer.Universal)
            {
                //Copy settings
                extentionName = "UP ";
#if UPPipeline
                //Bloom
                if (profile.TryGetSettings(out bloom))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.Bloom>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.Bloom>();
                    }
                    if (volumeProfile.TryGet(out URPBloom))
                    {
                        URPBloom.active = true;
                        URPBloom.intensity.value = bloom.intensity;
                        URPBloom.threshold.value = bloom.threshold;
                        URPBloom.tint.value = bloom.color;
                        URPBloom.scatter.value = bloom.softKnee;
                        URPBloom.highQualityFiltering.value = true;
                        URPBloom.dirtTexture.value = bloom.dirtTexture;
                        URPBloom.dirtIntensity.value = bloom.dirtIntensity;
                        URPBloom.SetAllOverridesTo(true);
                    }
                }

                //Chromatic Aberration
                if (profile.TryGetSettings(out chromaticAberration))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.ChromaticAberration>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.ChromaticAberration>();
                    }
                    if (volumeProfile.TryGet(out URPChromaticAberration))
                    {
                        URPChromaticAberration.active = true;
                        URPChromaticAberration.intensity.value = chromaticAberration.intensity;
                        URPChromaticAberration.SetAllOverridesTo(true);
                    }
                }

                //Color Grading
                if (profile.TryGetSettings(out colorGrading))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.ColorAdjustments>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.ColorAdjustments>();
                    }
                    if (volumeProfile.TryGet(out URPColorAdjustments))
                    {
                        URPColorAdjustments.active = true;
                        URPColorAdjustments.postExposure.value = colorGrading.postExposure;
                        URPColorAdjustments.contrast.value = colorGrading.contrast;
                        URPColorAdjustments.colorFilter.value = colorGrading.colorFilter;
                        URPColorAdjustments.hueShift.value = colorGrading.hueShift;
                        URPColorAdjustments.saturation.value = colorGrading.saturation;
                        URPColorAdjustments.SetAllOverridesTo(true);
                    }

                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.WhiteBalance>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.WhiteBalance>();
                    }
                    if (volumeProfile.TryGet(out URPWhiteBalance))
                    {
                        URPWhiteBalance.active = true;
                        URPWhiteBalance.temperature.value = colorGrading.temperature;
                        URPWhiteBalance.tint.value = colorGrading.tint;
                        URPWhiteBalance.SetAllOverridesTo(true);
                    }

                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.ChannelMixer>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.ChannelMixer>();
                    }
                    if (volumeProfile.TryGet(out URPChannelMixer))
                    {
                        URPChannelMixer.active = true;
                        URPChannelMixer.blueOutBlueIn.value = colorGrading.mixerBlueOutBlueIn;
                        URPChannelMixer.blueOutGreenIn.value = colorGrading.mixerBlueOutGreenIn;
                        URPChannelMixer.blueOutRedIn.value = colorGrading.mixerBlueOutRedIn;

                        URPChannelMixer.greenOutBlueIn.value = colorGrading.mixerGreenOutBlueIn;
                        URPChannelMixer.greenOutGreenIn.value = colorGrading.mixerGreenOutGreenIn;
                        URPChannelMixer.greenOutRedIn.value = colorGrading.mixerGreenOutRedIn;

                        URPChannelMixer.redOutBlueIn.value = colorGrading.mixerRedOutBlueIn;
                        URPChannelMixer.redOutGreenIn.value = colorGrading.mixerRedOutGreenIn;
                        URPChannelMixer.redOutRedIn.value = colorGrading.mixerRedOutRedIn;
                        URPChannelMixer.SetAllOverridesTo(true);
                    }

                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.Tonemapping>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.Tonemapping>();
                    }
                    if (volumeProfile.TryGet(out URPTonemapping))
                    {
                        URPTonemapping.active = true;
                        switch (colorGrading.tonemapper.value)
                        {
                            case Tonemapper.None:
                                URPTonemapping.mode.value = TonemappingMode.None;
                                break;
                            case Tonemapper.Neutral:
                                URPTonemapping.mode.value = TonemappingMode.Neutral;
                                break;
                            case Tonemapper.ACES:
                                URPTonemapping.mode.value = TonemappingMode.ACES;
                                break;
                        }
                        URPTonemapping.SetAllOverridesTo(true);
                    }
                }

                //Depth Of Field
                if (profile.TryGetSettings(out depthOfField))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.DepthOfField>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.DepthOfField>();
                    }
                    if (volumeProfile.TryGet(out URPDepthOfField))
                    {
                        URPDepthOfField.active = true;
                        URPDepthOfField.mode.value = DepthOfFieldMode.Bokeh;
                        URPDepthOfField.aperture.value = depthOfField.aperture;
                        URPDepthOfField.focalLength.value = depthOfField.focalLength;
                        URPDepthOfField.focusDistance.value = depthOfField.focusDistance;
                        URPDepthOfField.SetAllOverridesTo(true);
                    }
                }

                //Film Grain
                if (profile.TryGetSettings(out grain))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.FilmGrain>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.FilmGrain>();
                    }
                    if (volumeProfile.TryGet(out URPFilmGrain))
                    {
                        URPFilmGrain.active = true;
                        URPFilmGrain.intensity.value = grain.intensity;
                        URPFilmGrain.SetAllOverridesTo(true);
                    }
                }

                //Lens Distortion
                if (profile.TryGetSettings(out lensDistortion))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.LensDistortion>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.LensDistortion>();
                    }
                    if (volumeProfile.TryGet(out URPLensDistortion))
                    {
                        URPLensDistortion.active = true;
                        Vector2 center = new Vector2(lensDistortion.centerX, lensDistortion.centerY);
                        URPLensDistortion.center.value = center;
                        URPLensDistortion.intensity.value = lensDistortion.intensity;
                        URPLensDistortion.xMultiplier.value = lensDistortion.intensityX;
                        URPLensDistortion.yMultiplier.value = lensDistortion.intensityY;
                        URPLensDistortion.scale.value = lensDistortion.scale;
                        URPLensDistortion.SetAllOverridesTo(true);
                    }
                }

                //Motion Blur
                if (profile.TryGetSettings(out motionBlur))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.MotionBlur>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.MotionBlur>();
                    }
                    if (volumeProfile.TryGet(out URPMotionBlur))
                    {
                        URPMotionBlur.active = true;
                        URPMotionBlur.intensity.value = motionBlur.shutterAngle;
                        URPMotionBlur.SetAllOverridesTo(true);
                    }
                }

                //Vignette
                if (profile.TryGetSettings(out vignette))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.Universal.Vignette>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.Universal.Vignette>();
                    }
                    if (volumeProfile.TryGet(out URPVignette))
                    {
                        URPVignette.active = true;
                        URPVignette.intensity.value = vignette.intensity;
                        URPVignette.rounded.value = vignette.rounded;
                        URPVignette.smoothness.value = vignette.smoothness;
                        URPVignette.SetAllOverridesTo(true);
                    }
                }

                //Split Toning
                if (!volumeProfile.Has<UnityEngine.Rendering.Universal.SplitToning>())
                {
                    volumeProfile.Add<UnityEngine.Rendering.Universal.SplitToning>();
                }
                if (volumeProfile.TryGet(out URPSplitToning))
                {
                    URPSplitToning.active = true;
                    URPSplitToning.shadows.value = GaiaUtils.GetColorFromHTML("636363");
                    URPSplitToning.highlights.value = GaiaUtils.GetColorFromHTML("909090");
                    URPSplitToning.balance.value = -20f;
                    URPSplitToning.SetAllOverridesTo(true);
                }

#endif
            }
            else if (renderPipeline == EnvironmentRenderer.HighDefinition)
            {
                extentionName = "HD ";
#if HDPipeline
                //Copy settings
                //Ambient Occlusion
                if (profile.TryGetSettings(out ambientOcclusion))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.AmbientOcclusion>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.AmbientOcclusion>();
                    }
                    if (volumeProfile.TryGet(out HDRPAmbientOcclusion))
                    {
                        HDRPAmbientOcclusion.active = true;
                        HDRPAmbientOcclusion.intensity.value = ambientOcclusion.intensity;
                        HDRPAmbientOcclusion.directLightingStrength.value = ambientOcclusion.directLightingStrength;
                        HDRPAmbientOcclusion.radius.value = ambientOcclusion.thicknessModifier;
                        HDRPAmbientOcclusion.quality.value = 2;
                        HDRPAmbientOcclusion.SetAllOverridesTo(true);
                    }
                }

                //Bloom
                if (profile.TryGetSettings(out bloom))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.Bloom>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.Bloom>();
                    }
                    if (volumeProfile.TryGet(out HDRPBloom))
                    {
                        HDRPBloom.active = true;
                        HDRPBloom.intensity.value = bloom.intensity;
                        HDRPBloom.threshold.value = bloom.threshold;
                        HDRPBloom.tint.value = bloom.color;
                        HDRPBloom.scatter.value = bloom.softKnee;
                        HDRPBloom.quality.value = 3;
                        HDRPBloom.dirtTexture.value = bloom.dirtTexture;
                        HDRPBloom.dirtIntensity.value = bloom.dirtIntensity;
                        HDRPBloom.SetAllOverridesTo(true);
                    }
                }

                //Chromatic Aberration
                if (profile.TryGetSettings(out chromaticAberration))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.ChromaticAberration>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.ChromaticAberration>();
                    }
                    if (volumeProfile.TryGet(out HDRPChromaticAberration))
                    {
                        HDRPChromaticAberration.active = true;
                        HDRPChromaticAberration.intensity.value = chromaticAberration.intensity;
                        HDRPChromaticAberration.SetAllOverridesTo(true);
                    }
                }

                //Color Grading
                if (profile.TryGetSettings(out colorGrading))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.ColorAdjustments>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.ColorAdjustments>();
                    }
                    if (volumeProfile.TryGet(out HDRPColorAdjustments))
                    {
                        HDRPColorAdjustments.active = true;
                        HDRPColorAdjustments.postExposure.value = colorGrading.postExposure;
                        HDRPColorAdjustments.contrast.value = colorGrading.contrast;
                        HDRPColorAdjustments.colorFilter.value = colorGrading.colorFilter;
                        HDRPColorAdjustments.hueShift.value = colorGrading.hueShift;
                        HDRPColorAdjustments.saturation.value = colorGrading.saturation;
                        HDRPColorAdjustments.SetAllOverridesTo(true);
                    }

                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.WhiteBalance>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.WhiteBalance>();
                    }
                    if (volumeProfile.TryGet(out HDRPWhiteBalance))
                    {
                        HDRPWhiteBalance.active = true;
                        HDRPWhiteBalance.temperature.value = colorGrading.temperature;
                        HDRPWhiteBalance.tint.value = colorGrading.tint;
                        HDRPWhiteBalance.SetAllOverridesTo(true);
                    }

                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.ChannelMixer>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.ChannelMixer>();
                    }
                    if (volumeProfile.TryGet(out HDRPChannelMixer))
                    {
                        HDRPChannelMixer.active = true;
                        HDRPChannelMixer.blueOutBlueIn.value = colorGrading.mixerBlueOutBlueIn;
                        HDRPChannelMixer.blueOutGreenIn.value = colorGrading.mixerBlueOutGreenIn;
                        HDRPChannelMixer.blueOutRedIn.value = colorGrading.mixerBlueOutRedIn;

                        HDRPChannelMixer.greenOutBlueIn.value = colorGrading.mixerGreenOutBlueIn;
                        HDRPChannelMixer.greenOutGreenIn.value = colorGrading.mixerGreenOutGreenIn;
                        HDRPChannelMixer.greenOutRedIn.value = colorGrading.mixerGreenOutRedIn;

                        HDRPChannelMixer.redOutBlueIn.value = colorGrading.mixerRedOutBlueIn;
                        HDRPChannelMixer.redOutGreenIn.value = colorGrading.mixerRedOutGreenIn;
                        HDRPChannelMixer.redOutRedIn.value = colorGrading.mixerRedOutRedIn;
                        HDRPChannelMixer.SetAllOverridesTo(true);
                    }

                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.Tonemapping>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.Tonemapping>();
                    }
                    if (volumeProfile.TryGet(out HDRPTonemapping))
                    {
                        HDRPTonemapping.active = true;
                        switch (colorGrading.tonemapper.value)
                        {
                            case Tonemapper.None:
                                HDRPTonemapping.mode.value = UnityEngine.Rendering.HighDefinition.TonemappingMode.None;
                                break;
                            case Tonemapper.Neutral:
                                HDRPTonemapping.mode.value = UnityEngine.Rendering.HighDefinition.TonemappingMode.Neutral;
                                break;
                            case Tonemapper.ACES:
                                HDRPTonemapping.mode.value = UnityEngine.Rendering.HighDefinition.TonemappingMode.ACES;
                                break;
                        }
                        HDRPTonemapping.SetAllOverridesTo(true);
                    }
                }

                //Depth Of Field
                if (profile.TryGetSettings(out depthOfField))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.DepthOfField>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.DepthOfField>();
                    }
                    if (volumeProfile.TryGet(out HDRPDepthOfField))
                    {
                        HDRPDepthOfField.active = true;
                        HDRPDepthOfField.focusMode.value = UnityEngine.Rendering.HighDefinition.DepthOfFieldMode.UsePhysicalCamera;
                        HDRPDepthOfField.focusDistance.value = depthOfField.focusDistance;
                        HDRPDepthOfField.SetAllOverridesTo(true);
                    }
                }

                //Film Grain
                if (profile.TryGetSettings(out grain))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.FilmGrain>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.FilmGrain>();
                    }
                    if (volumeProfile.TryGet(out HDRPFilmGrain))
                    {
                        HDRPFilmGrain.active = true;
                        HDRPFilmGrain.intensity.value = grain.intensity;
                        HDRPFilmGrain.SetAllOverridesTo(true);
                    }
                }

                //Lens Distortion
                if (profile.TryGetSettings(out lensDistortion))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.LensDistortion>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.LensDistortion>();
                    }
                    if (volumeProfile.TryGet(out HDRPLensDistortion))
                    {
                        HDRPLensDistortion.active = true;
                        Vector2 center = new Vector2(lensDistortion.centerX, lensDistortion.centerY);
                        HDRPLensDistortion.center.value = center;
                        HDRPLensDistortion.intensity.value = lensDistortion.intensity;
                        HDRPLensDistortion.xMultiplier.value = lensDistortion.intensityX;
                        HDRPLensDistortion.yMultiplier.value = lensDistortion.intensityY;
                        HDRPLensDistortion.scale.value = lensDistortion.scale;
                        HDRPLensDistortion.SetAllOverridesTo(true);
                    }
                }

                //Motion Blur
                if (profile.TryGetSettings(out motionBlur))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.MotionBlur>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.MotionBlur>();
                    }
                    if (volumeProfile.TryGet(out HDRPMotionBlur))
                    {
                        HDRPMotionBlur.active = true;
                        HDRPMotionBlur.intensity.value = motionBlur.shutterAngle;
                        HDRPMotionBlur.SetAllOverridesTo(true);
                    }
                }

                //Vignette
                if (profile.TryGetSettings(out vignette))
                {
                    if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.Vignette>())
                    {
                        volumeProfile.Add<UnityEngine.Rendering.HighDefinition.Vignette>();
                    }
                    if (volumeProfile.TryGet(out HDRPVignette))
                    {
                        HDRPVignette.active = true;
                        HDRPVignette.intensity.value = vignette.intensity;
                        HDRPVignette.rounded.value = vignette.rounded;
                        HDRPVignette.smoothness.value = vignette.smoothness;
                        HDRPVignette.SetAllOverridesTo(true);
                    }
                }

                //Split Toning
                if (!volumeProfile.Has<UnityEngine.Rendering.HighDefinition.SplitToning>())
                {
                    volumeProfile.Add<UnityEngine.Rendering.HighDefinition.SplitToning>();
                }
                if (volumeProfile.TryGet(out HDRPSplitToning))
                {
                    HDRPSplitToning.active = true;
                    HDRPSplitToning.shadows.value = GaiaUtils.GetColorFromHTML("636363");
                    HDRPSplitToning.highlights.value = GaiaUtils.GetColorFromHTML("909090");
                    HDRPSplitToning.balance.value = -20f;
                    HDRPSplitToning.SetAllOverridesTo(true);
                }
#endif
            }
            else
            {
                Debug.Log("This feature only supported to create URP/HDRP post processing profiles from Built-In Post FX Stack V2. Please make sure the correct pipeline is selected to create to.");
            }

            //Save file
            if (saveLocation.Contains(Application.dataPath))
            {
                saveLocation = saveLocation.Replace(Application.dataPath, "Assets/");
            }

            AssetDatabase.CreateAsset(volumeProfile, saveLocation + "/" + extentionName + profile.name + ".asset");
            AssetDatabase.SaveAssets();
        }
#endif
#endif

#if HDPipeline
        /// <summary>
        /// Used to check if the light render mode is in LUX
        /// </summary>
        /// <param name="lightData"></param>
        /// <param name="currentLightIntensity"></param>
        public static void CheckHDRPLightRenderMode(HDAdditionalLightData lightData, float currentLightIntensity)
        {
            if (lightData != null)
            {
                if (lightData.lightUnit != LightUnit.Lux)
                {
                    lightData.lightUnit = LightUnit.Lux;
                    lightData.intensity = currentLightIntensity;
                }
            }
        }

        /// <summary>
        /// Checks to see if HDRP Light data is on active light set as 'checkLight'
        /// </summary>
        /// <param name="checkLight"></param>
        /// <returns></returns>
        public static HDAdditionalLightData GetOrAddHDRPLightData(Light checkLight)
        {
            HDAdditionalLightData HDRPLightData = null;
            if (checkLight != null)
            {
                HDRPLightData = checkLight.GetComponent<HDAdditionalLightData>();
                if (HDRPLightData == null)
                {
                    HDRPLightData = checkLight.gameObject.AddComponent<HDAdditionalLightData>();
                }
            }

            return HDRPLightData;
        }

        /// <summary>
        /// Multiplies the value by multiply value, which is defaulted to 3.14
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float SetHDRPFloat(float value, float multiply = 3.14f)
        {
            float newValue = value * multiply;
            return newValue;
        }

        public static Color SetHDRPColor(Color value)
        {
            Color newColor = value;
            return newColor;
        }
#endif

        /// <summary>
        /// Gets and returns the main camera in the scene
        /// </summary>
        /// <returns></returns>
        public static Camera GetCamera()
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                return camera;
            }

            camera = GameObject.FindObjectOfType<Camera>();
            if (camera != null)
            {
                return camera;
            }

            return null;
        }

        /// <summary>
        /// Gets the player that has a character ocntroller attached to the camera/player
        /// </summary>
        /// <returns></returns>
        public static GameObject GetCharacter()
        {
            GameObject playerObject = null;

            CharacterController characterController = GameObject.FindObjectOfType<CharacterController>();
            if (characterController != null)
            {
                playerObject = characterController.gameObject;
            }

            return playerObject;
        }

        /// <summary>
        /// Get the main directional light in the scene
        /// </summary>
        /// <returns>Main light or null</returns>
        public static Light GetMainDirectionalLight()
        {
            GameObject lightObject = GameObject.Find("Directional Light");
            Light mainSunLight = null;

            if (lightObject != null)
            {
                mainSunLight = lightObject.GetComponent<Light>();
            }
            if (lightObject == null)
            {
                //Grab the first directional light we can find
                Light[] lights = GameObject.FindObjectsOfType<Light>();
                foreach (var light in lights)
                {
                    if (light.type == LightType.Directional && light.name != "Moon Light")
                    {
                        lightObject = light.gameObject;
                        mainSunLight = light;
                    }
                }

                if (lightObject == null)
                {
                    lightObject = new GameObject("Directional Light");
                    lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                    Light lightSettings = lightObject.AddComponent<Light>();
                    lightSettings.type = LightType.Directional;
                    mainSunLight = lightSettings;
                    mainSunLight.shadowStrength = 0.8f;
                }
            }

            GameObject parentObject = GameObject.Find("Gaia Lighting");
            if (parentObject != null)
            {
                lightObject.transform.SetParent(parentObject.transform);
            }

            return mainSunLight;
        }

        /// <summary>
        /// Gets or creates the moon object
        /// </summary>
        /// <returns></returns>
        public static Light GetMainMoonLight()
        {
            Light moonLight = null;
            GameObject moonObject = GameObject.Find("Moon Light");
            if (moonObject == null)
            {
                moonObject = new GameObject("Moon Light");
            }

            moonLight = moonObject.GetComponent<Light>();
            if (moonLight == null)
            {
                moonLight = moonObject.AddComponent<Light>();
            }

            moonLight.type = LightType.Directional;
            moonLight.shadows = LightShadows.Soft;
            moonLight.color = GaiaUtils.GetColorFromHTML("6A95CF");
            moonLight.intensity = 0f;

            return moonLight;
        }

        /// <summary>
        /// Gets the water material on the object
        /// </summary>
        /// <returns></returns>
        public static Material GetWaterMaterial(string waterObjectName)
        {
            Material material = null;
            MeshRenderer meshRender = null;
            GameObject waterObject = GameObject.Find(waterObjectName);
            if (waterObject != null)
            {
                meshRender = waterObject.GetComponent<MeshRenderer>();
                if (meshRender != null)
                {
                    material = meshRender.sharedMaterial;
                }
            }

            return material;
        }

        /// <summary>
        /// Write a scriptable object out into a new asset that can be shared
        /// </summary>
        /// <typeparam name="T">The scriptable object to be saved as an asset</typeparam>
        public static void CreateAsset<T>(string path = "", string name = "") where T : ScriptableObject
        {
#if UNITY_EDITOR
            T asset = ScriptableObject.CreateInstance<T>();

            if (path == "")
            {
                path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (path == "")
                {
                    path = "Assets";
                }
                else if (Path.GetExtension(path) != "")
                {
                    path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
                }
            }
            if (name == "")
            {
                name = path + "/New " + typeof(T).ToString() + ".asset";
            }
            else
            {
                name = path + "/New " + name + ".asset";
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(name);
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
#endif
        }

        /// <summary>
        /// Get the path of the unity object supplied
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        public static string GetAssetPath(UnityEngine.Object uo)
        {
            string path = "";
#if UNITY_EDITOR
            path = Path.Combine(Application.dataPath, AssetDatabase.GetAssetPath(uo));
            path = path.Replace("/Assets", "");
            path = path.Replace("\\", "/");
#endif
            return path;
        }


        /// <summary>
        /// Wrap the scriptable object up so that it can be transferred without causing unity errors
        /// </summary>
        /// <param name="so"></param>
        public static string WrapScriptableObject(ScriptableObject so)
        {
            string newpath = "";
#if UNITY_EDITOR
            string path = GetAssetPath(so);
            if (File.Exists(path))
            {
                newpath = Path.ChangeExtension(path, "bytes");
                UnityEditor.FileUtil.CopyFileOrDirectory(path, newpath);
            }
            else
            {
                Debug.LogError("There is no file at the path supplied: " + path);
            }
#endif
            return newpath;
        }


        public static void UnwrapScriptableObject(string path, string newpath)
        {
#if UNITY_EDITOR
            if (File.Exists(path))
            {
                if (!File.Exists(newpath))
                {
                    UnityEditor.FileUtil.CopyFileOrDirectory(path, newpath);
                }
                else
                {
                    Debug.LogError("There is already a file with this name at the path supplied: " + newpath);
                }
            }
            else
            {
                Debug.LogError("There is no file at the path supplied: " + path);
            }
#endif
        }

        public static string WrapGameObjectAsPrefab(GameObject go)
        {
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
            string name = go.name;
            UnityEngine.Object prefab = PrefabUtility.SaveAsPrefabAsset(new GameObject(), "Assets/" + name + ".prefab");
            PrefabUtility.SavePrefabAsset(go);
            AssetDatabase.Refresh();
            return AssetDatabase.GetAssetPath(prefab);
#else
            string name = go.name;
            UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + name + ".prefab");
            PrefabUtility.ReplacePrefab(go, prefab);
            AssetDatabase.Refresh();
            return AssetDatabase.GetAssetPath(prefab);
#endif
#else
            return "";
#endif
        }

        public static bool HasTerrains()
        {
            return (GaiaTerrainLoaderManager.TerrainScenes.Count > 0 || Terrain.activeTerrains.Where(x => !TerrainHelper.IsWorldMapTerrain(x)).Count() > 0);
        }

        public static EnvironmentSize IntToEnvironmentSize(int terrainSize)
        {
            switch (terrainSize)
            {
                case 256:
                    return GaiaConstants.EnvironmentSize.Is256MetersSq;
                case 512:
                    return GaiaConstants.EnvironmentSize.Is512MetersSq;
                case 1024:
                    return GaiaConstants.EnvironmentSize.Is1024MetersSq;
                case 2048:
                    return GaiaConstants.EnvironmentSize.Is2048MetersSq;
                case 4096:
                    return GaiaConstants.EnvironmentSize.Is4096MetersSq;
                case 8192:
                    return GaiaConstants.EnvironmentSize.Is8192MetersSq;
                case 16384:
                    return GaiaConstants.EnvironmentSize.Is16384MetersSq;
            }
            return GaiaConstants.EnvironmentSize.Is256MetersSq;
        }

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="fileName">File name to search for</param>
        /// <returns></returns>
        public static string GetAssetPath(string fileName)
        {
#if UNITY_EDITOR
            string fName = Path.GetFileNameWithoutExtension(fileName);
            string[] assets = AssetDatabase.FindAssets(fName, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == fileName)
                {
                    return path;
                }
            }
#endif
            return "";
        }

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <param name="name">Type to search for</param>
        /// <returns></returns>
        public static string GetAssetPath(string name, string type)
        {
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            string[] file;
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                //Make sure its an exact match
                file = Path.GetFileName(path).Split('.');
                if (file.GetLength(0) != 2)
                {
                    continue;
                }
                if (file[0] != name)
                {
                    continue;
                }
                if (file[1] != type)
                {
                    continue;
                }
                return path;
            }
#endif
            return "";
        }

        /// <summary>
        /// Returns the first asset that matches the file path and name passed. Will try
        /// full path first, then will try just the file name.
        /// </summary>
        /// <param name="fileNameOrPath">File name as standalone or fully pathed</param>
        /// <returns>Object or null if it was not found</returns>
        public static UnityEngine.Object GetAsset(string fileNameOrPath, Type assetType)
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(fileNameOrPath))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(fileNameOrPath, assetType);
                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    string path = GetAssetPath(Path.GetFileName(fileNameOrPath));
                    if (!string.IsNullOrEmpty(path))
                    {
                        return AssetDatabase.LoadAssetAtPath(path, assetType);
                    }
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// Return the first prefab that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the prefab or null</returns>
        public static GameObject GetAssetPrefab(string name)
        {
#if UNITY_EDITOR
            string path = GetAssetPath(name, "prefab");
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
#endif
            return null;
        }

        /// <summary>
        /// Return the first scriptable that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the prefab or null</returns>
        public static ScriptableObject GetAssetScriptableObject(string name)
        {
#if UNITY_EDITOR
            string path = GetAssetPath(name, "asset");
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }
#endif
            return null;
        }

        /// <summary>
        /// Return the first texture that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the texture or null</returns>
        public static Texture2D GetAssetTexture2D(string name)
        {
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (path.Contains(".jpg") || path.Contains(".psd") || path.Contains(".png"))
                {
                    //Make sure its an exact match
                    string filename = Path.GetFileNameWithoutExtension(path);
                    if (filename == name)
                    {
                        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    }
                }
            }
#endif
            return null;
        }

        #endregion

        #region Image helpers

        /// <summary>
        /// Make the texture supplied into a normal map
        /// </summary>
        /// <param name="texture">Texture to convert</param>
        public static void MakeTextureNormal(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null && tImporter.textureType != TextureImporterType.NormalMap)
            {
                tImporter.textureType = TextureImporterType.NormalMap;
                tImporter.SaveAndReimport();
                AssetDatabase.Refresh();
            }
#endif
        }

        /// <summary>
        /// Make the texture supplied readable
        /// </summary>
        /// <param name="texture">Texture to convert</param>
        public static void MakeTextureReadable(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null && tImporter.isReadable != true)
            {
                tImporter.isReadable = true;
                tImporter.SaveAndReimport();
                AssetDatabase.Refresh();
            }
#endif
        }

        /// <summary>
        /// Make the texture supplied uncompressed
        /// </summary>
        /// <param name="texture">Texture to convert</param>
        public static void MakeTextureUncompressed(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null && tImporter.textureCompression != TextureImporterCompression.Uncompressed)
            {
                tImporter.textureCompression = TextureImporterCompression.Uncompressed;
                tImporter.SaveAndReimport();
                AssetDatabase.Refresh();
            }
#endif
        }

        /// <summary>
        /// Compress / encode a single layer map file to an image
        /// </summary>
        /// <param name="input">Single layer map in format x,y</param>
        /// <param name="imageName">Output image name - image image index and extension will be added</param>
        /// <param name="exportPNG">True if a png is wanted</param>
        /// <param name="exportJPG">True if a jpg is wanted</param>
        public static void CompressToSingleChannelFileImage(float[,] input, string imageName, TextureFormat imageStorageFormat = Gaia.GaiaConstants.defaultTextureFormat, bool exportPNG = true, bool exportJPG = true)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);

            Texture2D exportTexture = new Texture2D(width, height, imageStorageFormat, false);
            Color pixelColor = new Color();
            pixelColor.a = 1f;
            pixelColor.r = pixelColor.g = pixelColor.b = 0f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixelColor.r = pixelColor.b = pixelColor.g = input[x, y];
                    exportTexture.SetPixel(x, y, pixelColor);
                }
            }

            exportTexture.Apply();

            // Write JPG
            if (exportJPG)
            {
                ExportJPG(imageName, exportTexture);
            }

            // Write PNG
            if (exportPNG)
            {
                ExportPNG(imageName, exportTexture);
            }

            //Lose the texture
            DestroyImmediate(exportTexture);
        }

        /// <summary>
        /// Compress / encode a multi layer map file to an image
        /// </summary>
        /// <param name="input">Multi layer map in format x,y,layer</param>
        /// <param name="imageName">Output image name - image image index and extension will be added</param>
        /// <param name="exportPNG">True if a png is wanted</param>
        /// <param name="exportJPG">True if a jpg is wanted</param>
        public static void CompressToMultiChannelFileImage(float[,,] input, string imageName, TextureFormat imageStorageFormat = Gaia.GaiaConstants.defaultTextureFormat, bool flip = false, bool exportPNG = true, bool exportJPG = true)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            int layers = input.GetLength(2);
            int images = (layers + 3) / 4;

            for (int image = 0; image < images; image++)
            {
                Texture2D exportTexture = new Texture2D(width, width, imageStorageFormat, false);
                Color pixelColor = new Color();
                int layer = image * 4;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        pixelColor.r = layer < layers ? input[x, y, layer] : 0f;
                        pixelColor.g = (layer + 1) < layers ? input[x, y, (layer + 1)] : 0f;
                        pixelColor.b = (layer + 2) < layers ? input[x, y, (layer + 2)] : 0f;
                        pixelColor.a = (layer + 3) < layers ? input[x, y, (layer + 3)] : 0f;
                        if (flip)
                        {
                            exportTexture.SetPixel(y, x, pixelColor);
                        }
                        else
                        {
                            exportTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                }
                exportTexture.Apply();

                // Write JPG
                if (exportJPG)
                {
                    byte[] jpgBytes = exportTexture.EncodeToJPG();
                    PWCommon2.Utils.WriteAllBytes(imageName + image + ".jpg", jpgBytes);
                }

                // Write PNG
                if (exportPNG)
                {
                    byte[] pngBytes = exportTexture.EncodeToPNG();
                    PWCommon2.Utils.WriteAllBytes(imageName + image + ".png", pngBytes);
                }

                //Lose the texture
                DestroyImmediate(exportTexture);
            }
        }

        /// <summary>
        /// Compress / encode a multi layer map file to an image
        /// </summary>
        /// <param name="input">Multi layer map in format x,y,layer</param>
        /// <param name="imageName">Output image name - image image index and extension will be added</param>
        /// <param name="exportPNG">True if a png is wanted</param>
        /// <param name="exportJPG">True if a jpg is wanted</param>
        public static void CompressToMultiChannelFileImage(string imageName, HeightMap r, HeightMap g, HeightMap b, HeightMap a, TextureFormat imageStorageFormat, GaiaConstants.ImageFileType imageFileType)
        {
            int width = 0;
            int height = 0;

            if (r != null)
            {
                width = r.Width();
                height = r.Depth();
            }
            else if (g != null)
            {
                width = g.Width();
                height = g.Depth();
            }
            else if (b != null)
            {
                width = b.Width();
                height = b.Depth();
            }
            else if (a != null)
            {
                width = a.Width();
                height = a.Depth();
            }

            if (string.IsNullOrEmpty(imageName))
            {
                Debug.LogError("Cannot write image - no name supplied!");
                return;
            }

            if (width == 0 || height == 0)
            {
                Debug.LogError("Cannot write image - invalid dimensions : " + width + ", " + height);
                return;
            }

            Texture2D exportTexture = new Texture2D(width, height, imageStorageFormat, true, false);
            Color pixelColor = new Color();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixelColor.r = r != null ? r[x, y] : 0f;
                    pixelColor.g = g != null ? g[x, y] : 0f;
                    pixelColor.b = b != null ? b[x, y] : 0f;
                    pixelColor.a = a != null ? a[x, y] : 1f;
                    exportTexture.SetPixel(x, y, pixelColor);
                }
            }
            exportTexture.Apply();

#if UNITY_2017_1_OR_NEWER
            switch (imageFileType)
            {
                case GaiaConstants.ImageFileType.Jpg:
                    byte[] jpgBytes = ImageConversion.EncodeToJPG(exportTexture, 100);
                    PWCommon2.Utils.WriteAllBytes(imageName + ".jpg", jpgBytes);
                    break;
                case GaiaConstants.ImageFileType.Png:
                    byte[] pngBytes = ImageConversion.EncodeToPNG(exportTexture);
                    PWCommon2.Utils.WriteAllBytes(imageName + ".png", pngBytes);
                    break;
                case GaiaConstants.ImageFileType.Exr:
                    byte[] exrBytes = ImageConversion.EncodeToEXR(exportTexture, Texture2D.EXRFlags.CompressZIP);
                    PWCommon2.Utils.WriteAllBytes(imageName + ".exr", exrBytes);
                    break;
            }
#else
            switch (imageFileType)
            {
                case GaiaConstants.ImageFileType.Jpg:
                    byte[] jpgBytes = exportTexture.EncodeToJPG();
                    PWCommon1.Utils.WriteAllBytes(imageName + ".jpg", jpgBytes);
                    break;
                case GaiaConstants.ImageFileType.Png:
                    byte[] pngBytes = exportTexture.EncodeToPNG();
                    PWCommon1.Utils.WriteAllBytes(imageName + ".png", pngBytes);
                    break;
                case GaiaConstants.ImageFileType.Exr:
                    byte[] exrBytes = exportTexture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
                    PWCommon1.Utils.WriteAllBytes(imageName + ".exr", exrBytes);
                    break;
            }
#endif

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            //Lose the texture
            DestroyImmediate(exportTexture);
        }


        /// <summary>
        /// Convert the supplied texture to an array based on grayscale value
        /// </summary>
        /// <param name="texture">Input texture - must be read enabled</param>
        /// <returns>Texture as grayscale array</returns>
        public static float[,] ConvertTextureToArray(Texture2D texture)
        {
            float[,] array = new float[texture.width, texture.height];
            for (int x = 0; x < texture.width; x++)
            {
                for (int z = 0; z < texture.height; z++)
                {
                    array[x, z] = texture.GetPixel(x, z).grayscale;
                }
            }
            return array;
        }


        /// <summary>
        /// Decompress a single channel from the provided file into a float array.
        /// </summary>
        /// <param name="fileName">File to process</param>
        /// <param name="channelR">Take data from R channel</param>
        /// <param name="channelG">Take data from G channel</param>
        /// <param name="channelB">Take data from B channel</param>
        /// <param name="channelA">Take data from A channel</param>
        /// <returns>Array of float values from the selected channel</returns>
        public static float[,] DecompressFromSingleChannelFileImage(string fileName, int width, int height, TextureFormat imageStorageFormat = Gaia.GaiaConstants.defaultTextureFormat, bool channelR = true, bool channelG = false, bool channelB = false, bool channelA = false)
        {
            float[,] retArray = null;

            if (System.IO.File.Exists(fileName))
            {
                byte[] bytes = PWCommon2.Utils.ReadAllBytes(fileName);
                Texture2D importTexture = new Texture2D(width, height, imageStorageFormat, false);
                importTexture.LoadImage(bytes);
                retArray = new float[width, height];
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).r;
                    }
                }
                //Lose the texture
                DestroyImmediate(importTexture);
            }
            else
            {
                Debug.LogError("Unable to find " + fileName);
            }
            return retArray;
        }

        /// <summary>
        /// Decompress a single channel from the provided file into a float array.
        /// </summary>
        /// <param name="fileName">File to process</param>
        /// <param name="channelR">Take data from R channel</param>
        /// <param name="channelG">Take data from G channel</param>
        /// <param name="channelB">Take data from B channel</param>
        /// <param name="channelA">Take data from A channel</param>
        /// <returns>Array of float values from the selected channel</returns>
        public static float[,] DecompressFromSingleChannelTexture(Texture2D importTexture, bool channelR = true, bool channelG = false, bool channelB = false, bool channelA = false)
        {
            if ((importTexture == null) || importTexture.width <= 0 || importTexture.height <= 0)
            {
                Debug.LogError("Unable to import from texture");
                return null;
            }

            float[,] retArray = new float[importTexture.width, importTexture.height];

            if (channelR)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).r;
                    }
                }
            }
            else if (channelG)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).g;
                    }
                }
            }
            else if (channelB)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).b;
                    }
                }
            }
            if (channelA)
            {
                for (int x = 0; x < importTexture.width; x++)
                {
                    for (int y = 0; y < importTexture.height; y++)
                    {
                        retArray[x, y] = importTexture.GetPixel(x, y).a;
                    }
                }
            }
            return retArray;
        }

        /// <summary>
        /// Export a texture to jpg
        /// </summary>
        /// <param name="fileName">File name to us - will have .jpg appended</param>
        /// <param name="texture">Texture source</param>
        public static void ExportJPG(string fileName, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToJPG();
            PWCommon2.Utils.WriteAllBytes(fileName + ".jpg", bytes);
        }

        /// <summary>
        /// Export a texture to png
        /// </summary>
        /// <param name="fileName">File name to us - will have .png appended</param>
        /// <param name="texture">Texture source</param>
        public static void ExportPNG(string fileName, Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();
            PWCommon2.Utils.WriteAllBytes(fileName + ".png", bytes);
        }

        /// <summary>
        /// Will import the raw file provided - it assumes that it is in a square 16 bit PC format
        /// </summary>
        /// <param name="fileName">Fully qualified file name</param>
        /// <returns>File contents or null</returns>
        public static float[,] LoadRawFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                Debug.LogError("Could not locate heightmap file : " + fileName);
                return null;
            }

            float[,] heights = null;
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                using (BinaryReader br = new BinaryReader(fileStream))
                {
                    int mapSize = Mathf.CeilToInt(Mathf.Sqrt(fileStream.Length / 2));
                    heights = new float[mapSize, mapSize];
                    for (int x = 0; x < mapSize; x++)
                    {
                        for (int y = 0; y < mapSize; y++)
                        {
                            heights[x, y] = (float)(br.ReadUInt16() / 65535.0f);
                        }
                    }
                }
                fileStream.Close();
            }

            return heights;
        }


        #endregion

        #region Mesh helpers

        /// <summary>
        /// Fucntion used to fix prefabs layers
        /// </summary>
        /// <param name="prefabs"></param>
        public static void FixPrefabLayers(List<string> paths)
        {
            #if UNITY_EDITOR
            if (paths.Count < 1)
            {
                Debug.LogWarning(("No prefab paths provided"));
            }

            try
            {
                var SerializedObjectTagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                if (SerializedObjectTagManager != null)
                {
                    SerializedProperty serializedProperties = SerializedObjectTagManager.FindProperty("layers");
                    if (serializedProperties != null)
                    {
                        //Add layers if missing
                        string[] layers = UnityEditorInternal.InternalEditorUtility.layers;
                        if (layers.Length > 0)
                        {
                            bool addVFX = true;
                            bool addObjectSmall = true;
                            bool addObjectMedium = true;
                            bool addObjectLarge = true;
                            foreach (string name in layers)
                            {
                                if (name == GaiaPrefabUtility.m_vfxLayerName)
                                {
                                    addVFX = false;
                                }

                                if (name == GaiaPrefabUtility.m_objectSmallLayerName)
                                {
                                    addObjectSmall = false;
                                }

                                if (name == GaiaPrefabUtility.m_objectMediumLayerName)
                                {
                                    addObjectMedium = false;
                                }

                                if (name == GaiaPrefabUtility.m_objectLargeLayerName)
                                {
                                    addObjectLarge = false;
                                }
                            }

                            for (int i = 8; i < 31; i++)
                            {
                                SerializedProperty property = serializedProperties.GetArrayElementAtIndex(i);

                                if (property.stringValue.Length < 1)
                                {
                                    if (addVFX)
                                    {
                                        property.stringValue = GaiaPrefabUtility.m_vfxLayerName;
                                        addVFX = false;
                                        continue;
                                    }

                                    if (addObjectSmall)
                                    {
                                        property.stringValue = GaiaPrefabUtility.m_objectSmallLayerName;
                                        addObjectSmall = false;
                                        continue;
                                    }

                                    if (addObjectMedium)
                                    {
                                        property.stringValue = GaiaPrefabUtility.m_objectMediumLayerName;
                                        addObjectMedium = false;
                                        continue;
                                    }

                                    if (addObjectLarge)
                                    {
                                        property.stringValue = GaiaPrefabUtility.m_objectLargeLayerName;
                                        addObjectLarge = false;
                                        continue;
                                    }
                                }

                            }
                        }

                        SerializedObjectTagManager.ApplyModifiedProperties();
                        for (int i = 0; i < paths.Count; i++)
                        {
                            GameObject loadedPrefab = PrefabUtility.LoadPrefabContents(paths[i]);
                            DisplayProgressBarNoEditor("Updating Prefab Layers", "Updating " + loadedPrefab.name,
                                (float) i / (float) paths.Count);
                            if (loadedPrefab != null)
                            {
                                if (loadedPrefab.layer == GaiaPrefabUtility.m_vfxLayerIndex)
                                {
                                    loadedPrefab.layer = LayerMask.NameToLayer(GaiaPrefabUtility.m_vfxLayerName);
                                    PrefabUtility.SaveAsPrefabAsset(loadedPrefab, paths[i]);
                                }
                                else if (loadedPrefab.layer == GaiaPrefabUtility.m_objectSmallLayerIndex)
                                {
                                    loadedPrefab.layer =
                                        LayerMask.NameToLayer(GaiaPrefabUtility.m_objectSmallLayerName);
                                    PrefabUtility.SaveAsPrefabAsset(loadedPrefab, paths[i]);
                                }
                                else if (loadedPrefab.layer == GaiaPrefabUtility.m_objectMediumLayerIndex)
                                {
                                    loadedPrefab.layer =
                                        LayerMask.NameToLayer(GaiaPrefabUtility.m_objectMediumLayerName);
                                    PrefabUtility.SaveAsPrefabAsset(loadedPrefab, paths[i]);
                                }
                                else if (loadedPrefab.layer == GaiaPrefabUtility.m_objectLargeLayerIndex)
                                {
                                    loadedPrefab.layer =
                                        LayerMask.NameToLayer(GaiaPrefabUtility.m_objectLargeLayerName);
                                    PrefabUtility.SaveAsPrefabAsset(loadedPrefab, paths[i]);
                                }
                            }

                            PrefabUtility.UnloadPrefabContents(loadedPrefab);
                        }
                    }
                    else
                    {
                        Debug.LogError(("SerializedProperty is missing from TagManager.asset"));
                    }
                }
                else
                {
                    Debug.LogError(("Missing TagManager.asset"));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error while updating prefab layers. Message: " + ex.Message + " Stack Trace: " +
                               ex.StackTrace);
            }
            finally
            {
                ClearProgressBarNoEditor();
            }
            #endif
        }

        /// <summary>
        /// Create a mesh for the heightmap
        /// </summary>
        /// <param name="heightmap"></param>
        /// <param name="targetSize"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static Mesh CreateMesh(float[,] heightmap, Vector3 targetSize)
        {
            //Need to sample these to not blow unity mesh sizes
            int width = heightmap.GetLength(0);
            int height = heightmap.GetLength(1);
            int targetRes = 1;

            Vector3 targetOffset = Vector3.zero - (targetSize / 2f);
            Vector2 uvScale = new Vector2(1.0f / (width - 1), 1.0f / (height - 1));

            //Choose best possible target res
            for (targetRes = 1; targetRes < 100; targetRes++)
            {
                if (((width / targetRes) * (height / targetRes)) < 65000)
                {
                    break;
                }
            }

            targetSize = new Vector3(targetSize.x / (width - 1) * targetRes, targetSize.y, targetSize.z / (height - 1) * targetRes);
            width = (width - 1) / targetRes + 1;
            height = (height - 1) / targetRes + 1;

            Vector3[] vertices = new Vector3[width * height];
            Vector2[] uvs = new Vector2[width * height];
            Vector3[] normals = new Vector3[width * height];
            Color[] colors = new Color[width * height];
            int[] triangles = new int[(width - 1) * (height - 1) * 6];

            // Build vertices and UVs
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors[y * width + x] = Color.black;
                    normals[y * width + x] = Vector3.up;
                    //vertices[y * w + x] = Vector3.Scale(targetSize, new Vector3(-y, heightmap[x * tRes, y * tRes], x)) + targetOffset;
                    vertices[y * width + x] = Vector3.Scale(targetSize, new Vector3(x, heightmap[x * targetRes, y * targetRes], y)) + targetOffset;
                    uvs[y * width + x] = Vector2.Scale(new Vector2(x * targetRes, y * targetRes), uvScale);
                }
            }

            // Build triangle indices: 3 indices into vertex array for each triangle
            int index = 0;
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    triangles[index++] = (y * width) + x;
                    triangles[index++] = ((y + 1) * width) + x;
                    triangles[index++] = (y * width) + x + 1;
                    triangles[index++] = ((y + 1) * width) + x;
                    triangles[index++] = ((y + 1) * width) + x + 1;
                    triangles[index++] = (y * width) + x + 1;
                }
            }

            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }

        /// <summary>
        /// Return the bounds of both the object and any colliders it has
        /// </summary>
        /// <param name="go">Game object to check</param>
        public static Bounds GetBounds(GameObject go)
        {
            Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }
            foreach (Collider c in go.GetComponentsInChildren<Collider>())
            {
                bounds.Encapsulate(c.bounds);
            }
            return bounds;
        }

        /// <summary>
        /// Calculate a normal map for a terrain
        /// </summary>
        /// <returns>Normals for the terrain in a Texture 2D</returns>
        public static Texture2D CalculateNormals(Terrain terrain)
        {
            int width = terrain.terrainData.heightmapResolution;
            int height = terrain.terrainData.heightmapResolution;
            float ux = 1.0f / (width - 1.0f);
            float uy = 1.0f / (height - 1.0f);
            float terrainHeight = width / 2f;
            float scaleX = terrainHeight / (float)width;
            float scaleY = terrainHeight / (float)height;
            float[] heights = new float[width * height];
            Buffer.BlockCopy(terrain.terrainData.GetHeights(0, 0, width, height), 0, heights, 0, heights.Length * sizeof(float));
            Texture2D normalMap = new Texture2D(width, height, TextureFormat.RGBAFloat, false, true);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int xp1 = (x == width - 1) ? x : x + 1;
                    int xn1 = (x == 0) ? x : x - 1;

                    int yp1 = (y == height - 1) ? y : y + 1;
                    int yn1 = (y == 0) ? y : y - 1;

                    float l = heights[xn1 + y * width] * scaleX;
                    float r = heights[xp1 + y * width] * scaleX;

                    float b = heights[x + yn1 * width] * scaleY;
                    float t = heights[x + yp1 * width] * scaleY;

                    float dx = (r - l) / (2.0f * ux);
                    float dy = (t - b) / (2.0f * uy);

                    Vector3 normal;
                    normal.x = -dx;
                    normal.y = -dy;
                    normal.z = 1;
                    normal.Normalize();

                    Color pixel;
                    pixel.r = normal.x * 0.5f + 0.5f;
                    pixel.g = normal.y * 0.5f + 0.5f;
                    pixel.b = normal.z;
                    pixel.a = 1.0f;

                    normalMap.SetPixel(x, y, pixel);
                }
            }
            normalMap.Apply();
            return normalMap;
        }

        #endregion

        #region Direction helpers

        /// <summary>
        /// Rotate a direction vector left 90% around X axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90LeftXAxis(Vector3 input)
        {
            return new Vector3(input.x, -input.z, input.y);
        }



        /// <summary>
        /// Rotate a direction vector right 90% around X axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90RightXAxis(Vector3 input)
        {
            return new Vector3(input.x, input.z, -input.y);
        }

        /// <summary>
        /// Rotate a direction vector left 90% around Y axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90LeftYAxis(Vector3 input)
        {
            return new Vector3(-input.z, input.y, input.x);
        }

        /// <summary>
        /// Rotate a direction vector right 90% around Y axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90RightYAxis(Vector3 input)
        {
            return new Vector3(input.z, input.y, -input.x);
        }

        /// <summary>
        /// Rotate a direction vector left 90% around Z axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90LeftZAxis(Vector3 input)
        {
            return new Vector3(input.y, -input.x, input.z);
        }

        /// <summary>
        /// Rotate a direction vector right 90% around Y axis
        /// </summary>
        /// <param name="input">Direction vector</param>
        /// <returns>Rotated direction vector</returns>
        Vector3 Rotate90RightZAxis(Vector3 input)
        {
            return new Vector3(-input.y, input.x, input.z);
        }

        #endregion

        #region Math helpers

        /// <summary>
        /// Return true if the values are approximately equal
        /// </summary>
        /// <param name="a">Parameter A</param>
        /// <param name="b">Parameter B</param>
        /// <param name="threshold">Threshold to test for</param>
        /// <returns>True if approximately equal</returns>
        public static bool Math_ApproximatelyEqual(float a, float b, float threshold)
        {

            if (a == b || Mathf.Abs(a - b) < threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if the values are approximately equal
        /// </summary>
        /// <param name="a">Parameter A</param>
        /// <param name="b">Parameter B</param>
        /// <returns>True if approximately equal</returns>
        public static bool Math_ApproximatelyEqual(float a, float b)
        {
            return Math_ApproximatelyEqual(a, b, float.Epsilon);
        }

        /// <summary>
        /// Return true if the value is a power of 2
        /// </summary>
        /// <param name="value">Value to be checked</param>
        /// <returns>True if a power of 2</returns>
        public static bool Math_IsPowerOf2(int value)
        {
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Returned value clamped in range of min to max
        /// </summary>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="value">Value to check</param>
        /// <returns>Clamped value</returns>
        public static float Math_Clamp(float min, float max, float value)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        /// <summary>
        /// Return mod of value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="mod">Mod value</param>
        /// <returns>Mode of value</returns>
        public static float Math_Modulo(float value, float mod)
        {
            return value - mod * (float)Math.Floor(value / mod);
        }

        /// <summary>
        /// Return mod of value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="mod">Mod value</param>
        /// <returns>Mode of value</returns>
        public static int Math_Modulo(int value, int mod)
        {
            return (int)(value - mod * (float)Math.Floor((float)value / mod));
        }

        /// <summary>
        /// Linear interpolation between two values
        /// </summary>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <param name="fraction">Fraction</param>
        /// <returns></returns>
        public static float Math_InterpolateLinear(float value1, float value2, float fraction)
        {
            return value1 * (1f - fraction) + value2 * fraction;
        }

        /// <summary>
        /// Smooth interpolation between two values
        /// </summary>
        /// <param name="value1">Value 1</param>
        /// <param name="value2">Value 2</param>
        /// <param name="fraction">Fraction</param>
        /// <returns></returns>
        public static float Math_InterpolateSmooth(float value1, float value2, float fraction)
        {
            if (fraction < 0.5f)
            {
                fraction = 2f * fraction * fraction;
            }
            else
            {
                fraction = 1f - 2f * (fraction - 1f) * (fraction - 1f);
            }
            return value1 * (1f - fraction) + value2 * fraction;
        }

        /// <summary>
        /// Calculate the distance between two points
        /// </summary>
        /// <param name="x1">X1</param>
        /// <param name="y1">Y1</param>
        /// <param name="x2">X2</param>
        /// <param name="y2">Y2</param>
        /// <returns></returns>
        public static float Math_Distance(float x1, float y1, float x2, float y2)
        {
            return Mathf.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }

        public static float Math_InterpolateSmooth2(float v1, float v2, float fraction)
        {
            float fraction2 = fraction * fraction;
            fraction = 3 * fraction2 - 2f * fraction * fraction2;
            return v1 * (1f - fraction) + v2 * fraction;
        }

        public static float Math_InterpolateCubic(float v0, float v1, float v2, float v3, float fraction)
        {
            float p = (v3 - v2) - (v0 - v1);
            float q = (v0 - v1) - p;
            float r = v2 - v0;
            float fraction2 = fraction * fraction;
            return p * fraction * fraction2 + q * fraction2 + r * fraction + v1;
        }


        /// <summary>
        /// Rotate the point around the pivot - used to handle rotation
        /// </summary>
        /// <param name="point">Point to move</param>
        /// <param name="pivot">Pivot</param>
        /// <param name="angle">Angle to pivot</param>
        /// <returns>New location</returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
        {
            Vector3 dir = point - pivot;
            dir = Quaternion.Euler(angle) * dir;
            point = dir + pivot;
            return point;
        }


        /// <summary>
        /// Outputs a Render texture as a .png file for debug purposes
        /// </summary>
        /// <param name="path">The path to write to, including filename ending in .png</param>
        /// <param name="sourceRenderTexture">The render texture to export</param>
        public static void DebugWriteRenderTexture(string path, RenderTexture sourceRenderTexture)
        {
            RenderTexture origTex = RenderTexture.active;
            RenderTexture.active = sourceRenderTexture;
            Texture2D exportTexture = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.RGBA32, false);
            exportTexture.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);
            exportTexture.Apply();
            byte[] exrBytes = ImageConversion.EncodeToPNG(exportTexture);//, Texture2D.EXRFlags.CompressZIP);
            PWCommon2.Utils.WriteAllBytes(path, exrBytes);
            RenderTexture.active = origTex;

        }

        #endregion

        #region Programming helpers

        /// <summary>
        /// Copies all fields from one object to the other, as long as they are from the same type.
        /// </summary>
        /// <param name="sourceOBJ">The source.</param>
        /// <param name="destOBJ">The destination.</param>
        public static void CopyFields(object sourceOBJ, object destOBJ)
        {

            if (sourceOBJ == null || destOBJ == null)
            {
                Debug.LogError("Error while copying object properties, source / destination not filled!");
                return;
            }

            Type typeDest = destOBJ.GetType();
            Type typeSrc = sourceOBJ.GetType();

            if (typeSrc != typeDest)
            {
                Debug.LogError("Type mismatch when trying to copy fields");
            }


            FieldInfo[] srcFields = typeSrc.GetFields();
            foreach (FieldInfo srcField in srcFields)
            {
                FieldInfo targetField = typeDest.GetField(srcField.Name);
                if (targetField == null)
                {
                    continue;
                }

                targetField.SetValue(destOBJ, srcField.GetValue(sourceOBJ));
            }
        }

        /// <summary>
        /// Returns the order number of an enum (this is NOT the enum int value, but the position in the definition of possible enum values)
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public static int GetEnumOrder<T>(T enumValue) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select((x, i) => new { item = x, index = i }).Single(x => (Enum)x.item == (Enum)enumValue).index;
        }

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

            // Don't serialize a null object, simply return the default for that object
            if (UnityEngine.Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }


        /// <summary>
        /// Generic Function to remove an array element at a certain index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputArray"></param>
        /// <param name="indexToRemove"></param>
        /// <returns></returns>
        public static T[] RemoveArrayIndexAt<T>(T[] inputArray, int indexToRemove)
        {
            T[] newArray = new T[inputArray.Length - 1];
            for (int i = 0; i < newArray.Length; ++i)
            {
                if (i < indexToRemove)
                {
                    newArray[i] = inputArray[i];
                }
                else if (i >= indexToRemove)
                {
                    newArray[i] = inputArray[i + 1];
                }
            }
            return newArray;
        }


        /// <summary>
        /// Generic Function to add a new array element at the end of an array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputArray"></param>
        /// <param name="newElement"></param>
        /// <returns></returns>
        public static T[] AddElementToArray<T>(T[] inputArray, T newElement)
        {
            T[] newArray = new T[inputArray.Length + 1];
            for (int i = 0; i < inputArray.Length; ++i)
            {
                newArray[i] = inputArray[i];
            }
            newArray[newArray.Length - 1] = newElement;
            return newArray;
        }

        /// <summary>
        /// Generic Function to insert a new array element at a certain index position in an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputArray"></param>
        /// <param name="newElement"></param>
        /// <param name="indexToInsertAt"></param>
        /// <returns></returns>
        public static T[] InsertElementInArray<T>(T[] inputArray, T newElement, int indexToInsertAt)
        {
            T[] newArray = new T[inputArray.Length + 1];
            for (int i = 0; i < newArray.Length; ++i)
            {
                if (i < indexToInsertAt)
                {
                    newArray[i] = inputArray[i];
                }
                else if (i > indexToInsertAt)
                {
                    newArray[i] = inputArray[i - 1];
                }
                else
                {
                    newArray[i] = newElement;
                }
            }

            return newArray;
        }

        /// <summary>
        /// Generic Function to swap the position of two array elements
        /// </summary>
        /// <param name="m_reorderableRuleMasksLists"></param>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        /// <returns></returns>
        public static T[] SwapElementsInArray<T>(T[] inputArray, int firstIndex, int secondIndex)
        {
            //sanity check
            int maxIndex = inputArray.Length - 1;
            if (firstIndex > maxIndex || secondIndex > maxIndex || firstIndex < 0 || secondIndex < 0)
            {
                Debug.LogError("Could not swap array elements: First Index " + firstIndex.ToString() + " or Second Index " + secondIndex.ToString() + " are out of bounds.");
                return inputArray;
            }

            T temp = inputArray[firstIndex];
            inputArray[firstIndex] = inputArray[secondIndex];
            inputArray[secondIndex] = temp;

            return inputArray;
        }


        #endregion

        #region Adhoc helpers

        /// <summary>
        /// Inverts an animation curve
        /// </summary>
        /// <param name="curve">The curve to invert</param>
        public static void InvertAnimationCurve(ref AnimationCurve curve)
        {
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int i = 0; i < curve.keys.Length; i += curve.keys.Length - 1)
            {
                if (curve.keys[i].value > maxValue)
                {
                    maxValue = curve.keys[i].value;
                }
                if (curve.keys[i].value < minValue)
                {
                    minValue = curve.keys[i].value;
                }
            }

            float middleValue = (maxValue + minValue) * 0.5f;

            Keyframe[] keys = curve.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].value = keys[i].value + 2 * (middleValue - keys[i].value);
                keys[i].inTangent *= -1f;
                keys[i].inWeight *= -1f;
                keys[i].outTangent *= -1f;
                keys[i].outWeight *= -1f;
            }
            curve.keys = keys;
        }


        public static Transform GetGOSpawnTarget(SpawnRule rule, string protoName, Terrain currentTerrain, bool enabledByDefault)
        {
            //If spawn mode is set to single transform, and we do have a transform, we are done already
            if (rule.m_goSpawnTargetMode == GaiaConstants.SpawnerTargetMode.SingleTransform && rule.m_goSpawnTarget != null)
            {
                return rule.m_goSpawnTarget;
            }

            //No transform yet? We might need to build it first then

            //The "container" is either the Gaia Game Object or the current terrain
            Transform containerTransform;

            if (rule.m_goSpawnTargetMode == GaiaConstants.SpawnerTargetMode.SingleTransform)
            {
                containerTransform = GaiaUtils.GetGaiaGameObject().transform;
            }
            else
            {
                containerTransform = currentTerrain.transform;
            }

            //The "parent" transform is the parent under which the target transforms for all spawn rules are collected under
            string parentName = GaiaConstants.defaultGOSpawnTarget;

            //Default name may be overridden by user input
            if (rule.m_goSpawnTargetMode == GaiaConstants.SpawnerTargetMode.Terrain)
            {
                parentName = rule.m_terrainGOSpawnTargetName;
            }

            Transform parentTransform = containerTransform.Find(parentName);

            if (parentTransform == null)
            {
                GameObject newParent = new GameObject();
                newParent.name = parentName;
                newParent.transform.parent = containerTransform;
                parentTransform = newParent.transform;
            }

            GaiaHierarchyUtils hierarchyUtils = parentTransform.GetComponent<GaiaHierarchyUtils>();
            if (hierarchyUtils == null)
            {
                hierarchyUtils = parentTransform.gameObject.AddComponent<GaiaHierarchyUtils>();
                hierarchyUtils.m_hideAllParentsInHierarchy = enabledByDefault;
            }

            Transform ruleTransform = parentTransform.Find(protoName);

            if (ruleTransform == null)
            {
                GameObject newGO = new GameObject();
                newGO.name = protoName;
                newGO.transform.parent = parentTransform;
                if (rule.m_goSpawnTargetMode == GaiaConstants.SpawnerTargetMode.SingleTransform)
                {
                    rule.m_goSpawnTarget = newGO.transform;
                }
                ruleTransform = newGO.transform;
            }

            return ruleTransform;

        }

        /// <summary>
        /// Get a color from a html string
        /// </summary>
        /// <param name="htmlString">Color in RRGGBB or RRGGBBBAA or #RRGGBB or #RRGGBBAA format.</param>
        /// <returns>Color or white if unable to parse it.</returns>
        public static Color GetColorFromHTML(string htmlString)
        {
            Color color = Color.white;
            if (!htmlString.StartsWith("#"))
            {
                htmlString = "#" + htmlString;
            }
            if (!ColorUtility.TryParseHtmlString(htmlString, out color))
            {
                color = Color.white;
            }
            return color;
        }

        public static Texture2D GetBGTexture(Color backgroundColor, List<Texture2D> tempTextureList)
        {
            int res = 1;

            Color[] colors = new Color[res * res];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = backgroundColor;
            }

            Texture2D tex = new Texture2D(res, res);
            tex.SetPixels(colors);
            tex.Apply(true);
            tempTextureList.Add(tex);

            return tex;
        }


        public static Texture2D GetBGTexture(Color backgroundColor, Color borderColor, List<Texture2D> tempTextureList)
        {
            int res = 6;

            Color[] colors = new Color[res * res];

            for (int x = 0; x < res; x++)
            {
                for (int y = 0; y < res; y++)
                {
                    int i = x * res + y;

                    if (x == 0 || x == res - 1 || y == 0 || y == res - 1)
                    {
                        // Apply the border color
                        colors[i] = borderColor;
                    }
                    else
                    {
                        // Apply the background color
                        colors[i] = backgroundColor;
                    }
                }
            }

            Texture2D tex = new Texture2D(res, res);
            tex.SetPixels(colors);
            tex.Apply(true);
            tempTextureList.Add(tex);

            return tex;
        }

        /// <summary>
        /// Converts a Render Texture to texture 2D by reading the pixels from it.
        /// </summary>
        /// <param name="renderTexture"></param>
        /// <param name="texture"></param>
        public static Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
        {
            Texture2D output = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.R16, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            output.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            output.Apply();
            RenderTexture.active = currentRT;
            return output;
        }


        /// <summary>
        /// Checks if two colors are equal or "look the same" according to their argb color values. 
        /// A regular comparison with "==" or .Equals does not work to compare if two colors are "the same" since the objects can still have different fields beyond the argb values.
        /// </summary>
        /// <param name="firstColor">first color to compare</param>
        /// <param name="secondColor">second color to compare</param>
        /// <returns></returns>
        public static bool ColorsEqual(Color firstColor, Color secondColor)
        {
            return (firstColor.r == secondColor.r) &&
                    (firstColor.g == secondColor.g) &&
                    (firstColor.b == secondColor.b) &&
                    (firstColor.a == secondColor.a);
        }


        /// <summary>
        /// Workaround to release all temporary render textures. Even when Releasing temporary render textures in code, it can still
        /// happen that additional temp render textures stay in memory, this goes over all temporary render textures and releases them.
        /// </summary>
        public static void ReleaseAllTempRenderTextures()
        {
            var rendTex = (RenderTexture[])Resources.FindObjectsOfTypeAll(typeof(RenderTexture));
            for (int i = rendTex.Length - 1; i >= 0; i--)
            {
                if (rendTex[i].name.StartsWith("TempBuffer"))
                {
                    RenderTexture.ReleaseTemporary(rendTex[i]);
                }
            }
            GC.Collect();
        }

        /// <summary>
        /// Calculates the scalar u/v position on a terrain from a world space position
        /// </summary>
        /// <param name="terrain">The terrain for which to perform the calculation.</param>
        /// <param name="worldSpacePosition">The world space position to transform to UV space.</param>
        /// <returns></returns>
        public static Vector2 ConvertPositonToTerrainUV(Terrain terrain, Vector2 worldSpacePosition)
        {
            float u = (worldSpacePosition.x - terrain.transform.position.x) / terrain.terrainData.size.x;
            float v = (worldSpacePosition.y - terrain.transform.position.z) / terrain.terrainData.size.z;
            return new Vector2(u, v);
        }

        public static long GetUnixTimestamp()
        {
            return new System.DateTimeOffset(System.DateTime.UtcNow).ToUnixTimeMilliseconds();
        }



        /// <summary>
        /// Returns true if the given operation is a stamping operation 
        /// </summary>
        /// <param name="operation">The operation to check against</param>
        /// <returns></returns>
        public static bool IsStampOperation(GaiaConstants.FeatureOperation operation)
        {
            return operation == GaiaConstants.FeatureOperation.RaiseHeight ||
                   operation == GaiaConstants.FeatureOperation.LowerHeight ||
                   operation == GaiaConstants.FeatureOperation.BlendHeight ||
                   operation == GaiaConstants.FeatureOperation.SetHeight ||
                   operation == GaiaConstants.FeatureOperation.AddHeight ||
                   operation == GaiaConstants.FeatureOperation.SubtractHeight;
        }



        /// <summary>
        /// Return GaiaSettings or null;
        /// </summary>
        /// <returns>Gaia settings or null if not found</returns>
        public static GaiaSettings GetGaiaSettings()
        {
            return GetAsset("GaiaSettings.asset", typeof(Gaia.GaiaSettings)) as Gaia.GaiaSettings;
        }


        /// <summary>
        /// Returns the default Gaia Game Object from the scene hierarchy or creates it if it does not exist
        /// </summary>
        /// <returns></returns>
        public static GameObject GetGaiaGameObject()
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            return gaiaObj;
        }

        /// <summary>
        /// Returns the default Game Object for Global Gaia Objects (Water, Wind, etc.) from the scene hierarchy or creates it if it does not exist
        /// </summary>
        /// <returns></returns>
        public static GameObject GetGlobalSceneObject()
        {
            GameObject gaiaObj = GameObject.Find(GaiaConstants.gaiaGlobalObjects);
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject(GaiaConstants.gaiaGlobalObjects);
            }
            return gaiaObj;
        }

        /// <summary>
        /// Returns the default Game Object for Temporary Session Tools from the scene hierarchy or creates it if it does not exist
        /// </summary>
        /// <returns></returns>
        public static GameObject GetTempSessionToolsObject()
        {
            GameObject gaiaObj = GameObject.Find(GaiaConstants.gaiaTempSessionToolsObject);
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject(GaiaConstants.gaiaTempSessionToolsObject);
                gaiaObj.transform.SetParent(GetGaiaGameObject().transform);
            }
            return gaiaObj;
        }

        /// <summary>
        /// Returns the default Game Object for Stopwatch Data from the scene hierarchy or creates it if it does not exist
        /// </summary>
        /// <returns></returns>
        public static GameObject GetStopwatchDataObject()
        {
            GameObject gaiaObj = GameObject.Find(GaiaConstants.gaiaStopWatchDataObject);
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject(GaiaConstants.gaiaStopWatchDataObject);
            }
            return gaiaObj;
        }

        /// <summary>
        /// Returns the default Game Object for Terrain Objects from the scene hierarchy or creates it if it does not exist
        /// </summary>
        /// <returns></returns>
        public static GameObject GetTerrainObject(bool create = true)
        {
            GameObject gaiaObj = GameObject.Find(GaiaConstants.gaiaTerrainObjects);
            //still null? lets search deactivated objects as well
            if (gaiaObj == null)
            {
                gaiaObj = GaiaUtils.FindObjectDeactivated(GaiaConstants.gaiaTerrainObjects, true);
            }
            //still null? lets create
            if (gaiaObj == null && create)
            {
                gaiaObj = new GameObject(GaiaConstants.gaiaTerrainObjects);
            }
            return gaiaObj;
        }

        /// <summary>
        /// Returns the default Game Object for Terrain Loader Manager Objects from the scene hierarchy or creates it if it does not exist
        /// </summary>
        /// <returns></returns>
        public static GameObject GetTerrainLoaderManagerObject()
        {
            GameObject loaderMgrObj = GameObject.Find(GaiaConstants.gaiaTerrainLoaderManagerObjects);
            if (loaderMgrObj == null)
            {
                GameObject gaiaObject = GetGaiaGameObject();
                loaderMgrObj = new GameObject(GaiaConstants.gaiaTerrainLoaderManagerObjects);
                loaderMgrObj.transform.parent = gaiaObject.transform;
                //Add the overview script per default
                loaderMgrObj.AddComponent<GaiaTerrainLoaderManager>();
            }
            return loaderMgrObj;
        }

        public static GameObject GetPlayerObject()
        {
            GameObject gaiaObj = GameObject.Find(GaiaConstants.gaiaPlayerObject);
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject(GaiaConstants.gaiaPlayerObject);
                gaiaObj.transform.SetParent(GetGlobalSceneObject().transform);
            }
            return gaiaObj;
        }

        public static bool DisplayDialogNoEditor(string title, string message, string ok, string cancel, bool resultIfNoDialog = false, string NoDialogFailedConsoleText = "")
        {
#if UNITY_EDITOR
            return EditorUtility.DisplayDialog(title, message, ok, cancel);
#else
            if (!resultIfNoDialog)
            {
                Debug.LogError(NoDialogFailedConsoleText);
            }
            return resultIfNoDialog;
#endif
        }

        public static bool UsesFloatingPointFix()
        {
            return GaiaTerrainLoaderManager.Instance.TerrainSceneStorage.m_useFloatingPointFix;
        }

        public static bool HasDynamicLoadedTerrains()
        {
            return GaiaTerrainLoaderManager.TerrainScenes.Count >= 1;
        }

        public static void FocusLightingProfile()
        {
            GaiaSettings settings = GaiaUtils.GetGaiaSettings();
            if (settings != null)
            {
                if (settings.m_gaiaLightingProfile != null)
                {
#if UNITY_EDITOR
                    Selection.activeObject = settings.m_gaiaLightingProfile;
#endif
                }
                else
                {
                    Debug.LogError("Lighting Profile in Gaia Settings has not been assigned");
                }
            }
            else
            {
                Debug.LogError("Unable to find Gaia Settings");
            }
        }

        public static void FocusWaterProfile()
        {
            GaiaSettings settings = GaiaUtils.GetGaiaSettings();
            if (settings != null)
            {
                if (settings.m_gaiaLightingProfile != null)
                {
#if UNITY_EDITOR
                    Selection.activeObject = settings.m_gaiaWaterProfile;
#endif
                }
                else
                {
                    Debug.LogError("Water Profile in Gaia Settings has not been assigned");
                }
            }
            else
            {
                Debug.LogError("Unable to find Gaia Settings");
            }
        }

        public static int EnvironmentSizeToInt(EnvironmentSize size)
        {
            switch (size)
            {
                case GaiaConstants.EnvironmentSize.Is256MetersSq:
                    return 256;
                case GaiaConstants.EnvironmentSize.Is512MetersSq:
                    return 512;
                case GaiaConstants.EnvironmentSize.Is1024MetersSq:
                    return 1024;
                case GaiaConstants.EnvironmentSize.Is2048MetersSq:
                    return 2048;
                case GaiaConstants.EnvironmentSize.Is4096MetersSq:
                    return 4096;
                case GaiaConstants.EnvironmentSize.Is8192MetersSq:
                    return 8192;
                case GaiaConstants.EnvironmentSize.Is16384MetersSq:
                    return 16384;
            }
            return 0;
        }

        /// <summary>
        /// Select or create the WorldMapEditor
        /// </summary>
        public static GameObject GetOrCreateWorldDesigner()
        {
            GameObject worldMapObj = GameObject.Find(GaiaConstants.worldDesignerObject);
            if (worldMapObj == null)
            {
                GameObject gaiaObj = GaiaUtils.GetGaiaGameObject();
                worldMapObj = new GameObject(GaiaConstants.worldDesignerObject);
                worldMapObj.transform.parent = gaiaObj.transform;
                worldMapObj.transform.position = Vector3.zero;
                WorldMap worldMap = worldMapObj.AddComponent<WorldMap>();
                worldMap.hideFlags = HideFlags.HideInInspector;
                //worldMapObj.transform.hideFlags = HideFlags.HideInInspector;
                worldMap.CreateWorldMapTerrain();
                worldMap.SyncLocalMapToWorldMap();
                WorldMap.ShowWorldMapStampSpawner();
            }
            return worldMapObj;
        }

        /// <summary>
        /// Select or create the WorldMap Temp tools object
        /// </summary>
        public static GameObject GetOrCreateWorldMapTempTools()
        {
            GameObject worldMapTempTools = GameObject.Find(GaiaConstants.worldTempTools);
            if (worldMapTempTools == null)
            {
                GameObject worldMapObj = GetOrCreateWorldDesigner();
                worldMapTempTools = new GameObject(GaiaConstants.worldTempTools);
                worldMapTempTools.transform.parent = worldMapObj.transform;
            }
            return worldMapTempTools;
        }

        public static void SetSettingsForEnvironment(GaiaSettings settings, EnvironmentTarget targetEnv)
        {
            switch (targetEnv)
            {
                case GaiaConstants.EnvironmentTarget.UltraLight:
                    settings.m_currentDefaults.m_heightmapResolution = 33;
                    settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 8, 256, 512);
                    settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 8, 128, 512);
                    settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 8, 64, 512);
                    settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 8, 64, 512);
                    break;
                case GaiaConstants.EnvironmentTarget.MobileAndVR:
                    settings.m_currentDefaults.m_heightmapResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 4, 64, 512) + 1;
                    settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 4, 256, 512);
                    settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 4, 64, 512);
                    settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 4, 64, 512);
                    settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 4, 64, 512);
                    break;
                case GaiaConstants.EnvironmentTarget.Desktop:
                    settings.m_currentDefaults.m_heightmapResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 2, 256, 2048) + 1;
                    settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 2, 256, 2048);
                    settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 2, 128, 4096);
                    settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 2, 256, 2048);
                    settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize / 2, 256, 2048);
                    break;
                case GaiaConstants.EnvironmentTarget.PowerfulDesktop:
                    settings.m_currentDefaults.m_heightmapResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize, 256, 4096) + 1;
                    settings.m_currentDefaults.m_baseMapDist = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize, 256, 2048);
                    settings.m_currentDefaults.m_detailResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize, 256, 4096);
                    settings.m_currentDefaults.m_controlTextureResolution = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize, 256, 2048);
                    settings.m_currentDefaults.m_baseMapSize = Mathf.Clamp(settings.m_currentDefaults.m_terrainSize, 256, 2048);
                    break;
                case GaiaConstants.EnvironmentTarget.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }





        #endregion
    }
}