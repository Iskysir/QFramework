using UnityEngine;
using System.Collections;

public class KeepHorizontal : MonoBehaviour
{

    private Transform mParTrans = null;

	// Use this for initialization
	void Start ()
	{
	    mParTrans = this.transform.parent;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    float x = -mParTrans.localEulerAngles.x;
	    float y = -transform.localEulerAngles.y;
	    float z = -mParTrans.localEulerAngles.z;
        this.transform.localEulerAngles = new Vector3(x,y,z);
	}
}
