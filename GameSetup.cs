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

        StartCoroutine(RespawnTimer(20));
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

        StartCoroutine(SpawnFighters(_team, _teamSize));               
    }

    IEnumerator SpawnFighters(int _team, int _teamSize)
    {
        Debug.Log("spawn fighters called");
        int spawned = GameManager.instance.aiFighters[_team].Count;

        Debug.Log("spawned fighters = " + spawned);

        while (spawned < _teamSize)
        {
            for (int i = 0; i < teamSpawns[_team].spawnNodes.Length; i++)
            {

                if (spawned < _teamSize)
                {
                    Debug.Log("spawning fighter");
                    SpawnAIFighter(_team, teamSpawns[_team].spawnNodes[i]);
                    spawned++;
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

            Debug.Log("respawning fighters");
           StartCoroutine( SpawnFighters(0, teamSize));
           StartCoroutine( SpawnFighters(1, teamSize));
        }
    }
}

[System.Serializable]
public struct SpawnNodes
{
    public Transform[] spawnNodes;
}
