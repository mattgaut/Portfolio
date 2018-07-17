using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour {

    [SerializeField] GridLayoutGroup items_grid;
    [SerializeField] int items_per_row;
    [SerializeField] ItemDisplay item_display_prefab;

    private void Start() {
        float width = items_grid.GetComponent<RectTransform>().rect.width;
        width -= items_grid.padding.horizontal + (items_grid.spacing.x * (items_per_row - 1));
        width = width / items_per_row;
        items_grid.cellSize = new Vector2(width, width);
    }

    public void AddItem(Item i) {
        ItemDisplay new_item_display = Instantiate(item_display_prefab, items_grid.transform);
        new_item_display.Display(i);
    }
}
