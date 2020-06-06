using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FindPlayer : MonoBehaviour
{    
    void Awake()
    {
        CinemachineFreeLook  vcam = transform.GetChild(0).GetComponent<CinemachineFreeLook >();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        vcam.LookAt = player;
        vcam.Follow = player;
    }
}

