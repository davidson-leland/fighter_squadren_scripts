using UnityEngine;
using System.Collections;

public class Projectile_Blast : MonoBehaviour {

    public float speed = 1000;

    public int damage = 1;

    public Fighter owner;
    public string ownerName;

    public GameObject blastHit;


    public DamageType damageType;

    public int team;
    
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 movement = Vector3.forward;
        movement *= speed * Time.deltaTime;

        transform.Translate(movement);
    }


    /*void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);
    }*/

    void OnTriggerEnter(Collider other)//i will need to add this code to a common class for all damage dealing things. possibly damagaeType?

    {

        //Debug.Log(other.transform);

        /* GameObject topLevel = other.gameObject;

         bool b = false;

         while(!b)// i need to completly re-do this to take into account ship components.
         {           

             if (topLevel.tag == "Ship")
             {
                 if (topLevel.name != ownerName)
                 {
                     var otherController = topLevel.GetComponent<Ship_Controller>();

                     if (otherController != null)
                     {
                         otherController.TakeDamage(damage, damageType.damageType);
                     }
                 }

                 b = true;
             }
             else if(topLevel.tag == "Ship_Component")
             {

                 if (topLevel.name != ownerName)
                 {
                     var otherController = topLevel.GetComponent<Ship_Component>();

                     if (otherController != null)
                     {
                         otherController.TakeDamage(damage, damageType.damageType);
                     }
                 }

                 b = true;
             }
             else if(topLevel.tag == "Fighter" || topLevel.tag == "Player")
             {

                 if (topLevel.name != ownerName)
                 {
                     var otherController = topLevel.GetComponent<FighterController>();

                     if (otherController != null)
                     {
                         otherController.TakeDamage(damage, damageType.damageType);
                     }
                 }

                 b = true;
             }

             if(topLevel.transform.parent == null)
             {
                 b = true;
             }
             else
             {
                 topLevel = topLevel.transform.parent.gameObject;
             }
             //break;
         }        

         //Debug.Log(topLevel);*/

        GameObject topLevel = damageType.HitCollider(other, ownerName, team, damage);

        if (topLevel.name != ownerName)
        {
            //Debug.Log(topLevel);

            var blast = (GameObject)Instantiate(blastHit, transform.position, transform.rotation);
            blast.transform.SetParent(topLevel.transform);


            Destroy(blast, 01f);
            Destroy(gameObject);

        }
    }
}
