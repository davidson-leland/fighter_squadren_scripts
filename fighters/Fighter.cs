using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fighter : MonoBehaviour {

    public float topSpeed = 100;

    public float maxTurnAngle = 30;
    public float maxPitchAngle = 30;
    public float turnInertia = 01f;//currently does nothing
    public float fighterTurnRot = 60;//max bank angle when turning

    [System.NonSerialized]
    public bool testControl = false;

    public float fireRate = 30;
    float timeSinceLastFire;

    public Transform[] gunPorts;
    public GameObject blastPrefab;
  
    public MeshRenderer shieldMesh;

    public GameObject ordinancePrefab;

    //public Transform fighterBody;

    int firePortNum = 0, directionMod = 1;//fire port num used for firing the laser blasts, direction mod is used for launching missles and is a temp measure

    public Renderer testRend;//holder for checking to see if fighter is on camera. will probably change
   
    [System.NonSerialized]
    public FighterController controller;
    [SerializeField]
    GameObject reticules;

    public Health health = new Health();

    Coroutine healthCo;

    public bool alive = true;

    // Use this for initialization
    void Start ()
    {
        timeSinceLastFire = fireRate + 0.1f;
        HideShield();
        health.SetStats();

        if(shieldMesh != null)
        {
            health.sheildMesh = shieldMesh.gameObject;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(timeSinceLastFire < fireRate)
        {
            timeSinceLastFire += Time.deltaTime;
        } 
    }
  
    public void ActivateReticules()
    {
        reticules.SetActive(true);
    }

    void AttemptFire(int portNum)
    {
        var blast = (GameObject)Instantiate(blastPrefab, gunPorts[portNum].position, transform.rotation);
        var blastScript = blast.GetComponent<Projectile_Blast>();

        blastScript.speed += topSpeed;
        blastScript.owner = this;
        blastScript.ownerName = controller.gameObject.name;
        blastScript.team = controller.team;

        Destroy(blast, 3.0f);
    } 

    void OnCollisionEnter(Collision collision)
    {
        ShowSheild();
    }

    void ShowSheild()
    {
        CancelInvoke("HideShield");
        shieldMesh.enabled = true;
        Invoke("HideShield", 0.1f);
    }

    void HideShield()
    {
        shieldMesh.enabled = false;
    }
    
    public void AttemptFireBlasters()
    {
        if ( timeSinceLastFire > fireRate)
        {
            timeSinceLastFire = 0;
            AttemptFire(firePortNum);
            firePortNum++;

            if (firePortNum >= gunPorts.Length)
            {
                firePortNum = 0;
            }
        }
    }

    public virtual void FireOrdinance(float _speed, Transform _target)
    {
        if(ordinancePrefab != null)
        {
            var spawnedOrdinance = (GameObject)Instantiate(ordinancePrefab, transform.position, transform.rotation);
            Missle_controller mController = spawnedOrdinance.GetComponent<Missle_controller>();

            mController.currentSpeed = _speed;
            mController.target = _target;
            mController.directionMod = directionMod;

            if(directionMod == 1)
            {
                directionMod = -1;
            }
            else
            {
                directionMod = 1;
            }
        }
    }

    public int TakeDamage(int ammount, DamageType.DamageTypes dType = DamageType.DamageTypes.Default)//need to move this to fighter script
    {
        if (alive)
        {
            if (health.sheilds > 0)
            {
                ShowSheild();   
            }

            int newHull = health.TakeDamage(ammount, dType);

            if (newHull > 0)
            {
                if (healthCo != null)
                {
                    StopCoroutine(healthCo);
                }

                healthCo = StartCoroutine(health.RefreshSheilds());  
            }
            else
            {
                alive = false;
            }

            return newHull;
        }

        return 0;
    }
}
