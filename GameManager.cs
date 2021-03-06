﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public List<FighterInformation>[] aiFighters = { new List<FighterInformation>(), new List<FighterInformation>(), new List<FighterInformation>()};
    public List<FighterInformation>[] PlayerFighters = { new List<FighterInformation>(), new List<FighterInformation>(), new List<FighterInformation>()};

    public List<Transform>[] CruiseMissles = { new List<Transform>(), new List<Transform>(), new List<Transform>()};
    public List<FighterController>[] fightersWaitingToRespawn = { new List<FighterController>(), new List<FighterController>(), new List<FighterController>() };

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
       
	}
	
	// Update is called once per frame
	void Update () {
	
       
	}
   
    public void AddEnemyFighterToList(Transform _transform, Fighter _fighter,Renderer _visible, FighterController _fighterController, int _team)
    {
        FighterInformation newFighter = new FighterInformation(_transform, _fighter, _visible, _fighterController);
        aiFighters[_team].Add(newFighter);
    }

    public void RemoveEnemyFighterFromLists(Fighter _fighter, int _team)
    {
        aiFighters[_team].Remove(aiFighters[_team].Find(x => x.fighterScript == _fighter));
    }

    public void AddHeroFighterToList(Transform _transform, Fighter _fighter, Renderer _visible, FighterController _fighterController, int _team)
    {
        FighterInformation newFighter = new FighterInformation(_transform, _fighter, _visible, _fighterController);
        PlayerFighters[_team].Add(newFighter);
    }

    public List<Transform> GetEnemiesOnScreen(int _team)
    {
        List<Transform> toSend = new List<Transform>();

        foreach (FighterInformation fI in aiFighters[_team])
        {
            if (fI.renderer.isVisible)
            {
                toSend.Add(fI.baseTransform);
            }
        }

        return toSend;
    }

    public Fighter FindTargetForAIFighter( int _team)
    {
        int i = 0;

        if(i == _team)
        {
            i = 1;
        }

        if(aiFighters[i].Count == 0)
        {
            return null;
        }

        int randomIndex = (int)Random.Range(0, aiFighters[i].Count + PlayerFighters[i].Count - 0.1f);

        if(randomIndex >= aiFighters[i].Count)
        {
            randomIndex -= aiFighters[i].Count;
            return PlayerFighters[i][randomIndex].fighterScript;
        }

        return aiFighters[i][randomIndex].fighterScript;
    }

    public void AddMissleToLists(int _team, Transform _transform)
    {
        CruiseMissles[_team].Add(_transform);
    }

    public void RemoveMissleFromLists( int _team, Transform _transform)
    {
        CruiseMissles[_team].Remove(CruiseMissles[_team].Find(x => x.transform == _transform));
    }

    public Transform CheckForMissles(int _team)
    {
        int i = 0;

        if (i == _team)
        {
            i = 1;
        }

        if(CruiseMissles[i].Count == 0)
        {
            return null;
        }

        int randomIndex = (int)Random.Range(0, CruiseMissles[i].Count - 0.1f);

        return CruiseMissles[i][randomIndex];
    }
 }

public struct FighterInformation
{
    public Transform baseTransform;
    public Fighter fighterScript;
    public Renderer renderer;
    public FighterController fighterController;

    public FighterInformation(Transform _transform, Fighter _fighterScript, Renderer  _visible, FighterController _fighterController)
    {
        baseTransform = _transform;
        fighterScript = _fighterScript;
        renderer = _visible;
        fighterController = _fighterController;
    }
}


