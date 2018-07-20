using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Slider : MonoBehaviour {

    [SerializeField]
    bool x_axis, y_axis;
    float width, height;

    [SerializeField]
    float start_fill;

    [SerializeField]
    Image fill_image;
    [SerializeField]
    Image back_image;

    protected virtual void Start() {
        fill_image.rectTransform.sizeDelta = Vector2.zero;
        width = fill_image.GetComponent<RectTransform>().rect.width;
        height = fill_image.GetComponent<RectTransform>().rect.height;
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
        fill_image.transform.localScale = new Vector3(x_axis ? fill : 1, y_axis ? fill : 1, 0);
        fill_image.GetComponent<RectTransform>().localPosition = new Vector3(x_axis ? -(width / 2) * (1 - fill) : 0, y_axis ? -(height / 2) * (1 - fill) : 0, 0) + back_image.transform.GetComponent<RectTransform>().localPosition;
    }

    public void SetFill(float percent) {
        fill = percent;
    }

    public void SetFill(float over, float under) {
        fill = (over / under);
    }


}