using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHeart : MonoBehaviour {

    public bool half_heart;

    public Sprite half_half, half_empty, full, empty, half;

    [SerializeField]
    Image heart;

    public void SetState(int state) {
        if (state >= 2) {
            heart.sprite = full;
        } else if (state == 1) {
            heart.sprite = half_heart ? half_half : half;
        } else if (state <= 0) {
            heart.sprite = half_heart ? half_empty : empty;
        }
    }
}
