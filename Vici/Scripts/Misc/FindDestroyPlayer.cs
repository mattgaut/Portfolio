using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindDestroyPlayer : MonoBehaviour {

    public void FindDestroy() {
        Destroy(FindObjectOfType<PlayerCharacter>().gameObject);
    }
}
