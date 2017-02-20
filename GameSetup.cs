﻿using UnityEngine;
using System.Collections;

public class GameSetup : MonoBehaviour {

    public GameObject gameManager;

    public GameObject enemyPrefab;

    public Transform[] testcourse;

    int enemies = 1;
    
    void Awake()
    {
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        }

        //Invoke("SpawnAnEnemy", 0.1f);
    }
    
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SpawnAnEnemy()
    {


        var spawned = (GameObject)Instantiate(enemyPrefab, transform.position, transform.rotation);

        var fighter = spawned.GetComponentInChildren<Fighter>();

        fighter.testCourse = testcourse;

        fighter.currentTarget = testcourse[0];


        

        enemies++;
        //Debug.Log(enemies);
        //Debug.Log(Time.deltaTime);
        //Debug.Log(1 / 120f);
        if (enemies < 10)
        {
            Invoke("SpawnAnEnemy", 0.1f);
        }
    }
}