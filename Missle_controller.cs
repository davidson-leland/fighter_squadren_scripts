using UnityEngine;
using System.Collections;

public class Missle_controller : MonoBehaviour {

    public float topSpeed = 700, maxTurnAngle = 200, igniteTimer = 0.3f, lifeTimer = 10;

    public GameObject explosionEffect;

    public float currentSpeed;
    public Transform target;

    bool trackTarget = false;

    public GameObject exhaustEffect, missleBody;

    public int directionMod = 1;

    float turnAngle = 0f, lastAnglex = 360, lastAngley = 360;

    bool canExplode = true, zonedIn = false, hasExploded = false;



    RotationalPosition targetRotationalPosition = new RotationalPosition();

    [SerializeField]
    SphereCollider sphereCollider;

    // Use this for initialization
    void Start () {
        Invoke("IgniteMissle", igniteTimer);
        Invoke("Explode", lifeTimer);
        Invoke("SetCanExplode", 2);

    }

    // Update is called once per frame
    void Update() {

        if (hasExploded)
        {
            return;
        }

        Vector3 movement = Vector3.forward;
        movement *= currentSpeed * Time.deltaTime;


        if (trackTarget)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            //Debug.Log(distanceToTarget);

            if (distanceToTarget < 50 && canExplode)
            {
                Explode();
            }
            //do tracking
            float x = 0, y = 0;
            Vector3 compVector = transform.InverseTransformPoint(target.position);
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

            currentSpeed += topSpeed * Time.deltaTime;

            if(currentSpeed > topSpeed)
            {
                currentSpeed = topSpeed;
            }
            //horizontal tracking
            if (targetRotationalPosition.horizontalAngle > -1 && targetRotationalPosition.horizontalAngle < 1)
            {
                turnAngle = targetRotationalPosition.horizontalAngle;
            }
            else if (targetRotationalPosition.horizontalAngle > 0)
            {
                turnAngle = maxTurnAngle * Time.deltaTime;
            }
            else if (targetRotationalPosition.horizontalAngle < 0)
            {
                turnAngle = -maxTurnAngle * Time.deltaTime;
            }

            //vert tracking
           if (targetRotationalPosition.verticalAngle > -1 && targetRotationalPosition.verticalAngle < 1)
            {
                y = -targetRotationalPosition.verticalAngle;
            }
            else if (targetRotationalPosition.verticalAngle > 0)
            {
                y = -maxTurnAngle * Time.deltaTime;
            }
            else if (targetRotationalPosition.verticalAngle < 0)
            {
                y = maxTurnAngle * Time.deltaTime;
            }

            x = turnAngle;//not sure why keeping this.

            Vector3 rotate = new Vector3(y, x, 0);

            transform.Rotate(rotate);

            lastAnglex = targetRotationalPosition.horizontalAngle;
            lastAngley = targetRotationalPosition.verticalAngle;


            if(zonedIn == false)//if target is in a 20 degree cone, this is for tracking if we eventually lose the target for some reason.
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
            movement += (Vector3.down + (Vector3.right * directionMod)) * (30 * Time.deltaTime); 
        }

        transform.Translate(movement);

    }


    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform);

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
            othercontroller.TakeDamage( 10);
        }
        
    }

    protected void IgniteMissle()
    {
        trackTarget = true;
        exhaustEffect.SetActive(true);
        //currentSpeed = topSpeed;
    }

    protected void Explode(bool dealDamage = false)
    {
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
