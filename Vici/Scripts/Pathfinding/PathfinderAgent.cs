using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PathfinderAgent : MonoBehaviour {
    [SerializeField]
    protected Enemy agent;
    [SerializeField]
    protected GameObject target;

    protected List<Vector3> path;

    public virtual Vector3 move_towards {
        get {
            if (path.Count > 0)
                return path[0];
            else
                return agent.transform.position;
        }
    }

    public abstract void SetTarget(GameObject target);

    public abstract void Initiate();

    public abstract void Analyze();
}
