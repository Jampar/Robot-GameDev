using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy
{
    void Update()
    {
        if(!isAlerted()){
            FollowPatrol();
        }
        else 
        {
            ChasePlayer();
        }

        if(IsPlayerViewed() || IsPlayerHeard()) AlertEnemy();        
    }   
}
