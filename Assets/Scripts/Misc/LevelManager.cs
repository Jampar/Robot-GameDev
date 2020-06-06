using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    int indexToBeLoaded;

    static GameObject _instance;
    
    void Awake()
    {
         //if we don't have an [_instance] set yet
        if(!_instance)
            _instance = this.gameObject;
         //otherwise, if we do, kill this thing
        else
            Destroy(this.gameObject);
 
        DontDestroyOnLoad(this.gameObject);
     }

    public void LoadLoadingScreen(int destinationIndex)
    {
        indexToBeLoaded = destinationIndex;
        SceneManager.LoadScene(1);
    }

    public int GetIndexToBeLoaded(){
        return indexToBeLoaded;
    }
}
