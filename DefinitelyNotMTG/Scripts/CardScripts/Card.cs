using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, ITargetable {

    public FieldCardDisplay card_field_display {
        get; private set;
    }
    public CardFaceDisplay card_face_display {
        get; private set;
    }

[SerializeField]
    GameObject face_view, field_view;

    [SerializeField]
    protected string _card_name;
    public string card_name {
        get { return _card_name; }
    }
    [SerializeField][TextArea(1,5)]
    protected string _card_description;
    public string card_description {
        get { return _card_description; }
    }

    [SerializeField]
    protected Sprite _art;
    public Sprite art {
        get { return _art; }
    }

    [SerializeField]
    protected MajorCardType _major_card_type;
    public MajorCardType major_card_type {
        get { return _major_card_type; }
    }

    [SerializeField]
    AbilitySet _triggers;
    public AbilitySet triggers {
        get { return _triggers; }
    }

    public EffectHolder effects {
        get; private set;
    }

    protected virtual void Awake() {
        effects = new EffectHolder(this);

        face_view = Instantiate(face_view);
        face_view.transform.SetParent(transform, false);
        card_face_display = face_view.GetComponent<CardFaceDisplay>();
        card_face_display.Display(this);

        field_view = Instantiate(field_view);
        field_view.transform.SetParent(transform, false);
        card_field_display = field_view.GetComponent<FieldCardDisplay>();
        card_field_display.Display(this);
    }

    protected virtual void Start() {

    }

    Zone _zone;
    public Zone zone {
        get { return _zone; }
        protected set {
            _zone = value;

            if (zone == Zone.field) {
                field_view.SetActive(true);
                card_field_display.Display(this);

                face_view.SetActive(false);
            } else if (zone == Zone.exile) {
                field_view.SetActive(false);
                face_view.SetActive(false);
            } else if (zone == Zone.hand) {
                field_view.SetActive(false);
                face_view.SetActive(true);
            } else if (zone == Zone.library) {
                field_view.SetActive(false);
                face_view.SetActive(false);
            } else if (zone == Zone.graveyard) {
                field_view.SetActive(false);
                face_view.SetActive(false);
            } else if (zone == Zone.stack) {
                field_view.SetActive(false);
                face_view.SetActive(true);
            }
        }
    }
    public void SetView(int i) {
        field_view.SetActive(i == 1);
        face_view.SetActive(i == 2);
    }
    public CardContainer contained_in {
        get; private set;
    }
    public void SetContainer(CardContainer c) {
        if (contained_in != null)
            contained_in.RemoveCard(this);
        if (c != null) {
            contained_in = c;
            zone = contained_in.zone;
        } else {
            contained_in = c;
            zone = Zone.none;
        }
    }

    public Player controller {
        get; protected set;
    }
    public void SetController(Player p) {
        controller = p;
    }

    public Player owner {
        get; protected set;
    }
    public void SetOwner(Player p) {
        owner = p;
    }

    protected virtual void ContextClickHandler() {
        if (GameManager.instance.targeting && GameManager.instance.targeter.controller == GameManager.instance.player1) {
            GameManager.instance.ToggleTarget(this);
        } else if (controller == GameManager.instance.active_player) {
            if (zone == Zone.hand) {
                ClickInHand();
            } else if (zone == Zone.library) {
                ClickInLibrary();
            } else if (zone == Zone.exile) {
                ClickInExile();
            } else if (zone == Zone.field) {
                ClickInField();
            } else if (zone == Zone.graveyard) {
                ClickInGraveyard();
            }
        }
        GameManager.instance.Hover(false, this);
        GameManager.instance.Hover(true, this);
    }
    protected virtual void ClickInHand() {

    }
    protected virtual void ClickInLibrary() {

    }
    protected virtual void ClickInExile() {

    }
    protected virtual void ClickInField() {

    }
    protected virtual void ClickInGraveyard() {

    }

    Coroutine check_click, check_zoom_click;

    void OnMouseEnter() {
        GameManager.instance.Hover(true, this);
        check_click = StartCoroutine(CheckClick());
        check_zoom_click = StartCoroutine(CheckZoomClick());
    }
    void OnMouseExit() {
        if (check_click != null) {
            StopCoroutine(check_click);
            check_click = null;
        }
        StopCoroutine(check_zoom_click);
        GameManager.instance.Hover(false, this);
        GameManager.instance.ClearZoom();
    }
    IEnumerator CheckClick() {
        while (true) {
            while (!Input.GetMouseButtonDown(0)) {
                yield return null;
            }
            while (!Input.GetMouseButtonUp(0)) {
                yield return null;
            }
            if (GameManager.instance.active_player == GameManager.instance.player1) {
                ContextClickHandler();
            }
        }
    }
    IEnumerator CheckZoomClick() {
        while (true) {
            while (!Input.GetButton("Zoom")) {
                yield return null;
            }
            GameManager.instance.Zoom(this);
            while (!Input.GetButtonUp("Zoom")) {
                yield return null;
            }
            GameManager.instance.ClearZoom();        
        }
    }

    public virtual bool CanBeTargeted(ITargets targeter) {
        return true;
    }

    public void UpdateDisplay() {
        card_field_display.Display(this);
    }

    public void Highlight(bool is_highlighted) {
        card_face_display.highlight_image.enabled = is_highlighted;
        card_field_display.highlight_image.enabled = is_highlighted;
    }

    public int DealDamage(IDamageable com, int damage) {
        int dealt = com.TakeDamage(this, damage);
        if (triggers.keywords.lifelink) {
            controller.Heal(dealt);
        }
        return dealt;
    }

    public virtual void EOTCleanup() {
        effects.RemoveDuration(EffectDuration.eot);
    }
    public virtual void BeginTurn() {
    }

}