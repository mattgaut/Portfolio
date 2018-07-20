using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetingDisplay : MonoBehaviour {

    [SerializeField]
    GameObject target, source, blocker, attacker;

    GameObject instantiated_attacker;
    Dictionary<GameObject, GameObject> instantiated_blockers;
    Dictionary<GameObject, GameObject> instantiated_targets;
    GameObject instantiated_source;

    Vector3 rotation = new Vector3(90, 0, 0);
    Vector3 scale = new Vector3(1, 1, 1);

    bool force_active = false;


    void Awake() {
        instantiated_blockers = new Dictionary<GameObject, GameObject>();
        instantiated_targets = new Dictionary<GameObject, GameObject>();
    }

    public void SetForceActive(bool force) {
        force_active = force;
        if (force) {
            SetActive(true);
        }
    }

    public void SetAttacker(GameObject attach) {
        instantiated_attacker = Instantiate(attacker);
        instantiated_attacker.transform.SetParent(attach.transform, true);
        instantiated_attacker.transform.localPosition = Vector3.zero;
    }
    public void AddBlocker(GameObject attach) {
        GameObject new_blocker = Instantiate(blocker);
        new_blocker.transform.SetParent(attach.transform, true);
        new_blocker.transform.localPosition = Vector3.zero;
        instantiated_blockers.Add(attach, new_blocker);
    }
    public void SetBlockerNumber(GameObject blocker, int number) {
        if (instantiated_blockers.ContainsKey(blocker)) {
            Text text = instantiated_blockers[blocker].GetComponentInChildren<Text>();
            if (number > 0) {
                text.text = "" + number;
            } else {
                text.text = "";
            }
        }
    }
    public void RemoveBlocker(GameObject attach) {
        Destroy(instantiated_blockers[attach]);
        instantiated_blockers.Remove(attach);
    }

    public void SetSource(GameObject attach) {
        instantiated_source = Instantiate(source);
        instantiated_source.transform.SetParent(attach.transform, true);
        instantiated_source.transform.localPosition = Vector3.zero;
    }
    public void AddTarget(GameObject attach) {
        GameObject new_target = Instantiate(target);
        new_target.transform.SetParent(attach.transform, true);
        new_target.transform.localPosition = Vector3.zero;
        instantiated_targets.Add(attach, new_target);
    }
    public void RemoveTarget(GameObject attach) {
        Destroy(instantiated_targets[attach]);
        instantiated_targets.Remove(attach);
    }

    public void SetActive(bool active = true) {
        if (instantiated_attacker) instantiated_attacker.SetActive(active || force_active);
        if (instantiated_source) instantiated_source.SetActive(active || force_active);
        foreach (GameObject go in instantiated_blockers.Values) {
            go.SetActive(active || force_active);
        }
        foreach (GameObject go in instantiated_targets.Values) {
            go.SetActive(active || force_active);
        }
    }

    public void Clear(bool destroy_gameobject = true) {
        if (instantiated_attacker) Destroy(instantiated_attacker);
        instantiated_attacker = null;
        if (instantiated_source) Destroy(instantiated_source);
        instantiated_source = null;
        foreach (GameObject go in instantiated_blockers.Values) {
            Destroy(go);
        }
        instantiated_blockers.Clear();
        foreach (GameObject go in instantiated_targets.Values) {
            Destroy(go);
        }
        instantiated_targets.Clear();
        if (destroy_gameobject) Destroy(gameObject);
    }

}
