using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gaia
{

    public class GaiaTerrainLoaderManager : MonoBehaviour
    {


        /// <summary>
        /// Loading Bounds around the world origin - controlled directly in the scene view and displays the part of the world the user wants to edit.
        /// Use Get/SetLoadingRange and Get/SetOrigin to access
        /// </summary>
        private BoundsDouble m_originLoadingBounds = new BoundsDouble(Vector3Double.zero, new Vector3Double(500f, 500f, 500f));
#if GAIA_PRO_PRESENT
        public List<FloatingPointFixMember> m_allFloatingPointFixMembers = new List<FloatingPointFixMember>();
        public List<ParticleSystem> m_allWorldSpaceParticleSystems = new List<ParticleSystem>();
#endif
        public int m_originTargetTileX;
        public int m_originTargetTileZ;
        public long m_terrainUnloadMemoryTreshold = 4294967296;

        private static GaiaTerrainLoaderManager instance = null;

        public static GaiaTerrainLoaderManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GaiaUtils.GetTerrainLoaderManagerObject().GetComponent<GaiaTerrainLoaderManager>();
                }

                return instance;
            }
        }

        [SerializeField]
        private TerrainSceneStorage m_terrainSceneStorage;
        public TerrainSceneStorage TerrainSceneStorage
        {
            get
            {
                if (m_terrainSceneStorage == null)
                {

                    LoadStorageData(); 

                }
                return m_terrainSceneStorage;
            }
        }

        public void ResetStorage()
        {
            m_terrainSceneStorage = null;
        }

        public void LoadStorageData()
        {
#if UNITY_EDITOR
            GaiaSessionManager gsm = GaiaSessionManager.GetSessionManager();
            if (gsm != null && gsm.m_session!=null)
            {
                string path = GaiaDirectories.GetScenePath(gsm.m_session) + "/TerrainScenes.asset";
                if (File.Exists(path))
                {
                    m_terrainSceneStorage = (TerrainSceneStorage)AssetDatabase.LoadAssetAtPath(path, typeof(TerrainSceneStorage));
                }
                else
                {
                    m_terrainSceneStorage = ScriptableObject.CreateInstance<TerrainSceneStorage>();
                }
            }
            else
            {
                m_terrainSceneStorage = ScriptableObject.CreateInstance<TerrainSceneStorage>();
            }
#endif
        }

        private Terrain m_worldMapTerrain;
        public Terrain WorldMapTerrain
        {
            get
            {
                if (m_worldMapTerrain == null)
                {
                    m_worldMapTerrain = TerrainHelper.GetWorldMapTerrain();
                }
                return m_worldMapTerrain;
            }
        }


        public static List<TerrainScene> TerrainScenes
        {
            get
            {
                return Instance.TerrainSceneStorage.m_terrainScenes;
            }
        }


        private bool m_showWorldMapTerrain;
        public bool ShowWorldMapTerrain
        {
            get
            {
                return m_showWorldMapTerrain;
            }
            private set
            {
                m_showWorldMapTerrain = value;
                if (WorldMapTerrain != null)
                {
                    if (m_showWorldMapTerrain)
                    {
                        WorldMapTerrain.gameObject.SetActive(true);
                    }
                    else
                    {
                        WorldMapTerrain.gameObject.SetActive(false);
                    }
                }
            }
        }

        private bool m_showLocalTerrain = true;
        public bool ShowLocalTerrain
        {
            get
            {
                return m_showLocalTerrain;
            }
            private set
            {
                if (value != m_showLocalTerrain)
                {
                    m_showLocalTerrain = value;
                    if (GaiaUtils.HasDynamicLoadedTerrains())
                    {
                        if (!m_showLocalTerrain)
                        {
                            GaiaTerrainLoaderManager.Instance.UnloadAll();
                        }
                    }
                    else
                    {
                        GameObject terrainObject = GaiaUtils.GetTerrainObject();
                        foreach (Transform child in terrainObject.transform)
                        {
                            Terrain t = child.GetComponent<Terrain>();
                            if (t != null)
                            {
                                t.drawHeightmap = m_showLocalTerrain;
                                t.drawTreesAndFoliage = m_showLocalTerrain;
                                //Activate / deactivate all Childs below the terrain
                                foreach (Transform subTrans in t.transform)
                                {
                                    subTrans.gameObject.SetActive(m_showLocalTerrain);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                if (instance != this)
                {
                    Destroy(this);
                }
            }
            UnloadAll();
            if (Application.isPlaying)
            {
                //SwitchToLocalMap();
            }
        }

        public void Reset()
        {
#if GAIA_PRO_PRESENT
            m_allFloatingPointFixMembers.Clear();
            m_allWorldSpaceParticleSystems.Clear();
#endif
        }

        void OnApplicationQuit()
        {
            UnloadAll();
        }

        private void OnEnable()
        {

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                if (instance != this)
                {
                    Destroy(this);
                }
            }
            GaiaSettings gaiaSettings = GaiaUtils.GetGaiaSettings();
            //m_terrainUnloadMemoryTreshold = gaiaSettings.m_terrainUnloadMemoryTreshold;
        }

        public Vector3Double GetOrigin()
        {
            return new Vector3Double(m_originLoadingBounds.center);
        }

        public void SetOrigin(Vector3Double newOrigin)
        {
#if GAIA_PRO_PRESENT
            if (newOrigin != m_originLoadingBounds.center)
            {
                //a origin shift has occured, 
                Vector3Double shiftDifference = newOrigin - m_originLoadingBounds.center;

                //Don't shift on y-axis this will only lead to problems with sea level, height based rules, etc.
                //and should not be required under normal circumstances.
                shiftDifference.y = 0;

                //shift all tools such as stampers and spawners
                //Stamper[] allStampers = Resources.FindObjectsOfTypeAll<Stamper>();
                //foreach (Stamper stamper in allStampers)
                //{
                //    stamper.transform.position = (Vector3)((Vector3Double)stamper.transform.position + m_originLoadingBounds.center - shiftDifference);
                //}

                Spawner[] allSpawners = Resources.FindObjectsOfTypeAll<Spawner>();
                foreach (Spawner spawner in allSpawners)
                {
                    spawner.transform.position = (Vector3)((Vector3Double)spawner.transform.position - shiftDifference);
                }

                WorldMapStampToken[] mapStampTokens = Resources.FindObjectsOfTypeAll<WorldMapStampToken>();
                foreach (WorldMapStampToken token in mapStampTokens)
                {
                    token.UpdateGizmoPos();
                }

                //When the application is not playing we can look for all floating point fix members, if it is playing we should
                //rely on the list of members being filled correctly at the start of the scene
                if (!Application.isPlaying)
                {
                    m_allFloatingPointFixMembers = Resources.FindObjectsOfTypeAll<FloatingPointFixMember>().ToList();
                }
                m_allFloatingPointFixMembers.RemoveAll(x => x == null);
                foreach (FloatingPointFixMember member in m_allFloatingPointFixMembers)
                {
                    member.transform.position = (Vector3)((Vector3Double)member.transform.position - shiftDifference);
                }


                //shift world space particles accordingly - only worth dealing with during playmode 
                if (Application.isPlaying)
                {
                    m_allWorldSpaceParticleSystems.RemoveAll(x => x == null);
                    foreach (ParticleSystem ps in m_allWorldSpaceParticleSystems)
                    {
                        bool wasPaused = ps.isPaused;
                        bool wasPlaying = ps.isPlaying;
                        ParticleSystem.Particle[] currentParticles = null;

                        if (!wasPaused)
                            ps.Pause();

                        if (currentParticles == null || currentParticles.Length < ps.main.maxParticles)
                        {
                            currentParticles = new ParticleSystem.Particle[ps.main.maxParticles];
                        }

                        int num = ps.GetParticles(currentParticles);

                        for (int i = 0; i < num; i++)
                        {
                            currentParticles[i].position -= (Vector3)shiftDifference;
                        }

                        ps.SetParticles(currentParticles, num);

                        if (wasPlaying)
                            ps.Play();
                    }
                }

                m_originLoadingBounds.center = newOrigin;

                //if (WorldMapTerrain != null)
                //{
                //    WorldMapTerrain.transform.position = -m_originLoadingBounds.center - (new Vector3Double(WorldMapTerrain.terrainData.size.x / 2f, 0f, WorldMapTerrain.terrainData.size.z / 2f));
                //}

                //Update terrain loading state for all terrains since the session manager loads itself around the origin
                UpdateTerrainLoadState(m_originLoadingBounds, gameObject);
            }
#endif
        }

        public void SetOriginByTargetTile(int tileX = -99, int tileZ = -99)
        {
            if (tileX == -99)
            {
                tileX = m_originTargetTileX;
            }

            if (tileZ == -99)
            {
                tileZ = m_originTargetTileZ;
            }

            if (GaiaUtils.HasDynamicLoadedTerrains())
            {
                //Get the terrain tile by X / Z tile in the scene path
                TerrainScene targetScene = GaiaTerrainLoaderManager.TerrainScenes.Find(x => x.m_scenePath.Contains("Terrain_" + tileX.ToString() + "_" + tileZ.ToString()));
                if (targetScene != null)
                {
                    SetOrigin(new Vector3Double(targetScene.m_pos.x + (m_terrainSceneStorage.m_terrainTilesSize / 2f), 0f, targetScene.m_pos.z + (m_terrainSceneStorage.m_terrainTilesSize / 2f)));
                    string terrainName = targetScene.GetTerrainName();
                    GameObject go = GameObject.Find(terrainName);
                    if (go != null)
                    {
#if UNITY_EDITOR
                        Selection.activeObject = go;
#endif
                    }
                }
                else
                {
                    Debug.LogWarning("Could not find a terrain with the tile coordinates " + tileX.ToString() + "-" + tileZ.ToString() + " in the session.");
                }
            }
            else
            {
                Terrain t = Terrain.activeTerrains.Where(x => x.name.Contains("Terrain_" + tileX.ToString() + "_" + tileZ.ToString())).First();
                if (t != null)
                {
                    SetOrigin(new Vector3Double(t.transform.position.x + (t.terrainData.size.x / 2f), 0f, t.transform.position.z + (t.terrainData.size.z / 2f)));
#if UNITY_EDITOR
                    Selection.activeObject = t.gameObject;
#endif
                }
                else
                {
                    Debug.LogWarning("Could not find a terrain with the tile coordinates " + tileX.ToString() + "-" + tileZ.ToString() + " in the scene.");
                }
            }


        }

        /// <summary>
        /// Load and unload the terrain scenes stored in the current session for a certain object
        /// </summary>
        public void UpdateTerrainLoadState(BoundsDouble loadingBounds = null, GameObject requestingObject = null)
        {
            if (loadingBounds == null)
            {
                loadingBounds = m_originLoadingBounds;
            }

            if (requestingObject == null)
            {
                requestingObject = gameObject;
            }


            foreach (TerrainScene terrainScene in GaiaTerrainLoaderManager.TerrainScenes)
            {
                terrainScene.m_currentOriginOffset = m_originLoadingBounds.center;
                //only evaluate load state if local terrain is supposed to be displayed
                if (m_showLocalTerrain)
                {
                    if (terrainScene.m_bounds.Intersects(loadingBounds))
                    {
                        terrainScene.AddReference(requestingObject);
                        terrainScene.m_useFloatingPointFix = m_terrainSceneStorage.m_useFloatingPointFix;
                    }
                    else
                    {
                        terrainScene.RemoveReference(requestingObject, m_terrainUnloadMemoryTreshold);
                    }
                }
                terrainScene.ShiftLoadedTerrain();
            }
        }

        public double GetLoadingRange()
        {
            return m_originLoadingBounds.extents.x;
        }

        public Vector3Double GetLoadingSize()
        {
            return new Vector3Double(m_originLoadingBounds.size);
        }

        public void SwitchToWorldMap()
        {
            ShowWorldMapTerrain = true;
            ShowLocalTerrain = false;
            if (!Application.isPlaying)
            {
                UpdateTerrainLoadState();
            }
        }

        public void SwitchToLocalMap()
        {
            ShowLocalTerrain = true;
            ShowWorldMapTerrain = false;
            if (!Application.isPlaying)
            {
                UpdateTerrainLoadState();
            }
        }


        public void SetLoadingRange(Double range)
        {
            m_originLoadingBounds.extents = new Vector3Double(range, range, range);
            UpdateTerrainLoadState(m_originLoadingBounds, gameObject);
        }

        public void UnloadAll(bool forceUnload = false)
        {
            if (m_terrainSceneStorage != null)
            {
                foreach (TerrainScene terrainScene in m_terrainSceneStorage.m_terrainScenes)
                {
                    terrainScene.RemoveAllReferences(forceUnload);
                }
            }
        }

        public void EmptyCache()
        {
            if (m_terrainSceneStorage != null)
            {
                foreach (TerrainScene terrainScene in m_terrainSceneStorage.m_terrainScenes.Where(x=>x.References.Count()<=0))
                {
                    terrainScene.RemoveAllReferences(true);
                }
            }
        }

        public void SaveStorageData()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(m_terrainSceneStorage);
            AssetDatabase.SaveAssets();
            LoadStorageData();
#endif
        }
    }
}
