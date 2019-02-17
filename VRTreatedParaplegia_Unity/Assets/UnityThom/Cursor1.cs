using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor1 : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.GetHairTriggerDown() && gameObject.activeSelf)
        {
            if (collidingObject /*&& collidingObject.CompareTag("point_vr")*/)
            {
                Destroy(collidingObject);
            }
            else
            {
                CreatePoint(transform);
            }
        }
        

    }

    private SteamVR_TrackedObject trackedObj;

    //create
    public Transform Point_vr;
    //destroy
    private GameObject collidingObject;
    //highlight
    private Color startcolor;
    private Renderer rend;

    private float cursorVelocity = 0.005f;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }


    void Awake()
    {
        trackedObj = GetComponentInParent<SteamVR_TrackedObject>();
    }

    private void CreatePoint(Transform tr)
    {

        //create
        Instantiate(Point_vr, tr.position, Quaternion.identity);

        //return position in coor.txt
        /* using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(path + "coor.txt", true))
         {
             file.WriteLine(tr.position);
         }*/
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);

        //start highlight
        if (other.gameObject.CompareTag("point_vr"))
        {
            rend = other.gameObject.GetComponent<Renderer>();
            startcolor = rend.material.color;
            rend.material.color = Color.red;
        }

    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        //end highlight
        if (other.gameObject.CompareTag("point_vr"))
        {
            rend = other.gameObject.GetComponent<Renderer>();
            rend.material.color = startcolor;
        }

        collidingObject = null;
    }


    public void CursorUpdate()
    {
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            
        }
        else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            transform.localPosition = new Vector3(0, 0, 0);
        }

        //cursor movement
        if (Controller.GetAxis().y != 0 && gameObject.activeSelf && transform.localPosition.z+ Controller.GetAxis().y * cursorVelocity>=0)
        {
            transform.localPosition += new Vector3(0, 0, Controller.GetAxis().y * cursorVelocity);
        }

    }
}
