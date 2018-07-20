using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utility {
    public static bool EdgeIntersects(Vector2 a_1, Vector2 a_2, Vector2 b_1, Vector2 b_2) {
        return Geometry.Edge.Intersects(new Geometry.Edge(a_1, a_2), new Geometry.Edge(b_1, b_2));
    }

    public static float Angle(Vector2 vector, Vector2 origin = default(Vector2)) {
        if (origin == default(Vector2)) {
            origin = Vector2.zero;
        }
        Vector2 relative_neighbor = vector - origin;
        float angle = Vector2.Angle(Vector2.up, (vector - origin));
        if (relative_neighbor.x < 0) {
            angle = 360 - angle;
        }
        return angle;
    }
    public static float AngleBetween(Vector2 vector_one, Vector2 vector_two, Vector2 origin = default(Vector2)) {
        if (origin == default(Vector2)) {
            origin = Vector2.zero;
        }
        float angle = Angle(vector_two, origin) - Angle(vector_one, origin);
        if (angle < 0) {
            angle += 360;
        }
        return angle;
    }
    public static Vector3 Midpoint(Vector3 a , Vector3 b) {
        return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z)/2;
    }
    public static Vector2 Midpoint(Vector2 a, Vector2 b) {
        return new Vector2(a.x + b.x, a.y + b.y) / 2;
    }
    public enum Axis { x, y, z };
    public static Vector2 Vector3ToVector2(Vector3 v, Axis to_remove) {
        Vector2 vec;
        if (to_remove == Axis.x) {
            vec = new Vector2(v.y, v.z);
        } else if (to_remove == Axis.y) {
            vec = new Vector2(v.x, v.z);
        } else {
            vec = new Vector2(v.x, v.y);
        }
        return vec;
    }
    public static Vector2[] Vector3ToVector2(Vector3[] v, Axis to_remove) {
        Vector2[] vecs = new Vector2[v.Length];
        for (int i = 0; i < v.Length; i++) {
            vecs[i] = Vector3ToVector2(v[i], to_remove);
        }
        return vecs;
    }
    public static Vector3 Vector2ToVector3(Vector2 v, Axis to_add, float added_value = 0f) {
        Vector3 vec = Vector3.zero;
        if (to_add == Axis.x) {
            vec = new Vector3(added_value, v.x, v.y);
        } else if (to_add == Axis.y) {
            vec = new Vector3(v.x, added_value, v.y);
        } else if (to_add == Axis.z) {
            vec = new Vector3(v.x, v.y, added_value);
        }
        return vec;
    }
    public static Vector3[] Vector2ToVector3(Vector2[] v, Axis to_add, float added_value = 0f) {
        Vector3[] vec = new Vector3[v.Length];
        for (int i = 0; i < v.Length; i++) {
            if (to_add == Axis.x) {
                vec[i] = new Vector3(added_value, v[i].x, v[i].y);
            } else if (to_add == Axis.y) {
                vec[i] = new Vector3(v[i].x, added_value, v[i].y);
            } else {
                vec[i] = new Vector3(v[i].x, v[i].y, added_value);
            }
        }
        return vec;
    }

    public static int SortVector2ByY(Vector2 a, Vector2 b) {
        if (a.y == b.y) {
            return (RoundAwayFromZero(a.x - b.x));
        } else {
            return (RoundAwayFromZero(a.y - b.y));
        }
    }
    public static int SortVector2ByX(Vector2 a, Vector2 b) {
        if (a.x == b.x) {
            return (RoundAwayFromZero(a.y - b.y));
        } else {
            return (RoundAwayFromZero(a.x - b.x));
        }
    }

    public static int SortVector2(Vector2 a, Vector2 b, Axis first) {
        if (first == Axis.z) {
            return 0;
        }
        if (first == Axis.x) {
            if (a.x == b.x) {
                return (RoundAwayFromZero(a.y - b.y));
            } else {
                return (RoundAwayFromZero(a.x - b.x));
            }
        } else {
            if (a.y == b.y) {
                return (RoundAwayFromZero(a.x - b.x));
            } else {
                return (RoundAwayFromZero(a.y - b.y));
            }
        }
    }

    public static int RoundAwayFromZero(float a) {
        if (a < 1f && a > -1f) {
            return (int)(1 * Mathf.Sign(a));
        } else {
            return Mathf.RoundToInt(a);
        }
    }

    public class RenderEdge {
        Vector3 A;
        Vector3 B;
        GameObject render_local;
        GameObject loaded_render;
        public static GameObject render {
            private get;
            set;
        }
        public RenderEdge(Vector3 A, Vector3 B, GameObject render = null) {
            this.A = A;
            this.B = B;

            render_local = render;

            CreateRender();
        }
        public void ClearRender() {
            if (loaded_render) {
                GameObject.Destroy(loaded_render);
            }
        }

        public void CreateRender() {
            ClearRender();
            GameObject new_object;
            if (render_local) {
                new_object = GameObject.Instantiate(render_local);
            } else {
                new_object = GameObject.Instantiate(render);
            }

            loaded_render = new_object;

            LineRenderer l_r = new_object.GetComponent<LineRenderer>();
            l_r.positionCount = 2;
            l_r.SetPosition(0, A);
            l_r.SetPosition(1, B);
        }

        public static bool operator ==(RenderEdge rhs, RenderEdge lhs) {
            if (ReferenceEquals(rhs, null) || ReferenceEquals(lhs, null)) {
                return ReferenceEquals(rhs, lhs);
            }
            return (rhs.A == lhs.A) && (rhs.B == lhs.B); 
        }
        public static bool operator !=(RenderEdge rhs, RenderEdge lhs) {
            return !(rhs == lhs);
        }
    }

}

public class HashList<T> {
    List<T> list;
    HashSet<T> hash;

    public HashList(){
        list = new List<T>();
        hash = new HashSet<T>();
    }

    public bool Add(T to_add) {
        if (!hash.Contains(to_add)) {
            list.Add(to_add);
            hash.Add(to_add);
            return true;
        }
        return false;
    }
    public bool Remove(T to_remove) {
        if (hash.Contains(to_remove)) {
            list.Remove(to_remove);
            hash.Remove(to_remove);
            return true;
        }
        return false;
    }

    public void Clear() {
        hash.Clear();
        list.Clear();
    }

    public T this[int key]{
        get {
            return list[key];
        }
    }

    public int Count {
        get { return list.Count; }
    }
}
