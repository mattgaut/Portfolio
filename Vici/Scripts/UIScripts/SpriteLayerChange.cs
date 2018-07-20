using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerChange : MonoBehaviour {

    [SerializeField]
    SpriteRenderer sprite;

    public void ChangeLayer(string layer_name) {
        sprite.sortingLayerName = layer_name;
    }
    public void ChangeOrder(int order) {
        sprite.sortingOrder = order;
    }
}
