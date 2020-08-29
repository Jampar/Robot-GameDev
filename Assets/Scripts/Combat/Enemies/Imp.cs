using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy
{
    void Update()
    {
        if(IsFoeViewed()) IncreaseAlertLevel();
        else if(IsFoeHeard()) SetAlertLevel(searchAlertThreshold);
        else if(!isSearching()) DecreaseAlertLevel();
        else if(isDamaged()) {
            SetAlertLevel(1);
             SetFoe(lastDamagedBy);
        }

        if(GetAlertLevel() == 1) PerformHostileBehaviour();
        else if (GetAlertLevel() >= searchAlertThreshold) Search();
        else PerformIdleBehaviour();

        MaintainAlertMeter();

        if(canMelee()){
            Melee();
        }
    }   
}
