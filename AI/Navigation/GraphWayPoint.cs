using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJS.AI
{
    public class GraphWayPoint : Graph
    {

        public override void Load()
        {
            Vertex[] verts =  GameObject.FindObjectsOfType<Vertex>();
            vertices = new List<Vertex>(verts);
            
            for (int i = 0; i < vertices.Count; i++)
            {
                VertexVisibility vv = vertices[i] as VertexVisibility;
                vv.id = i;
                vv.ConnectNeighbours(vertices);

            }
           
        }

        public override Vertex GetNearestVertex(Vector2 position)
        {
            Vertex vertex = null;
            float minDistance = Mathf.Infinity,dis;
            foreach (var v in vertices)
            {
                dis = Vector2.Distance(position, v.transform.position);
                if (dis < minDistance)
                {
                    dis = minDistance;
                    vertex = v;

                }
            }
            return vertex;
        }

        public override Vertex[] GetNeighbours(Vertex v)
        {
            List<Edge> edges = v.neighbours;
            Vertex[] ns = new Vertex[edges.Count];
            for (int i = 0; i < edges.Count; i++)
            {
                ns[i] = edges[i].vertex;
            }
            return ns;
        }

        public override Edge[] GetEdges(Vertex v)
        {
            return vertices[v.id].neighbours.ToArray();
        }
    }
}

