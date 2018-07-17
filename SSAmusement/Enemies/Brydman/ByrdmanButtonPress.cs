using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByrdmanButtonPress : MonoBehaviour {

    [SerializeField] Animator anim;
    [SerializeField] string idle;
    [SerializeField] List<string> buttons;

    
	// Update is called once per frame
	void Update () {
        Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName(idle));
		if (anim.GetCurrentAnimatorStateInfo(0).IsName(idle)) {
            anim.SetTrigger(buttons[Random.Range(0,4)]);
        }
	}
}
