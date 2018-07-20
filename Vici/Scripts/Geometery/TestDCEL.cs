using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDCEL : MonoBehaviour {
    [SerializeField]
    GameObject LineRender;

    void Awake() {
        Utility.RenderEdge.render = LineRender;
    }

    public void TestFace(DCEL.Face d) {
        Utility.RenderEdge.render = LineRender;
        foreach (DCEL.HalfEdge he in d.inner_components) {
            DCEL.HalfEdge current = he;
            do {
                new Utility.RenderEdge(current.from.location, current.next.from.location);
                current = current.next;
            } while (current != he);
        }

        DCEL.HalfEdge current2 = d.outer_component;
        do {
            new Utility.RenderEdge(current2.from.location, current2.next.from.location);
            current2 = current2.next;
        } while (current2 != d.outer_component);
    }

    public void Test(DCEL d) {
        foreach (DCEL.Face f in d.faces) {
            foreach (DCEL.HalfEdge he in f.inner_components) {
                DCEL.HalfEdge current = he;
                do {
                    new Utility.RenderEdge(current.from.location, current.next.from.location);
                    current = current.next;
                } while (current != he);
            }

            DCEL.HalfEdge current2 = f.outer_component;
            if (current2 != null) {
                do {
                    new Utility.RenderEdge(current2.from.location, current2.next.from.location);
                    current2 = current2.next;
                } while (current2 != f.outer_component);
            }
        }

    }

    public void TestEdges(List<DCEL.DCELEdge> edges) {
        Utility.RenderEdge.render = LineRender;
        foreach (DCEL.DCELEdge he in edges) {
            new Utility.RenderEdge(he.from, he.to);
        }
    }
}
