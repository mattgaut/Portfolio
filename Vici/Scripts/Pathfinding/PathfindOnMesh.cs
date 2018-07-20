using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathfindOnMesh : PathfinderAgent {

    Pathfinder mesh;
    float agent_mesh_size;
    bool on_mesh;

    Vector3 last_direction;

    void Awake() {
        if (target == null) {
            target = FindObjectOfType<PlayerCharacter>().gameObject;
        }
    }

    public override void Analyze() {
        if (!on_mesh) {
            Vector2 pos = Utility.Vector3ToVector2(agent.transform.position, Utility.Axis.z);
            on_mesh = mesh.OnMesh(pos);
        } else {
            List<Vector3> new_path = new List<Vector3>(Utility.Vector2ToVector3(mesh.FindPath(agent.transform.position, target.transform.position, out on_mesh).ToArray(), Utility.Axis.z));
            if (on_mesh) {
                path = new_path;
            }
        }
    }

    public override void Initiate() {
        path = new List<Vector3>();
        mesh = agent.room.pathfinding_mesh;
        on_mesh = true;
    }

    public override Vector3 move_towards {
        get {
            if (path.Count > 0) {
                Vector3 pos = agent.transform.position;
                pos.y = 0;
                if (Vector3.Distance(pos, path[0]) < 0.05f) {
                    if (path.Count > 1) {
                        path.RemoveAt(0);
                    } else {
                        return path[0];
                    }
                }
                return path[0];
            } else {
                return agent.transform.position;
            }
        }
    }

    public override void SetTarget(GameObject target) {
        this.target = target;
    }

    public void SetMesh(PathfindingMesh mesh) {
        this.mesh = mesh.pathfinder;
    }
}