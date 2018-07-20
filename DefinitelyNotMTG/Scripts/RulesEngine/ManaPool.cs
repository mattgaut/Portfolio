using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPool {

    Dictionary<ManaType, int> current;
    Dictionary<ManaType, int> max;

    DisplayManaPool display;

    public ManaPool(DisplayManaPool display = null) {
        current = new Dictionary<ManaType, int>();
        max = new Dictionary<ManaType, int>();

        foreach (ManaType t in System.Enum.GetValues(typeof(ManaType))) {
            current[t] = 0;
            max[t] = 0;
        }
        this.display = display;
        Display();

    }

    public void SetCurrent(ManaType type, int count) {
        current[type] = count;
        Display();
    }
    public void SetMax(ManaType type, int count) {
        max[type] = count;
        Display();
    }

    public void AddCurrent(ManaType type, int count) {
        current[type] += count;
        Display();
    }
    public void AddCurrent(ManaPool pool, bool overflow = false) {
        foreach (ManaType t in System.Enum.GetValues(typeof(ManaType))) {
            current[t] += pool.current[t];
            if (!overflow && current[t] > max[t]) {
                current[t] = max[t];
            }
        }
        Display();
    }
    public void AddMax(ManaType type, int count, bool also_add_current = false) {
        max[type] += count;

        if (also_add_current) {
            AddCurrent(type, count);
        }
        Display();
    }

    public int SubtractCurrent(ManaType type, int count) {
        int subtracted = current[type];
        current[type] -= count;
        if (current[type] < 0) {
            current[type] = 0;
        }
        Display();
        return subtracted - current[type];
    }
    public void SubtractCurrent(ManaPool pool, int count) {
        foreach (ManaType t in System.Enum.GetValues(typeof(ManaType))) {
            current[t] -= pool.current[t];
            if (current[t] < 0) {
                current[t] = 0;
            }
        }
    }
    public void SubtractMax(ManaType type, int count, bool also_sub_current = false) {
        max[type] -= count;

        if (also_sub_current) {
            SubtractCurrent(type, count);
        }
        Display();
    }

    public void Fill() {
        foreach (ManaType t in System.Enum.GetValues(typeof(ManaType))) {
            current[t] = max[t];
        }
        Display();
    }

    public void Display() {
        if (display != null) {
            display.Display(this);
        }
    }

    public int GetCurrent(ManaType type) {
        return current[type];
    }
    public int GetMax(ManaType type) {
        return max[type];
    }
}
