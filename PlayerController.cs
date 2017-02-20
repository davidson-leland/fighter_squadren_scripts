using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerController : FighterController{

    [SerializeField]
    protected Camera mainCamera;//follow cam

    [SerializeField]
    protected Canvas baseCanvas;//canvas UI elements
    [SerializeField]
    protected RectTransform canvasTargetRet;

    [SerializeField]
    protected GameObject targetLockRetPreFab;

    [SerializeField]
    protected ArrowController arrowController;//this is the pointer that locks ontu your current target


    Transform tempTarget; //currently this is placeholder untill i redo the targeting mechanics. which will come after splitting the fighter controller

    float turnY = 0, turnX = 0;//turning parameters. most important for holding a turn.

    protected override void FighterUpdate()
    {
        base.FighterUpdate();

        GetInput();

        if(tempTarget != null)
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

                newCanvasTargetRet *= -100;
            }

            newCanvasTargetRet.z = 0;
            canvasTargetRet.position = newCanvasTargetRet;
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

        //arrowController.gameObject.SetActive(false);
        //canvasTargetRet.gameObject.SetActive(false);
    }


    void GetInput()
    {
        float desiredX = Input.GetAxis("Horizontal");
        float desiredY = Input.GetAxis("Vertical");

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

            
                for (int i = 0; i < targetList.Count; i++)
                {
                    Vector3 tempV = mainCamera.WorldToScreenPoint(targetList[i].position);
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
            if (launchCounter >= launchTimer)
            {
                myFighter.FireOrdinance(currentSpeed, targetList[iM]);
                Destroy(targetLockReticules[iM].gameObject);
                iM++;
                launchCounter = 0;
            }

            launchCounter += Time.deltaTime;
            yield return null;
        }
    }
}

