using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    RoomBuilder rb;
    [SerializeField] Pathway p;
    Room.DoorPosition dp;
    public bool is_doorway { get; private set; }

    private void Awake() {
        rb = FindObjectOfType<RoomBuilder>();
        is_doorway = false;
    }

    //private void OnMouseEnter() {
    //    GetComponent<SpriteRenderer>().enabled = true;
    //}
    //private void OnMouseExit() {
    //    GetComponent<SpriteRenderer>().enabled = false;
    //}
    //private void OnMouseOver() {
    //    if (Input.GetKeyDown(KeyCode.Mouse0)) {
    //        is_doorway = !is_doorway;
    //        rb.TogglePathway(this);
    //    }
    //}

    public void SetDoorPosition(Vector2Int block_position) {
        dp = new Room.DoorPosition(block_position, p);
    }

    public Room.DoorPosition Position() {
        return dp;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        is_doorway = !is_doorway;
        rb.TogglePathway(this);
    }
}
