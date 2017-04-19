using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerController : FighterController{

    [SerializeField]
    protected Camera mainCamera;//follow cam

    [SerializeField]
    protected Canvas baseCanvas;//canvas UI elements
    [SerializeField]
    protected CanvasController canvasController;
    [SerializeField]
    protected RectTransform canvasTargetRet;

    [SerializeField]
    protected GameObject targetLockRetPreFab;

    [SerializeField]
    protected ArrowController arrowController;//this is the pointer that locks ontu your current target


    Transform tempTarget; //currently this is placeholder untill i redo the targeting mechanics. which will come after splitting the fighter controller

    float turnY = 0, turnX = 0;//turning parameters. most important for holding a turn.

    bool usePauseMenu = false;

    [SerializeField]
    GameObject PauseMenu;


    protected override void FighterUpdate()
    {
        base.FighterUpdate();

        GetInput();

        if (tempTarget != null)
        {
            Vector3 compVector = CalcCompVector(tempTarget.position);
            Vector3 newCanvasTargetRet = mainCamera.WorldToScreenPoint(tempTarget.position);

            if (compVector.z > 0)
            {
                newCanvasTargetRet.x = Mathf.Clamp(newCanvasTargetRet.x, 0, mainCamera.pixelWidth);
                newCanvasTargetRet.y = Mathf.Clamp(newCanvasTargetRet.y, 0, mainCamera.pixelHeight);
            }
            else
            {

                newCanvasTargetRet.x = -100;
                newCanvasTargetRet.y = -100;
            }

            newCanvasTargetRet.z = 0;
            canvasTargetRet.position = newCanvasTargetRet;
        }
        else
        {
            canvasTargetRet.position = new Vector2(-100, -100);
        }


    }

    protected override void SpawnFighter()
    {
        var spawned = (GameObject)Instantiate(fighterPrefab,transform.position,transform.rotation);
        myFighter = spawned.GetComponent<Fighter>();

        isVisible = myFighter.testRend.isVisible;
        myFighter.controller = this as PlayerController;
        myFighter.transform.SetParent(transform);
        myFighter.ActivateReticules();

        GameManager.instance.AddHeroFighterToList(transform, myFighter, myFighter.testRend);

        //arrowController.gameObject.SetActive(false);
        //canvasTargetRet.gameObject.SetActive(false);

        canvasController.InitHealthBar(myFighter.health.hull, myFighter.health.sheilds);
        myFighter.health.canvasController = canvasController;     
    }


    void GetInput()
    {
        float desiredX = Input.GetAxis("Horizontal");
        float desiredY = Input.GetAxis("Vertical");

        var newCameraPosition = new Vector3();
        var actualCameraPosition = mainCamera.transform.localPosition;
        //var tempRotation = myFighter.transform.localRotation.eulerAngles;

        float desiredCameraX = 15 * desiredX;
        float desiredCameraY = (-5 * desiredY) + 16;
        float desiredCameraZ = -42;

        if(desiredCameraX > actualCameraPosition.x)
        {
            newCameraPosition.x = actualCameraPosition.x +  100 * Time.deltaTime;

            if(newCameraPosition.x > desiredCameraX)
            {
                newCameraPosition.x = desiredCameraX;
            }
        }
        else if(desiredCameraX < actualCameraPosition.x)
        {
            newCameraPosition.x = actualCameraPosition.x - 100 * Time.deltaTime;

            if (newCameraPosition.x < desiredCameraX)
            {
                newCameraPosition.x = desiredCameraX;
            }
        }
        else
        {
            newCameraPosition.x = desiredCameraX;
        }

        if (desiredCameraY > actualCameraPosition.y)
        {
            newCameraPosition.y = actualCameraPosition.y + 100 * Time.deltaTime;

            if (newCameraPosition.y > desiredCameraY)
            {
                newCameraPosition.y = desiredCameraY;
            }
        }
        else if (desiredCameraY < actualCameraPosition.y)
        {
            newCameraPosition.y = actualCameraPosition.y - 100 * Time.deltaTime;

            if (newCameraPosition.y < desiredCameraY)
            {
                newCameraPosition.y = desiredCameraY;
            }
        }
        else
        {
            newCameraPosition.y = desiredCameraY;
        }
        

        /*tempRotation.x = 5f * desiredY;
        tempRotation.y = 5 * desiredX;
        tempRotation.z = 0;*/

        //myFighter.transform.localRotation = Quaternion.Euler(tempRotation);//during eventual replication onto networked games remove this rotation.
       

        float x = CalculateTurnRate(desiredX, ref turnX);
        float y = CalculateTurnRate(desiredY, ref turnY);

        float speedMod = 1f;

        if (Input.GetAxis("Brake") == 1)
        {
            speedMod *= 0.3f;
            //z = -8;was used to handle camera movment
        }
        else if (Input.GetAxis("Boost") == 1)
        {
            speedMod *= 1.6f;
            //z = -18;was used to handle camera movement
        }

        if (speedMod > 1)
        {
            desiredCameraZ = -80f;
        }
        else if(speedMod < 1)
        {
            desiredCameraZ = -30f;
        }

        if (desiredCameraZ > actualCameraPosition.z)
        {
            newCameraPosition.z = actualCameraPosition.z + 100 * Time.deltaTime;

            if (newCameraPosition.z > desiredCameraZ)
            {
                newCameraPosition.z = desiredCameraZ;
            }
        }
        else if (desiredCameraZ < actualCameraPosition.z)
        {
            newCameraPosition.z = actualCameraPosition.z - 400 * Time.deltaTime;

            if (newCameraPosition.z < desiredCameraZ)
            {
                newCameraPosition.z = desiredCameraZ;
            }
        }
        else
        {
            newCameraPosition.z = desiredCameraZ;
        }

        mainCamera.transform.localPosition = newCameraPosition;

        if (Input.GetButton("Fire1"))
        {
            myFighter.AttemptFireBlasters();
        }

        if (Input.GetButtonDown("Target"))
        {
            AquireNewTarget();
        }

        if (Input.GetButtonDown("Fire_Ordinance"))
        {
            StartCoroutine(AquireOrdinanceTargetLocks());
        }

        MoveFighter(new Vector3(y, x, 0f), speedMod);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwapHud(!usePauseMenu);
        }

    }

    float CalculateTurnRate(float desiredRate, ref float lastRate)
    {
        float toReturn = desiredRate;
        float turnRate = 50;

        //Debug.Log(lastRate);

        if (desiredRate >= 1 || desiredRate <= -1)
        {
            turnRate = lastRate + (100 * Time.deltaTime);

            if (turnRate > myFighter.maxTurnAngle)
            {
                turnRate = myFighter.maxTurnAngle;
            }
        }
        else if (lastRate > 50)
        {
            //Debug.Log("ting ting");
            turnRate = lastRate - (100 * Time.deltaTime);

        }
        lastRate = turnRate;

        //Debug.Log(turnRate);
        toReturn = turnRate * desiredRate * Time.deltaTime;

        //Debug.Log(toReturn);

        return toReturn;
    }

    void AquireNewTarget()
    {
        var targetList = GameManager.instance.GetEnemiesOnScreen();
        float lastDist = 1000;

        foreach (Transform t in targetList)
        {
            float newT = Vector2.Distance(mainCamera.WorldToViewportPoint(t.position), new Vector2(0.5f, 0.5f));

            if (newT < lastDist)
            {
                tempTarget = t;
                lastDist = newT;
            }

            tempFTarget = tempTarget;
            //Debug.Log(newT);
        }

        if (tempTarget != null)
        {
            canvasTargetRet.gameObject.SetActive(true);
            arrowController.gameObject.SetActive(true);
            arrowController.target = tempTarget;

        }
    }

    /*will add new targets on screen based on enemies closest to the center of the screen
        to a list over time then launch ordinance at them*/
    IEnumerator AquireOrdinanceTargetLocks()
    {
        float lockTime = 0;

        //Debug.Log("rawr");

        List<Transform> targetList = new List<Transform>();
        List<RectTransform> targetLockReticules = new List<RectTransform>();

        while (Input.GetButton("Fire_Ordinance"))
        {
            lockTime += Time.deltaTime;

            if (lockTime > 0.3)
            {              

                if ( targetList.Contains(tempTarget) || tempTarget == null )
                {
                    var tempTargetList = GameManager.instance.GetEnemiesOnScreen();

                    if(tempTargetList.Count > 0)
                    {
                        float lastDist = 1000;
                        Transform toADD = tempTargetList[0];
                       
                        foreach (Transform t in tempTargetList)
                        {

                            float newT = Vector2.Distance(mainCamera.WorldToViewportPoint(t.position), new Vector2(0.5f, 0.5f));

                            if (newT < lastDist && !targetList.Contains(t))
                            {
                                toADD = t;
                                lastDist = newT;
                            }
                            //Debug.Log(newT);
                        }

                        if (!targetList.Contains(toADD) && toADD != null)
                        {
                            targetList.Add(toADD);
                            var newRet = Instantiate(targetLockRetPreFab);
                            newRet.transform.SetParent(baseCanvas.transform, false);
                            targetLockReticules.Add(newRet.GetComponent<RectTransform>());
                            lockTime = 0;
                        }
                    }
                    
                }
                else if(tempTarget != null)
                {
                    targetList.Add(tempTarget);
                    var newRet = Instantiate(targetLockRetPreFab);
                    newRet.transform.SetParent(baseCanvas.transform, false);
                    targetLockReticules.Add(newRet.GetComponent<RectTransform>());
                    lockTime = 0;
                }
            }

            //Debug.Log(targetList.Count);
            
            PositionTargetLocks(targetLockReticules, targetList);

            yield return null;
        }

        int iM = 0;
        float launchTimer = 0.1f;
        float launchCounter = launchTimer;

        while (iM < targetList.Count)
        {
            if (launchCounter >= launchTimer)
            {
                if(targetList[iM] != null)
                {
                    myFighter.FireOrdinance(currentSpeed, targetList[iM]);
                    Destroy(targetLockReticules[iM].gameObject);
                }              
               
                iM++;
                launchCounter = 0;
            }

            launchCounter += Time.deltaTime;
            PositionTargetLocks(targetLockReticules, targetList);
            yield return null;
        }
    }

    void PositionTargetLocks(List<RectTransform> targetLockReticules, List<Transform> targetList)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetLockReticules[i] != null)
            {
                if(targetList[i] != null)
                {
                    Vector3 tempV = mainCamera.WorldToScreenPoint(targetList[i].position);
                    tempV.z = 0;

                    targetLockReticules[i].position = tempV;
                    //Debug.Log(targetList[i].position);
                }
                else
                {
                    Destroy(targetLockReticules[i].gameObject);
                }


            }
        }
    }

    public void SwapHud(bool bpauseMenu)
    {
        if (bpauseMenu)
        {
            PauseMenu.SetActive(true);
            usePauseMenu = true;
            Time.timeScale = 0.0f;
        }
        else
        {
            PauseMenu.SetActive(false);
            usePauseMenu = false;
            Time.timeScale = 1f;
        }
    }

    public override void TakeDamage(int ammount)
    {
        //Debug.Log("take it and like it");
        base.TakeDamage(ammount);
        canvasController.UpdateHealthBar(myFighter.health.hull, myFighter.health.sheilds);
    }
}

