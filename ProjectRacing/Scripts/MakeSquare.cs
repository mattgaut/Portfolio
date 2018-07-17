using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSquare : MonoBehaviour {

    RectTransform rect_trans;

	// Use this for initialization
	void Start () {
        rect_trans = GetComponent<RectTransform>();
        rect_trans.sizeDelta = new Vector2(rect_trans.sizeDelta.x, rect_trans.rect.width);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
