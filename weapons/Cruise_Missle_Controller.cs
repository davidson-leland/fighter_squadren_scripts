﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cruise_Missle_Controller : Missle_controller {


    public Vector3 drift = new Vector3();
    public Health health;
    public int team = 0;

	// Use this for initialization
	void Start () {
       
        Invoke("SetCanExplode", 2 + Random.Range(-0.5f, 0.5f));
        Invoke("IgniteMissle", timeToTopSpeed);

        directionMod = 0;
        currentSpeed = topSpeed;

        GameManager.instance.AddMissleToLists(team, transform);
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
        if (!isExploding && trackTarget)
        {
            GameObject topLevel = other.gameObject;

            bool b = false;

            while (!b)
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

    public void TakeDamage(int ammount, DamageType.DamageTypes dType = DamageType.DamageTypes.Default)
    {
        if(health.hull == 0)
        {
            return;
        }

        int healthCheck = health.TakeDamage(ammount, dType);

        if (healthCheck <= 0)
        {
            Explode();
        }
    }

    protected override void Explode(bool dealDamage = false)
    {
        base.Explode(dealDamage);
        GameManager.instance.RemoveMissleFromLists(team, transform);
    }

}
