using UnityEngine;
using System.Collections;

public class Missle_controller : MonoBehaviour {

    public float topSpeed = 700, maxTurnAngle = 200, igniteTimer = 0.3f, lifeTimer = 10;

    public GameObject explosionEffect;

    public float currentSpeed;
    public Transform target;

    bool trackTarget = false;

    public GameObject exhaustEffect;

    public int directionMod = 1;

    float turnAngle = 0f;

    bool canExplode = true;

    // Use this for initialization
    void Start () {
        Invoke("IgniteMissle", igniteTimer);
        Invoke("Explode", lifeTimer);
        Invoke("SetCanExplode", 2);
	}
	
	// Update is called once per frame
	void Update () {
            Vector3 movement = Vector3.forward;
            movement *= currentSpeed * Time.deltaTime;
        

        if (trackTarget)
        {
            //do tracking
            float x = 0, y = 0;
            
            Vector3 compVector = transform.InverseTransformPoint(target.position);

            currentSpeed += topSpeed * Time.deltaTime;

            if(currentSpeed > topSpeed)
            {
                currentSpeed = topSpeed;
            }


            if (compVector.y > 5)
            {
                y = -maxTurnAngle * Time.deltaTime;
            }
            else if (compVector.y < -5)
            {
                y = maxTurnAngle * Time.deltaTime;
            }

            if (compVector.x > 10)
            {
                //Debug.Log("is to right");
                turnAngle = maxTurnAngle;
            }
            else if (compVector.x < -10)
            {
                //Debug.Log("is to left");
                turnAngle = -maxTurnAngle;
            }
            else if (compVector.z > 0)
            {
               // Debug.Log("is in front");
                turnAngle = 0;

                transform.LookAt(target);
            }
            else if (compVector.z < 0)
            {

                //Debug.Log("is in back");
                if (compVector.x > -.5 && compVector.x < 0.5)
                {
                    if (turnAngle == 0 )
                    {
                        float t = Random.Range(0, 10);

                        if (t > 5)
                        {
                            turnAngle = -30;
                        }
                        else
                        {
                            turnAngle = 30;
                        }
                    }
                }
            }
                
            x = turnAngle * Time.deltaTime;
            Vector3 rotate = new Vector3(y, x, 0);

            transform.Rotate(rotate);

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

        if(topLevel.GetComponent<Missle_controller>() != null)
        {
            return;
        }

        //Debug.Log(topLevel);

        if (topLevel.name != "Player")
        {
            //Debug.Log(topLevel);

            Explode();

        }
    }

    protected void IgniteMissle()
    {
        trackTarget = true;
        exhaustEffect.SetActive(true);
        //currentSpeed = topSpeed;
    }

    protected void Explode()
    {
        var blast = (GameObject)Instantiate(explosionEffect, transform.position, transform.rotation);

        
        Destroy(blast, 05f);
        Destroy(gameObject);
    }

    void SetCanExplode()
    {
        canExplode = true;
    }

}
