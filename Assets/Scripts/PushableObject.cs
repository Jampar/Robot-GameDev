using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PushableObject : MonoBehaviour
{
    public float pushStep;
    Vector3 targetPos;

    private void Start()
    {
        targetPos = transform.position;
    }

    public void MoveInDirection(Vector3 direction)
    {
        targetPos = transform.position + direction * pushStep;
    }
}
