using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DCEL {

    public class DCELEdge : IEqualityComparer<DCELEdge> {
        public DCELEdge(HalfEdge h) {
            from = h.from.location;
            to = h.next.from.location;
        }
        public DCELEdge(Vector2 f, Vector2 t) {
            from = f;
            to = t;
        }

        public Vector2 from;
        public Vector2 to;

        public bool Equals(DCELEdge x, DCELEdge y) {

            return x.from == y.from && x.to == y.to;
        }

        public DCELEdge reverse {
            get { return new DCELEdge(to, from); }
        }

        public bool flat {
            get { return from.y == to.y; }
        }

        public int GetHashCode(DCELEdge obj) {
            return (int)(from.x + to.x);
        }
        public override int GetHashCode() {
            return GetHashCode(this);
        }
        public override bool Equals(object obj) {
            DCELEdge other = obj as DCELEdge;
            return other != null && Equals(other, this);
        }
    }

    public class DCELPoint {
        public DCELPoint(Vector2 location, List<Vector2> n = null) {
            this.location = location;
            neighbors = new List<Vector2>();

            if (n != null) {
                foreach (Vector2 v in n) {
                    neighbors.Add(v);
                }
            }
        }

        public Vector2 location;
        public List<Vector2> neighbors;

        public void AddNeighbor(Vector2 v) {
            if (neighbors.Count == 0) {
                neighbors.Add(v);
            } else {
                neighbors.Add(v);
                neighbors.Sort((Vector2 a, Vector2 b) => CompareNeighbors(a, b));
            }
        }

        public int CompareNeighbors(Vector2 neighbor_a, Vector2 neighbor_b) {
            if (neighbor_a == neighbor_b) {
                return 0;
            }
            return Utility.RoundAwayFromZero(NeighborAngle(neighbor_a) - NeighborAngle(neighbor_b));
        }
        public float NeighborAngle(Vector2 neighbor) {
            return Utility.AngleBetween(Vector2.up, (neighbor - location)); ;
        }

        public int NextNeighbor(Vector2 neighbor) {
            for (int i = 0; i < neighbors.Count; i++) {
                if (neighbors[i] == neighbor) {
                    return i - 1 < 0 ? neighbors.Count - 1 : i - 1;
                }
            }
            return -1;
        }
        public int PrevNeighbor(Vector2 neighbor) {
            for (int i = 0; i < neighbors.Count; i++) {
                if (neighbors[i] == neighbor) {
                    return i + 1 > neighbors.Count - 1 ? 0 : i + 1;
                }
            }
            return -1;
        }
    }

    public DCEL(List<DCELEdge> edges) {
        vertexes = new Dictionary<Vector2, Vertex>();
        faces = new List<Face>();
        half_edges = new List<HalfEdge>();
        this.edges = new List<Edge>();

        List<DCELPoint> nodes = new List<DCELPoint>();

        Dictionary<Vector2, List<Vector2>> points = new Dictionary<Vector2, List<Vector2>>();

        foreach (DCELEdge e in edges) {
            if (!points.ContainsKey(e.from)) {
                points[e.from] = new List<Vector2>();
            }
            points[e.from].Add(e.to);

            if (!points.ContainsKey(e.to)) {
                points[e.to] = new List<Vector2>();
            }
            points[e.to].Add(e.from);
        }

        foreach (KeyValuePair<Vector2, List<Vector2>> kvp in points) {
            DCELPoint p = new DCELPoint(kvp.Key);
            p.neighbors = kvp.Value;

            nodes.Add(p);
        }

        AnalyzePoints(nodes);
    }

    List<Edge> edges;
    public Dictionary<Vector2, Vertex> vertexes;
    public List<Face> faces;
    public List<HalfEdge> half_edges;

    public DCEL(List<DCELPoint> nodes) {
        vertexes = new Dictionary<Vector2, Vertex>();
        faces = new List<Face>();
        half_edges = new List<HalfEdge>();

        List<DCELPoint> points = new List<DCELPoint>();

        AnalyzePoints(nodes);
    }

    void AnalyzePoints(List<DCELPoint> nodes) {
        Dictionary<Vector2, DCELPoint> points = new Dictionary<Vector2, DCELPoint>();
        Dictionary<DCELEdge, HalfEdge> edges = new Dictionary<DCELEdge, HalfEdge>(new DCELEdge(Vector2.zero, Vector2.zero));

        List<DCELPoint> dcel_points = new List<DCELPoint>();

        foreach (DCELPoint p in nodes) {
            points.Add(p.location, p);
            //vertexes.Add(p.location, new Vertex(p.location));
        }

        for (int i = 0; i < nodes.Count; i++) {
            if (nodes[i].neighbors.Count == 2) {
                if (new Geometry.Line(nodes[i].neighbors[0], nodes[i].location) == new Geometry.Line(nodes[i].neighbors[1], nodes[i].location)) {
                    points[nodes[i].neighbors[0]].AddNeighbor(nodes[i].neighbors[1]);
                    points[nodes[i].neighbors[1]].AddNeighbor(nodes[i].neighbors[0]);
                    points[nodes[i].neighbors[0]].neighbors.Remove(nodes[i].location);
                    points[nodes[i].neighbors[1]].neighbors.Remove(nodes[i].location);
                    nodes.RemoveAt(i);
                    i--;
                }
            }
        }

        foreach (DCELPoint p in nodes) {
            vertexes.Add(p.location, new Vertex(p.location));
        }

        foreach (DCELPoint p in nodes) {
            p.neighbors.Sort((Vector2 a, Vector2 b) => p.CompareNeighbors(a, b));
            //p.neighbors.Reverse();
            for (int i = 0; i < p.neighbors.Count; i++) {

                DCELEdge first = ((new DCELEdge(p.neighbors[i], p.location)));
                if (!edges.ContainsKey(first)) {
                    edges.Add(first, new HalfEdge(vertexes[first.from]));
                }

                DCELEdge second = ((new DCELEdge(p.location, p.neighbors[(i + 1) % p.neighbors.Count])));
                if (!edges.ContainsKey(second)) {
                    edges.Add(second, new HalfEdge(vertexes[second.from]));
                }
                edges[first].SetNext(edges[second]);

                //Debug.Log("Made Edge: "  + p.neighbors[i] + " : " + p.location + " : " + p.neighbors[(i + 1) % p.neighbors.Count]);
            }
        }

        foreach (HalfEdge he in edges.Values) {
            half_edges.Add(he);
        }

        foreach (HalfEdge he in half_edges) {
            if (he.from.incident_edge == null) {
                he.from.incident_edge = he;
            }
        }

        foreach (DCELEdge e in edges.Keys) {
            if (edges.ContainsKey(e.reverse)) {
                if (edges[e].twin == null) {
                    edges[e].SetTwin(edges[e.reverse]);
                    this.edges.Add(new Edge(edges[e]));
                }
            }
        }

        FindFaces();
    }

    void FindFaces() {
        //Find cycles and leftmost in cycle

        //True if Hole False if Boundary
        Dictionary<HalfEdge, bool> is_outer_cycle = new Dictionary<HalfEdge, bool>();
        List<HalfEdge> leftmost_edges = new List<HalfEdge>();
        HashList<HalfEdge> edges_left = new HashList<HalfEdge>();
        foreach (HalfEdge he in half_edges) {
            edges_left.Add(he);
        }

        while (edges_left.Count > 0) {
            HalfEdge first_edge = edges_left[0];
            HalfEdge current = first_edge;
            HalfEdge leftmost = first_edge;
            do {
                edges_left.Remove(current);
                if (current.from.location.x < leftmost.from.location.x) {
                    leftmost = current;
                } else if (current.from.location.x == leftmost.from.location.x) {
                    if (leftmost.from.location.y > current.from.location.y) {
                    } else {
                        leftmost = current;
                    }
                }
                current = current.next;
            } while (current != first_edge);

            leftmost_edges.Add(leftmost);
        }

        foreach (HalfEdge he in leftmost_edges) {
            if (he.Angle() > 180) {
                is_outer_cycle.Add(he, true);
            } else {
                is_outer_cycle.Add(he, false);
            }
        }

        Dictionary<HalfEdge, CycleGroup> cycle_groups = new Dictionary<HalfEdge, CycleGroup>();

        foreach (HalfEdge he in is_outer_cycle.Keys) {
            cycle_groups.Add(he, new CycleGroup(he));
        }
        // combine cycle groups

        foreach (HalfEdge he in is_outer_cycle.Keys) {
            if (!is_outer_cycle[he]) {
                List<Edge> active_edges = new List<Edge>();
                foreach (Edge e in edges) {
                    if (e.ActiveOnY(he.from.location.y) && e.ActiveOnX(he.from.location.x)) {
                        active_edges.Add(e);
                    }
                }
                active_edges.Sort(SortEdge);

                int i = 0;
                for (; i < active_edges.Count; i++) {
                    if (active_edges[i].HasEdge(he)) {
                        break;
                    }
                }
                if (i > 0) {
                    HalfEdge hit_edge = active_edges[i - 1].downward_edge;

                    while (!cycle_groups.ContainsKey(hit_edge)) {
                        hit_edge = hit_edge.next;
                    }

                    if (hit_edge == he) {
                        continue;
                    }

                    CycleGroup cg = cycle_groups[he].CombineGroup(cycle_groups[hit_edge]);

                    cycle_groups[he] = cg;
                    cycle_groups[hit_edge] = cg;
                }
            }
        }

        List<CycleGroup> groups = new List<CycleGroup>();
        foreach (CycleGroup g in cycle_groups.Values) {
            if (!groups.Contains(g)) {
                groups.Add(g);
            }
        }

        foreach (CycleGroup g in groups) {
            Face f = new Face();
            foreach (HalfEdge he in g.edges_in_group) {
                if (is_outer_cycle[he]) {
                    f.outer_component = he;
                } else {
                    f.inner_components.Add(he);
                }
            }
            faces.Add(f);
        }

        foreach (Face f in faces) {
            HalfEdge initial_edge;
            HalfEdge current;
            if (f.outer_component != null) {
                initial_edge = f.outer_component;
                current = initial_edge;
                do {
                    current.incident_face = f;
                } while (current != initial_edge);
            }
            foreach (HalfEdge inner_component in f.inner_components) {
                initial_edge = inner_component;
                current = initial_edge;
                do {
                    current.incident_face = f;
                } while (current != initial_edge);
            }
        }
    }

    int SortEdge(Edge a, Edge b) {
        return Utility.RoundAwayFromZero(a.upper_endpoint.y - b.upper_endpoint.y);
    }

    class CycleGroup {

        public List<HalfEdge> edges_in_group;

        public CycleGroup(HalfEdge e) {
            edges_in_group = new List<HalfEdge>();
            edges_in_group.Add(e);
        }

        public CycleGroup CombineGroup(CycleGroup cg) {
            edges_in_group.AddRange(cg.edges_in_group);
            return this;
        }

    }

    public HalfEdge Next(HalfEdge e) {
        return e.next;
    }
    public HalfEdge FindEdge(Vector2 start, Vertex end) {
        HalfEdge start_edge = end.incident_edge.twin;
        if (start_edge.from.location == start) {
            return start_edge;
        }
        HalfEdge e = start_edge;
        do {
            e = e.next.twin;
        } while (e.from.location != start && e != start_edge);

        if (e == start_edge) {
            throw new Exception("Edge " + start + " -> " + end.location + "not found.");
        }
        return e;
    }
    public HalfEdge FindEdge(Vector2 start, Vector2 end) {
        if (!vertexes.ContainsKey(end)) {
            throw new Exception("Edge " + start + " -> " + end + "not found.");
        }
        Vertex v = vertexes[end];
        return FindEdge(start, v);
    }
    public HalfEdge FindEdge(Vertex start, Vector2 end) {
        if (!vertexes.ContainsKey(end)) {
            throw new Exception("Edge " + start + " -> " + end + "not found.");
        }
        Vertex v = vertexes[end];
        return FindEdge(start, v);
    }
    public HalfEdge FindEdge(Vertex start, Vertex end) {
        return FindEdge(start.location, end);
    }
    public static List<Vector2> FindPointsInCycle(HalfEdge start) {
        List<Vector2> points = new List<Vector2>();
        HalfEdge current = start;
        if (current != null) {
            do {
                points.Add(current.from.location);
                current = current.next;
            } while (current != start);
        }

        return points;
    }
    public static List<HalfEdge> FindEdgesInCycle(HalfEdge start) {
        List<HalfEdge> points = new List<HalfEdge>();
        HalfEdge current = start;
        if (current != null) {
            do {
                points.Add(current);
                current = current.next;
            } while (current != start);
        }

        return points;
    }

    public class Vertex {
        public Vertex(Vector2 v) {
            location = v;
        }
        public override string ToString() {
            return location + "";
        }

        public Vector2 location;
        public HalfEdge incident_edge;
    }
    class Edge {
        public HalfEdge downward_edge;
        public HalfEdge upward_edge;

        public Edge(HalfEdge edge) {
            HalfEdge edge_1 = edge;
            HalfEdge edge_2 = edge.twin;

            if (edge_1.from.location.y == edge_2.from.location.y) {
                if (edge_1.from.location.x > edge_2.from.location.x) {
                    downward_edge = edge_1;
                    upward_edge = edge_2;
                } else {
                    downward_edge = edge_2;
                    upward_edge = edge_1;
                }
            } else if (edge_1.from.location.y > edge_2.from.location.y) {
                downward_edge = edge_1;
                upward_edge = edge_2;
            } else {
                downward_edge = edge_2;
                upward_edge = edge_1;
            }
        }

        public bool ActiveOnY(float y) {
            return lower_endpoint.y < y && y <= upper_endpoint.y;
        }
        public bool ActiveOnX(float x) {
            return lower_endpoint.x < x || upper_endpoint.x < x;
        }

        public bool HasEdge(HalfEdge e) {
            Edge new_edge = new Edge(e);
            return new_edge.upper_endpoint == upper_endpoint && new_edge.lower_endpoint == lower_endpoint;
        }

        public Vector2 upper_endpoint {
            get { return downward_edge.from.location; }
        }
        public Vector2 lower_endpoint {
            get { return upward_edge.from.location; }
        }
    }
    public class HalfEdge {
        public HalfEdge(Vertex f) {
            from = f;
        }

        public Vertex from {
            get;
            private set;
        }
        public HalfEdge next;
        public HalfEdge prev;
        public HalfEdge twin;
        public Face incident_face;

        public void SetNext(HalfEdge n) {
            next = n;
            n.prev = this;
        }
        public void SetPrevious(HalfEdge p) {
            prev = p;
            p.next = this;
        }
        public void SetTwin(HalfEdge t) {
            twin = t;
            t.twin = this;
        }

        public float Angle() {
            Vector2 first_vector = prev.from.location - from.location;
            Vector2 second_vector = next.from.location - from.location;

            return Utility.AngleBetween(second_vector, first_vector);
        }

        public override string ToString() {
            return "(" + prev.from + ", " + from + " , " + next.from + ")";
        }
    }
    public class Face {
        public HalfEdge outer_component;
        public List<HalfEdge> inner_components;

        public Face() {
            inner_components = new List<HalfEdge>();
        }
    }
}
