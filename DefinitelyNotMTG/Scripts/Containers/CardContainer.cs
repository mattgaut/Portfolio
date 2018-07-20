using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardContainer : MonoBehaviour {

    [SerializeField]
    public Zone _zone;
    public Zone zone {
        get { return _zone; }
    }

    public List<Card> cards {
        get; private set;
    }

    Dictionary<Card, Coroutine> lerp;

    protected virtual void Awake() {
        cards = new List<Card>();
        lerp = new Dictionary<Card, Coroutine>();
    }

    public void AddCard(Card c) {
        cards.Add(c);
        c.SetContainer(this);
        UpdateView();
    }
    public void RemoveCard(Card c) {
        if (lerp.ContainsKey(c)) {
            StopCoroutine(lerp[c]);
            lerp.Remove(c);
        }
        cards.Remove(c);
        UpdateView(false);
    }

    public void Shuffle() {
        int i = cards.Count;
        while (i > 1) {
            int j = Random.Range(0, i--);
            Card temp = cards[j];
            cards[j] = cards[i];
            cards[i] = temp;
        }
    }

    protected virtual void UpdateView(bool adding_card = true) {

    }

    protected void LerpToPosition(Card c, Vector3 position) {
        if (lerp.ContainsKey(c)) {
            StopCoroutine(lerp[c]);
            lerp.Remove(c);
        }
        lerp.Add(c, StartCoroutine(Lerp(c, position)));
    }
    protected IEnumerator Lerp(Card c, Vector3 position, float time = .25f) {
        float lerp_time = 0;
        Vector3 initial_position = c.transform.position;
        while (lerp_time < time) {
            yield return null;
            Vector3 towards = position - initial_position;
            c.transform.position = initial_position + towards * (lerp_time / time);
            lerp_time += Time.deltaTime;
        }
        c.transform.position = position;
        lerp.Remove(c);
    }
    protected void SetPosition(Card c, Vector3 position) {
        c.transform.position = position;
    }
}
