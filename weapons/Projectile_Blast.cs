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
	void Update ()
    {
        Vector3 movement = Vector3.forward;
        movement *= speed * Time.deltaTime;

        transform.Translate(movement);
    }

    void OnTriggerEnter(Collider other)//i will need to add this code to a common class for all damage dealing things. possibly damagaeType?
    {
        GameObject topLevel = damageType.HitCollider(other, ownerName, team, damage);

        if (topLevel.name != ownerName)
        {
            var blast = (GameObject)Instantiate(blastHit, transform.position, transform.rotation);
            blast.transform.SetParent(topLevel.transform);

            Destroy(blast, 01f);
            Destroy(gameObject);
        }
    }
}
