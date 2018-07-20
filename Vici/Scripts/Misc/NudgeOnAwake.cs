using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NudgeOnAwake : MonoBehaviour {

	void Awake () {
        StartCoroutine(Nudge());
	}
	
    IEnumerator Nudge() {
        transform.position += Vector3.up * .001f;
        yield return null;
        transform.position -= Vector3.up * .001f;
    }
}
