using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

[System.Serializable]
public class Marker
{
    public string name;
    public Sprite icon;
    public Transform transform;
    [HideInInspector]
    public GameObject uiElement;
}

public class Compass : MonoBehaviour
{
    public Marker[] markers;

    float[] clampHorizontal = { 0, 0 };
    float[] clampSize = { 20, 60 };

    float compassView;

    public float lerpSpeed;

    float cutoffDist = 20;
    float proximityDist = 5;
    Camera cam;
    GameObject player;


    private void Start()
    {
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");

        float width = GetComponent<RectTransform>().sizeDelta.x;
        clampHorizontal[0] = -width / 2;
        clampHorizontal[1] = width / 2;

        compassView = (GetComponent<RectTransform>().sizeDelta.x / Screen.width) * Camera.main.fieldOfView;

        foreach (Marker marker in markers)
        {
            marker.uiElement = CreateMarker(marker);
        }
    }

    private void Update()
    {
        foreach(Marker marker in markers)
        {
            RectTransform markerRectTransform = marker.uiElement.GetComponent<Image>().rectTransform;
            markerRectTransform.localPosition = Vector3.Lerp(markerRectTransform.localPosition, CalculateMarkerCompassPos(marker), lerpSpeed * Time.deltaTime);
            markerRectTransform.sizeDelta = CalculateMarkerSizeDelta(marker);

            if(Vector3.Distance(player.transform.position,marker.transform.position) <= proximityDist)
            {
                marker.uiElement.SetActive(false);
            }
            else
            {
                marker.uiElement.SetActive(true);
            }
        }
    }

    Vector2 CalculateMarkerSizeDelta(Marker marker)
    {
        float distance = Vector3.Distance(player.transform.position, marker.transform.position);
        if (distance > cutoffDist) distance = cutoffDist;

        float size = (clampSize[1] - clampSize[0]) / distance + clampSize[0];
        return new Vector2(size, size);
    }
    Vector2 CalculateMarkerCompassPos(Marker marker)
    {
        Vector3 toLocation = new Vector3(marker.transform.position.x - player.transform.position.x,
                                         0,
                                         marker.transform.position.z - player.transform.position.z).normalized;

        Vector3 camForward2d = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        float angle = Vector3.SignedAngle(camForward2d, toLocation,Vector3.up) * compassView;
        angle = Mathf.Clamp(angle, clampHorizontal[0], clampHorizontal[1]);

        return new Vector2(angle , 0);

    }
    GameObject CreateMarker(Marker marker)
    {
        GameObject mGameObject = new GameObject();
        mGameObject.name = marker.name + " Marker";
        mGameObject.transform.SetParent(transform);
        mGameObject.AddComponent<Image>();
        mGameObject.GetComponent<Image>().sprite = marker.icon;
        return mGameObject;
    }
}
