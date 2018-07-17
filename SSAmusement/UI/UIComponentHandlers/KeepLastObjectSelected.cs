using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeepLastObjectSelected : MonoBehaviour {

    GameObject last_selected;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (last_selected == null || (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject != last_selected)) {
            last_selected = EventSystem.current.currentSelectedGameObject;
        } else if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy) {
            EventSystem.current.SetSelectedGameObject(last_selected);
        }
	}
}
