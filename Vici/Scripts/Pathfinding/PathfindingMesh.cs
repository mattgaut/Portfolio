using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingMesh : MonoBehaviour {
    [SerializeField]
    GameObject outer_object;
    [SerializeField]
    List<GameObject> inner_objects;

    List<Vector2> outer_boundary;
    List<List<Vector2>> inner_boundaries;

    public Pathfinder pathfinder {
        get; private set;
    }

	// Use this for initialization
	void Awake () {
        outer_boundary = new List<Vector2>(outer_object.GetComponent<PolygonCollider2D>().points);
        inner_boundaries = new List<List<Vector2>>();
        foreach (GameObject go in inner_objects) {
            inner_boundaries.Add(new List<Vector2>(go.GetComponent<PolygonCollider2D>().points));
        }

        List<DCEL.DCELEdge> edges = new List<DCEL.DCELEdge>();
        for (int i = 0; i < outer_boundary.Count; i++) {
            edges.Add(new DCEL.DCELEdge(outer_boundary[i] + (Vector2)transform.position, outer_boundary[(i + 1) % outer_boundary.Count] + (Vector2)transform.position));
        }
        foreach (List<Vector2> list in inner_boundaries) {
            for (int i = 0; i < list.Count; i++) {
                edges.Add(new DCEL.DCELEdge(list[i] + (Vector2)transform.position, list[(i + 1) % list.Count] + (Vector2)transform.position));
            }
        }


        DCEL dcel = new DCEL(edges);

        int x = 0;
        while (dcel.faces[x].inner_components.Count > 0 && dcel.faces[x].outer_component == null) {
            x++;
        }

        if (dcel.faces.Count == 2) {
            if (dcel.faces[0].outer_component == null) {
                x = 0;
            } else {
                x = 1;
            }
        }

        Triangulation tri = new Triangulation(dcel.faces[x]);
        pathfinder = new Pathfinder(tri);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class Pathfinder {
    public List<Geometry.Triangle> floor_tiles {
        get; private set;
    }
    public Dictionary<Vector2, List<Vector2>> pathfinding_nodes;

    public Dictionary<Geometry.Edge, Geometry.Triangle> boundary_edges {
        get; private set;
    }
    public SortedList<Vector2, HashSet<Geometry.Edge>> sorted_boundary_edges {
        get; private set;
    }
    private Dictionary<Vector2, List<Geometry.Triangle>> points_to_triangle;
    private List<Vector2> sorted_boundary_edges_keys;
    public DCEL triangulated_floor {
        get; private set;
    }

    public Pathfinder(DCEL.Face f) {
        triangulated_floor = (new Triangulation(f)).triangulated_shape;
        CreateTriangles(triangulated_floor);
        FindBoundaryEdges();
        FindNeighborPoints();
        LoadPointTriangleDictionary();
    }
    public Pathfinder(Triangulation tri) {
        triangulated_floor = tri.triangulated_shape;
        CreateTriangles(triangulated_floor);
        FindBoundaryEdges();
        FindNeighborPoints();
        LoadPointTriangleDictionary();
    }

    public Pathfinder(DCEL floor_boundary) {
        triangulated_floor = (new Triangulation(floor_boundary)).triangulated_shape;
        CreateTriangles(triangulated_floor);
        FindBoundaryEdges();
        FindNeighborPoints();
        LoadPointTriangleDictionary();
    }

    void FindNeighborPoints() {
        pathfinding_nodes = new Dictionary<Vector2, List<Vector2>>();
        foreach (Geometry.Triangle t in floor_tiles) {
            if (!pathfinding_nodes.ContainsKey(t.a)) {
                pathfinding_nodes.Add(t.a, new List<Vector2>());
            }

            if (!pathfinding_nodes[t.a].Contains(t.b)) {
                pathfinding_nodes[t.a].Add(t.b);
            }
            if (!pathfinding_nodes[t.a].Contains(t.c)) {
                pathfinding_nodes[t.a].Add(t.c);
            }

            if (!pathfinding_nodes.ContainsKey(t.b)) {
                pathfinding_nodes.Add(t.b, new List<Vector2>());
            }

            if (!pathfinding_nodes[t.b].Contains(t.a)) {
                pathfinding_nodes[t.b].Add(t.a);
            }
            if (!pathfinding_nodes[t.b].Contains(t.c)) {
                pathfinding_nodes[t.b].Add(t.c);
            }
            if (!pathfinding_nodes.ContainsKey(t.c)) {
                pathfinding_nodes.Add(t.c, new List<Vector2>());
            }

            if (!pathfinding_nodes[t.c].Contains(t.a)) {
                pathfinding_nodes[t.c].Add(t.a);
            }
            if (!pathfinding_nodes[t.c].Contains(t.b)) {
                pathfinding_nodes[t.c].Add(t.b);
            }
        }
    }

    void FindNeighborTriangles() {
        Dictionary<Geometry.Edge, Geometry.Triangle> open_edges = new Dictionary<Geometry.Edge, Geometry.Triangle>();
        foreach (Geometry.Triangle t in floor_tiles) {
            Geometry.Edge ab = new Geometry.Edge(t.a, t.b), ac = new Geometry.Edge(t.a, t.c), bc = new Geometry.Edge(t.b, t.c);
            if (open_edges.ContainsKey(ab)) {
                open_edges[ab].AddNeighbor(t);
                open_edges.Remove(ab);
            } else {
                open_edges.Add(ab, t);
            }
            if (open_edges.ContainsKey(ac)) {
                open_edges[ac].AddNeighbor(t);
                open_edges.Remove(ac);
            } else {
                open_edges.Add(ac, t);
            }
            if (open_edges.ContainsKey(bc)) {
                open_edges[bc].AddNeighbor(t);
                open_edges.Remove(bc);
            } else {
                open_edges.Add(bc, t);
            }
        }
    }

    void CreateTriangles(DCEL triangulated_floor) {
        floor_tiles = new List<Geometry.Triangle>();
        Dictionary<Geometry.Edge, Geometry.Triangle> open_edges = new Dictionary<Geometry.Edge, Geometry.Triangle>();
        foreach (DCEL.Face f in triangulated_floor.faces) {
            if (f.outer_component != null) {
                List<DCEL.HalfEdge> edges = DCEL.FindEdgesInCycle(f.outer_component);
                if (edges.Count == 3) {
                    Geometry.Triangle t = new Geometry.Triangle(edges[0].from.location, edges[1].from.location, edges[2].from.location);
                    floor_tiles.Add(t);
                    Geometry.Edge ab = new Geometry.Edge(t.a, t.b), ac = new Geometry.Edge(t.a, t.c), bc = new Geometry.Edge(t.b, t.c);
                    if (open_edges.ContainsKey(ab)) {
                        open_edges[ab].AddNeighbor(t);
                        open_edges.Remove(ab);
                    } else {
                        open_edges.Add(ab, t);
                    }
                    if (open_edges.ContainsKey(ac)) {
                        open_edges[ac].AddNeighbor(t);
                        open_edges.Remove(ac);
                    } else {
                        open_edges.Add(ac, t);
                    }
                    if (open_edges.ContainsKey(bc)) {
                        open_edges[bc].AddNeighbor(t);
                        open_edges.Remove(bc);
                    } else {
                        open_edges.Add(bc, t);
                    }
                }
            }
        }
    }

    void LoadPointTriangleDictionary() {
        points_to_triangle = new Dictionary<Vector2, List<Geometry.Triangle>>();
        foreach (Geometry.Triangle triangle in floor_tiles) {
            if (points_to_triangle.ContainsKey(triangle.a)) {
                points_to_triangle[triangle.a].Add(triangle);
            } else {
                points_to_triangle.Add(triangle.a, new List<Geometry.Triangle>() { triangle });
            }
            if (points_to_triangle.ContainsKey(triangle.b)) {
                points_to_triangle[triangle.b].Add(triangle);
            } else {
                points_to_triangle.Add(triangle.b, new List<Geometry.Triangle>() { triangle });
            }
            if (points_to_triangle.ContainsKey(triangle.c)) {
                points_to_triangle[triangle.c].Add(triangle);
            } else {
                points_to_triangle.Add(triangle.c, new List<Geometry.Triangle>() { triangle });
            }
        }

    }

    void FindBoundaryEdges() {
        sorted_boundary_edges = new SortedList<Vector2, HashSet<Geometry.Edge>>(new Geometry.PointComparer(true));
        sorted_boundary_edges_keys = new List<Vector2>();
        boundary_edges = new Dictionary<Geometry.Edge, Geometry.Triangle>();
        SortedList<Vector2, BoundaryPoint> action_points = new SortedList<Vector2, BoundaryPoint>(new Geometry.PointComparer(true));
        foreach (Geometry.Triangle t in floor_tiles) {
            if (ReferenceEquals(t.ab, null)) {
                Geometry.Edge e = new Geometry.Edge(t.a, t.b);
                boundary_edges.Add(e, t);
                if (!action_points.ContainsKey(t.a))
                    action_points.Add(t.a, new BoundaryPoint(t.a));

                action_points[t.a].AddEdge(e);

                if (!action_points.ContainsKey(t.b))
                    action_points.Add(t.b, new BoundaryPoint(t.b));

                action_points[t.b].AddEdge(e);
            }
            if (ReferenceEquals(t.bc, null)) {
                Geometry.Edge e = new Geometry.Edge(t.b, t.c);
                boundary_edges.Add(e, t);

                if (!action_points.ContainsKey(t.b))
                    action_points.Add(t.b, new BoundaryPoint(t.b));

                action_points[t.b].AddEdge(e);

                if (!action_points.ContainsKey(t.c))

                    action_points.Add(t.c, new BoundaryPoint(t.c));
                action_points[t.c].AddEdge(e);
            }
            if (ReferenceEquals(t.ac, null)) {
                Geometry.Edge e = new Geometry.Edge(t.a, t.c);
                boundary_edges.Add(e, t);

                if (!action_points.ContainsKey(t.a))
                    action_points.Add(t.a, new BoundaryPoint(t.a));

                action_points[t.a].AddEdge(e);

                if (!action_points.ContainsKey(t.c))
                    action_points.Add(t.c, new BoundaryPoint(t.c));
                action_points[t.c].AddEdge(e);
            }
        }


        List<Geometry.Edge> current_active = new List<Geometry.Edge>();
        for (int i = 0; i < action_points.Count; i++) {
            BoundaryPoint p = action_points[action_points.Keys[i]];
            Geometry.Edge e = p.a;
            // edge a
            if (e.upper == action_points.Keys[i]) {
                current_active.Add(e);
                if (!sorted_boundary_edges.ContainsKey(e.upper)) {
                    sorted_boundary_edges.Add(e.upper, new HashSet<Geometry.Edge>(current_active));
                } else {
                    sorted_boundary_edges[e.upper].UnionWith(current_active);
                }

            } else {
                if (!sorted_boundary_edges.ContainsKey(e.lower)) {
                    sorted_boundary_edges.Add(e.lower, new HashSet<Geometry.Edge>(current_active));
                } else {
                    sorted_boundary_edges[e.lower].UnionWith(current_active);
                }
                current_active.Remove(e);
            }

            e = p.b;
            //Edge b
            if (e.upper == action_points.Keys[i]) {
                current_active.Add(e);
                if (!sorted_boundary_edges.ContainsKey(e.upper)) {
                    sorted_boundary_edges.Add(e.upper, new HashSet<Geometry.Edge>(current_active));
                } else {
                    sorted_boundary_edges[e.upper].UnionWith(current_active);
                }

            } else {
                if (!sorted_boundary_edges.ContainsKey(e.lower)) {
                    sorted_boundary_edges.Add(e.lower, new HashSet<Geometry.Edge>(current_active));
                } else {
                    sorted_boundary_edges[e.lower].UnionWith(current_active);
                }
                current_active.Remove(e);
            }
        }

        sorted_boundary_edges_keys.AddRange(sorted_boundary_edges.Keys);
        sorted_boundary_edges_keys.Sort(new Geometry.PointComparer());
    }

    bool RayInBounds(Vector2 point, Vector2 check) {
        if (points_to_triangle.ContainsKey(point)) {
            foreach (Geometry.Triangle t in points_to_triangle[point]) {
                if (t.PointBetweenRays(check, point)) {
                    return true;
                }
            }
        }
        return false;
    }

    bool EdgeInBounds(Geometry.Edge e) {
        foreach (Geometry.Edge edge in boundary_edges.Keys) {
            if (e.Intersects(edge)) {
                return false;
            }
        }
        return true;

        //HashSet<Geometry.Edge> active_edges = new HashSet<Geometry.Edge>();

        //int upper_bound = sorted_boundary_edges_keys.BinarySearch(e.upper, new Geometry.PointComparer());
        //int lower_bound = sorted_boundary_edges_keys.BinarySearch(e.lower, new Geometry.PointComparer());


        //if (lower_bound < 0) {
        //    lower_bound = ~lower_bound;
        //}
        //if (upper_bound < 0) {
        //    upper_bound = ~upper_bound;
        //}

        //if ((lower_bound == 0 && upper_bound == 0) || (lower_bound == sorted_boundary_edges.Count && upper_bound == sorted_boundary_edges.Count)) {
        //    return false;
        //} else {
        //    for (int i = lower_bound; i < sorted_boundary_edges.Count && i <= upper_bound; i++) {
        //        active_edges.UnionWith(sorted_boundary_edges[sorted_boundary_edges.Keys[i]]);
        //    }
        //    foreach (Geometry.Edge edge in active_edges) {
        //        if (edge.Intersects(e)) {
        //            return false;
        //        }
        //    }
        //}

        //return true;
    }

    public List<Vector2> FindPath(Vector2 start, Vector2 goal) {
        bool temp;
        return FindPath(start, goal, out temp);
    }
    public List<Vector2> FindPath(Vector2 start, Vector2 goal, out bool on_mesh) {
        Geometry.Triangle start_triangle = FindTriangleContainingPoint(start);
        Geometry.Triangle goal_triangle = FindTriangleContainingPoint(goal);

        if (ReferenceEquals(goal_triangle, null)) {
            goal = ClosestPoint(goal, out goal_triangle);
        }

        if (ReferenceEquals(start_triangle, null)) {
            on_mesh = false;
            return new List<Vector2>() { start };
        }

        if (ReferenceEquals(goal_triangle, start_triangle)) {
            on_mesh = true;
            return new List<Vector2>() { goal };
        }

        List<Vector2> path = Astar(start, goal, start_triangle, goal_triangle);
        path.Reverse();


        for (int i = 1; i < path.Count - 1; i++) {
            if (i == path.Count - 2 && i == 1) {
                if (EdgeInBounds(new Geometry.Edge(path[i + 1], path[i - 1]))) {
                    //Debug.DrawLine(Utility.Vector2ToVector3(path[i - 1], Utility.Axis.z), Utility.Vector2ToVector3(path[i + 1], Utility.Axis.z), Color.green, 0);
                    path.RemoveAt(i);
                    i--;
                } else {
                    //Debug.DrawLine(Utility.Vector2ToVector3(path[i - 1], Utility.Axis.z), Utility.Vector2ToVector3(path[i + 1], Utility.Axis.z), Color.red, 0);
                }
            } else if (i == 1) {
                if (EdgeInBounds(new Geometry.Edge(path[i + 1], path[i - 1]))) {
                   //Debug.DrawLine(Utility.Vector2ToVector3(path[i - 1], Utility.Axis.z), Utility.Vector2ToVector3(path[i + 1], Utility.Axis.z), Color.green, 0);
                    path.RemoveAt(i);
                    i--;
                } else {
                    //Debug.DrawLine(Utility.Vector2ToVector3(path[i - 1], Utility.Axis.z), Utility.Vector2ToVector3(path[i + 1], Utility.Axis.z), Color.red, 0);
                }
            } else if (i == path.Count - 2) {
                if (EdgeInBounds(new Geometry.Edge(path[i - 1], path[i + 1]))) {
                    //Debug.DrawLine(Utility.Vector2ToVector3(path[i - 1], Utility.Axis.z), Utility.Vector2ToVector3(path[i + 1], Utility.Axis.z), Color.green, 0);
                    path.RemoveAt(i);
                    i--;
                } else {
                    //Debug.DrawLine(Utility.Vector2ToVector3(path[i - 1], Utility.Axis.z), Utility.Vector2ToVector3(path[i + 1], Utility.Axis.z), Color.red, 0);
                }
            } else {
                if (EdgeInBounds(new Geometry.Edge(path[i + 1], path[i - 1]))) {
                    //Debug.DrawLine(Utility.Vector2ToVector3(path[i - 1], Utility.Axis.z), Utility.Vector2ToVector3(path[i + 1], Utility.Axis.z), Color.green, 0);
                    path.RemoveAt(i);
                    i--;
                } else {
                    //Debug.DrawLine(Utility.Vector2ToVector3(path[i - 1], Utility.Axis.z), Utility.Vector2ToVector3(path[i + 1], Utility.Axis.z), Color.red, 0);
                }
            }

        }
        while (path[0] == start) {
            path.RemoveAt(0);
        }
        on_mesh = true;
        return path;
    }

    public bool OnMesh(Vector2 pos) {
        return !ReferenceEquals(FindTriangleContainingPoint(pos), null);
    }
    Vector2 ClosestPoint(Vector2 point) {
        Geometry.Triangle temp = null;
        return (ClosestPoint(point, out temp));
    }
    Vector2 ClosestPoint(Vector2 point, out Geometry.Triangle point_on_triangle) {
        Vector2 current_closest = Vector2.zero;
        float current_dist = float.MaxValue;

        point_on_triangle = null;

        foreach (Geometry.Edge e in boundary_edges.Keys) {
            Vector2 intersection = e.ClosestPointOnEdge(point);
            float dist = Vector2.Distance(intersection, point);
            if (dist < current_dist) {
                current_dist = dist;
                point_on_triangle = boundary_edges[e];
                current_closest = intersection;
            }
        }
        return current_closest;
    }

    List<Vector2> Astar(Vector2 start, Vector2 goal, Geometry.Triangle start_t, Geometry.Triangle goal_t) {
        List<AstarNode<Vector2>> frontier = new List<AstarNode<Vector2>>();
        Dictionary<Vector2, AstarNode<Vector2>> frontier_hash = new Dictionary<Vector2, AstarNode<Vector2>>();
        frontier_hash.Add(start, new AstarNode<Vector2>(start, null, 0, Vector2.Distance(start, goal)));

        foreach (Vector2 node in start_t.nodes) {
            frontier.Add(new AstarNode<Vector2>(node, frontier_hash[start], Vector2.Distance(start, node), Vector2.Distance(goal, node)));
            frontier_hash.Add(node, frontier[frontier.Count - 1]);
        }
        AstarNode<Vector2> final_node = null;
        while (frontier.Count > 0) {
            frontier.Sort((AstarNode<Vector2> a, AstarNode<Vector2> b) => (AstarNode<Vector2>.Compare(a, b)));

            AstarNode<Vector2> current = frontier[0];
            frontier.RemoveAt(0);

            if (current.tile == goal) {
                final_node = current;
                break;
            }

            if (goal_t.HasEndPoint(current.tile)) {
                if (frontier_hash.ContainsKey(goal)) {
                    if (frontier_hash[goal].cost > current.cost_from_start + Vector2.Distance(current.tile, goal) + Vector2.Distance(current.tile, goal)) {
                        frontier_hash[goal].came_from = current;
                        frontier.Add(frontier_hash[goal]);
                    } else {
                        continue;
                    }
                } else {
                    AstarNode<Vector2> goal_node = new AstarNode<Vector2>(goal, current, current.cost_from_start + Vector2.Distance(goal, current.tile), 0);
                    frontier_hash.Add(goal, goal_node);
                    frontier.Add(goal_node);
                }
            }
            foreach (Vector2 node in pathfinding_nodes[current.tile]) {
                if (frontier_hash.ContainsKey(node)) {
                    if (frontier_hash[node].cost > current.cost_from_start + Vector2.Distance(current.tile, node) + Vector2.Distance(node, goal)) {
                        frontier_hash[node].came_from = current;
                        frontier.Add(frontier_hash[node]);
                    } else {
                        continue;
                    }
                } else {
                    AstarNode<Vector2> neighbor = new AstarNode<Vector2>(node, current, current.cost_from_start + Vector2.Distance(node, current.tile), Vector2.Distance(node, goal));
                    frontier_hash.Add(node, neighbor);
                    frontier.Add(neighbor);
                }
            }
        }
        if (final_node == null) {
            return new List<Vector2>() { start };
        } else {
            List<Vector2> to_ret = new List<Vector2>();
            while (final_node != null) {
                to_ret.Add(final_node.tile);
                final_node = final_node.came_from;
            }
            return to_ret;
        }
    }

    Geometry.Triangle FindTriangleContainingPoint(Vector2 point) {
        foreach (Geometry.Triangle t in floor_tiles) {
            if (t.ContainsPoint(point)) {
                return t;
            }
        }
        return null;
    }
}

class AstarNode<T> {
    public T tile { get; protected set; }
    public AstarNode<T> came_from;
    public float cost_to_finish;
    public float cost_from_start;
    public float cost { get { return cost_to_finish + cost_from_start; } }

    public AstarNode(T tile, AstarNode<T> came_from) {
        this.tile = tile;
        cost_from_start = float.MaxValue;
        cost_to_finish = float.MaxValue;
        this.came_from = came_from;
    }
    public AstarNode(T tile, AstarNode<T> came_from, float to_start, float to_finish) {
        this.tile = tile;
        cost_from_start = to_start;
        cost_to_finish = to_finish;
        this.came_from = came_from;
    }

    public static int Compare(AstarNode<T> a, AstarNode<T> b) {
        return System.Math.Sign(a.cost - b.cost);
    }
}

class BoundaryPoint {
    public Vector2 pos;
    public Geometry.Edge a, b;

    public BoundaryPoint(Vector2 pos) {
        this.pos = pos;
    }

    public void AddEdge(Geometry.Edge e) {
        if (ReferenceEquals(a, null)) {
            a = e;
        } else {
            b = e;
        }
    }

    public class Comparer : IComparer<BoundaryPoint> {
        public int Compare(BoundaryPoint x, BoundaryPoint y) {
            return Geometry.PointHeightCompare(x.pos, y.pos);
        }
    }
}
