using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;
    public MeshFilter walls;

    List<Vector3> vertices;
    List<int> triagles;

    Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();

    public void GenerateMesh(int[,] map, float squareSize) {

        triangleDictionary.Clear();
        outlines.Clear();
        checkedVertices.Clear();

        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triagles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
                TriangulateSquare(squareGrid.squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triagles.ToArray();
        mesh.RecalculateNormals();

        CreateWallMesh();
    }

    private void CreateWallMesh() {

        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 5;

        foreach (List<int> outline in outlines) {
            //Count-1을 하는 이유는 벽 정점을 설정할때, i + 1을 진행하기 때문에
            for (int i = 0; i < outline.Count -1; i++) {
                int startIndex = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]); //left
                wallVertices.Add(vertices[outline[i+1]]); //right
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); //bottom left
                wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight); //bottom right

                //wallTriangles의 인덱스는 위 벌텍스를 기준으로 하기때문에 4개를 가지고 2개의 삼각면을 만드는것,
                //벌텍스가 4개추가되기 때문에 다음 트라이앵글의 스타트 인덱스는 4부터 시작하여 겹치지 않을것임.
                wallTriangles.Add(startIndex + 0); //left 
                wallTriangles.Add(startIndex + 2); //bottom left
                wallTriangles.Add(startIndex + 3); //bottom right

                wallTriangles.Add(startIndex + 3); //bottom right
                wallTriangles.Add(startIndex + 1); //right
                wallTriangles.Add(startIndex + 0); //left
            }
        }

        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        walls.mesh = wallMesh;
        wallMesh.RecalculateNormals();
    }

    void TriangulateSquare(Square square) {
        //정정포인트는 시계방향으로 나열됨, 나중에 
        //16개의 경우에 대해서;
        switch (square.configuration) {
            case 0:
                break;
            //1 points : 정점(노드포인트)가 한개일때의 경우의 수
            case 1:
                MeshFromPoint(square.centreLeft, square.centreBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoint(square.bottomRight, square.centreBottom, square.centreRight);
                break;
            case 4:
                MeshFromPoint(square.topRight, square.centreRight, square.centreTop);
                break;
            case 8:
                MeshFromPoint(square.topLeft, square.centreTop, square.centreLeft);
                break;

            //2 points : 정점(노드포인트)가 두개일때의 경우의 수
            case 3:
                MeshFromPoint(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 6:
                MeshFromPoint(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
                break;
            case 9:
                MeshFromPoint(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoint(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
                break;
            case 5:
                MeshFromPoint(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 10:
                MeshFromPoint(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft); 
                break;

            //3 points : 정점이 3개일때의 경우의 수
            case 7:
                MeshFromPoint(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
                break;
            case 11:
                MeshFromPoint(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoint(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoint(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                break;

            //4 points : 정점이 4개일때
            case 15:
                MeshFromPoint(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                //정점이 4개인 경우는 면이 꽉차있는 경우인데 이 경우는 정점을 검사해줄 필요가 없다. 왜냐하면 외곽선이 될수없는 정점들이라서
                //그래서 미리 checkedVertices 에 값을 넣어줌으로서 최적화를 시켜준다
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;
        }
    }

    void MeshFromPoint(params Node[] points) {
        AssignVertices(points);

        //첫번째 정점에서부터, 정점의 개수에 따라 삼각메쉬를 배치해줍니다.
        if(points.Length >= 3)
            CreateTriangles(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangles(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangles(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangles(points[0], points[4], points[5]);
    }

    private void AssignVertices(Node[] points) {
        for (int i = 0; i < points.Length; i++) {
            if(points[i].vertexIndex == -1) {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }
    
    //삼각형을 만든다는건 알겠음.
    void CreateTriangles(Node a, Node b, Node c) {
        //정점 저장해 삼각형 정의, 맵상의 모든 정점을 삼각점으로 순서대로 저장
        triagles.Add(a.vertexIndex);
        triagles.Add(b.vertexIndex);
        triagles.Add(c.vertexIndex);

        //그리고 설정해둔 삼각형 구조체를 통해 삼각형을 정의해주고 
        //딕셔너리에 저장해준다.
        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }


    //트라이앵글의 3가지 정점을 트라이앵글에 딕셔너리 자료구조로 저장한다.
    void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle) {

        //(벌텍스가 겹쳐있는 경우) 벌텍스에 대해 붙어있는 삼각형이 추가됨
        if (triangleDictionary.ContainsKey(vertexIndexKey)) {
            triangleDictionary[vertexIndexKey].Add(triangle);
        }
        //최초 들어왔을때, 트라이앵글 딕셔너리가 생성되지 않으므로 이와같이 triangleList에다가 삼각형을 넣어주고,
        //딕셔너리에도 넣어준다.
        else {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triangleDictionary.Add(vertexIndexKey, triangleList);
        }
    }

    void CalculateMeshOutlines() {
        for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
            //검사된 정점이 아니라면 조건문 실행
            if (!checkedVertices.Contains(vertexIndex)) {
                //현재 정점과 연결된 아웃라인 정점을 가져와 변수에 저장
                int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
                if(newOutlineVertex != -1) {  // 연결된 외곽선 정점이 있을경우
                    checkedVertices.Add(vertexIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vertexIndex);
                    outlines.Add(newOutline);

                    //아래 함수에서 newOutline에 대해 vertexIndex와 이어질 newOutlineVertex 을 설정해줌 
                    FollowOutline(newOutlineVertex, outlines.Count - 1); //아직 생성되지 않은 아웃라인 정점을 가져오기 위해서
                    outlines[outlines.Count - 1].Add(vertexIndex); 
                    //이부분 이해가 잘안가네;; 아마도 선이 원을돌며 이어지기 때문에 마지막으로 끝나는 지점(즉, 스타트 인덱스)을 다시 추가해주는 듯하다
                }
            }
        }
    }

    //재귀함수로 이어지는 아웃라인 정점과 이어진 정점을 계속 찾아와 검사합니다.
    private void FollowOutline(int vertexIndex, int outlineIndex) {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);

        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if(nextVertexIndex != -1) {
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }

    //특정 정점에 대해서 이웃한 외곽선 정점을 가져온다.
    int GetConnectedOutlineVertex(int vertexIndex) {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++) {
            Triangle triangle = trianglesContainingVertex[i];
            
            for (int j = 0; j < 3; j++) {
                int vertexB = triangle[j];

                //vertexB가 VertexIndex와 같을때는 검사하지 않는다, 그리고 이미 체크된 정점의 경우에도 패스한다.
                if(vertexB != vertexIndex && !checkedVertices.Contains(vertexB)) {
                    if (IsOutlineEdge(vertexIndex, vertexB)) {
                        return vertexB;
                    }
                }
            }
        }

        return -1;
    }

    //두개의 정점이 딱 한개의 삼각형만 공유하는지 판단합니다.
    bool IsOutlineEdge(int vertexA, int vertexB) {
        List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
        int sharedTriangleCount = 0;

        for (int i = 0; i < trianglesContainingVertexA.Count; i++) {
            if (trianglesContainingVertexA[i].Contains(vertexB)) {
                sharedTriangleCount++;
                if (sharedTriangleCount > 1) {
                    break;
                }
            }
        }
        return sharedTriangleCount == 1;
    }

    //삼각메쉬에 대해 정의를 한다
    struct Triangle {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;

        //정점에 대한 인덱스를 저장하기 위한 변수
        int[] vertices;

        public Triangle (int a, int b, int c) {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        //인덱서 : 인덱스를 통해 데이터에 접근하도록 해줌
        public int this[int i] {
            get {
                return vertices[i];
            }
        }

        //
        public bool Contains(int vertexIndex) {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }

    public class SquareGrid {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize) {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++) {
                for (int y = 0; y < nodeCountY; y++) {
                    // + squareSize / 2 를 추가로 더해준 이유는 스퀘어 메쉬의 가운데 지점의 위치를 알기 위해서
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y * squareSize + squareSize / 2);
                    //벽(1)과 빈공간(0)을 기준으로 controlNode를 활성화시켜줍니다. 여기선 벽일경우 활성화
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }

            squares = new Square[nodeCountX - 1, nodeCountY -1];

            //4개의 노드(ControlNode)를 지닌 사각형 모양을 저장
            for (int x = 0; x < nodeCountX - 1; x++) {
                for (int y = 0; y < nodeCountY - 1; y++) {
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x+1, y], controlNodes[x, y]);
                }
            }
        }
    }

    //
    public class Square {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centreTop, centreRight, centreBottom, centreLeft;
        public int configuration;

        public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) {
            topLeft = _topLeft;
            topRight = _topRight;
            bottomRight = _bottomRight;
            bottomLeft = _bottomLeft;

            centreTop = topLeft.right;
            centreRight = bottomRight.above;
            centreBottom = bottomLeft.right;
            centreLeft = bottomLeft.above;

            //configuration은 구성을 의미하는데, 현재 스퀘어가 어떤 구성을 지니는지(각 Node들이 어디가 활성화 됐는지) 확인하기 위해
            //여기선 각 상태를 bytes 로 변환(?)하여 int 로 저장한다. 
            if (topLeft.active)
                configuration += 8; // 1000 = 8
            if (topRight.active)
                configuration += 4; // 0100 = 4
            if (bottomRight.active)
                configuration += 2; // 0010 = 2
            if (bottomLeft.active)
                configuration += 1; // 0001 = 1
        }
    }

    // 노트의 위치와 인덱스를 가짐
    public class Node {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _position) {
            position = _position;
        }
    }

    //특정 지점의 위와 오른쪽에 새로운 노드를 형성
    public class ControlNode : Node {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
            active = _active;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}
