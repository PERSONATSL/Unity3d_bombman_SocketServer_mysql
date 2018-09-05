using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatUI : MonoBehaviour {

	
	void Update () {
        transform.localPosition = new Vector3(transform.localPosition.x, 1+Mathf.PingPong(Time.time / 2, 0.45f), transform.localPosition.z);
	}
}
