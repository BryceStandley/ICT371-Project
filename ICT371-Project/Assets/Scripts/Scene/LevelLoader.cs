using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    public GameObject loadingScreen;
    public Slider progressSlider;

    private void Awake()
    {
        instance = this;
        SceneManager.LoadSceneAsync((int)SceneIndex.MenuToGame, LoadSceneMode.Additive);
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void LoadGame()
    {
        loadingScreen.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.MenuToGame));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndex.MainGame, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    float totalSceneProgress;
    public IEnumerator GetSceneLoadProgress()
    {
        for(int i = 0; i < scenesLoading.Count; i++)
        {
            while(!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;
                foreach(AsyncOperation opertaion in scenesLoading)
                {
                    totalSceneProgress += opertaion.progress;
                }

                totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100f;

                progressSlider.value = Mathf.RoundToInt(totalSceneProgress);
                yield return null;
            }
        }

        loadingScreen.SetActive(false);
        SceneManager.UnloadSceneAsync((int)SceneIndex.LevelLoader);
    }
}
