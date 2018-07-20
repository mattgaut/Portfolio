using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Geometry {
    public class Edge : IEqualityComparer<Edge> {
        public Vector2 upper {
            get;
            protected set;
        }
        public Vector2 lower {
            get;
            protected set;
        }

        public float slope {
            get { return (upper.y - lower.y) / (upper.x / lower.x); }
        }

        public Edge(Vector2 a, Vector2 b) {
            if (PointHeightCompare(a, b) > 0) {
                upper = a;
                lower = b;
            } else {
                upper = b;
                lower = a;
            }
        }

        public bool Intersects(Edge a) {
            return Intersects(this, a);
        }

        public bool HasEndPoint(Vector2 e) {
            return e == upper || e == lower;
        }

        public static bool Intersects(Edge a, Edge b) {
            if (a.upper == b.upper || a.upper == b.lower || a.lower == b.upper || a.lower == b.lower) {
                return false;
            }

            Line line_a = new Line(a.upper, a.lower);
            Line line_b = new Line(b.upper, b.lower);
            if (line_a.slope == line_b.slope) {
                if (line_a.b == line_b.b) {
                    return !(a.upper.y < b.lower.y || b.upper.y < a.lower.y);
                }
                return false;
            } else if (line_a.slope == 0) {
                Vector2 intersection = line_a.IntersectsAtPoint(line_b);
                return (intersection.x < a.upper.x && intersection.x > a.lower.x && line_a.b > b.lower.y && line_a.b < b.upper.y);
            } else if (line_b.slope == 0) {
                Vector2 intersection = line_a.IntersectsAtPoint(line_b);
                return (intersection.x < b.upper.x && intersection.x > b.lower.x && line_b.b > a.lower.y && line_b.b < a.upper.y);
            }
            Vector2 intersection_point = line_a.IntersectsAtPoint(line_b);

            float a_left, b_left, a_right, b_right, a_top = a.upper.y, b_top = b.upper.y, a_bottom = a.lower.y, b_bottom = b.lower.y;
            a_left = a.upper.x < a.lower.x ? a.upper.x : a.lower.x;
            a_right = a.upper.x > a.lower.x ? a.upper.x : a.lower.x;
            b_left = b.upper.x < b.lower.x ? b.upper.x : b.lower.x;
            b_right = b.upper.x > b.lower.x ? b.upper.x : b.lower.x;

            if (intersection_point.y <= a_top && intersection_point.y >= a_bottom
                && intersection_point.x <= a_right && intersection_point.x >= a_left) {
                return (intersection_point.y <= b_top && intersection_point.y >= b_bottom
                && intersection_point.x <= b_right && intersection_point.x >= b_left);
            } else {
                return false;
            }
        }

        public Vector2 ClosestPointOnEdge(Vector2 point) {
            Vector2 close = new Line(this).ClosestPointOnLine(point);
            if (upper.y == lower.y) {
                if (close.x > upper.x) {
                    return upper;
                } else if (close.x < lower.x) {
                    return lower;
                }
            } else if (close.y > upper.y) {
                return upper;
            } else if (close.y < lower.y) {
                return lower;
            }
            return close;
        }
        public int AboveOrBelowEdge(Vector2 point) {
            return new Line(this).AboveOrBelowLine(point);
        }

        public Vector2 Midpoint() {
            return (upper + lower) / 2;
        }

        public bool Equals(Edge x, Edge y) {
            return x.upper == y.upper && x.lower == y.lower;
        }
        public int GetHashCode(Edge obj) {
            return (int)(obj.upper.x + obj.lower.y);
        }
        public override int GetHashCode() {
            return GetHashCode(this);
        }
        public override bool Equals(object obj) {
            Edge other = obj as Edge;
            return other != null && Equals(other, this);
        }
        public override string ToString() {
            return "[" + upper + " -> " + lower + "]";
        }
    }

    public class Line {
        public float b {
            get;
            protected set;
        }
        public float slope {
            get;
            protected set;
        }
        public float perpindicular_slope {
            get { return PerpindicularSlope(slope); }
        }
        public Vector2 upward_ray {
            get {
                if (float.IsInfinity(slope)) {
                    return Vector2.up;
                } else if (slope == 0) {
                    return Vector2.right;
                } else  {
                    Vector2 v = new Vector2(1, this[1]) - new Vector2(0, this[0]);
                    if (slope > 0) {
                        return v.normalized;
                    } else {
                        return -v.normalized;
                    }
                }
            }
        }
        public Vector2 downward_ray {
            get {
                return -upward_ray;
            }
        }

        public Line(float _b, float _slope) {
            b = _b;
            slope = _slope;
        }
        public Line(Edge e) {
            Vector2 point_a = e.upper, point_b = e.lower;
            if (point_a.x == point_b.x) {
                slope = float.PositiveInfinity;
                b = point_a.x;
            } else {
                slope = (point_b.y - point_a.y) / (point_b.x - point_a.x);
                b = point_a.y - (slope * point_a.x);
            }
        }
        public Line(Vector2 point_a, Vector2 point_b) {
            if (point_a.x == point_b.x) {
                slope = float.PositiveInfinity;
                b = point_a.x;
            } else {
                slope = (point_b.y - point_a.y) / (point_b.x - point_a.x);
                b = point_a.y - (slope * point_a.x);
            }
        }
        public Line(float _slope, Vector2 point) {
            slope = _slope;
            b = point.y - (slope * point.x);
        }

        float this[float x] {
            get {
                return float.IsInfinity(slope) ? b : (x * slope) + b;
            }
        }

        public float GetY(float x) {
            return float.IsInfinity(slope) ? b : this[x];
        }

        public float GetX(float y) {
            return float.IsInfinity(slope) ? b : (y - b) / slope;
        }

        public Line PerpindicularAtX(float x) {
            return PerpindicularAt(new Vector2(x, this[x]));
        }
        public Line PerpindicularAtY(float y) {
            return PerpindicularAt(new Vector2(GetX(y), y));
        }

        public Line PerpindicularAt(Vector2 point) {
            if (float.IsInfinity(slope)) {
                return new Line(0, point);
            } else if (slope == 0) {
                return new Line(point.x, float.PositiveInfinity);
            }
            return new Line(-(1 / slope), point);
        }
        public static float PerpindicularSlope(float s) {
            if (s == 0) {
                return float.PositiveInfinity;
            }
            if (float.IsInfinity(s)) {
                return 0;
            }
            s = 1 / s;
            s = -s;
            return s;
        }

        public Line Translated(Vector2 v) {
            if (float.IsInfinity(slope)) {
                return new Line(b + v.x, slope);
            } else if (slope == 0) {
                return new Line(b + v.y, slope);
            } else
                return new Line(slope, new Vector2(0, b) + v);
        }

        bool PointOnLine(Vector2 point) {
            return float.IsInfinity(slope) ? b == point.x : ((point.x * slope + b) == point.y);
        }
        public int AboveOrBelowLine(Vector2 point) {
            if (float.IsInfinity(slope)) {
                return Math.Sign(point.x - b);
            } else {
                return Math.Sign(point.y - GetY(point.x));
            }
        }
        public Vector2 ClosestPointOnLine(Vector2 point) {
            Geometry.Line perp_line = new Geometry.Line(perpindicular_slope, point);

            return IntersectsAtPoint(perp_line);
        }

        bool IsParallel(Line line) {
            return slope == line.slope;
        }

        public virtual Vector2 IntersectsAtPoint(Line line) {
            if (line.IsParallel(this)) {
                return new Vector2(0, b);
            } else if (float.IsInfinity(slope)) {
                return new Vector2(b, line[b]);
            } else if (float.IsInfinity(line.slope)) {
                return new Vector2(line.b, this[line.b]);
            } else {
                //line. slope * x + line.b = slope * x + b
                float x = (b - line.b) / (line.slope - slope);
                float y = x * slope + b;
                return new Vector2(x, y);
            }
        }
        public static bool operator ==(Line lhs, Line rhs) {
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) {
                return false;
            }
            if (ReferenceEquals(lhs, rhs)) {
                return true;
            }

            return lhs.slope == rhs.slope && Mathf.Approximately(lhs.b, rhs.b);
        }

        public static bool operator !=(Line lhs, Line rhs) {
            return !(lhs == rhs);
        }

        public override int GetHashCode() {
            return Mathf.RoundToInt(slope) * Mathf.RoundToInt(b);
        }

        public override bool Equals(object obj) {
            Line l = obj as Line;
            return l != null && l == this;
        }
    }

    public class Triangle : IEqualityComparer<Triangle> {

        public Vector2 a { get; private set; }
        public Vector2 b { get; private set; }
        public Vector2 c { get; private set; }
        public Triangle ab { get; private set; }
        public Triangle ac { get; private set; }
        public Triangle bc { get; private set; }

        public Vector2 center { get; private set; }

        public List<Vector2> nodes {
            get {
                List<Vector2> list = new List<Vector2>();
                list.Add(a);
                list.Add(b);
                list.Add(c);
                return list;
            }
        }

        public List<Triangle> neighbors {
            get {
                List<Triangle> list = new List<Triangle>();
                if (!ReferenceEquals(ab, null)) {
                    list.Add(ab);
                }
                if (!ReferenceEquals(bc, null)) {
                    list.Add(bc);
                }
                if (!ReferenceEquals(ac, null)) {
                    list.Add(ac);
                }
                return list;
            }
        }

        public Triangle(Vector2 point_a, Vector2 point_b, Vector2 point_c) {
            a = point_a;
            b = point_b;
            c = point_c;

            SetCenter();

            if (!ContainsPoint(center)) {
                throw new System.Exception(center + " : not in triangle : " + a + " : " + b + " : " + c);
            }
        }

        void SetCenter() {
            Line line_a = new Line(a, Utility.Midpoint(b, c));
            Line line_b = new Line(b, Utility.Midpoint(a, c));

            center = line_a.IntersectsAtPoint(line_b);

            if (!ContainsPoint(center)) {
                line_a = new Line(c, Utility.Midpoint(b, a));
                center = line_a.IntersectsAtPoint(line_b);
            } else {
                return;
            }
            if (!ContainsPoint(center)) {
                line_b = new Line(a, Utility.Midpoint(c, b));
                center = line_a.IntersectsAtPoint(line_b);
            } else {
                return;
            }
            if (!ContainsPoint(center)) {
                throw new System.Exception("Cant find center in triangle : " + a + " : " + b + " : " + c);
            }

        }

        public bool HasEndPoint(Vector2 p) {
            return p == a || p == b || p == c;
        }
        public bool HasNeighbor(Triangle t) {
            return t == ab || t == ac || t == bc;
        }
        public bool ContainsPoint(Vector2 p) {
            return PointBetweenRays(p, a) && PointBetweenRays(p, b) && PointBetweenRays(p, c);
        }
        public bool IntersectsEdge(Vector2 a, Vector2 b) {
            return ab.IntersectsEdge(a, b) || ac.IntersectsEdge(a, b) || bc.IntersectsEdge(a, b);
        }

        public bool PointBetweenRays(Vector2 point, Vector2 origin) {
            if (origin == a) {
                float angle = Utility.AngleBetween(b, c, a);
                if (angle > 180) {
                    angle = Utility.AngleBetween(c, b, a);
                    if (Utility.AngleBetween(c, point, a) <= angle) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    if (Utility.AngleBetween(b, point, a) <= angle) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } else if (origin == b) {
                float angle = Utility.AngleBetween(a, c, b);
                if (angle > 180) {
                    angle = Utility.AngleBetween(c, a, b);
                    if (Utility.AngleBetween(c, point, b) <= angle) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    if (Utility.AngleBetween(a, point, b) <= angle) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } else if (origin == c) {
                float angle = Utility.AngleBetween(a, b, c);
                if (angle > 180) {
                    angle = Utility.AngleBetween(b, a, c);
                    if (Utility.AngleBetween(b, point, c) <= angle) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    if (Utility.AngleBetween(a, point, c) <= angle) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } else {
                return false;
            }
        }
        public static bool HasSharedEdge(Triangle a, Triangle b) {
            return SharedEdge(a, b) != null;
        }
        public static bool HasSharedEdge(Triangle a, Triangle b, out Edge e) {
            e = SharedEdge(a, b);
            return e != null;
        }
        public static Edge SharedEdge(Triangle a, Triangle b) {
            if (a.ab == b) {
                return new Edge(a.a, a.b);
            }
            if (a.bc == b) {
                return new Edge(a.b, a.c);
            }
            if (a.ac == b) {
                return new Edge(a.a, a.c);
            }
            return null;
        }

        public static bool HasSharedEndPoint(Triangle a, Triangle b) {
            return SharedEndPoint(a, b) != default(Vector2);
        }
        public static bool HasSharedEndPoint(Triangle a, Triangle b, out Vector2 v) {
            v = SharedEndPoint(a, b);
            return v != default(Vector2);
        }
        public static Vector2 SharedEndPoint(Triangle a, Triangle b) {
            if (a.a == b.a || a.a == b.b || a.a == b.c) {
                return a.a;
            }
            if (a.b == b.a || a.b == b.b || a.b == b.c) {
                return a.b;
            }
            if (a.c == b.a || a.c == b.c || a.c == b.c) {
                return a.c;
            }
            return default(Vector2);
        }

        public static bool HasSharedEndPoint(Triangle a, Triangle b, Triangle c) {
            return SharedEndPoint(a, b, c) != default(Vector2);
        }
        public static bool HasSharedEndPoint(Triangle a, Triangle b, Triangle c, out Vector2 v) {
            v = SharedEndPoint(a, b, c);
            return v != default(Vector2);
        }
        public static Vector2 SharedEndPoint(Triangle a, Triangle b, Triangle c) {
            if (a.a == b.a || a.a == b.b || a.a == b.c) {
                if (a.a == c.a || a.a == c.b || a.a == c.c) {
                    return a.a;
                }
            }
            if (a.b == b.a || a.b == b.b || a.b == b.c) {
                if (a.b == c.a || a.b == c.b || a.b == c.c) {
                    return a.b;
                }
            }
            if (a.c == b.a || a.c == b.c || a.c == b.c) {
                if (a.c == c.a || a.c == c.b || a.c == c.c) {
                    return a.c;
                }
            }
            return default(Vector2);
        }

        public void AddNeighbor(Triangle t) {
            if (t.HasEndPoint(a) && t.HasEndPoint(b) && t.HasEndPoint(c)) {
                return;
            }
            if (t.HasEndPoint(a)) {
                if (t.HasEndPoint(b)) {
                    ab = t;
                } else if (t.HasEndPoint(c)) {
                    ac = t;
                } else {
                    return;
                }
            } else if (t.HasEndPoint(b)) {
                if (t.HasEndPoint(c)) {
                    bc = t;
                } else {
                    return;
                }
            } else {
                return;
            }
            if (!t.HasNeighbor(this))
                t.AddNeighbor(this);
        }

        public static bool operator ==(Triangle lhs, Triangle rhs) {
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) {
                return false;
            }
            if (ReferenceEquals(lhs, rhs)) {
                return true;
            }

            return lhs.HasEndPoint(rhs.a) && lhs.HasEndPoint(rhs.b) && lhs.HasEndPoint(rhs.c);
        }

        public static bool operator !=(Triangle lhs, Triangle rhs) {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj) {
            if (typeof(Triangle) != obj.GetType()) {
                return false;
            } else {
                Triangle t = (Triangle)obj;
                return t == this;
            }
        }

        public override int GetHashCode() {
            return a.GetHashCode() + b.GetHashCode() + c.GetHashCode();
        }

        public bool Equals(Triangle x, Triangle y) {
            return x == y;
        }

        public int GetHashCode(Triangle obj) {
            return obj.GetHashCode();
        }

        public override string ToString() {
            return "A: " + a + "  B: " + b + "  C: " + c + " Center: " + center;
        }
    }

    public class PointComparer : IComparer<Vector2> {

        bool descending;

        public PointComparer() {
            descending = false;
        }
        public PointComparer(bool descending) {
            this.descending = descending;
        }
        public int Compare(Vector2 x, Vector2 y) {
            return descending ? -PointHeightCompare(x, y) : PointHeightCompare(x, y);
        }
    }

    public static Vector2 Midpoint(Vector2 a, Vector2 b) {
        return (a + b) / 2;
    }

    public static int PointHeightCompare(Vector2 a, Vector2 b) {
        if (a.y == b.y) {
            if (a.x == b.x) {
                return 0;
            } else {
                return Utility.RoundAwayFromZero(a.x - b.x);
            }
        } else {
            return Utility.RoundAwayFromZero(a.y - b.y);
        }
    }

}
