using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triangulation {
    public DCEL triangulated_shape {
        get;
        private set;
    }

    public Triangulation(DCEL.Face f) {

        HashSet<DCEL.DCELEdge> edges = new HashSet<DCEL.DCELEdge>();

        if (f.inner_components.Count > 0) {
            foreach (Geometry.Edge e in RemoveInnerComponents(f)) {
                edges.Add(new DCEL.DCELEdge(e.upper, e.lower));
            }
        }

        triangulated_shape = new DCEL(new List<DCEL.DCELEdge>(edges));

        Dictionary<DCEL.HalfEdge, DCEL.HalfEdge> next = new Dictionary<DCEL.HalfEdge, DCEL.HalfEdge>();
        Dictionary<DCEL.HalfEdge, DCEL.HalfEdge> prev = new Dictionary<DCEL.HalfEdge, DCEL.HalfEdge>();

        DCEL.Face face = triangulated_shape.faces[0];
        if (ReferenceEquals(face.outer_component, null)) {
            face = triangulated_shape.faces[1];
        }
        foreach (DCEL.HalfEdge he in DCEL.FindEdgesInCycle(face.outer_component)) {
            next.Add(he, he.next);
            prev.Add(he, he.prev);
        }

        HashSet<Geometry.Edge> geo_edges = new HashSet<Geometry.Edge>();
        foreach (DCEL.HalfEdge he in DCEL.FindEdgesInCycle(face.outer_component)) {
            geo_edges.Add(new Geometry.Edge(he.from.location, he.next.from.location));
        }


        DCEL.HalfEdge edge_to_be_triangulated = face.outer_component;

        DCEL.HalfEdge current = edge_to_be_triangulated;
        List<DCEL.HalfEdge> remaining = new List<DCEL.HalfEdge>();
        int count = 0;
        do {
            count++;
            current = next[current];
            remaining.Add(current);
        } while (!ReferenceEquals(current, edge_to_be_triangulated));
        while (count > 3) {
            current = edge_to_be_triangulated;
            int prev_count = count;
            do {

                float angle = Utility.AngleBetween(prev[current].from.location, next[current].from.location, current.from.location);
                if (angle < 180) {
                    if (!CheckIntersection(new Geometry.Edge(next[current].from.location, prev[current].from.location), geo_edges)
                        && EdgeInBound(new Geometry.Edge(next[current].from.location, prev[current].from.location), prev[current], next[current])) {
                        geo_edges.Add(new Geometry.Edge(next[current].from.location, prev[current].from.location));
                        prev[next[current]] = prev[current];
                        next[prev[current]] = next[current];

                        remaining.Remove(current);

                        next.Remove(current);
                        prev.Remove(current);

                        edge_to_be_triangulated = remaining[Random.Range(0, remaining.Count)];

                        count--;
                        break;
                    }
                }
                current = next[current];
            } while (!ReferenceEquals(current, edge_to_be_triangulated));
            if (count == prev_count) {
                throw new System.Exception("Couldn't find ear: " + count);
            }
        }

        edges.Clear();
        foreach (Geometry.Edge e in geo_edges) {
            edges.Add(new DCEL.DCELEdge(e.lower, e.upper));
        }


        triangulated_shape = new DCEL(new List<DCEL.DCELEdge>(edges));
    }
    public Triangulation(DCEL d) {
        List<DCEL.DCELEdge> edges = new List<DCEL.DCELEdge>();

        foreach (DCEL.Face f in d.faces) {
            if (f.inner_components.Count > 0) {
                foreach (Geometry.Edge e in RemoveInnerComponents(f)) {
                    edges.Add(new DCEL.DCELEdge(e.upper, e.lower));
                }
            }
        }

        triangulated_shape = new DCEL(edges);

        Debug.Log(triangulated_shape.faces.Count);

        Dictionary<DCEL.HalfEdge, DCEL.HalfEdge> next = new Dictionary<DCEL.HalfEdge, DCEL.HalfEdge>();
        Dictionary<DCEL.HalfEdge, DCEL.HalfEdge> prev = new Dictionary<DCEL.HalfEdge, DCEL.HalfEdge>();

        DCEL.Face face = triangulated_shape.faces[0];
        if (ReferenceEquals(face.outer_component, null)) {
            face = triangulated_shape.faces[1];
        }
        foreach (DCEL.HalfEdge he in DCEL.FindEdgesInCycle(face.outer_component)) {
            next.Add(he, he.next);
            prev.Add(he, he.prev);
        }

        HashSet<Geometry.Edge> geo_edges = new HashSet<Geometry.Edge>();
        foreach (DCEL.HalfEdge he in DCEL.FindEdgesInCycle(face.outer_component)) {
            geo_edges.Add(new Geometry.Edge(he.from.location, he.next.from.location));
        }


        DCEL.HalfEdge edge_to_be_triangulated = face.outer_component;

        DCEL.HalfEdge current = edge_to_be_triangulated;
        List<DCEL.HalfEdge> remaining = new List<DCEL.HalfEdge>();
        int count = 0;
        do {
            count++;
            current = next[current];
            remaining.Add(current);
        } while (!ReferenceEquals(current, edge_to_be_triangulated));
        while (count > 3) {
            current = edge_to_be_triangulated;
            int prev_count = count;
            do {

                float angle = Utility.AngleBetween(prev[current].from.location, next[current].from.location, current.from.location);
                if (angle < 180) {
                    if (!CheckIntersection(new Geometry.Edge(next[current].from.location, prev[current].from.location), geo_edges)
                        && EdgeInBound(new Geometry.Edge(next[current].from.location, prev[current].from.location), prev[current], next[current])) {
                        geo_edges.Add(new Geometry.Edge(next[current].from.location, prev[current].from.location));
                        prev[next[current]] = prev[current];
                        next[prev[current]] = next[current];

                        remaining.Remove(current);

                        next.Remove(current);
                        prev.Remove(current);

                        edge_to_be_triangulated = remaining[Random.Range(0, remaining.Count)];

                        count--;
                        break;
                    }
                }
                current = next[current];
            } while (!ReferenceEquals(current, edge_to_be_triangulated));
            if (count == prev_count) {
                throw new System.Exception("Couldn't find ear: " + count);
            }
        }

        edges.Clear();
        foreach (Geometry.Edge e in geo_edges) {
            edges.Add(new DCEL.DCELEdge(e.lower, e.upper));
        }

        triangulated_shape = new DCEL(edges);
    }

    HashSet<Geometry.Edge> RemoveInnerComponents(DCEL.Face f) {
        HashSet<Geometry.Edge> edges = new HashSet<Geometry.Edge>();

        foreach (DCEL.HalfEdge he in DCEL.FindEdgesInCycle(f.outer_component)) {
            edges.Add(new Geometry.Edge(he.from.location, he.next.from.location));
        }

        foreach (DCEL.HalfEdge he in f.inner_components) {
            foreach (DCEL.HalfEdge h in DCEL.FindEdgesInCycle(he)) {
                edges.Add(new Geometry.Edge(h.from.location, h.next.from.location));
            }
        }

        bool found = false;
        foreach (DCEL.HalfEdge he in f.inner_components) {

            found = false;
            
            foreach (DCEL.HalfEdge h in DCEL.FindEdgesInCycle(f.outer_component)) {
                Geometry.Edge edge_to_check = new Geometry.Edge(h.from.location, he.from.location);
                if (!CheckIntersection(edge_to_check, edges)) {
                    edges.Add(edge_to_check);
                    found = true;
                    break;
                }
            }

            if (!found) {
                foreach (DCEL.HalfEdge h in f.inner_components) {
                    if (!ReferenceEquals(h, he) && !found) {
                        foreach (DCEL.HalfEdge h2 in DCEL.FindEdgesInCycle(he)) {
                            Geometry.Edge edge_to_check = new Geometry.Edge(h2.from.location, he.from.location);
                            if (!CheckIntersection(edge_to_check, edges)) {
                                edges.Add(edge_to_check);
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
        return edges;
    }

    bool CheckIntersection(Geometry.Edge e, HashSet<Geometry.Edge> edges) {
        foreach (Geometry.Edge edge in edges) {
            if (e.Intersects(edge)) {
                return true;
            }
        }
        return false;
    }

    bool EdgeInBound(Geometry.Edge e, DCEL.HalfEdge prev, DCEL.HalfEdge next) {
        if (e.upper == prev.from.location) {
            if (Utility.AngleBetween(prev.prev.from.location, prev.next.from.location, prev.from.location) <= Utility.AngleBetween(e.lower, prev.next.from.location, prev.from.location)) {
                return false;
            }
            if (Utility.AngleBetween(next.prev.from.location, next.next.from.location, next.from.location) <= Utility.AngleBetween(next.prev.from.location, e.upper, next.from.location)) {
                return false;
            }
        } else {
            if (Utility.AngleBetween(prev.prev.from.location, prev.next.from.location, prev.from.location) <= Utility.AngleBetween(e.upper, prev.next.from.location, prev.from.location)) {
                return false;
            }
            if (Utility.AngleBetween(next.prev.from.location, next.next.from.location, next.from.location) <= Utility.AngleBetween(next.prev.from.location, e.lower, next.from.location)) {
                return false;
            }
        }
        return true;
    }
}