using UnityEngine;
using System.Collections;

public class AIFighterController : FighterController {

    Transform currentTravelTarget;
   
    public Transform[] testCourse;

    float turnAngle = 0f;
    float pitchAngle = 0f;

    GameObject leadIndicator;
    public GameObject leadprefab;

    public float aiReactionTime = 0.2f;
    public float stickSpeed = 500f;//think of this stat as how fast the pilot can move is joystick around.
    float adjustedStickSpeed = 0f;

    public Vector3[] previousLeadPoints = new Vector3[0];

    public bool chaseTarget = false;
    bool canLooseTarget = false, loosingTarget = false;

    public float accuracy_Drift = 30;
    public Vector3 drift = new Vector3();

    protected override void FighterStart()
    {
        base.FighterStart();

        currentTravelTarget = testCourse[Random.Range(0, testCourse.Length)];
        GameManager.instance.AddEnemyFighterToList(transform, myFighter, myFighter.testRend, this,team);       
        gameObject.name = "team-" + team + " " + gameObject.name + GameManager.instance.aiFighters[team].Count;      
        reactionTime = aiReactionTime;

        StartCoroutine(SetDrift());

    }

    protected override void SpawnFighter()
    {
        var spawned = (GameObject)Instantiate(fighterPrefab[1], transform.position, transform.rotation);
        myFighter = spawned.GetComponent<Fighter>();

        isVisible = myFighter.testRend.isVisible;
        myFighter.controller = this as AIFighterController;
        myFighter.transform.SetParent(transform);
    }

    public override void RespawnFighter()
    {
        SpawnFighter();
        GameManager.instance.AddEnemyFighterToList(transform, myFighter, myFighter.testRend, this, team);
    }


    protected override void FighterUpdate()
    {
        if (isDead)
        {
            return;
        }

        base.FighterUpdate();
        adjustedStickSpeed = stickSpeed * Time.deltaTime;
        
        CalcAITurn();
    }

    void CalcAITurn()
    {
        float distanceToTarget = 10000f;
        Vector3 compVector = new Vector3();

        //determin turn rate if chasing target or just patrolling
        float modMaxTurnAngle = myFighter.maxTurnAngle;
        float modMaxPitchAngle = myFighter.maxPitchAngle;

        if (!chaseTarget)
        {
            modMaxPitchAngle *= 0.5f;
            modMaxTurnAngle *= 0.5f;
        }

        if (chaseTarget)
        {
            if(target == null)//if we no longer have valid target go back to patrolling
            {
                chaseTarget = false;
                currentTravelTarget = testCourse[Random.Range(0, testCourse.Length)];
            }

            Vector3 targetLead = GetLead();

            distanceToTarget = Vector3.Distance(transform.position, targetLead);
            compVector = CalcCompVector(targetLead + drift);
        }
        else
        {
            distanceToTarget = Vector3.Distance(transform.position, currentTravelTarget.position);
            compVector = CalcCompVector(currentTravelTarget.position + drift);
        }

        targetRotationalPosition.CalcAngles(compVector);

        float desiredPitchAngle = 0;
        float desiredTurnAngle = 0f;        
       
        if (targetRotationalPosition.horizontalAngle > 0)
        {
            desiredTurnAngle = modMaxTurnAngle;           
        }
        else if (targetRotationalPosition.horizontalAngle < 0)
        {
            desiredTurnAngle = -modMaxTurnAngle;           
        }

        if (targetRotationalPosition.horizontalAngle < 90 && targetRotationalPosition.horizontalAngle > -90)
        {
            if (targetRotationalPosition.verticalAngle > 0)
            {
                desiredPitchAngle = -modMaxPitchAngle;
            }
            else if (targetRotationalPosition.verticalAngle < 0)
            {
                desiredPitchAngle = +modMaxPitchAngle;
            }
        }
        else
        {
            // first lets center vertical rotation back to zero. later on we can set v rotation based on height of target.
            float currentvAngle = transform.rotation.eulerAngles.x;

            if(currentvAngle > 180 && currentvAngle < 350)
            {
                desiredPitchAngle = modMaxPitchAngle;
            }
            else if( currentvAngle > 10 && currentvAngle < 180)
            {
                desiredPitchAngle = -modMaxPitchAngle;
            }
        }

        if (distanceToTarget < 1000f && chaseTarget)
        {
            if(targetRotationalPosition.horizontalAngle < 10 && targetRotationalPosition.horizontalAngle > -10)
            {
                if(targetRotationalPosition.verticalAngle < 10 && targetRotationalPosition.verticalAngle > -10)
                {
                    myFighter.AttemptFireBlasters();
                    canLooseTarget = true;
                }
            }
            else if (canLooseTarget)
            {
                if (targetRotationalPosition.horizontalAngle < -60 || targetRotationalPosition.horizontalAngle > 60 || targetRotationalPosition.verticalAngle < -60 || targetRotationalPosition.verticalAngle > 60)
                {
                    if (!loosingTarget)
                    {
                        StartCoroutine(AttemptFindTarget(02f));
                    }
                }
            }
        }

        turnAngle = AdjustAngle(desiredTurnAngle, turnAngle);
        pitchAngle = AdjustAngle(desiredPitchAngle, pitchAngle);       

        float x = turnAngle * Time.deltaTime;
       float y = pitchAngle * Time.deltaTime;

        //don't rotate past the target, save new turnangle that is applied.
        if( (x > 0 && x > targetRotationalPosition.horizontalAngle )|| (x < 0 && x < targetRotationalPosition.horizontalAngle))
        {
            if(targetRotationalPosition.horizontalAngle < 2 && targetRotationalPosition.horizontalAngle > -2)
            {
                x = targetRotationalPosition.horizontalAngle;               
                turnAngle = x / Time.deltaTime;
            }
        }

        if ((y > 0 && y > targetRotationalPosition.verticalAngle) || (y < 0 && y < targetRotationalPosition.verticalAngle))
        {
            if(targetRotationalPosition.verticalAngle < 90 && targetRotationalPosition.verticalAngle > -90)
            {
                if(targetRotationalPosition.verticalAngle < 2 && targetRotationalPosition.verticalAngle > -2)
                {
                    y = -targetRotationalPosition.verticalAngle;
                    pitchAngle = y / Time.deltaTime;
                }                
            }            
        }

        if (distanceToTarget <= 50f)
        {
            currentTravelTarget = testCourse[Random.Range(0, testCourse.Length)];
            
            if (Random.Range(0f, 10f) > 4)
            {
                AquireRandomTarget();
                chaseTarget = true;
            }  
        }

        MoveFighter(new Vector3(y, x, 0), 1);
    }

    float AdjustAngle( float desiredAngle, float actualAngle)
    {
        if (actualAngle < desiredAngle)
        {
            actualAngle += adjustedStickSpeed;

            if (actualAngle > desiredAngle)
            {
                actualAngle = desiredAngle;
            }
        }
        else
        {
            actualAngle -= adjustedStickSpeed;

            if (actualAngle < desiredAngle)
            {
                actualAngle = desiredAngle;
            }
        }
        return actualAngle;
    }

    protected override void DestroyFighter()
    {
        GameManager.instance.RemoveEnemyFighterFromLists(myFighter, team);
        base.DestroyFighter();
    }

    IEnumerator AttemptFindTarget(float AttemptTime)
    {
        loosingTarget = true;
        yield return new WaitForSeconds(AttemptTime);

        if (targetRotationalPosition.horizontalAngle < -60 || targetRotationalPosition.horizontalAngle > 60 || targetRotationalPosition.verticalAngle < -60 || targetRotationalPosition.verticalAngle > 60)
        {
            chaseTarget = false;
            currentTravelTarget = testCourse[Random.Range(0, testCourse.Length)];

        if(target.controller as PlayerController != null)
            {
                Debug.Log(" lost a atail");
            }
        }

        loosingTarget = false;
        canLooseTarget = false;
    }

    public void AquireRandomTarget()
    {
        var tempTarget = GameManager.instance.FindTargetForAIFighter(team);
               
        if(tempTarget == null)
        {
            chaseTarget = false;
        }
        else
        {
            if (tempTarget.controller as PlayerController != null)
            {
                Debug.Log("you have a tail");
            }

            target = tempTarget;
            chaseTarget = true;
        }
    }

    protected IEnumerator SetDrift()
    {
        while (!isDead)
        {
            drift.x = Random.Range(-accuracy_Drift, accuracy_Drift);
            drift.y = Random.Range(-accuracy_Drift, accuracy_Drift);
            drift.z = Random.Range(-accuracy_Drift, accuracy_Drift);

            yield return new WaitForSeconds(5f);
        }
    }
}
