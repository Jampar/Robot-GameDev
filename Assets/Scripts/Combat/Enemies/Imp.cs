using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy
{
    void Update()
    {
        if(!isAlerted()){
            PerformIdleBehaviour();
        }
        else 
        {
            PerformHostileBehaviour();
        }

        if(IsPlayerViewed() || IsPlayerHeard() || isDamaged()) AlertEnemy();        
    }   
}
