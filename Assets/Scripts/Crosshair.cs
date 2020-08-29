using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    Camera cam;
    float crosshairDistance = 10f;
    float lerpSpeed = 15;
    RaycastHit hit;

    Vector3 rootPos;

    public List<string> interestTags = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform);

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            if (isHitInterest(hit))
            {
                LerpToInterest(hit);
            }
            else
            {
                ReturnToRoot();
            }
        }
        else
        {
            ReturnToRoot();
        }

        FixDot();
    }

    void FixDot()
    {
        transform.GetChild(1).transform.position = cam.transform.position + cam.transform.forward * crosshairDistance;
    }

    bool isHitInterest(RaycastHit hit)
    {
        if (interestTags.Contains(hit.transform.tag)) return true;
        else return false;
    }

    void LerpToInterest(RaycastHit interestHit)
    {
        transform.position = Vector3.Lerp(transform.position, interestHit.transform.position - cam.transform.forward * 1f, lerpSpeed * Time.deltaTime);
    }

    void ReturnToRoot()
    {
        transform.position = Vector3.Lerp(transform.position, cam.transform.position + cam.transform.forward * crosshairDistance, lerpSpeed * Time.deltaTime);
    }
}
