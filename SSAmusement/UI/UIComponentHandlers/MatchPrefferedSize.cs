using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class MatchPrefferedSize : MonoBehaviour {

    [SerializeField] Text text_box;
    RectTransform rt;

    private void Awake() {
        Canvas.ForceUpdateCanvases();
    }
    private void Start() {
        rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(text_box.preferredWidth, text_box.preferredHeight);
    }
    // Update is called once per frame
    void LateUpdate () {
        rt.sizeDelta = new Vector2(text_box.preferredWidth, text_box.preferredHeight);
    }
}
