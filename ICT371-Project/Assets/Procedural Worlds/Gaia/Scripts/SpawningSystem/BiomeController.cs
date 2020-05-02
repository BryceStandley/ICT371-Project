﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
using static Gaia.GaiaConstants;
namespace Gaia
{



    /// <summary>
    /// A generic spawning system.
    /// </summary>
    [System.Serializable]
    public class BiomeController : MonoBehaviour
    {

        [SerializeField]
        private BiomeControllerSettings settings;
        /// <summary>
        /// The current spawner settings
        /// </summary>
        public BiomeControllerSettings m_settings
        {
            get
            {
                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<BiomeControllerSettings>();
                    settings.name = this.name;
                }
                return settings;
            }
            set
            {
                settings = value;
            }

        }

        public List<AutoSpawner> m_autoSpawners = new List<AutoSpawner>();
#if UNITY_POST_PROCESSING_STACK_V2
        public PostProcessProfile m_postProcessProfile;
        public BiomePostProcessingVolumeSpawnMode m_ppVSpawnMode = BiomePostProcessingVolumeSpawnMode.Add;

#endif

        public bool m_drawPreview;
        public bool m_biomePreviewDirty;
        private Terrain m_lastActiveTerrain;
        private float m_minWorldHeight;
        private float m_maxWorldHeight;
        private RenderTexture m_cachedPreviewRT;

        private GaiaSettings m_gaiaSettings;
        private GaiaSettings GaiaSettings
        {
            get
            {
                if (m_gaiaSettings == null)
                {
                    m_gaiaSettings = GaiaUtils.GetGaiaSettings();
                }
                return m_gaiaSettings;
            }
        }

        public AutoSpawnerArea m_autoSpawnerArea = AutoSpawnerArea.Local;


        public bool m_showSeaLevelPlane = true;
        public bool m_showSeaLevelinPreview = true;
        public bool m_showBoundingBox = true;

#if GAIA_PRO_PRESENT
        private GaiaTerrainLoader m_terrainLoader;
        public LoadMode m_loadTerrainMode = LoadMode.Disabled;
        public GaiaTerrainLoader TerrainLoader
        {
            get
            {
                if (m_terrainLoader == null)
                {
                    if (this != null)
                    {
                        m_terrainLoader = gameObject.GetComponent<GaiaTerrainLoader>();

                        if (m_terrainLoader == null)
                        {
                            m_terrainLoader = gameObject.AddComponent<GaiaTerrainLoader>();
                            m_terrainLoader.hideFlags = HideFlags.HideInInspector;
                        }
                    }
                }
                return m_terrainLoader;
            }
        }
#endif

        private GaiaSessionManager m_sessionManager;
        private GaiaSessionManager SessionManager
        {
            get
            {
                if (m_sessionManager == null)
                {
                    m_sessionManager = GaiaSessionManager.GetSessionManager(false);
                }
                return m_sessionManager;
            }
        }

        public void UpdateAutoLoadRange()
        {
#if GAIA_PRO_PRESENT
            if (m_loadTerrainMode != LoadMode.Disabled)
            {
                float width = m_settings.m_range * 2f;
                //reduce the loading width a bit => this is to prevent loading in terrains when the spawner bounds end exactly at the border of
                //surrounding terrains, this loads in a lot of extra terrains which are not required for the spawn 
                width -= 0.5f;
                TerrainLoader.m_loadingBounds = new BoundsDouble(transform.position, new Vector3(width, width, width));
            }
            TerrainLoader.LoadMode = m_loadTerrainMode;
#endif
        }

        //void OnEnable()
        //{
        //    if (m_gaiaSettings == null)
        //    {
        //        m_gaiaSettings = GaiaUtils.GetGaiaSettings();
        //    }
        //}


        void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            if (Selection.activeObject == gameObject)
            {
                if (m_showBoundingBox)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(transform.position, new Vector3(m_settings.m_range * 2f, m_settings.m_range * 2f, m_settings.m_range * 2f));
                }

                //Water
                if (m_showSeaLevelPlane)
                {
                    BoundsDouble bounds = new BoundsDouble();
                    if (TerrainHelper.GetTerrainBounds(ref bounds) == true)
                    {
                        bounds.center = new Vector3Double(bounds.center.x, SessionManager.GetSeaLevel(), bounds.center.z);
                        bounds.size = new Vector3Double(bounds.size.x, 0.05f, bounds.size.z);
                        Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, Color.blue.a / 4f);
                        Gizmos.DrawCube(bounds.center, bounds.size);
                    }
                }
            }
#endif
        }


        public void LoadSettings(BiomeControllerSettings settingsToLoad)
        {
            //set position according to the stored settings
            transform.position = new Vector3(settingsToLoad.m_x, settingsToLoad.m_y, settingsToLoad.m_z);

            //Set existing settings = null to force a new scriptable object
            m_settings = null;
            m_settings = Instantiate(settingsToLoad);
        }

        public Terrain GetCurrentTerrain()
        {
            Terrain currentTerrain = Gaia.TerrainHelper.GetTerrain(transform.position, false);
            //Check if the stamper is over a terrain currently
            //if not, we will draw a preview based on the last active terrain we were over
            //if that is null either we can't draw a stamp preview
            if (currentTerrain)
            {
                //Update last active terrain with current
                if (m_lastActiveTerrain != currentTerrain)
                {
                    //if the current terrain is a new terrain, we should refresh the min max values in case this terrain has never been calculated before
                    SessionManager.GetWorldMinMax(ref m_minWorldHeight, ref m_maxWorldHeight);
                }
                m_lastActiveTerrain = currentTerrain;
            }
            //if not, we check if there is any terrain within the bounds of the biome spawner
            if (currentTerrain == null)
            {
                float width = m_settings.m_range * 2f;
                Bounds stamperBounds = new Bounds(transform.position, new Vector3(width, width, width));

                foreach (Terrain t in Terrain.activeTerrains)
                {
                    //only look at this terrain if it matches the selected world map mode
                    if (!TerrainHelper.IsWorldMapTerrain(t))
                    {
                        Bounds worldSpaceBounds = t.terrainData.bounds;
                        worldSpaceBounds.center = new Vector3(worldSpaceBounds.center.x + t.transform.position.x, worldSpaceBounds.center.y + t.transform.position.y, worldSpaceBounds.center.z + t.transform.position.z);

                        if (worldSpaceBounds.Intersects(stamperBounds))
                        {
                            currentTerrain = t;
                            break;
                        }
                    }
                }
            }
            return currentTerrain;
        }

        public void DrawBiomePreview()
        {
            if (m_drawPreview)
            {

                //Set up a multi-terrain operation once, all rules can then draw from the data collected here
                Terrain currentTerrain = GetCurrentTerrain();
                if (currentTerrain == null)
                {
                    return;
                }

                GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_settings.m_range * 2f);
                operation.GetHeightmap();


                //only re-generate all textures etc. if settings have changed and the preview is dirty, otherwise we can just use the cached textures
                if (m_biomePreviewDirty == true)
                {
                    //Get additional op data (required for certain image masks)
                    operation.GetNormalmap();
                    operation.CollectTerrainBakedMasks();

                    //Clear texture cache first
                    if (m_cachedPreviewRT != null)
                    {
                        m_cachedPreviewRT.Release();
                        DestroyImmediate(m_cachedPreviewRT);
                    }

                    m_cachedPreviewRT = new RenderTexture(operation.RTheightmap);
                    RenderTexture currentRT = RenderTexture.active;
                    RenderTexture.active = m_cachedPreviewRT;
                    GL.Clear(true, true, Color.black);
                    RenderTexture.active = currentRT;

                    Graphics.Blit(ApplyBrush(operation), m_cachedPreviewRT);
                    RenderTexture.active = currentRT;
                    //ImageProcessing.WriteRenderTexture("D:\\previewRT.png", m_cachedPreviewRT);
                    //Everything processed, preview not dirty anymore
                    m_biomePreviewDirty = false;
                }

                //Now draw the preview according to the cached textures
                Material material = GaiaMultiTerrainOperation.GetDefaultGaiaSpawnerPreviewMaterial();
                material.SetInt("_zTestMode", (int)UnityEngine.Rendering.CompareFunction.Always);

                //assign the first color texture in the material
                material.SetTexture("_colorTexture0", m_cachedPreviewRT);

                //remove all other potential color textures, there can be caching issues if other visualisers were used in the meantime

                for (int colorIndex = 1; colorIndex < GaiaConstants.maxPreviewedTextures; colorIndex++)
                {
                    material.SetTexture("_colorTexture" + colorIndex, null);
                }



                //set the color
                material.SetColor("_previewColor0", m_settings.m_visualisationColor);

                Color seaLevelColor = GaiaSettings.m_stamperSeaLevelTintColor;
                if (!m_showSeaLevelinPreview)
                {
                    seaLevelColor.a = 0f;
                }
                material.SetColor("_seaLevelTintColor", seaLevelColor);
                material.SetFloat("_seaLevel", SessionManager.m_session.m_seaLevel);
                operation.Visualize(MultiTerrainOperationType.Heightmap, operation.RTheightmap, material, 1);

                //Clean up
                operation.CloseOperation();
                //Clean up temp textures
                GaiaUtils.ReleaseAllTempRenderTextures();
            }
        }



        private RenderTexture ApplyBrush(GaiaMultiTerrainOperation operation)
        {
            Terrain currentTerrain = GetCurrentTerrain();

            RenderTextureDescriptor rtDescriptor = operation.RTheightmap.descriptor;
            //Random write needs to be enabled for certain mask types to function!
            rtDescriptor.enableRandomWrite = true;

            RenderTexture inputTexture = RenderTexture.GetTemporary(rtDescriptor);

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = inputTexture;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = currentRT;

            //Iterate through all image masks and set up the current paint context in case the shader uses heightmap data
            foreach (ImageMask mask in m_settings.m_imageMasks)
            {
                mask.m_multiTerrainOperation = operation;
                mask.m_seaLevel = SessionManager.GetSeaLevel();
                mask.m_maxWorldHeight = m_maxWorldHeight;
                mask.m_minWorldHeight = m_minWorldHeight;
            }


            //Get the combined masks for the biome 
            RenderTexture biomeOutputTexture = RenderTexture.GetTemporary(rtDescriptor);
            Graphics.Blit(ImageProcessing.ApplyMaskStack(inputTexture, m_settings.m_imageMasks, ImageMaskInfluence.Local), biomeOutputTexture);
            //ImageProcessing.WriteRenderTexture("D:\\previewRT.png", m_cachedPreviewRT);


            //if (opType == MultiTerrainOperationType.Tree)
            //{
            //    ImageProcessing.WriteRenderTexture("D:\\spawnerOutputTexture.png", spawnerOutputTexture);
            //    ImageProcessing.WriteRenderTexture("D:\\ruleOutputTexture.png", ruleOutputTexture);
            //}




            //clean up temporary textures
            ReleaseRenderTexture(inputTexture);
            inputTexture = null;
            return biomeOutputTexture;
        }


        private void ReleaseRenderTexture(RenderTexture texture)
        {
            if (texture != null)
            {
                RenderTexture.ReleaseTemporary(texture);
                texture = null;
            }
        }

        public void RemoveForeignTrees(List<SpawnerSettings> biomeSpawnerSettings, List<string> validTerrainNames = null)
        {
            //GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign Trees", "Preparing...", 0);
            //List<GameObject> knownTreePrefabs = new List<GameObject>();
            //List<TreePrototype> treeProtosToRemove = new List<TreePrototype>();
            //Terrain currentTerrain = GetCurrentTerrain();
            //foreach (SpawnerSettings spawnerSettings in biomeSpawnerSettings)
            //{
            //    foreach (SpawnRule sr in spawnerSettings.m_spawnerRules)
            //    {
            //        if (sr.m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTree)
            //        {
            //            knownTreePrefabs.Add(spawnerSettings.m_resources.m_treePrototypes[sr.m_resourceIdx].m_desktopPrefab);
            //        }
            //    }
            //}

            //GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_settings.m_range * 2f,false, validTerrainNames);
            //operation.GetHeightmap();
            //operation.GetNormalmap();
            //operation.CollectTerrainDetails();
            //operation.CollectTerrainTrees();
            //operation.CollectTerrainGameObjects();
            //operation.CollectTerrainBakedMasks();

            //int protoIndex = 0;
            //foreach (TreePrototype t in currentTerrain.terrainData.treePrototypes)
            //{
            //    GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign Trees", "Removing Trees...", (float)(protoIndex + 1) / (float)(currentTerrain.terrainData.treePrototypes.Count()));
            //    if (!knownTreePrefabs.Contains(t.prefab))
            //    {
            //        int counter = 0;
            //        operation.SetTerrainTrees(ApplyBrush(operation), protoIndex, null, null, GaiaConstants.SpawnMode.Remove, 0, ref counter, m_settings.m_removeForeignTreesStrength);
            //    }
            //    protoIndex++;
            //}
            //GaiaUtils.ClearProgressBarNoEditor();
            //operation.CloseOperation();
        }

        public void RemoveForeignGameObjects(List<SpawnerSettings> biomeSpawnerSettings, List<string> validTerrainNames=null)
        {
            GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign GameObjects", "Preparing...", 0);
            List<ResourceProtoGameObjectInstance> knownProtoInstances = new List<ResourceProtoGameObjectInstance>();
            List<ResourceProtoGameObjectInstance> GoProtoInstancesToRemove = new List<ResourceProtoGameObjectInstance>();
            Terrain currentTerrain = GetCurrentTerrain();
            foreach (SpawnerSettings spawnerSettings in biomeSpawnerSettings)
            {
                foreach (SpawnRule sr in spawnerSettings.m_spawnerRules)
                {
                    if (sr.m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
                    {
                        foreach (ResourceProtoGameObjectInstance instance in spawnerSettings.m_resources.m_gameObjectPrototypes[sr.m_resourceIdx].m_instances)
                        {
                            knownProtoInstances.Add(instance);
                        }
                    }
                }
            }

            GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_settings.m_range * 2f, false, validTerrainNames);
            operation.GetHeightmap();
            operation.GetNormalmap();
            operation.CollectTerrainDetails();
            operation.CollectTerrainTrees();
            operation.CollectTerrainGameObjects();
            operation.CollectTerrainBakedMasks();

            int protoIndex = 0;
            var allSpawners = Resources.FindObjectsOfTypeAll<Spawner>();
            foreach (Spawner spawner in allSpawners)
            {
                GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign GameObjects", "Removing Game Objects...", (float)(protoIndex + 1) / (float)(allSpawners.Length));
                foreach (SpawnRule sr in spawner.m_settings.m_spawnerRules)
                {
                    if (sr.m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
                    {
                        ResourceProtoGameObject protoGO = spawner.m_settings.m_resources.m_gameObjectPrototypes[sr.m_resourceIdx];
                        foreach (ResourceProtoGameObjectInstance instance in protoGO.m_instances)
                        {
                            if (!knownProtoInstances.Contains(instance))
                            {
                                operation.SetTerrainGameObjects(ApplyBrush(operation), protoGO, sr, GaiaConstants.SpawnMode.Remove, 0, ref sr.m_spawnedInstances, m_settings.m_removeForeignGameObjectStrength,false,GaiaSettings.m_hideObjectsInHierachy);
                                //no need to look at other instances if this one triggered the removal already
                                break;
                            }
                        }
                    }
                }
                protoIndex++;
            }
            operation.CloseOperation();

#if UNITY_EDITOR
            //need to dirty the scene when we remove game objects
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
            GaiaUtils.ClearProgressBarNoEditor();
        }

        public void RemoveForeignTerrainDetails(List<SpawnerSettings> biomeSpawnerSettings, List<string> validTerrainNames = null)
        {
            //GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign Terrain Details", "Preparing...", 0);
            //List<ResourceProtoDetail> knownTerrainDetails = new List<ResourceProtoDetail>();
            //List<DetailPrototype> detailProtosToRemove = new List<DetailPrototype>();
            //Terrain currentTerrain = GetCurrentTerrain();
            //foreach (SpawnerSettings spawnerSettings in biomeSpawnerSettings)
            //{
            //    foreach (SpawnRule sr in spawnerSettings.m_spawnerRules)
            //    {
            //        if (sr.m_resourceType == GaiaConstants.SpawnerResourceType.TerrainDetail)
            //        {
            //            knownTerrainDetails.Add(spawnerSettings.m_resources.m_detailPrototypes[sr.m_resourceIdx]);
            //        }
            //    }
            //}

            //GaiaMultiTerrainOperation operation = new GaiaMultiTerrainOperation(currentTerrain, transform, m_settings.m_range * 2f, false, validTerrainNames);
            //operation.GetHeightmap();
            //operation.GetNormalmap();
            //operation.CollectTerrainDetails();
            //operation.CollectTerrainTrees();
            //operation.CollectTerrainGameObjects();
            //operation.CollectTerrainBakedMasks();

            //int protoIndex = 0;
            //foreach (DetailPrototype t in currentTerrain.terrainData.detailPrototypes)
            //{
            //    GaiaUtils.DisplayProgressBarNoEditor("Removing Foreign GameObjects", "Removing Game Objects...", (float)(protoIndex + 1) / (float)(currentTerrain.terrainData.detailPrototypes.Count()));
            //    if ((knownTerrainDetails.Find(x => x.m_detailProtoype == t.prototype) == null || t.prototype == null) && (knownTerrainDetails.Find(x => x.m_detailTexture == t.prototypeTexture) == null || t.prototypeTexture == null))
            //    {
            //        int counter = 0;
            //        operation.SetTerrainDetails(ApplyBrush(operation), GaiaConstants.SpawnMode.Remove, 0.5f, 0.6f, m_settings.m_removeForeignTerrainDetailsDensity, 0, ref counter);
            //    }
            //    protoIndex++;
            //}
            //operation.CloseOperation();
            //GaiaUtils.ClearProgressBarNoEditor();
        }
    }


}