using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTimerHolder : MonoBehaviour {
    [SerializeField]
    GameObject timer_prefab;
    
    public void SpawnTimer(Sprite s, Color c, float length) {
        ItemTimer new_timer = Instantiate(timer_prefab, transform).GetComponent<ItemTimer>();
        new_timer.Init(s, c);
        StartCoroutine(TimerRoutine(new_timer, length));
    }

    IEnumerator TimerRoutine(ItemTimer t, float length) {
        float start_time = length;
        while (length > 0) {
            length -= Time.deltaTime;
            t.SetFill(length/ start_time);
            yield return null;
        }
        Destroy(t.gameObject);
    }
}
