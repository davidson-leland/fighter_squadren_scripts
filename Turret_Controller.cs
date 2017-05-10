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

    protected bool canFire = true;
    bool isfiring = false;

    [SerializeField]
    GameObject blastPrefab;

    [SerializeField]
    protected float reactionTime = 0;

    Vector3 delayedVR;

    Vector3 drift = new Vector3();

    [SerializeField]
    float accuracy = 30f;

   // int targetIndex = 0;

   

    //public Transform leadIndicator;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(SetDrift());
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(target == null)
        {
            // target = GameManager.instance.PlayerFighters[0][0].fighterScript;

             AquireRandomTarget();

            //AquireNextTarget();          
            
        }

        TurretUpdate(Time.deltaTime);
	}

    protected void TurretUpdate(float tick)//im going to operate on the assumption that this will be faster than accessing time.deltatime everytine i need it.
    {
        if(target != null )
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if(distanceToTarget < 2000)
            {
                       

                Vector3 compVector = transform.InverseTransformPoint(target.transform.position);
                //compVector = transform.InverseTransformPoint(targetLead);
                targetRotationalPosition.CalcAngles(compVector);

                if ((targetRotationalPosition.horizontalAngle < angleMax && targetRotationalPosition.horizontalAngle > -angleMax)
                    && (targetRotationalPosition.verticalAngle < angleMax && targetRotationalPosition.verticalAngle > -angleMax))
                {
                    TrackTarget(tick);

                    //Debug.Log(distanceToTarget);                    

                    if (distanceToTarget < 1500 && canFire && !isfiring)
                    {
                        //Debug.Log("try fire turret");

                        /* if (targetRotationalPosition.verticalAngle < 10 && targetRotationalPosition.verticalAngle > -10)
                         {
                             if (targetRotationalPosition.verticalAngle < 10 && targetRotationalPosition.verticalAngle > -10)
                             {
                                 StartCoroutine(FireTurret());
                             }
                         }   */

                        StartCoroutine(FireTurret());
                    }
                }
                else
                {
                    target = null;
                }
            }
            else
            {
                target = null;
               // Debug.Log("+++++++++++++++ Target too far ++++++++++++++");
            }            
        }
    }

    protected void TrackTarget(float tick)
    {


        Vector3 targetLead = GetLead() + drift;

        
        /*if (leadIndicator != null)
        {
            leadIndicator.position = targetLead;
        }*/


        Vector3 compVector = new Vector3();

        //compVector = rotator.InverseTransformPoint(target.transform.position);

        compVector = rotator.InverseTransformPoint(targetLead);


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

        //attemptFire();
        //yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(1f + Random.Range(0f, 0.3f));
        canFire = true;
        isfiring = false;  
    }

    void AquireRandomTarget()
    {
        
       // var tempTarget = GameManager.instance.PlayerFighters[0][0].fighterScript;
        var tempTarget = GameManager.instance.FindTargetForAIFighter(0);

        if (tempTarget != null)
        {
            if (!isfiring)
            {
                canFire = true;
            }

            target = tempTarget;
        }

    }

    void AquireNextTarget()
    {
        //Debug.Log("trying to find target");
       /* if (GameManager.instance.aiFighters.Length != 0)
        {
            //Debug.Log("are fighters");
            if (GameManager.instance.aiFighters[1].Count > 0)
            {
               // Debug.Log("looking for enemy fighter");
               target = GameManager.instance.aiFighters[1][targetIndex].fighterScript;

                targetIndex++;

                if (targetIndex >= GameManager.instance.aiFighters[1].Count)
                {
                    targetIndex = 0;
                }
            }
        }*/
        
    }


    protected Vector3 GetLead()
    {
        Vector3 toReturn = new Vector3();

        if (target == null)
        {
            return toReturn;
        }
       // Debug.Log(target.transform.position - targetsLastPosition);
        Vector3 delta = target.transform.position - transform.position;
        Vector3 vr = ((target.transform.position - targetsLastPosition) /*- ( transform.position - myFightersLastPosition)*/) / Time.deltaTime;

        if (reactionTime > 0)
        {
            StartCoroutine(DelayedSetVR(vr));
        }
        else
        {
            delayedVR = vr;
        }
        //    ^
        // super accurate version, but goes all over the place if we include the noted out code above. |
        float t = AimAhead(delta, vr, 500);

        //idea, add a reaction time coroutine to change VR with a delay.


        if (t > 0)
        {
           // Debug.Log("not accurate");
            toReturn = target.transform.position + t * vr;
        }
        else
        {

            toReturn = target.transform.position;
        }


        targetsLastPosition = target.transform.position;       
        //Debug.Log(toReturn);

        return toReturn;
    }

    protected float AimAhead(Vector3 delta, Vector3 vr, float muzzleV)
    {
        float a = Vector3.Dot(vr, vr) - muzzleV * muzzleV;
        float b = 2f * Vector3.Dot(vr, delta);
        float c = Vector3.Dot(delta, delta);

        float det = b * b - 4f * a * c;

        if (det > 0)
        {
            return 2f * c / (Mathf.Sqrt(det) - b);
        }
        else { return -1; }
    }

    IEnumerator DelayedSetVR(Vector3 newVR)
    {
        yield return new WaitForSeconds(0.2f);

        delayedVR = newVR;
    }

    IEnumerator SetDrift()
    {
        while (true)
        {
            drift.x = Random.Range(-accuracy, accuracy);
            drift.y = Random.Range(-accuracy, accuracy);
            drift.z = Random.Range(-accuracy, accuracy);

            yield return new WaitForSeconds(5f);
        }
    }


}
