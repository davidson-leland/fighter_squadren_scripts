using UnityEngine;
using System.Collections;

public class Ship_Controller : MonoBehaviour {

    public Health health;

    public int team = 0;
    public Ship_Component[] shipComponents;

    public enum ShipSize//today i learned about enums. im a big boy now!
    {
        Medium, Capital
    }

    public const ShipSize shipSize = ShipSize.Medium;//0 will be medium sized ships. will have components but only one shield. only a small number of components
                                  //1 will be large capital ships, components will have their own seperate shield

	void Start ()
    {
        ShipStart();
    }

    protected virtual void ShipStart()
    {
        health.SetStats();

        foreach (Ship_Component sc in shipComponents)
        {
            sc.shipController = this;
            sc.team = team;
        }
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
