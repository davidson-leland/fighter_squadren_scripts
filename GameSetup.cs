using UnityEngine;
using System.Collections;

public class GameSetup : MonoBehaviour {

    public GameObject gameManager;
    public GameObject enemyPrefab;
    public Transform[] testcourse;
    public SpawnNodes[] teamSpawns;

   

    public int teamSize = 10;
    
    void Awake()
    {
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
    }
    
    // Use this for initialization
	void Start () {

        StartCoroutine(InitialSpawnFighters(0, teamSize));
        StartCoroutine(InitialSpawnFighters(1, teamSize));

        Invoke("StartAttacking", 10f);    
    }

    //only used atm for testing purposes to get ai fighters to attack eachother. should eventually be moved to game manager
    void StartAttacking()
    {
        int i = 0;

      /*  foreach (FighterInformation f in GameManager.instance.aiFighters[1])
        {           
            AIFighterController ai = f.fighterScript.controller as AIFighterController;

            ai.AquireRandomTarget();
            ai.chaseTarget = true;

            i++;

            if(i > 5)
            {
                break;
            }
        }

        i = 0;
      foreach (FighterInformation f in GameManager.instance.aiFighters[0])
        {
            AIFighterController ai = f.fighterScript.controller as AIFighterController;

            ai.AquireRandomTarget();
            ai.chaseTarget = true;

            i++;

            if (i > 5)
            {
                break;
            }
        }*/
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void SpawnAIFighter(int _team, Transform _transform)
    {
        var spawned = (GameObject)Instantiate(enemyPrefab, _transform.position, _transform.rotation);
        var fighter = spawned.GetComponentInChildren<AIFighterController>();

        fighter.testCourse = testcourse;
        fighter.team = _team;

    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator InitialSpawnFighters( int _team, int _teamSize)
    {
        yield return new WaitForSeconds(0.5f);

        int spawned = 0;

        while(spawned < _teamSize)
        {
            for(int i = 0; i < teamSpawns[_team].spawnNodes.Length; i++)
            {

                if(spawned < _teamSize)
                {
                    SpawnAIFighter(_team, teamSpawns[_team].spawnNodes[i]);
                    spawned++;
                } 
            }

            yield return new WaitForSeconds(02f);
        }
        
    }
}

[System.Serializable]
public struct SpawnNodes
{
    public Transform[] spawnNodes;
}
