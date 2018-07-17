using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour {

    [SerializeField] List<UnityEvent> ue;

    public void Trigger(int i) {
        ue[i].Invoke();
    }
}
