using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    float TakeDamage(float dmg);
    bool immune {
        get;
    }
}

public interface IBuffable {
    void ApplyBuff();
    void RemoveBuff();
}
