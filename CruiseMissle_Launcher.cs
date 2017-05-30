using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiseMissle_Launcher : MonoBehaviour {

    public float restTime = 20;

    public Transform[] launchPoints;

    public GameObject missle_Prefab;

    public Transform enemyShipTransform;
    public Vector3 shipSize;

    [SerializeField]
    protected Ship_Component component;

    public int team = 0;
    
    // Use this for initialization
	void Start ()
    {
        StartCoroutine(launchMissles());
	}
	
	// Update is called once per frame
	void Update ()
    {

        if(component.health.hull > 0)
        {
            MissleLauncherUpdate(Time.deltaTime);
        }       

    }

    protected virtual void MissleLauncherUpdate(float tick)
    {

    }

    IEnumerator launchMissles()
    {
        yield return new WaitForSeconds(2);

        while (component.health.hull > 0)
        {          

            for (int i = 0; i < launchPoints.Length; i++)
            {
                var spawnedOrdinance = (GameObject)Instantiate(missle_Prefab, launchPoints[i].position, launchPoints[i].rotation);
                Cruise_Missle_Controller mController = spawnedOrdinance.GetComponent<Cruise_Missle_Controller>();

                mController.team = team;
                mController.target = enemyShipTransform;
                mController.drift.x = Random.Range(-shipSize.x, shipSize.x);
                mController.drift.y = Random.Range(-shipSize.y, shipSize.y);
                mController.drift.z = Random.Range(-shipSize.z, shipSize.z);

                float t = 0.1f + Random.Range(0, 0.1f);
                //  yield return new WaitForSeconds(Random.Range(0, 0.3f));
                yield return new WaitForSeconds(t);
            }
            yield return new WaitForSeconds(restTime);
        }
        
    }
}
