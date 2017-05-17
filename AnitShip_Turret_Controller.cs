using UnityEngine;
using System.Collections;

public class AnitShip_Turret_Controller : Turret_Controller
{
    [SerializeField]
    protected Transform EnemyShipOrigin;
    [SerializeField]
    protected Vector3 EnemyShipSize;

    protected Vector3 targetPoint = new Vector3();

    protected float distanceToEndPoint;
    Vector3 startPoint = new Vector3(), endPoint = new Vector3();

    float widthMax = 12, widthCurrent = 0;

    [SerializeField]
    protected LineRenderer lineRenderer;

    //[SerializeField]
    //Transform targetindicator;

    protected override void TurretStart()
    {
        canFire = false;
        AquireRandomTarget();
        canFire = true;       
    }

    protected override void TurretUpdate(float tick)
    {
        if (!isfiring)
        {
            TrackTarget(tick, targetPoint);
        }


        if (targetRotationalPosition.horizontalAngle < 1 && targetRotationalPosition.horizontalAngle > -1)
        {
            if (targetRotationalPosition.verticalAngle < 1 && targetRotationalPosition.verticalAngle > -1)
            {
                if(canFire && !isfiring)
                {
                    StartCoroutine(FireTurret());
                    //Debug.Log("shoot");
                }                
            }
        }
        else
        {
            
        }
        
    }


    /*protected override IEnumerator FireTurret()
    {
        isfiring = true;
        canFire = false;

        yield return new WaitForSeconds(0.3f);

        distanceToEndPoint = Vector3.Distance(transform.position, targetPoint);
        //Debug.Log(distanceToEndPoint);

        while(endPoint.z < distanceToEndPoint)
        {
            endPoint.z += 20000 * Time.deltaTime;

            if(endPoint.z > distanceToEndPoint)
            {
                endPoint.z = distanceToEndPoint;
            }

            lineRenderer.SetPosition(1, endPoint);

            if(widthCurrent < widthMax)
            {
                widthCurrent += widthMax * 3 * Time.deltaTime;

                if(widthCurrent > widthMax)
                {
                    widthMax = widthCurrent;
                }

                lineRenderer.SetWidth(widthCurrent, widthCurrent);
            }

            yield return null;
        }

        //yield return new WaitForSeconds(2);

        float t = 0;
        while( t < 2f)
        {
            t += Time.deltaTime;

            if (widthCurrent < widthMax)
            {
                widthCurrent += widthMax * 3 * Time.deltaTime;

                if (widthCurrent > widthMax)
                {
                    widthCurrent = widthMax;
                }
                lineRenderer.SetWidth(widthCurrent, widthCurrent);
            }

            yield return null;
        }


        while (widthCurrent > 7)
        {
            widthCurrent -= widthMax * 3 * Time.deltaTime;
            lineRenderer.SetWidth(widthCurrent, widthCurrent);

            yield return null;
        }

        while (startPoint.z < distanceToEndPoint)
        {
            startPoint.z += 20000 * Time.deltaTime;

            if (startPoint.z > distanceToEndPoint)
            {
                endPoint.z = distanceToEndPoint;
            }

            lineRenderer.SetPosition(0, startPoint);

            if(widthCurrent > 0)
            {
                widthCurrent -= widthMax * 3 * Time.deltaTime;

                if(widthCurrent < 0)
                {
                    widthCurrent = 0;
                }

                lineRenderer.SetWidth(widthCurrent, widthCurrent);
            }
            yield return null;
        }
        widthCurrent = 0;
        lineRenderer.SetWidth(widthCurrent, widthCurrent);

        startPoint.z = 0;
        endPoint.z = 0;

        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.SetPosition(0, startPoint);        

        yield return new WaitForSeconds(2);

        isfiring = false;
        AquireRandomTarget();


        canFire = true;
    }*/


    protected override IEnumerator FireTurret()
    {
        isfiring = true;
        canFire = false;

        yield return new WaitForSeconds(0.3f);

        distanceToEndPoint = Vector3.Distance(transform.position, targetPoint);
        //Debug.Log(distanceToEndPoint);

        endPoint.z = distanceToEndPoint;
        lineRenderer.SetPosition(1, endPoint);

        //yield return new WaitForSeconds(2);

        while (widthCurrent < widthMax)
        {
            widthCurrent += widthMax * 3 * Time.deltaTime;

            if (widthCurrent > widthMax)
            {
                widthCurrent = widthMax;
            }
            lineRenderer.SetWidth(widthCurrent, widthCurrent);
            yield return null;
        }


        yield return new WaitForSeconds(2f);


        while (widthCurrent > 0)
        {
            widthCurrent -= widthMax * 3 * Time.deltaTime;
            lineRenderer.SetWidth(widthCurrent, widthCurrent);

            yield return null;
        }

        widthCurrent = 0;
        lineRenderer.SetWidth(widthCurrent, widthCurrent);

        startPoint.z = 0;
        endPoint.z = 0;

        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.SetPosition(0, startPoint);

        yield return new WaitForSeconds(2);

        isfiring = false;
        AquireRandomTarget();
        canFire = true;
    }



    protected override void AquireRandomTarget()
    {
        targetPoint = EnemyShipOrigin.position;

        targetPoint.x += Random.Range(-EnemyShipSize.x, EnemyShipSize.x);
        targetPoint.y += Random.Range(-EnemyShipSize.y, EnemyShipSize.y);
        targetPoint.z += Random.Range(-EnemyShipSize.z, EnemyShipSize.z);

        //Debug.Log(targetPoint);

        //targetindicator.position = targetPoint;

        Vector3 compVector = transform.InverseTransformPoint(targetPoint);
        targetRotationalPosition.CalcAngles(compVector);

        //canFire = true;
    }
}
