using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cruise_Missle_Controller : Missle_controller {


    public Vector3 drift = new Vector3();

	// Use this for initialization
	void Start () {
       
        Invoke("SetCanExplode", 2);
        Invoke("IgniteMissle", timeToTopSpeed);
        directionMod = 0;

        currentSpeed = topSpeed;
    }


    protected override void Missle_Update()
    {

        if(currentSpeed < topSpeed)
        {
            currentSpeed += topSpeed * (Time.deltaTime / timeToTopSpeed);

            if (currentSpeed > topSpeed)
            {
                currentSpeed = topSpeed;
            }
        }
       

        /*if (currentSpeed >= topSpeed)
        {
            IgniteMissle();
        }*/

        base.Missle_Update();
    }


    protected override void IgniteMissle()
    {
        trackTarget = true;
    }

    protected override Vector3 GetCompVector()
    {
        return transform.InverseTransformPoint(target.position + drift);
    }

    protected override Vector3 ApplyDropDrift()
    {
        return Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isExploding)
        {
            GameObject topLevel = other.gameObject;

            bool b = false;

            while (!b)// i need to completly re-do this to take into account ship components.
            {

                if (topLevel.tag == "Ship")
                {

                    var otherController = topLevel.GetComponent<Ship_Controller>();

                    if (otherController != null)
                    {
                        otherController.TakeDamage(20);
                    }

                    b = true;

                    Explode();

                }
                else if (topLevel.tag == "Ship_Component")
                {


                    var otherController = topLevel.GetComponent<Ship_Component>();

                    if (otherController != null)
                    {
                        otherController.TakeDamage(20);
                    }

                    b = true;

                    Explode();

                }

                if (topLevel.transform.parent == null)
                {
                    b = true;
                }
                else
                {
                    topLevel = topLevel.transform.parent.gameObject;
                }
            }
        }

        else
        {
            Missle_OnTriggerEnter(other);
        }

       
    }
}
