using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class L_DestroyAll : TriggeredAbility {

    protected List<SpellPermanent> to_destroy;
    [SerializeField]
    List<MajorCardType> destroy_types;
    [SerializeField]
    bool yours, their;

    public override IEnumerator OnResolution() {
        to_destroy = new List<SpellPermanent>();
        if (yours) {
            to_destroy.AddRange(GameManager.instance.GetCardsInZone(Zone.field, source.controller).OfType<SpellPermanent>().Where((SpellPermanent sp) => destroy_types.Contains(sp.major_card_type)));
        }
        if (their) {
            to_destroy.AddRange(GameManager.instance.GetCardsInZone(Zone.field, GameManager.instance.OtherPlayer(source.controller)).OfType<SpellPermanent>().Where((SpellPermanent sp) => destroy_types.Contains(sp.major_card_type)));
        }
        foreach (SpellPermanent sp in to_destroy) {
            yield return GameManager.instance.DestroyPermanent(sp);
        }
    }
}
