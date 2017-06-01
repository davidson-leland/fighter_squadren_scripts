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
                }                
            }
        }
    }

    protected override IEnumerator FireTurret()
    {
        canFire = false;
        isfiring = true;

        yield return new WaitForSeconds(0.3f);

        performLineChecks = true;
        StartCoroutine(CheckLaserDamage());

        distanceToEndPoint = Vector3.Distance(transform.position, targetPoint);

        /*
        if (true)//eventually, if target has no shields, go all the way through target
        {
           // distanceToEndPoint *= 2000;
        }*/

        endPoint.z = distanceToEndPoint;
        lineRenderer.SetPosition(1, endPoint);

        while (widthCurrent < widthMax)
        {
            widthCurrent += widthMax * 3 * Time.deltaTime;

            if (widthCurrent > widthMax)
            {
                widthCurrent = widthMax;
            }
            
            lineRenderer.startWidth = widthCurrent;
            lineRenderer.endWidth = widthCurrent;
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        performLineChecks = false;

        while (widthCurrent > 0)
        {
            widthCurrent -= widthMax * 3 * Time.deltaTime;
           
            lineRenderer.startWidth = widthCurrent;
            lineRenderer.endWidth = widthCurrent;

            yield return null;
        }

        widthCurrent = 0;
        
        lineRenderer.startWidth = widthCurrent;
        lineRenderer.endWidth = widthCurrent;

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
            }
            else
            {
                LineCheck(Vector3.zero, distanceToEndPoint, dir);
            }  
        }
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
    }

    protected override void AquireRandomTarget()
    {
        targetPoint = EnemyShipOrigin.position;

        targetPoint.x += Random.Range(-EnemyShipSize.x, EnemyShipSize.x);
        targetPoint.y += Random.Range(-EnemyShipSize.y, EnemyShipSize.y);
        targetPoint.z += Random.Range(-EnemyShipSize.z, EnemyShipSize.z);

        Vector3 compVector = transform.InverseTransformPoint(targetPoint);
        targetRotationalPosition.CalcAngles(compVector);
    }
}
