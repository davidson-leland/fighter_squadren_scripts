using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public List<FighterInformation> Hero_fighters = new List<FighterInformation>();
    public List<FighterInformation> Enemy_fighters = new List<FighterInformation>();
    
   
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    
    
    // Use this for initialization
	void Start () {
        //Debug.Log("doing stuff");
	}
	
	// Update is called once per frame
	void Update () {
	
       
	}
   
    public void AddEnemyFighterToList(Transform _transform, Fighter _fighter,Renderer _visible)
    {
        //Debug.Log("adding to lists");
        FighterInformation newFighter = new FighterInformation(_transform, _fighter, _visible);

        Enemy_fighters.Add(newFighter);

       // Enemy_fighters[Enemy_fighters.Count].baseTransform.gameObject.name = "enemy_" + Enemy_fighters.Count;
    }

    public void RemoveEnemyFighterFromLists(Fighter _fighter)
    {
        //Debug.Log(Enemy_fighters.FindIndex(x => x.fighterScript == _fighter));

        Enemy_fighters.Remove(Enemy_fighters.Find(x => x.fighterScript == _fighter));

        Debug.Log(Enemy_fighters.Count);         
    }

    public void AddHeroFighterToList(Transform _transform, Fighter _fighter, Renderer _visible)
    {
        FighterInformation newFighter = new FighterInformation(_transform, _fighter, _visible);

        Hero_fighters.Add(newFighter);
    }

    public List<Transform> GetEnemiesOnScreen()
    {
        List<Transform> toSend = new List<Transform>();

        foreach (FighterInformation fI in Enemy_fighters)
        {
           // Debug.Log(fI.renderer.isVisible);

            if (fI.renderer.isVisible)
            {
                toSend.Add(fI.baseTransform);

            }

        }

        return toSend;
    }

 }

public struct FighterInformation
{
    public Transform baseTransform;
    public Fighter fighterScript;
    public Renderer renderer;


    public FighterInformation(Transform _transform, Fighter _fighterScript, Renderer  _visible)
    {
        baseTransform = _transform;
        fighterScript = _fighterScript;
        renderer = _visible;
    }
}


