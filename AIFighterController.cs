using UnityEngine;
using System.Collections;

public class AIFighterController : FighterController {

    Transform currentTravelTarget;
   
    public Transform[] testCourse;

    Vector3 previousvector = new Vector3();
    Vector3 currentTargetLead = new Vector3();

    const float newLeadTime = 0.1f;
    float currentLeadTime = 00.5f;

    float turnAngle = 0f;
    float pitchAngle = 0f;

    GameObject leadIndicator;
    public GameObject leadprefab;

    public float aiReactionTime = 0.2f;
    public float stickSpeed = 500f;//think of this stat as how fast the pilot can move is joystick around.
    float adjustedStickSpeed = 0f;

    public Vector3[] previousLeadPoints = new Vector3[0];

    Vector3 delayedTargetLead = new Vector3();

    protected override void FighterStart()
    {
        base.FighterStart();

        currentTravelTarget = testCourse[Random.Range(0, testCourse.Length)];
        GameManager.instance.AddEnemyFighterToList(transform, myFighter, myFighter.testRend);

        //currentTravelTarget = GameManager.instance.Hero_fighters[0].baseTransform;
        gameObject.name = gameObject.name + GameManager.instance.Enemy_fighters.Count;
        target = GameManager.instance.Hero_fighters[0].fighterScript;

        leadIndicator = (GameObject)Instantiate(leadprefab);

        reactionTime = aiReactionTime;

    }

    protected override void SpawnFighter()
    {
        var spawned = (GameObject)Instantiate(fighterPrefab, transform.position, transform.rotation);
        myFighter = spawned.GetComponent<Fighter>();

        isVisible = myFighter.testRend.isVisible;
        myFighter.controller = this as AIFighterController;
        myFighter.transform.SetParent(transform);
    }


    protected override void FighterUpdate()
    {
        base.FighterUpdate();
        adjustedStickSpeed = stickSpeed * Time.deltaTime;
        //myFighter.AttemptFireBlasters();
        CalcAITurn();
    }

    void CalcAITurn()
    {
        //for following a fighter
         Vector3 targetLead = GetLead();

       // StartCoroutine(DelayedSetAimTargetLocation(targetLead));

          float distanceToTarget = Vector3.Distance(transform.position, targetLead);       
          Vector3 compVector = CalcCompVector(targetLead);
          leadIndicator.transform.position = targetLead;


        if(distanceToTarget < 1000f)
        {
            myFighter.AttemptFireBlasters();
        }

        //for following navpoints
      /* float distanceToTarget = Vector3.Distance(transform.position, currentTravelTarget.position);
       Vector3 compVector = CalcCompVector(currentTravelTarget.position);
       */
       
        targetRotationalPosition.CalcAngles(compVector);

        float desiredPitchAngle = 0;
        float desiredTurnAngle = 0f;        
       
        if (targetRotationalPosition.horizontalAngle > 0)
        {
            desiredTurnAngle = myFighter.maxTurnAngle;           
        }
        else if (targetRotationalPosition.horizontalAngle < 0)
        {
            desiredTurnAngle = -myFighter.maxTurnAngle;           
        }

        if (targetRotationalPosition.horizontalAngle < 90 && targetRotationalPosition.horizontalAngle > -90)
        {
            if (targetRotationalPosition.verticalAngle > 0)
            {
                desiredPitchAngle = -myFighter.maxTurnAngle;
            }
            else if (targetRotationalPosition.verticalAngle < 0)
            {
                desiredPitchAngle = +myFighter.maxTurnAngle;
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
            //Debug.Log("change targets");
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
        GameManager.instance.RemoveEnemyFighterFromLists(myFighter);

        base.DestroyFighter();
    }

    IEnumerator DelayedSetAimTargetLocation(Vector3 newLead)
    {
        yield return new WaitForSeconds(0.2f);

        delayedTargetLead = newLead;


    }
}
