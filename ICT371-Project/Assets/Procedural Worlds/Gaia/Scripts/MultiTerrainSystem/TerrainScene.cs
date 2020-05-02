using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace Gaia
{
    [System.Serializable]
    public enum LoadState { Loaded, Unloaded }

    [System.Serializable]
    public class TerrainScene
    {
        private List<GameObject> m_references = new List<GameObject>();
        public List<GameObject> References { get{ return m_references; } }

        

        public Vector3Double m_pos;
        public Vector3Double m_currentOriginOffset;
        public BoundsDouble m_bounds;
        public string m_scenePath;
        public LoadState m_loadState;
        public bool m_useFloatingPointFix;
        private AsyncOperation asyncLoadOp;
#if GAIA_PRO_PRESENT
        private bool m_loadRequested;
        private bool m_unloadRequested;
#endif
        private long m_loadCacheTreshold = 4294967296;
        private bool m_forceSceneRemove = false;
        public bool m_isFoldedOut;
#if UNITY_EDITOR
        private GameObject m_terrainObj;
        public GameObject TerrainObj { get => m_terrainObj; }
#endif
        public string GetTerrainName()
        {
            return m_scenePath.Substring(m_scenePath.IndexOf("Terrain")).Replace(".unity", "");
        }

        public void AddReference(GameObject gameObject)
        {
            if (!m_references.Contains(gameObject))
            {
                m_forceSceneRemove = false;
                m_references.Add(gameObject);
            }
            UpdateLoadState();

        }

        public void RemoveAllReferences(bool forceSceneRemove = false)
        {
            m_references.Clear();
            #if GAIA_PRO_PRESENT
            m_loadRequested = false;
            m_unloadRequested = false;
            #endif
            m_forceSceneRemove = forceSceneRemove;
            UpdateLoadState();
        }

        public void RemoveReference(GameObject gameObject, long cacheSize=0)
        {
            if (m_references.Contains(gameObject))
            {
                m_references.Remove(gameObject);
            }
            if (cacheSize != 0)
            {
                m_loadCacheTreshold = cacheSize;
            }
            UpdateLoadState();
        }

        public bool HasReference(GameObject gameObject)
        {
            return m_references.Contains(gameObject);
        }

        public void UpdateLoadState()
        {
#if GAIA_PRO_PRESENT
            //sanity check on the references: does the GO still exist?
            for (int i = m_references.Count - 1; i >= 0; i--)
            {
                if (m_references[i] == null)
                {
                    m_references.RemoveAt(i);
                }
                else
                {
                    //Is it still relevant?
                    GaiaTerrainLoader loader = m_references[i].GetComponent<GaiaTerrainLoader>();
                    if (loader != null)
                    {
                        if (loader.LoadMode == LoadMode.Disabled || (!loader.m_isSelected && loader.LoadMode == LoadMode.EditorSelected))
                        {
                            m_references.RemoveAt(i);
                        }
                    }
                }
            }

            //locked - no state change for now, used when the loader is dragged around in the scene, etc.
#if UNITY_EDITOR
            Scene scene = EditorSceneManager.GetSceneByPath(m_scenePath);
#else
            Scene scene = SceneManager.GetSceneByPath(m_scenePath);
#endif

            if (m_references.Count >= 1 && scene.isLoaded)
            {
                foreach (GameObject go in scene.GetRootGameObjects())
                {
                    go.SetActive(true);
                    if (go.GetComponent<Terrain>() != null)
                    {
                        m_terrainObj = go;
                    }
                }
            }

            if (m_references.Count >= 1 && !scene.isLoaded)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    if (!m_loadRequested)
                    {
                        asyncLoadOp = SceneManager.LoadSceneAsync(m_scenePath, LoadSceneMode.Additive);
                        asyncLoadOp.completed += SceneLoadCompleted;
                        m_loadRequested = true;
                    }
                }
                else
                {
                    GaiaUtils.DisplayProgressBarNoEditor("Loading Terrains", "Loading Terrains...", 0.5f);
                    EditorSceneManager.OpenScene(m_scenePath, OpenSceneMode.Additive);
                    EditorSceneManager.sceneOpened += SceneOpened;
                    GaiaUtils.ClearProgressBarNoEditor();
                }
#else
                    if (!m_loadRequested)
                    {
                        asyncLoadOp = SceneManager.LoadSceneAsync(m_scenePath, LoadSceneMode.Additive);
                        asyncLoadOp.completed += SceneLoadCompleted;
                        m_loadRequested = true;
                    }
#endif
            }

            //Re-read the scene in - could have changed its load state by now
#if UNITY_EDITOR
            scene = EditorSceneManager.GetSceneByPath(m_scenePath);
#else
            scene = SceneManager.GetSceneByPath(m_scenePath);
#endif

            if (m_references.Count <= 0 && scene.isLoaded)
            {
#if UNITY_EDITOR

                if (Application.isPlaying)
                {
                    if (!m_unloadRequested)
                    {
                        SceneManager.UnloadSceneAsync(scene.name);
                        m_unloadRequested = true;
                    }
                }
                else
                {
                    if (scene.isDirty)
                    {
                        EditorSceneManager.SaveScene(scene);
                    }


                    if (Profiler.GetTotalReservedMemoryLong() < m_loadCacheTreshold && !m_forceSceneRemove)
                    {
                        //only deactivate the root game objects first, actual unload will happen later
                        foreach (GameObject go in scene.GetRootGameObjects())
                        {
                            go.SetActive(false);
                            m_unloadRequested = false;
                        }
                    }
                    else
                    {
                        //no memory left for keeping terrains alive, do full unload now
                        GaiaUtils.DisplayProgressBarNoEditor("Unloading Terrains", "Unloading Terrains...", 0.5f);
                        EditorSceneManager.CloseScene(scene, true);
                        GaiaTerrainLoaderManager.Instance.EmptyCache();
                        GaiaUtils.ClearProgressBarNoEditor();
                        m_unloadRequested = false;
                    }
                }

#else
                  if (!m_unloadRequested)
                    {
                        SceneManager.UnloadSceneAsync(scene.name);
                        m_unloadRequested = true;
                    }
#endif
            }


            if (scene.isLoaded)
            {
                m_loadState = LoadState.Loaded;
                m_loadRequested = false;
            }
            else
            {
                m_loadState = LoadState.Unloaded;
                m_unloadRequested = false;
                //for (int i= EditorSceneManager.loadedSceneCount-1; i > 0; i--)
                //{
                //    Debug.Log(m_scenePath);
                //    Debug.Log(EditorSceneManager.GetSceneAt(i).path);
                //    if (EditorSceneManager.GetSceneAt(i).path == m_scenePath)
                //    {
                //        EditorSceneManager.CloseScene(EditorSceneManager.GetSceneAt(i),true);
                //    }
                //}
            }

#endif
        }
#if UNITY_EDITOR
        private void SceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (scene == EditorSceneManager.GetSceneByPath(m_scenePath))
            {
                if (m_loadState != LoadState.Loaded)
                {
                    foreach (GameObject go in scene.GetRootGameObjects())
                    {
                        go.transform.position = m_bounds.center - m_currentOriginOffset - new Vector3Double(m_bounds.size.x / 2f, 0f, m_bounds.size.z / 2f);
                        if (go.GetComponent<Terrain>() != null)
                        {
                            m_terrainObj = go;
                        }
                    }
                    m_loadState = LoadState.Loaded;
                }
            }
        }
#endif

        private void SceneLoadCompleted(AsyncOperation obj)
        {
#if GAIA_PRO_PRESENT
            if (m_useFloatingPointFix)
            {
            Scene scene = SceneManager.GetSceneByPath(m_scenePath);
                foreach (GameObject go in scene.GetRootGameObjects())
                {
                    go.transform.position += FloatingPointFix.Instance.totalOffset;
                    if (go.transform.GetComponent<FloatingPointFixMember>() == null)
                    {
                        go.AddComponent<FloatingPointFixMember>();
                    }
                    if (go.GetComponent<Terrain>() != null)
                    {
                        m_terrainObj = go;
                    }
                }
            }
#endif
            obj.completed -= SceneLoadCompleted;
        }

        public void ShiftLoadedTerrain()
        {
            if (m_loadState == LoadState.Loaded)
            {
#if UNITY_EDITOR
                Scene scene = EditorSceneManager.GetSceneByPath(m_scenePath);
#else
                Scene scene = SceneManager.GetSceneByPath(m_scenePath);
#endif
                if (scene.isLoaded)
                {
                    foreach (GameObject go in scene.GetRootGameObjects())
                    {
                        go.transform.position = m_bounds.center - m_currentOriginOffset - new Vector3Double(m_bounds.size.x / 2f, 0f, m_bounds.size.z / 2f);
                    }
                }
                else
                {
                    m_loadState = LoadState.Unloaded;
                }
            }
        }
    }
}
