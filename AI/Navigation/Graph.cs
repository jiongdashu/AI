using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;


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
        public List<Vertex> GetPathBFS(GameObject srcObj, GameObject desObj)
        {

            if (srcObj == null || desObj == null)
            {
                return new List<Vertex>();
            }
            Vertex srcVertex = GetNearestVertex(srcObj.transform.position);
            Vertex desVertex = GetNearestVertex(desObj.transform.position);
            Vertex v;
            Vertex[] neighbours;
            Queue<Vertex> frontier = new Queue<Vertex>();
            Dictionary<Vertex,Vertex> comeFrom = new Dictionary<Vertex, Vertex>();
            frontier.Enqueue(srcVertex);
            comeFrom.Add(srcVertex, null);
            while (frontier.Count != 0)
            {
                v = frontier.Dequeue();
                //Goal Test
                if(ReferenceEquals(v, desVertex))
                {
                    return BuildPath();
                }
                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (comeFrom.ContainsKey(n))
                        continue;
                    comeFrom[n] = v;
                }


            }
            return new List<Vertex>();
        }

        
        //As Same as BFS,Except for frointer list is stack
        public List<Vertex> GetPathDFS(GameObject srcObj,GameObject desObj)
        {

            if (srcObj == null || desObj == null)
            {
                return new List<Vertex>();
            }
            Vertex srcVertex = GetNearestVertex(srcObj.transform.position);
            Vertex desVertex = GetNearestVertex(desObj.transform.position);
            Vertex v;
            Vertex[] neighbours;
            Queue<Vertex> frontier = new Queue<Vertex>();
            Dictionary<Vertex, Vertex> comeFrom = new Dictionary<Vertex, Vertex>();
            frontier.Enqueue(srcVertex);
            comeFrom.Add(srcVertex, null);
            while (frontier.Count != 0)
            {
                v = frontier.Dequeue();
                //Goal Test
                if (ReferenceEquals(v, desVertex))
                {
                    return BuildPath();
                }
                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (comeFrom.ContainsKey(n))
                        continue;
                    comeFrom[n] = v;
                }


            }
            return new List<Vertex>();
        }
        

        public List<Vertex> GetPathDijkstra(GameObject srcObj, GameObject desObj)
        {
            if (srcObj == null || desObj == null)
            {
                return new List<Vertex>();
            }
            Vertex srcVertex = GetNearestVertex(srcObj.transform.position);
            Vertex desVertex = GetNearestVertex(desObj.transform.position);
            Vertex vertex;
            Edge[] edges;
            Vertex[] neighbours;
            SimplePriorityQueue<Vertex> frontier =new SimplePriorityQueue<Vertex>();
            Dictionary<Vertex, Vertex> comeFrom = new Dictionary<Vertex, Vertex>();
            HashSet<Vertex> closed = new HashSet<Vertex>();
            comeFrom.Add(srcVertex, null);
            //将每个节点的代价置为最大，同时更新源节点
            foreach (Vertex v in vertices)
            {
                frontier.Enqueue(v, Mathf.Infinity);
            }
            //先搜索源节点
            frontier.UpdatePriority(srcVertex, 0);

            while (frontier.Count != 0)
            {
                
                vertex = frontier.Dequeue();
                closed.Add(vertex);
                if (ReferenceEquals(vertex, desVertex))
                {
                    return BuildPath();
                }
                edges = GetEdges(vertex);
                foreach(Edge e in edges)
                {
                    //计算新的带价值，如果小于目前的代价那么进行更新
                    float costNew = frontier.GetPriority(vertex) + e.cost;
                    //如果在闭集中则直接删除，因为此时闭集中已经时最短路了
                    if (closed.Contains(e.vertex))
                    {
                        continue;
                    }
                    //如果新的路径带价值小则进行更新
                    if(costNew< frontier.GetPriority(e.vertex))
                    {
                        
                        frontier.UpdatePriority(e.vertex, costNew);
                        comeFrom[e.vertex] = vertex;
                    }
                    //Relax 
                    

                }
                
            }
            return new List<Vertex>();
        }

        //与DJISTRA的区别：
        //动态扩展Open列表，更加适合与应用实现
        public List<Vertex> GetPathUCS(GameObject srcObj, GameObject desObj)
        {
            if (srcObj == null || desObj == null)
            {
                return new List<Vertex>();
            }
            Vertex srcVertex = GetNearestVertex(srcObj.transform.position);
            Vertex desVertex = GetNearestVertex(desObj.transform.position);
            Vertex vertexNow;
            Edge[] edges;           
            SimplePriorityQueue<Vertex> frontier = new SimplePriorityQueue<Vertex>();
            Dictionary<Vertex, Vertex> comeFrom = new Dictionary<Vertex, Vertex>();
            HashSet<Vertex> closed = new HashSet<Vertex>();
            comeFrom.Add(srcVertex, null);
           
            frontier.Enqueue(srcVertex, 0);

            while (frontier.Count != 0)
            {
                
                vertexNow = frontier.First;
                closed.Add(vertexNow);
                if (ReferenceEquals(vertexNow, desVertex))
                {
                    return BuildPath();
                }
                edges = GetEdges(vertexNow);
                foreach (Edge e in edges)
                {
                    if (closed.Contains(e.vertex))
                    {
                        continue;
                    }
                    float costNew = frontier.GetPriority(vertexNow) + e.cost;
                    //如果不在open列表中则直接添加，如果在看当前代价如果更小则更新open列表
                    if (!frontier.Contains(e.vertex))
                    {
                        frontier.Enqueue(e.vertex, costNew);
                        comeFrom[e.vertex] = vertexNow;
                    }
                    else if (costNew < frontier.GetPriority(e.vertex))
                    {

                        frontier.UpdatePriority(e.vertex, costNew);
                        comeFrom[e.vertex] = vertexNow;
                    }
                   


                }
                frontier.Dequeue();

            }
            return new List<Vertex>();
        }


        private List<Vertex> BuildPath()
        {
            throw new NotImplementedException();
        }




    }


}

