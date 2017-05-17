using UnityEngine;
using System.Collections;

public class Ship_Controller : MonoBehaviour {

    // Use this for initialization

    public Health health = new Health();

    public Ship_Component[] shipComponents;

    public enum ShipSize//today i learned about enums. im a big boy now!
    {
        Medium, Capital
    }

    public const ShipSize shipSize = ShipSize.Medium;//0 will be medium sized ships. will have components but only one shield. only a small number of components
                                  //1 will be large capital ships, components will have their own seperate shield

	void Start ()
    {
	    
	}

    protected virtual void ShipStart()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        ShipUpdate(Time.deltaTime);
	}

    protected virtual void ShipUpdate(float tick)
    {

    }

    public virtual void TakeDamage(int ammount, DamageType.DamageTypes dType = DamageType.DamageTypes.Default)
    {
        health.TakeDamage(ammount, dType);
    }
}
