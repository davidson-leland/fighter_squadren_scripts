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

    [SerializeField]
    protected DamageType damageType;

    [SerializeField]
    protected int damage = 2;

    //[SerializeField]
    //Transform targetindicator;

    bool performLineChecks = false;

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
       
        canFire = false;
        isfiring = true;

        yield return new WaitForSeconds(0.3f);

        performLineChecks = true;


        StartCoroutine(CheckLaserDamage());

        distanceToEndPoint = Vector3.Distance(transform.position, targetPoint);
        //Debug.Log(distanceToEndPoint);

        if (true)//eventually, if target has no shields, go all the way through target
        {
           // distanceToEndPoint *= 2000;
        }

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

        performLineChecks = false;
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
        
        AquireRandomTarget();
        canFire = true;
        isfiring = false;
    }

    protected IEnumerator CheckLaserDamage()
    {
        int dDealt = 0;

        while (performLineChecks)
        {            
            yield return new WaitForSeconds(0.3f);

            //performLine Checks
            var dir = lineRenderer.transform.rotation * Vector3.forward;
            if (widthCurrent >= 1)
            {
                LineCheck(new Vector3(widthCurrent /2.3f,widthCurrent /2.3f), distanceToEndPoint, dir);
                LineCheck(new Vector3(widthCurrent/ 2.3f, -widthCurrent/ 2.3f), distanceToEndPoint, dir);
                LineCheck(new Vector3(-widthCurrent/ 2.3f, widthCurrent/ 2.3f), distanceToEndPoint, dir);
                LineCheck(new Vector3(-widthCurrent/ 2.3f, -widthCurrent/ 2.3f), distanceToEndPoint, dir);

                dDealt += damage * 4;
                //Debug.Log("LargeLine");
            }
            else
            {
                LineCheck(Vector3.zero, distanceToEndPoint, dir);
                dDealt += damage;
                //Debug.Log("tiny line");
            }           

        }

        //Debug.Log("damage dealt = " + dDealt);
        
    }

    protected void LineCheck(Vector3 startPosition, float Length , Vector3 dir)
    {
        // do a line check, damage everything with health in the line.
        startPosition += transform.position;
        Vector3 endPosition = startPosition + (dir * Length);

        RaycastHit[] Hits;
        Hits = Physics.RaycastAll(startPosition, endPosition);

        foreach(RaycastHit rH in Hits)
        {
            damageType.HitCollider(rH.collider, name, team,damage);
        }
           
        //Debug.DrawLine(startPosition, endPosition,Color.green);
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
