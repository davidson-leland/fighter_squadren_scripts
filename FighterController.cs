using UnityEngine;
using System.Collections;

public class FighterController : MonoBehaviour {

    protected Transform fighterTransform;//base transform of the fighter
    protected Fighter myFighter; // handles speed/firing guns/ turn radius/ health/ and other stats specific to individual fighters

    protected Fighter target = null; //current locked target
    protected Transform tempFTarget = null;

    [SerializeField]
    protected GameObject fighterPrefab;//prefab we will use to instantiate/spawn

    protected bool isVisible;//will eventually be how we check visibility
    
    protected float currentSpeed = 0f;

    public Health fighterHealth;

    public Vector3 targetsLastPosition = new Vector3();
    public Vector3 myFightersLastPosition = new Vector3();

   protected RotationalPosition targetRotationalPosition = new RotationalPosition();


    // Use this for initialization
    void Start ()
    {
        SpawnFighter();
        FighterStart();

        Debug.Log(gameObject.name);

    }

    protected virtual void FighterStart()
    {
        //override in every childclass
    }
	
	// Update is called once per frame
	void Update ()
    {       
       FighterUpdate();
	}

    protected virtual void FighterUpdate()
    {
        //may use for things in the future? leaving options open. def used for child classes
    }

    /*will use this to spawn our figher from the prefab and set all variables so controller can communicate
       with the fighter and vice versa*/
    protected virtual void SpawnFighter()
    {

    }

    //will give us a vector we can use to compare enemy position based on our rotation
    protected Vector3 CalcCompVector(Vector3 other)
    {
        return transform.InverseTransformPoint(other);
    }

    protected void MoveFighter(Vector3 rotate, float speedMod)
    {
        if(Time.timeScale == 0)
        {
            return;
        }
        
        //*****************************
        //sets rotation
        //*****************************
        transform.Rotate(rotate);

        //removes z axis rotation
        var oldRotation = transform.rotation.eulerAngles;
        oldRotation.z = 0;
        transform.rotation = Quaternion.Euler(oldRotation);

        //*****************************
        //sets movement
        //*****************************
        currentSpeed = speedMod * myFighter.topSpeed;
        Vector3 movement = Vector3.forward * currentSpeed * Time.deltaTime;
        transform.Translate(movement);

        //*****************************
        //sets fighter bank rotation
        //*****************************
        Vector3 newfighterRot = myFighter.transform.rotation.eulerAngles;
        newfighterRot.z = myFighter.fighterTurnRot * ((rotate.y / Time.deltaTime) / myFighter.maxTurnAngle) * -1;
        myFighter.transform.rotation = Quaternion.Euler(newfighterRot);
    }

    protected Vector3 GetLead()//change to a return vector3?
    {
        Vector3 toReturn = new Vector3();
                
        Vector3 delta = target.transform.position - transform.position;
        Vector3 vr = ((target.transform.position - targetsLastPosition) - (transform.position - myFightersLastPosition)) / Time.deltaTime;        

        float t = AimAhead(delta, vr, currentSpeed + 200f);
        //Debug.Log(t);

        if (t > 0)
        {
           toReturn = target.transform.position + t * vr;
        }
        else
        {
            
            toReturn = target.transform.position;
        }

        targetsLastPosition = target.transform.position;
        myFightersLastPosition = transform.position;

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

    public virtual void TakeDamage(int ammount)//need to move this to fighter script
    {
       

        if (!myFighter.alive)
        {
            return;
        }

        int healthCheck = myFighter.TakeDamage(ammount);

        if (healthCheck <= 0)
        {
            //DestroyFighter();
        }
        //Debug.Log(myFighter.health.hull);
 
    }

    protected virtual void DestroyFighter()
    {
        Destroy(gameObject);
    }
}
