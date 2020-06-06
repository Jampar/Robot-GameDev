using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Puzzle : MonoBehaviour
{
    public virtual bool isComplete()
    {
        return false;
    }

    public virtual void CompletePuzzle()
    {

    }
}
