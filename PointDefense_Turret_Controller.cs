using UnityEngine;
using System.Collections;

public class PointDefense_Turret_Controller : Turret_Controller
{
    protected override void TurretUpdate(float tick)
    {
        
        if (target == null)
        {
            AquireRandomTarget();
        }

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget < 2000)
            {

                Vector3 compVector = transform.InverseTransformPoint(target.transform.position);
                targetRotationalPosition.CalcAngles(compVector);

                if ((targetRotationalPosition.horizontalAngle < angleMax && targetRotationalPosition.horizontalAngle > -angleMax)
                    && (targetRotationalPosition.verticalAngle < angleMax && targetRotationalPosition.verticalAngle > -angleMax))
                {
                    Vector3 targetLead = GetLead() + drift;

                    TrackTarget(tick, targetLead);

                    //Debug.Log(distanceToTarget);                    

                    if (distanceToTarget < 1500 && canFire && !isfiring)
                    {
                        //Debug.Log("try fire turret");

                         if (targetRotationalPosition.verticalAngle < 10 && targetRotationalPosition.verticalAngle > -10)
                         {
                             if (targetRotationalPosition.verticalAngle < 10 && targetRotationalPosition.verticalAngle > -10)
                             {
                                 StartCoroutine(FireTurret());
                             }
                         }   

                        //StartCoroutine(FireTurret());
                    }
                }
                else
                {
                    target = null;
                }
            }
            else
            {
                target = null;
            }
        }



    }


}
