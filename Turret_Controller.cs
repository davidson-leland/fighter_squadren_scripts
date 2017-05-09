using UnityEngine;
using System.Collections;

//This (anti fighter turret) is going to end up being very similar to ai controller.
public class Turret_Controller : MonoBehaviour {

    [SerializeField]
    protected float angleMax = 60f;

    [SerializeField]
    protected Transform[] gunPorts;

    [SerializeField]
    protected float trackingSpeed = 50;

    public Health health = new Health();

    public int team;

    protected Fighter target = null;
    public Vector3 targetsLastPosition = new Vector3();

    RotationalPosition targetRotationalPosition = new RotationalPosition();

    [SerializeField]
    protected Transform rotator;

    protected bool canFire = false;
    bool isfiring = false;

    [SerializeField]
    GameObject blastPrefab;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(target == null)
        {
            // target = GameManager.instance.PlayerFighters[0][0].fighterScript;

            AquireRandomTarget(); 
            
        }

        TurretUpdate(Time.deltaTime);
	}

    protected void TurretUpdate(float tick)//im going to operate on the assumption that this will be faster than accessing time.deltatime everytine i need it.
    {
        if(target != null )
        {
            Vector3 compVector = new Vector3();
            compVector = transform.InverseTransformPoint(target.transform.position);
            targetRotationalPosition.CalcAngles(compVector);

            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

           // if (distanceToTarget < 1000) { }

            if ((targetRotationalPosition.horizontalAngle < angleMax && targetRotationalPosition.horizontalAngle > -angleMax)
                && (targetRotationalPosition.verticalAngle < angleMax && targetRotationalPosition.verticalAngle > -angleMax))
            {
                    TrackTarget(tick);

                if (distanceToTarget < 5000 && canFire && !isfiring)
                {
                    StartCoroutine(FireTurret());
                }
            } 
        }
    }

    protected void TrackTarget(float tick)
    {
       
        Vector3 compVector = new Vector3();

        compVector = rotator.InverseTransformPoint(target.transform.position);
        

        targetRotationalPosition.CalcAngles(compVector);

        float desiredAngleX = 0;
        float desiredAngleY = 0;

            if (targetRotationalPosition.horizontalAngle > 0)
            {
                desiredAngleX = trackingSpeed;
            }
            else if(targetRotationalPosition.horizontalAngle < 0)
            {
                desiredAngleX = -trackingSpeed;
            }

            if(targetRotationalPosition.verticalAngle > 0)
            {
                desiredAngleY = -trackingSpeed;
            }
            else if(targetRotationalPosition.verticalAngle < 0)
            {
                desiredAngleY = trackingSpeed;
            }

            float x = desiredAngleX * Time.deltaTime;
            float y = desiredAngleY * Time.deltaTime;

            //don't rotate past the target, save new turnangle that is applied.
            if ((x > 0 && x > targetRotationalPosition.horizontalAngle) || (x < 0 && x < targetRotationalPosition.horizontalAngle))
            {
                if (targetRotationalPosition.horizontalAngle < 2 && targetRotationalPosition.horizontalAngle > -2)
                {
                    x = targetRotationalPosition.horizontalAngle;                    
                }
            }

            if ((y > 0 && y > targetRotationalPosition.verticalAngle) || (y < 0 && y < targetRotationalPosition.verticalAngle))
            {
                if (targetRotationalPosition.verticalAngle < 90 && targetRotationalPosition.verticalAngle > -90)
                {
                    if (targetRotationalPosition.verticalAngle < 2 && targetRotationalPosition.verticalAngle > -2)
                    {
                        y = -targetRotationalPosition.verticalAngle;
                    }
                }
            }

        RotateTurret( new Vector3(y,x,0));       
    }

    protected void RotateTurret( Vector3 _rotate)
    {
        rotator.Rotate(_rotate, Space.Self);
    }

    protected void attemptFire()
    {
        var blast = (GameObject)Instantiate(blastPrefab, gunPorts[0].position, gunPorts[0].rotation);
        var blastScript = blast.GetComponent<Projectile_Blast>();
        blastScript.ownerName = gameObject.name;
        //blast.name = ("EnergyBlast" + gameObject);
        Destroy(blast, 3.0f);
    }

    IEnumerator FireTurret()
    {
        isfiring = true;
        canFire = false;

        for( int i = 0; i < 3; i++)
        {
            attemptFire();

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1f + Random.Range(0f, 0.3f));
        canFire = true;
        isfiring = false;  
    }

    void AquireRandomTarget()
    {
        var tempTarget = GameManager.instance.FindTargetForAIFighter(team);

        if (tempTarget != null)
        {
            if (!isfiring)
            {
                canFire = true;
            }

            target = tempTarget;
        }

    }
}
