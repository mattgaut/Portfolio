using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField]
    GameObject open_on_hover;

    public GameObject instantiated_object {
        get; private set;
    }

    public DisplayInfo display;

    Coroutine counter;

    public void OnPointerEnter(PointerEventData eventData) {
        hover = true;
        counter = StartCoroutine(CountDown());
    }

    bool hover;
    public void OnPointerExit(PointerEventData eventData) {
        hover = false;
    }

    IEnumerator CountDown() {
        float counter = .15f;
        while (counter > 0 && hover) {
            counter -= Time.deltaTime;
            yield return null;
        }
        if (hover) {
            instantiated_object = Instantiate(open_on_hover, Input.mousePosition, Quaternion.identity);
            instantiated_object.transform.SetParent(transform.parent);
            instantiated_object.GetComponent<DisplaySkill>().Set(display);
        }
        while (hover) {
            if (instantiated_object) instantiated_object.transform.position = Input.mousePosition + Vector3.up * 5;
            yield return null;
        }
        Destroy(instantiated_object);
    }
}

