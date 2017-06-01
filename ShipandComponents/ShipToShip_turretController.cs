using UnityEngine;
using System.Collections;

public class ShipToShip_turretController : AnitShip_Turret_Controller {

    protected override void TurretUpdate(float tick)
    {
        TrackTarget(tick, targetPoint);

        if (targetRotationalPosition.horizontalAngle < 1 && targetRotationalPosition.horizontalAngle > -1)
        {
            if (targetRotationalPosition.verticalAngle < 1 && targetRotationalPosition.verticalAngle > -1)
            {

                if (canFire && !isfiring)
                {
                    StartCoroutine(FireTurret());
                }                
            }
        }
    }


    protected override IEnumerator FireTurret()
    {
        isfiring = true;
        canFire = false;

        attemptFire();

        yield return new WaitForSeconds(1f + Random.Range(0f, 0.3f));

        isfiring = false;
        canFire = true;

        AquireRandomTarget();
    }

    protected override void attemptFire()
    {
        var blast = (GameObject)Instantiate(blastPrefab, gunPorts[0].position, gunPorts[0].rotation);
        var blastScript = blast.GetComponent<Projectile_Blast>();
        blastScript.ownerName = gameObject.name;
        blastScript.team = team;
        //blast.name = ("EnergyBlast" + gameObject);//???
        Destroy(blast, 15.0f);

        blast = (GameObject)Instantiate(blastPrefab, gunPorts[1].position, gunPorts[1].rotation);
        blastScript = blast.GetComponent<Projectile_Blast>();
        blastScript.ownerName = gameObject.name;
        blastScript.team = team;
        Destroy(blast, 15.0f);
    }
}
