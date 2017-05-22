using UnityEngine;
using System.Collections;

public class Missle_controller : MonoBehaviour {

    public float topSpeed = 700, maxTurnAngle = 200, igniteTimer = 0.3f, lifeTimer = 10, timeToTopSpeed = 1;

    public GameObject explosionEffect;

    public float currentSpeed;
    public Transform target;

    protected bool trackTarget = false;

    public GameObject exhaustEffect, missleBody;

    public int directionMod = 1;

    float turnAngle = 0f, lastAnglex = 360, lastAngley = 360;

    protected bool canExplode = true, zonedIn = false, hasExploded = false;

    protected bool isExploding = false;



    RotationalPosition targetRotationalPosition = new RotationalPosition();

    [SerializeField]
    SphereCollider sphereCollider;

    // Use this for initialization
    void Start () {
        Invoke("IgniteMissle", igniteTimer);
        Invoke("TimedExplosion", lifeTimer);
        Invoke("SetCanExplode", 2);

    }

    // Update is called once per frame
    void Update()
    {
        Missle_Update();
    }

    protected virtual void Missle_Update()
    {
        if (hasExploded)
        {
            return;
        }

        Vector3 movement = Vector3.forward;
        movement *= currentSpeed * Time.deltaTime;


        if (trackTarget)
        {
            float x = 0, y = 0;

            if (target != null)
            {

                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                //Debug.Log(distanceToTarget);

                if (distanceToTarget < 50 && canExplode)
                {
                    Explode();
                }

                Vector3 compVector = GetCompVector();
                targetRotationalPosition.CalcAngles(compVector);

                if (zonedIn)//if target was infront of us but evaded, stop tracking and shortly after blow up.
                {
                    if ((lastAnglex < 50 && lastAnglex > -50) && (targetRotationalPosition.horizontalAngle > 50 || targetRotationalPosition.horizontalAngle < -50))
                    {
                        Debug.Log("lost horizontal");
                        trackTarget = false;

                        CancelInvoke("Explode");
                        Invoke("Explode", 0.5f);
                    }
                }

                float desiredAngleX = 0;
                float desiredAngleY = 0;

                //horizontal tracking
                if (targetRotationalPosition.horizontalAngle > 0)
                {
                    desiredAngleX = maxTurnAngle;
                }
                else if (targetRotationalPosition.horizontalAngle < 0)
                {
                    desiredAngleX = -maxTurnAngle;
                }

                //vert tracking
                if (targetRotationalPosition.verticalAngle > 0)
                {
                    desiredAngleY = -maxTurnAngle;
                }
                else if (targetRotationalPosition.verticalAngle < 0)
                {
                    desiredAngleY = maxTurnAngle;
                }

                x = desiredAngleX * Time.deltaTime;
                y = desiredAngleY * Time.deltaTime;

                if ((x > 0 && x > targetRotationalPosition.horizontalAngle) || (x < 0 && x < targetRotationalPosition.horizontalAngle))
                {
                    if (targetRotationalPosition.horizontalAngle < 0.5 && targetRotationalPosition.horizontalAngle > -0.5)
                    {
                        x = targetRotationalPosition.horizontalAngle;
                    }
                }

                if ((y > 0 && y > targetRotationalPosition.verticalAngle) || (y < 0 && y < targetRotationalPosition.verticalAngle))
                {
                    if (targetRotationalPosition.horizontalAngle < 90 && targetRotationalPosition.horizontalAngle > -90)
                    {
                        if (targetRotationalPosition.verticalAngle < 0.5 && targetRotationalPosition.verticalAngle > -0.5)
                        {
                            y = -targetRotationalPosition.verticalAngle;
                        }
                    }
                }
            }

            currentSpeed += topSpeed * (Time.deltaTime / timeToTopSpeed);

            if (currentSpeed > topSpeed)
            {
                currentSpeed = topSpeed;
            }

            Vector3 rotate = new Vector3(y, x, 0);

            transform.Rotate(rotate);

            lastAnglex = targetRotationalPosition.horizontalAngle;
            lastAngley = targetRotationalPosition.verticalAngle;


            if (zonedIn == false)//if target is in a 20 degree cone, this is for tracking if we eventually lose the target for some reason.
            {
                if ((lastAnglex > -20 && lastAnglex < 20) && (lastAngley > -20 && lastAngley < 20))
                {
                    zonedIn = true;
                    //Debug.Log("zonedIn");
                }
            }
        }
        else
        {
            movement += ApplyDropDrift();
        }

        transform.Translate(movement);   
}

    protected virtual Vector3 ApplyDropDrift()
    {
        return (Vector3.down + (Vector3.right * directionMod)) * (30 * Time.deltaTime);
    }

    protected virtual Vector3 GetCompVector()
    {
        return transform.InverseTransformPoint(target.position);
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform);

        Missle_OnTriggerEnter(other);
    }

    protected virtual void Missle_OnTriggerEnter(Collider other)
    {

        if (!canExplode)
        {
            return;
        }

        GameObject topLevel = other.gameObject;

        while (topLevel.transform.parent != null)
        {
            topLevel = topLevel.transform.parent.gameObject;
        }

        var othercontroller = topLevel.GetComponent<FighterController>();

        if (othercontroller != null && topLevel.name != "Player")
        {
            othercontroller.TakeDamage(10);
        }
    }

    protected virtual void IgniteMissle()
    {
        trackTarget = true;
        exhaustEffect.SetActive(true);
        //currentSpeed = topSpeed;
    }

    void TimedExplosion()
    {
        Explode();
    }

    protected void Explode(bool dealDamage = false)
    {
       // Debug.Log("try explode");
        isExploding = true;

        var blast = (GameObject)Instantiate(explosionEffect, transform.position, transform.rotation);

        sphereCollider.enabled = true;
        missleBody.SetActive(false);

        Destroy(blast, 05f);
        Destroy(gameObject, 0.2f);

        hasExploded = true;
    }

    void SetCanExplode()
    {
        canExplode = true;
    }
}
