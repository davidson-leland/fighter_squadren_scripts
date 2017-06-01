using UnityEngine;
using System.Collections;

public class GameSetup : MonoBehaviour {

    public GameObject gameManager;
    public GameObject enemyPrefab;
    public Transform[] testcourse;
    public SpawnNodes[] teamSpawns;

    public int teamSize = 10;
    
    //much of what is happening here needs to be moved over to the game manager.

    void Awake()
    {
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
    }
    
    // Use this for initialization
	void Start ()
    {

        StartCoroutine(InitialSpawnFighters(0, teamSize));
        StartCoroutine(InitialSpawnFighters(1, teamSize));

        StartCoroutine(RespawnTimer(20));
    }
    
	
	// Update is called once per frame
	void Update ()
    {
	
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
        StartCoroutine(SpawnFighters(_team, _teamSize));               
    }

    IEnumerator SpawnFighters(int _team, int _teamSize)
    {
        int spawned = GameManager.instance.aiFighters[_team].Count;

        while (spawned < _teamSize)
        {
            for (int i = 0; i < teamSpawns[_team].spawnNodes.Length; i++)
            {

                if (spawned < _teamSize)
                {
                    SpawnAIFighter(_team, teamSpawns[_team].spawnNodes[i]);
                    spawned++;
                }

                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    IEnumerator ReSpawnFighters(FighterController[] _Controllers)
    {
        if (_Controllers.Length != 0)
        {
            int i = 0;
            int t = _Controllers[0].team;

            foreach (FighterController fC in _Controllers)
            {
                fC.RespawnFighter();

                fC.transform.position = teamSpawns[t].spawnNodes[i].position;
                fC.transform.rotation = teamSpawns[t].spawnNodes[i].rotation;
                fC.isDead = false;

                i++;

                if (i >= teamSpawns[t].spawnNodes.Length)
                {
                    i = 0;
                }

                yield return new WaitForSeconds(0.2f);
            }
        }         
    }

    IEnumerator RespawnTimer(float respawnTime)
    {
        while (true)//replace with something else in future
        {
            yield return new WaitForSeconds(respawnTime);
            
            StartCoroutine( ReSpawnFighters(GameManager.instance.fightersWaitingToRespawn[0].ToArray()));
            StartCoroutine( ReSpawnFighters(GameManager.instance.fightersWaitingToRespawn[1].ToArray()));

            GameManager.instance.fightersWaitingToRespawn[0].Clear();
            GameManager.instance.fightersWaitingToRespawn[1].Clear();
        }
    }
}

[System.Serializable]
public struct SpawnNodes
{
    public Transform[] spawnNodes;
}
