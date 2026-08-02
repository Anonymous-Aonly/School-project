using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class ASyncLoader : MonoBehaviour
{

[SerializeField] private GameObject LoadingScreen;
[SerializeField] private GameObject MainMenu;
[SerializeField] private Slider LoadingSlider;

public void LoadLevelBtn(string levelToLoad)
    {
        MainMenu.SetActive(false);
        LoadingScreen.SetActive(true);

        StartCoroutine(LoadLevelASync(levelToLoad));
    }
    
    IEnumerator LoadLevelASync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.09f);
            LoadingSlider.value = progressValue;
            yield return null;
        }
    }

}
