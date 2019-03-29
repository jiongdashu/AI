using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS.AI
{


    public class VertexVisibility :Vertex
    {

        private Collider2D collider2D ;
        private void OnEnable()
        {
            collider2D = GetComponent<Collider2D>();
        }
        public void ConnectNeighbours(List<Vertex> vertices)
        {
            RaycastHit2D[] hits;
            float distance = 0;
            Vector2 direction = Vector2.zero;

            collider2D.enabled = false;
            foreach(Vertex v in vertices)
            {
                if (v == this)
                {
                    continue;
                }
                direction = v.transform.position - transform.position;
                distance = direction.magnitude;
                hits = Physics2D.RaycastAll(transform.position, direction, distance);
                
                if (hits.Length == 1)
                {
                    if (hits[0].collider.tag.Equals("Vertex"))
                    {
                        Vertex vertex = hits[0].collider.GetComponent<Vertex>();
                        Edge edge = new Edge();
                        edge.cost = distance;
                       
                        /*
                        if (vertex != v)
                        {
                            break;
                        }*/
                        edge.vertex = vertex;
                        neighbours.Add(edge);
                    }
                }
            }
            collider2D.enabled = true;

        }

    }
}
