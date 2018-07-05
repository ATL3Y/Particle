using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //transform.Rotate ( Vector3.right * Time.deltaTime );

        transform.Rotate ( Vector3.right * Time.deltaTime );
        float offset = (Time.timeSinceLevelLoad * 60f) % 360;
        float angle = -180f + offset;
        transform.rotation = Quaternion.AngleAxis ( angle, Vector3.up );

    }
}
