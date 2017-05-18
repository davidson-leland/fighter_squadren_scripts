using UnityEngine;
using System.Collections;

public class Ship_Component : MonoBehaviour {


    public Health health;

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
        }
    }
}
