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
    [SerializeField]
    protected float snapToTargetAngle = 2;

    public int team;

    protected Fighter target = null;
    public Vector3 targetsLastPosition = new Vector3();

    protected RotationalPosition targetRotationalPosition = new RotationalPosition();

    [SerializeField]
    protected Transform rotator;

    protected bool canFire = true;
    protected bool isfiring = false;

    [SerializeField]
    protected GameObject blastPrefab;

    protected Vector3 drift = new Vector3();

    [SerializeField]
    protected float accuracy_Drift = 30f;

    [SerializeField]
    protected Ship_Component component;


   // int targetIndex = 0;

   

    //public Transform leadIndicator;

	// Use this for initialization
	void Start ()
    {
        TurretStart();

    }

    protected virtual void TurretStart()
    {
        StartCoroutine(SetDrift());
    }
	
	// Update is called once per frame
	void Update ()
    {

        if(component != null && component.health.hull != 0)
        {
            TurretUpdate(Time.deltaTime);
        }

       // TurretUpdate(Time.deltaTime);
    }

    //used individually in the child classes
    protected virtual void TurretUpdate(float tick)//im going to operate on the assumption that this will be faster than accessing time.deltatime everytine i need it.
    {
        
    }

    protected void TrackTarget(float tick, Vector3 toTrack)
    {
        Vector3 compVector = rotator.InverseTransformPoint(toTrack);
        targetRotationalPosition.CalcAngles(compVector);

        //Debug.Log(targetRotationalPosition.horizontalAngle);

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
                if (targetRotationalPosition.horizontalAngle < snapToTargetAngle && targetRotationalPosition.horizontalAngle > -snapToTargetAngle)
                {
                    x = targetRotationalPosition.horizontalAngle;                    
                }
            }

            if ((y > 0 && y > targetRotationalPosition.verticalAngle) || (y < 0 && y < targetRotationalPosition.verticalAngle))
            {
                if (targetRotationalPosition.horizontalAngle < 90 && targetRotationalPosition.horizontalAngle > -90)
                {
                    if (targetRotationalPosition.verticalAngle < snapToTargetAngle && targetRotationalPosition.verticalAngle > -snapToTargetAngle)
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

    protected virtual void attemptFire()
    {
        var blast = (GameObject)Instantiate(blastPrefab, gunPorts[0].position, gunPorts[0].rotation);
        var blastScript = blast.GetComponent<Projectile_Blast>();
        blastScript.ownerName = gameObject.name;
        blastScript.team = team;
        //blast.name = ("EnergyBlast" + gameObject);
        Destroy(blast, 3.0f);
    }

   protected virtual IEnumerator FireTurret()
    {
        isfiring = true;
        canFire = false;

        yield return new WaitForSeconds(Random.Range(0f, 0.3f));

        for ( int i = 0; i < 3; i++)
        {
            attemptFire();

            yield return new WaitForSeconds(0.3f);
        }

        //attemptFire();
        //yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(0);
        canFire = true;
        isfiring = false;  
    }

   protected virtual void AquireRandomTarget()
    {
         //var tempTarget = GameManager.instance.PlayerFighters[0][0].fighterScript;
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

    protected void AquireNextTarget()
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


    protected Vector3 GetLead( Vector3 _targetPosition)
    {
        Vector3 toReturn = new Vector3();
        
       // Debug.Log(target.transform.position - targetsLastPosition);
        Vector3 delta = _targetPosition - transform.position;
        Vector3 vr = (_targetPosition - targetsLastPosition) / Time.deltaTime;       
        float t = AimAhead(delta, vr, 500);

        if (t > 0)
        {
           // Debug.Log("not accurate");
            toReturn = _targetPosition + t * vr;
        }
        else
        {

            toReturn = _targetPosition;
        }

        targetsLastPosition = _targetPosition;
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

   protected IEnumerator SetDrift()
    {
        while (true)
        {
            drift.x = Random.Range(-accuracy_Drift, accuracy_Drift);
            drift.y = Random.Range(-accuracy_Drift, accuracy_Drift);
            drift.z = Random.Range(-accuracy_Drift, accuracy_Drift);

            yield return new WaitForSeconds(5f);
        }
    }
}
