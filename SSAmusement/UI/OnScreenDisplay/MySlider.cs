using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MySlider : MonoBehaviour {
    [SerializeField] Image target;
    [SerializeField] Image background;
    [SerializeField] Text target_text;

    bool set_text;
    RectTransform rct, brct;

    [SerializeField]
    bool x_axis, y_axis;
    float width, height;

    [SerializeField]
    float start_fill;

    protected virtual void Start() {
        rct = target.GetComponent<RectTransform>();
        brct = background.GetComponent<RectTransform>();
        target.rectTransform.sizeDelta = Vector2.zero;
        width = brct.rect.width;
        height = brct.rect.height;
        fill = start_fill;
    }

    float _fill;
    public float fill {
        get { return _fill; }
        protected set {
            _fill = value;
            if (_fill > 1) {
                _fill = 1;
            } else if (_fill < 0) {
                _fill = 0;
            }
            SetImage();
        }
    }

    protected virtual void SetImage() {
        target.transform.localScale = new Vector3(x_axis ? fill : 1, y_axis ? fill : 1, 0);
        rct.localPosition = new Vector3(x_axis ? -(width / 2) * (1 - fill) : 0, y_axis ? -(height / 2) * (1 - fill) : 0, 0) + brct.localPosition;
    }

    public void SetFill(float percent) {
        fill = percent;
        target_text.text = Mathf.RoundToInt(percent) + "%";
    }

    public void SetFill(float over, float under) {
        if (under == 0) fill = 0;
        else fill = (over / under);
        target_text.text = Mathf.RoundToInt(over) + " / " + Mathf.RoundToInt(under);
    }
}
