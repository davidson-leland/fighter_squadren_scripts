using UnityEngine;
using System.Collections;

public class Ship_Component : MonoBehaviour {


    public Health health = new Health();

    [System.NonSerialized]
    public Ship_Controller shipController;
    // Use this for initialization
    void Start ()
    {
        ShipComponent_Start();
	}

    protected virtual void ShipComponent_Start()
    {

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
        int newhull = health.TakeDamage(ammount);

        if(newhull < 1)
        {
            shipController.TakeDamage(health.maxHull, DamageType.DamageTypes.Direct);            
        }
    }
}
