using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class create : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public Transform Point_vr;
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log(Input.mousePosition);
            Instantiate(Point_vr, new Vector3(0, 2, 0), Quaternion.identity);
        }
    }
}
