using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour {

    public Transform target;
    
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log(target);
        transform.LookAt(target);

	}
}
