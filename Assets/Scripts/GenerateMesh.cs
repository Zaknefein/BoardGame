using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    public GridDraw grid;

    List<Vector3> vertices;
    List<int> triangles;
    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
   
    HashSet<int> checkedVertices = new HashSet<int>();
    List<List<int>> outlines = new List<List<int>>();

    public MeshFilter hexMesh;
    public Material materialBase;
    public Material materialAgua;

    void Start(){
        hexMesh = GetComponent<MeshFilter>();
       
    }
    public void GenerateMeshes(Hexagon hex)
    {
        triangleDictionary.Clear();
        outlines.Clear();
        checkedVertices.Clear();


        var center = hex.hexNodes[0];
       

        vertices = new List<Vector3>();   //necesario para generar Mesh
        triangles = new List<int>();    //necesario para generar Mesh

      AssignVertices(hex.meshNodes);

        for (int x = 0; x< 6 ;x++)
        {
            if (x < 5)
            {
                CreateTriangle(hex.meshNodes[6],  hex.meshNodes[x], hex.meshNodes[x+1]);
            }
            else
            {
                CreateTriangle(hex.meshNodes[6], hex.meshNodes[x], hex.meshNodes[0]);
            }
        }

       

        Mesh mesh = new Mesh();
        hexMesh.mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        hex.hexMesh.mesh= mesh;
       
        if (hex.isWater)
        {
            hex.render.material = materialAgua;
        }
        else
        {
            hex.render.material = materialBase;
        }
        

        hex.hexMesh.mesh.RecalculateNormals();
        hex.collider.sharedMesh = mesh;


       



        CreateWallMesh(hex);
    }


    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
     //   AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    void AddTriangleToDictionary(int triangleIndexKey, Triangle triangle)
    {
        if (triangleDictionary.ContainsKey(triangleIndexKey))
        {
            triangleDictionary[triangleIndexKey].Add(triangle);
        }
        else
        {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triangleDictionary.Add(triangleIndexKey, triangleList);
        }
    }

    struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertices;
        public Triangle(int a, int b, int c)
        {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;
            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;

        }

        public int this[int i]
        {
            get
            {
                return vertices[i];

            }
        }

        public bool Contains(int vertexIndex)
        {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }


    void CalculatedMeshOutlines(Hexagon hex)
    {

        
        

          for (int vertexIndex = 0; vertexIndex < vertices.Count-1; vertexIndex++)
          {
              if (!checkedVertices.Contains(vertexIndex))
              {
                

                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                
                if (newOutlineVertex != -1)
                  {
                      checkedVertices.Add(vertexIndex);

                      List<int> newOutline = new List<int>();
                      newOutline.Add(vertexIndex);
                      outlines.Add(newOutline);
                      FollowOutline(newOutlineVertex, outlines.Count-1);
                      outlines[outlines.Count - 1].Add(vertexIndex);

                  }

                
            }
          }
    }

    int GetConnectedOutlineVertex(int vertexIndex)
    {
        try {
         //   if (triangleDictionary.ContainsKey(vertexIndex)  ) { 
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle triangle = trianglesContainingVertex[i];

            for (int j = 0; j < 3; j++)
            {
                int vertexB = triangle[j];

                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB))
                {

                    if (IsOutlineEdge(vertexIndex, vertexB))
                    {
                        return vertexB;
                    }
                }
            }

            }

           // }
            return -1;

            
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException(vertexIndex.ToString()
                + " was not found in the dictionary");
        }
    }

    bool IsOutlineEdge(int vertexA, int vertexB)
    {
        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
        int sharedTriangleCount = 0;

        for (int i = 0; i < trianglesContainingVertexA.Count; i++)
        {
            if (trianglesContainingVertexA[i].Contains(vertexB))
            {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1)
                {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }


    void FollowOutline(int vertexIndex, int outlineIndex)
    {
       // if (vertexIndex < 6) { 
            outlines[outlineIndex].Add(vertexIndex);
            checkedVertices.Add(vertexIndex);
            int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

            if (nextVertexIndex != -1 )
            {
                FollowOutline(nextVertexIndex, outlineIndex);
            }
        //}

    }

    void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }

    }

    void CreateWallMesh(Hexagon hex)   //crea los base Grid
    {

        CalculatedMeshOutlines(hex);

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 2.5f;

       // hex.cornersA

        foreach (List<int> outline in outlines)
        {

            for (int i = 0; i <  outline.Count -1; i++)
            {
                int startIndex = wallVertices.Count;
                
                wallVertices.Add(vertices[outline[i + 1]]);//right
                wallVertices.Add(vertices[outline[i]]);//left
                wallVertices.Add(vertices[outline[i + 1]] - Vector3.back * wallHeight);//bottom right
                wallVertices.Add(vertices[outline[i]] - Vector3.back * wallHeight);//bottom left


                //aqui esta el orden de pintado de triangulos
                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }
        wallTriangles.Reverse(); //voltea los triangulos
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();

        
      
        hex.wallMesh.mesh = wallMesh;

        

        if (hex.isWater)
        {
            hex.wallRender.material = materialAgua;
        }
        else
        {
            hex.wallRender.material = materialBase;
        }

        hex.wallMesh.mesh.RecalculateNormals();
        hex.wallMesh.name = "wall";


    }

}
