using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridDraw : MonoBehaviour
{
    public int worldWidth = 1;
    public int worldHeight = 1;

    public bool rotar = false;
    public float radius = 3;

    public string seed;
    public bool useRandomSeed;

    public Sprite spriteAreas;

    public Dictionary<HexSide, int> worldHexSideswithHex = new Dictionary<HexSide, int>();
    public Dictionary<HexSide, int> worldHexSideswithHexInCommon = new Dictionary<HexSide, int>();
    public Hexagon hex; //prueba

    public Hexagon[,] worldGrid; //prueba

    float sideLength;

    public GenerateMesh meshGen;

    public System.Random pseudoRandom;


    [Range(0, 100)]
    public int randomFillPercent;  // 

    int[,] map;

    public GridDraw()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        meshGen = GetComponent<GenerateMesh>();

        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

         pseudoRandom = new System.Random(seed.GetHashCode());

        GenerarGrid();


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {

            GenerarGrid();
        }



    }

    void CleanGrid()
    {
      for(int x= 0; x< worldWidth;x ++)
        {
            for (int y = 0; y < worldHeight; y++)
            {

                worldGrid[x, y].Destroy();
            }
        }
    }


    void GenerarGrid()
    {
        CrearPiezasGridBase();  //  creamos las piezas base con los lugares

        // Falta el smooth del mapping
        // 
        RefinarSalidasNuevas();



     
    }

    void RefinarSalidasNuevas()
    {
        List<Hexagon> HexagonosLimites = new List<Hexagon>();
        List<Hexagon> HexagonosConVecinos = new List<Hexagon>();

        //iteramos entre hexagonos para tratar los hexagonos limites
        //SI es un hexagono limite  y tiene un camino
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                var edgeHex=  isEdgeHexagon( worldGrid[x, y]);
                
                if (edgeHex)
                {HexagonosLimites.Add(worldGrid[x, y]);}
                else {HexagonosConVecinos.Add(worldGrid[x, y]);}

                foreach(HexSide sides in worldGrid[x, y].sides)
                {
                    if (!worldHexSideswithHex.ContainsKey(sides)){
                        worldHexSideswithHex.Add(sides, 0);
                    }
                    //else
                    //{
                    //    worldHexSideswithHex[sides]+=1 ;
                    //}
                }




            }
        }

        
        //Tras tener todos los lados del grid en un diccionario ahora tenemos que compararlos entre si y ver cuales son coincidentes
        foreach (KeyValuePair<HexSide, int> entry in worldHexSideswithHex)
        {

           foreach ( Hexagon hex in worldGrid)
            {
                foreach(HexSide _side in hex.sides)
                {
                    if (_side.CompareToSide(entry.Key))
                    {
                        var conteo = entry.Value + 1;
                        var ladoComun = new KeyValuePair<HexSide, int>(entry.Key, conteo);
                        worldHexSideswithHexInCommon.Add(entry.Key,conteo);
                       _side.midNode.active = true;
                    }
                    else
                    {
                        _side.midNode.active = false;
                    }
                    
                }
            }


            if (!worldHexSideswithHexInCommon.ContainsKey(entry.Key))
            {
                entry.Key.midNode.active = false;

            }
        }


       
       
        ////·Este bucle regenera los caminos con los nuevas Salidas.
        //for (int x = 0; x < worldWidth; x++)
        //{
        //    for (int y = 0; y < worldHeight; y++)
        //    {

        //        worldGrid[x, y].CrearCaminos();
        //    }
        //}
    }


    public void CrearPiezasGridBase()
    {
        if (worldGrid != null) CleanGrid();

        worldGrid = new Hexagon[worldWidth, worldHeight];
        sideLength = (2 * radius * 3.14f) / 6;


        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {

                if (rotar)
                {
                    if (y % 2 == 0)
                    {

                        var aguaTierra= (pseudoRandom.Next(0, 100) < randomFillPercent) ? false : true;
                        worldGrid[x, y] = new Hexagon(new Vector3((x * (Mathf.Sqrt(3) * radius)), y * ((radius * 2 * 0.75f)), 0), radius, rotar, x, y, pseudoRandom,this.transform, spriteAreas, aguaTierra);
                        worldGrid[x, y].spriteAreas = spriteAreas;
                        meshGen.GenerateMeshes(worldGrid[x, y]); //genera la forma del hexagono
                        


                    }
                    else
                    {
                        var aguaTierra = (pseudoRandom.Next(0, 100) < randomFillPercent) ? false : true;
                        worldGrid[x, y] = new Hexagon(new Vector3((x * (Mathf.Sqrt(3) * radius) + ((Mathf.Sqrt(3) * radius) / 2)), y * ((radius * 2 * 0.75f)), 0), radius, rotar, x, y, pseudoRandom, this.transform, spriteAreas, aguaTierra);
                        worldGrid[x, y].spriteAreas = spriteAreas;
                        meshGen.GenerateMeshes(worldGrid[x, y]);
                       
                    }



                }
                else
                {

                    if (x % 2 == 0)
                    {
                        var aguaTierra = (pseudoRandom.Next(0, 100) < randomFillPercent) ? false : true;
                        worldGrid[x, y] = new Hexagon(new Vector3(x * ((radius * 2 * 0.75f)), y * ((Mathf.Sqrt(3) * radius)) + ((Mathf.Sqrt(3) * radius) / 2), 0), radius, rotar, x, y, pseudoRandom, this.transform, spriteAreas, aguaTierra);
                        worldGrid[x, y].spriteAreas = spriteAreas;
                        meshGen.GenerateMeshes(worldGrid[x, y]);
                      
                    }
                    else
                    {
                        var aguaTierra = (pseudoRandom.Next(0, 100) < randomFillPercent) ? false : true;
                        worldGrid[x, y] = new Hexagon(new Vector3(x * ((radius * 2 * 0.75f)), y * ((Mathf.Sqrt(3) * radius)), 0), radius, rotar, x, y, pseudoRandom, this.transform, spriteAreas, aguaTierra);
                        worldGrid[x, y].spriteAreas = spriteAreas;
                        meshGen.GenerateMeshes(worldGrid[x, y]);
                       
                    }


                }
            }

        }

       
    }


    void OnDrawGizmos()
    {


        foreach (Hexagon hex in worldGrid)
        {

            var _exitWaysPos = hex.exitWaysPos;
            var size = new Vector3(.1f * radius, .1f * radius, .1f * radius);
            //foreach (Node node in _exitWaysPos)
            foreach (HexSide node in hex.sides)
            {
                if (node.midNode.active) Gizmos.DrawCube(node.midNode.position, size);
            }

            //foreach (Node node in hex.hexNodes)
            //{
            //    if (node.active) Gizmos.DrawSphere(node.position, .15f);
            //}
        }

    }

    public bool isEdgeHexagon(Hexagon hex)
    {
        if (hex.posXTile == 0 || hex.posXTile == worldWidth-1 || hex.posYTile == 0 || hex.posYTile == worldHeight - 1)
        {
            return true;
        }
        else
        {
            return false;
        }


    }



    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < worldWidth && y >= 0 && y < worldHeight;
    }

    public void CheckHexNeighborhood(Hexagon hex)
    {
        
    }

}


 

public class Hexagon
{

    public MeshFilter hexMesh  ;
    public MeshRenderer render  ;
    public MeshCollider collider  ;
    public GameObject go = new GameObject();
    public GameObject walls = new GameObject();
    public MeshFilter wallMesh;
    public MeshRenderer wallRender;
    public bool isWater = false;
    public float sideLength;

    public Vector3[] cornersA = new Vector3[6];
    public Vector3[] cornersB = new Vector3[6];
    public Vector3[] cornersC = new Vector3[6];
    public Vector3 position;

    public Node[] exitWaysPos = new Node[6];
    public Node[] hexNodes = new Node[25];

    public Node[] meshNodes = new Node[7];
    public List<Path> pathList = new List<Path>();
    public Sprite spriteAreas ;

    public int posXTile;
    public int posYTile;

    public HexSide[] sides = new HexSide[6];

    public Hexagon(Vector3 _pos, float _radiousSize, bool rotar,int x , int y, System.Random seed, Transform parent,Sprite spriteA,bool _isWater=false )
    {
        posXTile =x;
        posYTile =y;
        go.name = posXTile+","+posYTile;
        go.transform.parent = parent;
        walls.transform.parent = go.transform;
        isWater = _isWater;
        spriteAreas= spriteA;

        wallRender = walls.gameObject.AddComponent<MeshRenderer>();
        wallMesh = walls.gameObject.AddComponent<MeshFilter>();
       
        render =go.AddComponent<MeshRenderer>();
        hexMesh = go.AddComponent<MeshFilter>();
        collider = go.AddComponent<MeshCollider>();
        go.AddComponent<TextCursor>();
     
         position = _pos;

        generarHexagonoPrincipal(rotar, _radiousSize);

        GenerarSides();

        generarPuertas(); //genera todas las puertas
        var numeroPuertas = GenerateExits(seed.Next(2, 6), seed); //elige las puertas para salir de la ficha

        

        if (!isWater)
        {
                   // generarZonasAccion(); //genera todas las zonas donde puede haber accion
              //  var numeroZonas = seed.Next(numeroPuertas + 1, 7);
                   // generateAreas(numeroZonas, seed);//genera las zonas de accion
                   // CrearCaminos();
                     DrawHexagon();//dibuja las lineas del hexagono para debug
                   // BorrarNodosDesechados();
        }




    }


    private void GenerarSides()
    {

        for (int x = 0; x < 6;x++)
        {
            if (x == 5)
            {
                sides[x]= new HexSide( meshNodes[x], meshNodes[0],x);
            }
            else
            {
                sides[x] = new HexSide(meshNodes[x], meshNodes[x + 1], x);
            }
            
            
        }

    }

    private void BorrarNodosDesechados()
    {
       var nodosBorrar= new List<Node>();

        foreach ( Node node in hexNodes)
        {
            if (!node.active)
            {
                nodosBorrar.Add(node);
            }
           
        }


        foreach(Node delNode in nodosBorrar)
        {
            delNode.Destroy();
        }
    }

    private void generarHexagonoPrincipal(bool rotar,float _radiousSize)
    {
        float _radiousSizeA = _radiousSize;
        float _radiousSizeB = _radiousSize / 3 * 2;
        float _radiousSizeC = _radiousSize / 3;
        //generear Hexagono Principal
        if (rotar)
        {

            for (int a = 0; a < 6; a++)
            {
                cornersA[a] = new Vector3(
                    position.x + _radiousSizeA * (float)Math.Sin(a * 60 * Math.PI / 180f),
                    position.y + _radiousSizeA * (float)Math.Cos(a * 60 * Math.PI / 180f), 0);

                meshNodes[a] = new Node(cornersA[a],go.transform);

                cornersB[a] = new Vector3(
                   position.x + _radiousSizeB * (float)Math.Sin(a * 60 * Math.PI / 180f),
                   position.y + _radiousSizeB * (float)Math.Cos(a * 60 * Math.PI / 180f), 0);

                cornersC[a] = new Vector3(
                   position.x + _radiousSizeC * (float)Math.Sin(a * 60 * Math.PI / 180f),
                   position.y + _radiousSizeC * (float)Math.Cos(a * 60 * Math.PI / 180f), 0);



            }

        }

        else
        {
            for (int a = 0; a < 6; a++)
            {
                cornersA[a] = new Vector3(
                    position.x + _radiousSizeA * (float)Math.Cos(a * -60 * Math.PI / 180f),
                    position.y + _radiousSizeA * (float)Math.Sin(a * -60 * Math.PI / 180f), 0);

                meshNodes[a] = new Node(cornersA[a],go.transform);

                cornersB[a] = new Vector3(
                   position.x + _radiousSizeB * (float)Math.Cos(a * -60 * Math.PI / 180f),
                   position.y + _radiousSizeB * (float)Math.Sin(a * -60 * Math.PI / 180f), 0);

                cornersC[a] = new Vector3(
                   position.x + _radiousSizeC * (float)Math.Cos(a * -60 * Math.PI / 180f),
                   position.y + _radiousSizeC * (float)Math.Sin(a * -60 * Math.PI / 180f), 0);


            }


        }


    }

    private void generarPuertas()
    {
        meshNodes[6] = new Node(position, go.transform);

        //Coordenadas de las Puertas para la ficha
        for (int a = 0; a < 6; a++)
        {
            if (a == 0)
            {
                exitWaysPos[a] = new Node(Vector3.Lerp(cornersA[a], cornersA[cornersA.Length - 1], 0.5f),go.transform);
                Debug.Log("Posicion exitWay" + exitWaysPos[a].position.ToString() + " id :"+ a);
                for (int x= 0; x < 6; x++) {
                    var NodeA = new Node(new Vector3(cornersA[cornersA.Length - 1].x, cornersA[cornersA.Length - 1].y, cornersA[cornersA.Length - 1].z));
                   // Debug.Log("NodeA sides[" + x + "] :" + cornersA[a].ToString() + " id :" + a);
                    var NodeB = new Node(new Vector3(cornersA[a].x, cornersA[a].y, cornersA[a].z));
                    //Debug.Log("NodeB sides[" + x + "] :" + cornersA[a].ToString() + " id :" + a);

                    if (sides[x].CompareToSide(new HexSide(NodeA, NodeB)))
                    {
                        sides[x].midNode = exitWaysPos[a];
                        Debug.Log("Punto comun sides[" + x + "] :" + exitWaysPos[a].position.ToString() + " id :" + a);
                    }
                    
                }

            }
            else
            {
                exitWaysPos[a] = new Node(Vector3.Lerp(cornersA[a], cornersA[a - 1], 0.5f), go.transform);
                Debug.Log("Posicion exitWay" + exitWaysPos[a].position.ToString() + " id :" + a);
                sides[a].midNode = exitWaysPos[a];
               
                for (int x = 0; x < 6; x++)
                {
                    var NodeA = new Node(new Vector3(cornersA[a].x, cornersA[a].y, cornersA[a].z));
                  //  Debug.Log("NodeA sides[" + x + "] :" + cornersA[a].ToString() + " id :" + a);

                    var NodeB = new Node(new Vector3(cornersA[a-1].x, cornersA[a-1].y, cornersA[a-1].z));
                    //Debug.Log("NodeB sides[" + x + "] :" + cornersA[a].ToString() + " id :" + a);

                    if (sides[x].CompareToSide(new HexSide(NodeA, NodeB)))
                    {
                        sides[x].midNode = exitWaysPos[a];
                        Debug.Log("Punto comun sides["+ x+"] :" + exitWaysPos[a].position.ToString() + " id :" + a);
                    }

                }
            }
        }
    }
   
    private void generarZonasAccion()
    {
        //Coordenadas de las zonas de accion
        hexNodes[0] = new Node(position,go.transform);
        hexNodes[1] = new Node(cornersB[0], go.transform);
        hexNodes[2] = new Node(Vector3.Lerp(cornersB[0], cornersB[1], 0.5f), go.transform);
        hexNodes[3] = new Node(cornersB[1], go.transform);
        hexNodes[4] = new Node(Vector3.Lerp(cornersB[1], cornersB[2], 0.5f), go.transform);
        hexNodes[5] = new Node(cornersB[2], go.transform);
        hexNodes[6] = new Node(Vector3.Lerp(cornersB[2], cornersB[3], 0.5f), go.transform);
        hexNodes[7] = new Node(cornersB[3], go.transform);
        hexNodes[8] = new Node(Vector3.Lerp(cornersB[3], cornersB[4], 0.5f), go.transform);
        hexNodes[9] = new Node(cornersB[4], go.transform);
        hexNodes[10] = new Node(Vector3.Lerp(cornersB[4], cornersB[5], 0.5f), go.transform);
        hexNodes[11] = new Node(cornersB[5], go.transform);
        hexNodes[12] = new Node(Vector3.Lerp(cornersB[5], cornersB[0], 0.5f), go.transform);

        hexNodes[13] = new Node(cornersC[0], go.transform);
        hexNodes[14] = new Node(Vector3.Lerp(cornersC[0], cornersC[1], 0.5f), go.transform);
        hexNodes[15] = new Node(cornersC[1], go.transform);
        hexNodes[16] = new Node(Vector3.Lerp(cornersC[1], cornersC[2], 0.5f), go.transform);
        hexNodes[17] = new Node(cornersC[2], go.transform);
        hexNodes[18] = new Node(Vector3.Lerp(cornersC[2], cornersC[3], 0.5f), go.transform);
        hexNodes[19] = new Node(cornersC[3], go.transform);
        hexNodes[20] = new Node(Vector3.Lerp(cornersC[3], cornersC[4], 0.5f), go.transform);
        hexNodes[21] = new Node(cornersC[4], go.transform);
        hexNodes[22] = new Node(Vector3.Lerp(cornersC[4], cornersC[5], 0.5f), go.transform);
        hexNodes[23] = new Node(cornersC[5], go.transform);
        hexNodes[24] = new Node(Vector3.Lerp(cornersC[5], cornersC[0], 0.5f), go.transform);
    }



    public int SidesInCommon(Hexagon hexA , Hexagon hexB)   //funcion en la que se mandan dos hexagonos y nos indica cuantos lados tienen en comun
    {
        var conteoLadosComun = 0;
        foreach (HexSide sideA in hexA.sides)
        {
            foreach(HexSide sideB in hexB.sides)
            {
                var ladoComun = sideA.CompareToSide(sideB);
                if (ladoComun) conteoLadosComun++; 
            }
        }

        Debug.Log("HexagonoA: " + hexA.go.name + "HexagonoB: " + hexB.go.name + " Numero de lados en Comun: " + conteoLadosComun);
        return conteoLadosComun;
    }

    public void DrawHexagon()
    {


        for (int a = 0; a < 6; a++)
        {
            //dibuja los Hexagonos
            if (a == 0)
            {
                Debug.DrawLine(cornersA[a], cornersA[a + 1], Color.green, 200, false);
                Debug.DrawLine(cornersB[a], cornersB[a + 1], Color.green, 200, false);
                Debug.DrawLine(cornersC[a], cornersC[a + 1], Color.green, 200, false);
            }
            else if (a < 5)
            {
                Debug.DrawLine(cornersA[a], cornersA[a + 1], Color.yellow, 200, false);
                Debug.DrawLine(cornersB[a], cornersB[a + 1], Color.yellow, 200, false);
                Debug.DrawLine(cornersC[a], cornersC[a + 1], Color.yellow, 200, false);
            }
            else
            {
                Debug.DrawLine(cornersA[a], cornersA[0], Color.red, 200, false);
                Debug.DrawLine(cornersB[a], cornersB[0], Color.red, 200, false);
                Debug.DrawLine(cornersC[a], cornersC[0], Color.red, 200, false);
            }

            //dibujamos secciones
            Debug.DrawLine(position, cornersA[a], Color.magenta, 200, false);


            //dibujamos puntos intermedios
            if (a == 0)
            {
                Debug.DrawLine(position, Vector3.Lerp(cornersA[a], cornersA[cornersA.Length - 1], 0.5f), Color.cyan, 200, false);
            }
            else
            {
                Debug.DrawLine(position, Vector3.Lerp(cornersA[a], cornersA[a - 1], 0.5f), Color.cyan, 200, false);
            }


        }



    }

    public void DestroyAllNodes()
    {
        foreach (Node node in meshNodes)
        {
            node.Destroy();
        }

    }

    public void Destroy()
    {
        DestroyAllNodes();
        UnityEngine.GameObject.Destroy(go);
    }

    public int GenerateExits(int nDoors, System.Random random)
    {
        var contador = 0;
        for (int x = 0; x < nDoors; x++)
        {
            var ran = random.Next(1, 6);
            exitWaysPos[ran].active = true;
            exitWaysPos[ran].isExit= true;

          
            contador++;
        }

        return contador;
    }

    private int generateAreas(int nAreas, System.Random random)
    {
        var contador = 0;
        for (int x = 0; x < nAreas; x++)
        {
            var ran = random.Next(0, 25);
            hexNodes[ran].active = true;
            hexNodes[ran].SetNodeSprite(spriteAreas);
            contador++;
        }
  //      

        return contador;
    }

    public void CrearCaminos()
    {

        ExitNodesPath(); // creamos los caminos cercanos a las salidas

        AreaNodesPaths(); //creamos los caminos de las areas vacias al mas cercano


        FullfillPaths(0); // con el indice a 0 coge uno random del otro grupo
        FullfillPaths(1); // con el indice a 1 coge el mas cercano
        FullfillPaths(0); // con el indice a 1 coge el mas cercano

        AreaNodesPaths(); //creamos los caminos de las areas vacias al mas cercano

        //  CreatePassages();

    }


    public void FullfillPaths( int randNear =0)
    {
        List<Node> checkExits = new List<Node>(); //salidas del tablero activas
        List<Node> checkNodes = new List<Node>(); //nodos activos 
        List<Node> disconectedNodes = new List<Node>();
        var countConections = 20;  //numero alto no afecta contador de conexiones
        Node loneNode = null;

        //revisamos las salidas y guardamos las que estan activas
        foreach (Node nod in exitWaysPos)
        {
            if (nod.isExit && nod.active)
            {
                checkExits.Add(nod);
            }
        }

        //cogemos todos los nodos activos 
        foreach (Node nod in hexNodes)
        {
            if (  nod.accesToExit && nod.active && !checkExits.Contains(nod))
            {
                checkNodes.Add(nod);
            }
        }


        //recorremos los nodos y buscamos el que tenga menos conexionesque lo usaremos como referencia
        foreach(Node check in checkNodes)
        {
            
            if (check.connectedAreas.Count < countConections)
            {
                loneNode = check;
                countConections = check.connectedAreas.Count;
            }

        }


        System.Random ran = new System.Random();
        foreach (Node point in checkNodes)
        {

            if(point.active && !point.isExit)
            {
                disconectedNodes.Add(point);
            }

        }

        var x = ran.Next(0, disconectedNodes.Count - 1);
       
        if (randNear==1)
        {
            pathList.Add(new Path(loneNode, GetNearestNode(loneNode, loneNode.connectedAreas)));
        }
        else
        {
            pathList.Add(new Path(loneNode, GetFirstNodeToExit(disconectedNodes[x], loneNode.connectedAreas)));
        }

        
        loneNode.connectedAreas = disconectedNodes[x].connectedAreas;
        loneNode.connectedAreas.Add(disconectedNodes[x]);
        disconectedNodes[x].connectedAreas.Add(loneNode);
        loneNode.accesToExit = true;



    }

    public Node GetFirstNodeToExit(Node nodeMain, List<Node> evitar)
    {
    
        Node nearestNode = nodeMain;

        foreach (Node nod in hexNodes)
        {
           

                if (!nod.isExit   && nod.active  && !evitar.Contains(nod) && nod.accesToExit)
                {
                        nearestNode = nod;
                }
            

        }

        return nearestNode;

        /*
         * tengo que coger todas las puertas para tenerlas localzadas
         * evaluar 
         * 
         * */

    }

    public Node GetNearestNode(Node nodeMain , List<Node> excludesNodes = null)
    {
        var dist = 10000f;
        Node nearestNode = null;
        if (excludesNodes == null )excludesNodes = new List<Node>();

        foreach (Node node in hexNodes)
        {
            if (node.active && !node.isExit && node!= nodeMain && !excludesNodes.Contains(node))
            {
                float checkDist = Vector3.Distance(nodeMain.position, node.position);

                if (checkDist < dist)
                {
                    dist = checkDist;
                    nearestNode = node;
                }
            }
        }

        return nearestNode;
    }




    public void ExitNodesPath()
    {

            foreach (Node nod in exitWaysPos)
            {
                if (nod.active && !nod.accesToExit)
                {
                Node nearest = GetNearestNode(nod);
                    pathList.Add( new Path(nod, nearest)) ;

                nod.connectedAreas.Add(nearest);
                nearest.connectedAreas.Add(nod);
                nod.accesToExit = true;
                nearest.accesToExit = true;
            }
            }
    }

    public void AreaNodesPaths()
    {

        List<Node> disconnectedNodes = new List<Node>();

        foreach(Node nod in hexNodes){ 

        if (!nod.isExit && !nod.accesToExit && nod.active )
            {
                disconnectedNodes.Add(nod);
            }
        }

        List<Node> checkedNodes = new List<Node>();
        
        foreach (Node nod in disconnectedNodes)
        {

           
            checkedNodes.Add(nod);

            if (nod.active && !nod.accesToExit)
            {
                
                
                Node nearest = GetNearestNode(nod, checkedNodes);

                if (nearest.accesToExit)
                {
                    pathList.Add(new Path(nod, nearest));
                    nod.connectedAreas = nearest.connectedAreas;
                    nod.connectedAreas.Add(nearest);
                    nearest.connectedAreas.Add(nod);
                    nod.accesToExit = true;
                    continue;
                }
                else {
                    checkedNodes.Add(nearest);

                    Node nearest_neigh = GetNearestNode(nearest, checkedNodes);

                    if (nearest_neigh.accesToExit)
                    {
                        pathList.Add(new Path(nod, nearest));
                        nod.connectedAreas = nearest.connectedAreas;
                        nod.connectedAreas.Add(nearest);
                        nearest.connectedAreas.Add(nod);
                    }
                    else
                    {
                       
    



                    }

 
                }


            }
        }

    }

}



public class Node
    {

        public Vector3 position;
        public bool active = false;
        public int vertexIndex = -1; //necesario para creacion de hexagono mex
        public bool isExit = false;
        public bool accesToExit = false;

        public GameObject go = new GameObject();
        public SpriteRenderer renderer;

        public List<Node> connectedAreas = new List<Node>();

            

                public Node(Vector3 _pos, Transform parent=null,Sprite sprite = null)
                {
                    position = _pos;
                renderer= go.AddComponent<SpriteRenderer>();

                go.transform.position = _pos;
                go.transform.localScale = new Vector3(.5f, .5f, .5f);
                if(parent!=null)go.transform.parent = parent;
                go.name = position.ToString();
               
                 renderer.sprite = sprite;
                


                 }

    public void SetNodeSprite(Sprite sprite)
    {
        renderer.sprite = sprite;
        active = true;
    }

   

    public void Destroy()
    {

        //UnityEngine.GameObject.Destroy(renderer);
        UnityEngine.GameObject.Destroy(go);
    }
    }


public class HexSide
{
    public Node nodeA;
    public Node nodeB;
   public  Node midNode;
    int index;

    public HexSide(Node a, Node b , int i =0) {
        nodeA =a;
        nodeB = b;
        index = i;
        midNode = new Node(Vector3.Lerp(nodeA.position, nodeB.position, 0.5f));
    }

    public bool CompareToSide(HexSide side)
    {
        var result = false;

        if ( (nodeA == side.nodeA && nodeB == side.nodeB  ) ||(nodeA == side.nodeB && nodeB == side.nodeA) )
        { 
            result = true;
        }
        return result;
    }
}


public class Path
{
    Node pointA;
    Node pointB;
    int steps;

    

    public Path(Node nodeA,Node nodeB )
    {
        pointA = nodeA;
        pointB = nodeB;
      //  steps = _steps;




        Debug.DrawLine(pointA.position, pointB.position, Color.cyan, 100, false);
    }



}
