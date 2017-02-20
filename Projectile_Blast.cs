using UnityEngine;
using System.Collections;

public class Projectile_Blast : MonoBehaviour {

    public float speed = 1000;

    public Fighter owner;

    public GameObject blastHit;
    
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

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.transform);

        GameObject topLevel = other.gameObject;

        while(topLevel.transform.parent != null)
        {
            topLevel = topLevel.transform.parent.gameObject;
        }

        //Debug.Log(topLevel);

        if(topLevel.name != "Player")
        {
            //Debug.Log(topLevel);

            var blast = (GameObject)Instantiate(blastHit, transform.position, transform.rotation);

            blast.transform.SetParent(topLevel.transform);


            Destroy(blast, 01f);
            Destroy(gameObject);

        }
    }
}
