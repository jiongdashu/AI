using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS.AI
{
    public class Vertex : MonoBehaviour
    {
        public int id;
        public List<Edge> neighbours;      
        public Vertex prev;

        private void Awake()
        {
           
        }

    }

}
