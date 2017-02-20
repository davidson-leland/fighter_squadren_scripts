using UnityEngine;
using System.Collections;

public class AIFighterController : FighterController {

    Transform currentTravelTarget;

    public Transform[] testCourse;

    float turnAngle = 0f;

    protected override void FighterStart()
    {
        base.FighterStart();

        currentTravelTarget = testCourse[Random.Range(0, testCourse.Length)];
        GameManager.instance.AddEnemyFighterToList(transform, myFighter, myFighter.testRend);

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

        CalcAITurn();
    }

    void CalcAITurn()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTravelTarget.position);

        Vector3 compVector = CalcCompVector(currentTravelTarget.position);

        float y = 0;

        if (compVector.y > 5)
        {
            y = -50 * Time.deltaTime;
        }
        else if (compVector.y < -5)
        {
            y = 50 * Time.deltaTime;
        }

        if (compVector.x > 10)
        {
            //Debug.Log("is to right");
            turnAngle = myFighter.maxTurnAngle;
        }
        else if (compVector.x < -10)
        {
           // Debug.Log("is to left");
            turnAngle = -myFighter.maxTurnAngle;
        }
        else if (compVector.z > 0)
        {
            //Debug.Log("is in front");
            turnAngle = 0;
        }
        else if (compVector.z < 0)
        {

           // Debug.Log("is in back");
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

        float x = turnAngle * Time.deltaTime;

        if (distanceToTarget <= 50f)
        {
            currentTravelTarget = testCourse[Random.Range(0, testCourse.Length)];
        }

        MoveFighter(new Vector3(y, x, 0), 1);
    }
}
