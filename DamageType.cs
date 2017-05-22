using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageType  {

	public enum DamageTypes
    {
        Default, Direct, ShieldsOnly
    }

    public DamageTypes damageType;

    public GameObject HitCollider(Collider other, string ownerName, int damage, bool damageShip = true, bool damageComponent = true, bool damageFighter = true)
    {
        GameObject topLevel = other.gameObject;

        bool b = false;       

        while (!b)// i need to completly re-do this to take into account ship components.
        {

            if (topLevel.tag == "Ship" && damageShip)
            {
                if (topLevel.name != ownerName)
                {
                    var otherController = topLevel.GetComponent<Ship_Controller>();

                    if (otherController != null)
                    {
                        otherController.TakeDamage(damage, damageType);
                    }
                }

                b = true;
               
            }
            else if (topLevel.tag == "Ship_Component" && damageComponent)
            {

                if (topLevel.name != ownerName)
                {
                    var otherController = topLevel.GetComponent<Ship_Component>();

                    if (otherController != null)
                    {
                        otherController.TakeDamage(damage,damageType);
                    }
                }

                b = true;
               
            }
            else if ((topLevel.tag == "Fighter" || topLevel.tag == "Player") && damageFighter)
            {

                if (topLevel.name != ownerName)
                {
                    var otherController = topLevel.GetComponent<FighterController>();

                    if (otherController != null)
                    {
                        otherController.TakeDamage(damage, damageType);
                    }
                }

                b = true;
               
            }

            if (topLevel.transform.parent == null)
            {
                b = true;
            }
            else
            {
                topLevel = topLevel.transform.parent.gameObject;
            }
            //break;
        }

        return topLevel;
    }
}
