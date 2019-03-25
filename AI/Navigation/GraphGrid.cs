using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CJS.AI
{
    public class GraphGrid : Graph
    {
        public GameObject obstaclePrefab;
        public string mapName = "arena.map";
        public bool get8Vicinity = false;
        public float cellSize = 1f;
        [Range(0, Mathf.Infinity)]
        public float defaultCost = 1f;
        [Range(0, Mathf.Infinity)]
        public float maximumCost = Mathf.Infinity;

        string mapsDir = "Maps";
        int numCols;
        int numRows;
        GameObject[] vertexObjs;
        bool[,] mapVertices;


        private int GridToId(int x,int y)
        {
            return numCols * y + x;
        }
        private Vector2 IdToGrid(int id)
        {
            Vector2 location = Vector2.zero;
            location.y = id / numCols;
            location.x = id % numCols;
            return location;
        }

        private void LoadMap(string fileName)
        {
            string path = Application.dataPath + "/" + mapsDir + "/" + fileName;
            try
            {
                StreamReader streamReader = new StreamReader(path);
                using (streamReader)
                {
                    string line="";
                    line = streamReader.ReadLine();
                    line = streamReader.ReadLine();
                    numRows =int.Parse( line.Split(' ')[1]);
                    line = streamReader.ReadLine();
                    numCols = int.Parse(line.Split(' ')[1]);
                    line = streamReader.ReadLine();

                    vertices = new List<Vertex>(numRows * numCols);
                    neighbours = new List<List<Vertex>>(numRows * numCols);
                    costs = new List<List<Vertex>>(numRows * numCols);
                    vertexObjs = new GameObject[numRows * numCols];
                    mapVertices = new bool[numRows, numCols];

                    for (int i = 0; i < numRows; i++)
                    {
                        line = streamReader.ReadLine();
                        for(int j = 0; j < numCols; j++)
                        {
                            bool isGround = true;
                            if (line[j] != '.')
                                isGround = false;
                            mapVertices[i, j] = isGround;


                        }


                    }

                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void Load()
        {
            LoadMap(mapName);
        }

        public override Vertex GetNearestVertex(Vector3 position)
        {
            int col = Mathf.FloorToInt(position.x / cellSize);
            int row = Mathf.FloorToInt(position.y / cellSize);
            int id = GridToId(col, row);
            return vertices[id];
        }
        

    }

}
