using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS.AI
{
    public class Graph : MonoBehaviour
    {
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbors;
        protected List<List<float>> costs;

        protected virtual void Start()
        {
            Load();
        }
        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual Vertex GetNearestVertex(Vector2 position)
        {
            return null;
        }
        public virtual Vertex GetVertexObj(int id)
        {
            if (vertices == null || vertices.Count == 0)
            {
                return null;
            }
            if (id < 0 || id > vertices.Count)
                return null;
            return vertices[id];

        }
        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (neighbors == null || neighbors.Count == 0)
                return new Vertex[0];
            if(v.id<0||v.id>= neighbors.Count)
                return new Vertex[0];
            return neighbors[v.id].ToArray();
        }

        public virtual Edge[] GetEdges(Vertex v)
        {
            if (ReferenceEquals(neighbors, null) || neighbors.Count == 0)
                return new Edge[0];
            if (v.id < 0 || v.id >= neighbors.Count)
                return new Edge[0];
            int numEdges = neighbors[v.id].Count;
            Edge[] edges = new Edge[numEdges];
            List<Vertex> vertexList = neighbors[v.id];
            List<float> costList = costs[v.id];
            for (int i = 0; i < numEdges; i++)
            {
                edges[i] = new Edge();
                edges[i].cost = costList[i];
                edges[i].vertex = vertexList[i];
            }
            return edges;
        }


        
    }


}

