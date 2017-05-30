using UnityEngine;
using System.Collections;

public class Ship_Component : MonoBehaviour {


    public Health health;
    public int team = 0;

    public GameObject[] partsToHide;//will be the game objects to hide when component is destroyed.

    public GameObject[] destroyEffects;// effects to play when compenent is destroyed.

    [System.NonSerialized]
    public Ship_Controller shipController;
    // Use this for initialization
    void Start ()
    {
        ShipComponent_Start();
	}

    protected virtual void ShipComponent_Start()
    {
        health.SetStats();
    }
	
	// Update is called once per frame
	void Update ()
    {
        ShipComponent_Update(Time.deltaTime);
	}

    protected virtual void ShipComponent_Update(float tick)
    {

    }

    public virtual void TakeDamage(int ammount, DamageType.DamageTypes dtype = DamageType.DamageTypes.Default)
    {

        if(health.hull == 0)
        {
            //shipController.TakeDamage(ammount, DamageType.DamageTypes.Direct);//im wondering if we want these to be permenant holes in the shields or not.
            return;
        }

        int newhull = health.TakeDamage(ammount);

        if(newhull < 1)
        {
            shipController.TakeDamage(health.maxHull, DamageType.DamageTypes.Direct); 
            
            foreach(GameObject gO in partsToHide)
            {
                gO.SetActive(false);
            } 

            foreach( GameObject gO in destroyEffects)
            {
                gO.SetActive(true);
            }
        }
    }
}
