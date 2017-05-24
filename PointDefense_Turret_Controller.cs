using UnityEngine;
using System.Collections;

public class PointDefense_Turret_Controller : Turret_Controller
{
    protected Transform missleTransform;

    protected override void TurretUpdate(float tick)
    {
        
        if (target == null && missleTransform == null)
        {
            AquireRandomTarget();
        }

        //Debug.Log(missleTransform);

        if (target != null || missleTransform != null)
        {
           
            Vector3 targetPosition = new Vector3();

            if (missleTransform != null)
            {
                targetPosition = missleTransform.position;
            }
            else
            {
                targetPosition = target.transform.position;
            }

            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);           

            if (distanceToTarget < 2000)
            {

                Vector3 compVector = transform.InverseTransformPoint(targetPosition);
                targetRotationalPosition.CalcAngles(compVector);

                if ((targetRotationalPosition.horizontalAngle < angleMax && targetRotationalPosition.horizontalAngle > -angleMax)
                    && (targetRotationalPosition.verticalAngle < angleMax && targetRotationalPosition.verticalAngle > -angleMax))
                {
                    Vector3 targetLead = GetLead(targetPosition) + drift;

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

    protected override void AquireRandomTarget()
    {
        var missle = GameManager.instance.CheckForMissles(team);

        if(missle == null)
        {
            base.AquireRandomTarget();
        }
        else
        {
            missleTransform = missle;
        }     
    }


}
