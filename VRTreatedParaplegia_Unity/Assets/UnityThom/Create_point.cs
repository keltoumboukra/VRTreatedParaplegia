using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create_point : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public Transform Point_vr;
	// Update is called once per frame
	void Update () {
        if (Controller.GetHairTriggerDown())
        {
                if (collidingObject)
                {
                    Destroy(collidingObject);
            }else
            {
                Debug.Log(gameObject.name + " Trigger Press");
                Instantiate(Point_vr, transform.position, Quaternion.identity);
            }
            
        }
    }

    private SteamVR_TrackedObject trackedObj;
    // 2
    private GameObject collidingObject;
    private GameObject objectInHand;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col)
    {
        // 1
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // 2
        collidingObject = col.gameObject;
    }

    // 1
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // 2
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 3
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

}
