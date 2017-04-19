using UnityEngine;
using System.Collections;

public class AIFighterController : FighterController {

    Transform currentTravelTarget;

    public Transform[] testCourse;

    float turnAngle = 0f;
    float pitchAngle = 0f;

    GameObject leadIndicator;
    public GameObject leadprefab;

    public float stickSpeed = 10000f;//think of this stat as how fast the pilot can move is joystick around.

    protected override void FighterStart()
    {
        base.FighterStart();

       /* currentTravelTarget = testCourse[Random.Range(0, testCourse.Length)];*/
        GameManager.instance.AddEnemyFighterToList(transform, myFighter, myFighter.testRend);

        currentTravelTarget = GameManager.instance.Hero_fighters[0].baseTransform;
        gameObject.name = gameObject.name + GameManager.instance.Enemy_fighters.Count;
        target = GameManager.instance.Hero_fighters[0].fighterScript;

        leadIndicator = (GameObject)Instantiate(leadprefab);
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


        myFighter.AttemptFireBlasters();
        CalcAITurn();
    }

    void CalcAITurn()
    {
        //for following a fighter
        Vector3 targetLead = GetLead();
        float distanceToTarget = Vector3.Distance(transform.position, targetLead);
        Vector3 compVector = CalcCompVector(targetLead);

        //for following navpoints
        /*float distanceToTarget = Vector3.Distance(transform.position, currentTravelTarget.position);
        Vector3 compVector = CalcCompVector(currentTravelTarget.position);*/

        leadIndicator.transform.position = targetLead;

        targetRotationalPosition.CalcAngles(compVector);

        float desiredPitchAngle = 0;
        float desiredTurnAngle = 0f;

        float adjustedStickSpeed = stickSpeed * Time.deltaTime;
       
        if (targetRotationalPosition.horizontalAngle > 0)
        {
            desiredTurnAngle = myFighter.maxTurnAngle;

            if(desiredTurnAngle > targetRotationalPosition.horizontalAngle)
            {
                //Debug.Log("beep");
                desiredTurnAngle = targetRotationalPosition.horizontalAngle;
            }
        }
        else if (targetRotationalPosition.horizontalAngle < 0)
        {
            desiredTurnAngle = -myFighter.maxTurnAngle;

            if (desiredTurnAngle < targetRotationalPosition.horizontalAngle)
            {
               // Debug.Log("boop");
                desiredTurnAngle = targetRotationalPosition.horizontalAngle;
            }
        }

        if (targetRotationalPosition.horizontalAngle > 50 || targetRotationalPosition.horizontalAngle < -50)
        {
            // Debug.Log("is in back");
            /*float hDif = currentTravelTarget.position.y - transform.position.y;

            if( hDif > 5)
            {
                y = -50 * Time.deltaTime;
            }
            else if(hDif < -5)
            {
                y = 50 * Time.deltaTime;
            }    */       
        }
        else
        {
           if (targetRotationalPosition.verticalAngle > 0)
            {
                desiredPitchAngle = -myFighter.maxTurnAngle;

                if(desiredPitchAngle < targetRotationalPosition.verticalAngle)
                {
                    desiredPitchAngle = -targetRotationalPosition.verticalAngle;
                }
            }

            else if (targetRotationalPosition.verticalAngle < 0)
            {
                desiredPitchAngle = +myFighter.maxTurnAngle;

                if (desiredPitchAngle > targetRotationalPosition.verticalAngle)
                {
                    desiredPitchAngle = -targetRotationalPosition.verticalAngle;
                }
            }
        }

        if(turnAngle < desiredTurnAngle)
        {
            turnAngle += adjustedStickSpeed;

            if(turnAngle > desiredTurnAngle)
            {
                turnAngle = desiredTurnAngle;
            }
        }
        else
        {
            turnAngle -= adjustedStickSpeed;

            if(turnAngle < desiredTurnAngle)
            {
                turnAngle = desiredTurnAngle;
            }
        }

        if(pitchAngle < desiredPitchAngle)
        {
            pitchAngle += adjustedStickSpeed;

            if(pitchAngle > desiredPitchAngle)
            {
                pitchAngle = desiredPitchAngle;
            }
        }
        else
        {
            pitchAngle -= adjustedStickSpeed;

            if(pitchAngle < desiredPitchAngle)
            {
                pitchAngle = desiredPitchAngle;
            }
        }

        // y = 0;

        float x = turnAngle;
        float y = pitchAngle;

        Debug.Log(pitchAngle + " v " + desiredPitchAngle);
       
          
        if (distanceToTarget <= 50f)
        {
            currentTravelTarget = testCourse[Random.Range(0, testCourse.Length)];
        }

        MoveFighter(new Vector3(y, x, 0), 1);
    }

    protected override void DestroyFighter()
    {
        GameManager.instance.RemoveEnemyFighterFromLists(myFighter);

        base.DestroyFighter();
    }
}
