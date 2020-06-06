using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    GameObject player;

    [SerializeField]
    Color gizmoColor;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = transform.position;
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position+ new Vector3(0,1.5f,0),new Vector3(1,3,1));
    }
}
