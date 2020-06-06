using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    public int fadeToSceneIndex;

    public void FadeOut()
    {
        GameObject.Find("Level Manager").GetComponent<LevelManager>().LoadLoadingScreen(fadeToSceneIndex);
    }
}
