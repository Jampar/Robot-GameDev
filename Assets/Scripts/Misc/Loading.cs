using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField]
    public int sceneIndex;
    [SerializeField]
    Image loadingBar;
    
    // Start is called before the first frame update
    void Start()
    {
        sceneIndex = GameObject.Find("Level Manager").GetComponent<LevelManager>().GetIndexToBeLoaded();
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        while(asyncOperation.progress < 1)
        {
            loadingBar.fillAmount = asyncOperation.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}
