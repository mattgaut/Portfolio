using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSlider : Slider {

    [SerializeField]
    Character slider_target;
	
	// Update is called once per frame
	void Update () {
        SetFill(slider_target.health/slider_target.max_health.value);
	}
}
