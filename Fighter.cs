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
   
    //prototype npc movement
    float turnAngle = 0f, pitchAngle = 0f;//used for temp npc movement
    bool followPlayer = false;
    string[] movementModes = { "Patrol", "Combat", "Chase"};
    string currentMovement = "patrol";
    public Transform[] testCourse;
    public Transform currentTarget;

    [System.NonSerialized]
    public FighterController controller;
    [SerializeField]
    GameObject reticules;

    public Health health = new Health();

    Coroutine healthCo;

    public bool alive = true;

    // Use this for initialization
    void Start () {
        // Debug.Log(Vector3.forward);
        timeSinceLastFire = fireRate + 0.1f;

        HideShield();

        /*if (!testControl)
        {
            currentTarget = testCourse[Random.Range(0, testCourse.Length  )];
            GameManager.instance.AddEnemyFighterToList(transform.parent, this,testRend);
            
            //SetStraight();
        }*/
    }
	
	// Update is called once per frame
	void Update () {

        if(timeSinceLastFire < fireRate)
        {
            timeSinceLastFire += Time.deltaTime;
        }        

        if (testControl)
        {
            oldPlayerControlls();//what we used to do
        }
        else
        {
            oldAIControlls();//what we used to do
        }
        
       /* lastPosition = transform.parent.position;
        Debug.Log("when happens");*/

        /*if(controller as AIFighterController != null)
        {
            Debug.Log("health " + health.hull + ". shields = " + health.sheilds);
        }    */   

    }
    //holds old code for reference only
    void oldPlayerControlls()
    {
        /*
        float desiredX = Input.GetAxis("Horizontal");
        float desiredY = Input.GetAxis("Vertical");
        Vector3 newCanvasTargetRet = cameraMain.WorldToScreenPoint(currentTarget.transform.position);

        if (compVector.z > 0)
        {
            newCanvasTargetRet.x = Mathf.Clamp(newCanvasTargetRet.x, 0, cameraMain.pixelWidth);
            newCanvasTargetRet.y = Mathf.Clamp(newCanvasTargetRet.y, 0, cameraMain.pixelHeight);
        }
        else
        {

            newCanvasTargetRet *= -100;
        }

        newCanvasTargetRet.z = 0;

         if(compVector.z < 0)
         {
             newCanvasTargetRet.x *= -1;
         }

        canvastargetRet.position = newCanvasTargetRet;

        //x = maxTurnAngle * Input.GetAxis("Horizontal") * Time.deltaTime;
        x = CalculateTurnRate(desiredX, ref turnX);
        //y = maxPitchAngle * Input.GetAxis("Vertical") * Time.deltaTime;
        y = CalculateTurnRate(desiredY, ref turnY);


        //Debug.Log(Input.GetAxis("Speed"));

        Vector3 newCameraPos = cameraTransform.localPosition;
        newCameraPos.x = x * 2;
        newCameraPos.y = (-y * 2) + 5;

        //Debug.Log(Input.GetButton("Fire1"));
        float z = -12;

        if (Input.GetAxis("Brake") == 1)
        {
            speedToUse *= 0.3f;
            z = -8;
        }
        else if (Input.GetAxis("Boost") == 1)
        {
            speedToUse *= 1.6f;
            z = -18;
        }
        newCameraPos.z = z;

        // cameraTransform.localPosition = newCameraPos;
        if (Input.GetButton("Fire1") && timeSinceLastFire > fireRate)
        {
            timeSinceLastFire = 0;

            AttemptFire(firePortNum);

            //FireOrdinance(speedToUse, currentTarget);//for fun only

            firePortNum++;

            if (firePortNum >= gunPorts.Length)
            {
                firePortNum = 0;
            }
        }
        //Debug.Log(Time.deltaTime);
        //Debug.Log(Input.GetButton("Target"));

        if (Input.GetButtonDown("Target"))
        {
            //Debug.Log("getting new target");
            var targetList = GameManager.instance.GetEnemiesOnScreen();

            float lastDist = 1000;

            foreach (Transform t in targetList)
            {
                float newT = Vector2.Distance(cameraMain.WorldToViewportPoint(t.position), new Vector2(0.5f, 0.5f));

                if (newT < lastDist)
                {
                    currentTarget = t;
                    lastDist = newT;
                }

                //Debug.Log(newT);
            }

        }

        if (Input.GetButtonDown("Fire_Ordinance"))
        {
            //FireOrdinance(speedToUse, currentTarget);


            StartCoroutine(AquireTargets());


            foreach(Transform T in targetList)
            {
                FireOrdinance(speedToUse, T);
            }

        }*/

    }
    //holds old code for reference only
    void oldAIControlls()
    {
        /*
        //Debug.Log(testRend.isVisible);
        float distanceToTarget = Vector3.Distance(transform.parent.position, currentTarget.position);


        //Debug.Log(distanceToTarget);
        // Debug.Log(compVector.z);
        //Debug.Log(compVector);

        //float noTurn =  * Time.deltaTime + 0.1;

        if (true)
        {
            if (compVector.y > 5)
            {
                y = -50 * Time.deltaTime;
            }
            else if (compVector.y < -5)
            {
                y = 50 * Time.deltaTime;
            }
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
            //Debug.Log("is in front");
            turnAngle = 0;
        }
        else if (compVector.z < 0)
        {

            //Debug.Log("is in back");
            if (compVector.x > -.5 && compVector.x < 0.5)
            {
                if (turnAngle == 0 && distanceToTarget > 50)
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

        //turnAngle = 0;
        x = turnAngle * Time.deltaTime;

        if (distanceToTarget <= 50f)
        {
            currentTarget = testCourse[Random.Range(0, testCourse.Length)];
        }*/
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
        //blast.name = ("EnergyBlast" + gameObject);
        Destroy(blast, 3.0f);
    }   

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "EnergyBlast(Clone)") 
        {
            if(other.gameObject.GetComponent<Projectile_Blast>().owner != this)
            {
                //Debug.Log(other);

                ShowSheild();
            }            
        }        
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject);
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

            //FireOrdinance(speedToUse, currentTarget);//for fun only

            firePortNum++;

            if (firePortNum >= gunPorts.Length)
            {
                firePortNum = 0;
            }
        }

    }


    public void FireOrdinance(float _speed, Transform _target)
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

    public int TakeDamage(int ammount)//need to move this to fighter script
    {
        //return 1;

        if (alive)
        {
            int newHull = health.TakeDamage(ammount);


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
                Debug.Log("pop goes the weasle");
                alive = false;
            }
            return newHull;
        }

        return 0;
    }

    //old target lock code
    /* IEnumerator AquireTargets()
     {
         float lockTime = 0;

         //Debug.Log("rawr");

         List<Transform> targetList = new List<Transform>();
         List<RectTransform> targetLockReticules = new List<RectTransform>();    

         while (Input.GetButton("Fire_Ordinance"))
         {
             lockTime += Time.deltaTime;

             if(lockTime > 0.3)
             {
                 lockTime = 0;

                 if (targetList.Contains(currentTarget))
                 {
                     var tempTargetList = GameManager.instance.GetEnemiesOnScreen();
                     float lastDist = 1000;
                     Transform toADD = tempTargetList[0];

                     foreach (Transform t in tempTargetList)
                     {

                         float newT = Vector2.Distance(cameraMain.WorldToViewportPoint(t.position), new Vector2(0.5f, 0.5f));

                         if (newT < lastDist && !targetList.Contains(t))
                         {
                             toADD = t;
                             lastDist = newT;
                         }
                         //Debug.Log(newT);
                     }

                     if(!targetList.Contains(toADD) && toADD != null)
                     {
                         targetList.Add(toADD);
                         var newRet = Instantiate(targetLockRetPreFab);
                         newRet.transform.SetParent(baseCanvas.transform, false);
                         targetLockReticules.Add(newRet.GetComponent<RectTransform>());
                     }
                 }
                 else
                 {
                     targetList.Add(currentTarget);
                     var newRet = Instantiate(targetLockRetPreFab);
                     newRet.transform.SetParent(baseCanvas.transform, false);
                     targetLockReticules.Add(newRet.GetComponent<RectTransform>());
                 }
             }

             for (int i = 0; i < targetList.Count; i++)
             {
                 Vector3 tempV = cameraMain.WorldToScreenPoint(targetList[i].position);
                 tempV.z = 0;
                 targetLockReticules[i].position = tempV;
             }


             yield return null;
         }

         int iM = 0;
         float launchTimer = 0.1f;
         float launchCounter = launchTimer;

         while (iM < targetList.Count)
         {
             if(launchCounter >= launchTimer)
             {
                 FireOrdinance(launchSpeed, targetList[iM]);
                 Destroy(targetLockReticules[iM].gameObject);
                 iM++;
                 launchCounter = 0;               
             }

             launchCounter += Time.deltaTime;
             yield return null;
         }       
     }   */
}
