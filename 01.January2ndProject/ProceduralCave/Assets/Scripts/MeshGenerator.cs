using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public SquareGrid squareGrid;
    List<Vector3> vertices;
    List<int> triagles;

    public void GenerateMesh(int[,] map, float squareSize) {
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
    }

    void TriangulateSquare(Square square) {
        //정정포인트는 시계방향으로 나열됨, 나중에 
        //16개의 경우에 대해서;
        switch (square.configuration) {
            case 0:
                break;
            //1 points : 정점(노드포인트)가 한개일때의 경우의 수
            case 1:
                MeshFromPoint(square.centreBottom, square.bottomLeft, square.centreLeft);
                break;
            case 2:
                MeshFromPoint(square.centreRight, square.bottomRight, square.centreBottom);
                break;
            case 4:
                MeshFromPoint(square.centreTop, square.topRight, square.centreRight);
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

            //4 points : 정점이 3개일때의 경우의 수
            case 15:
                MeshFromPoint(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
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
        triagles.Add(a.vertexIndex);
        triagles.Add(b.vertexIndex);
        triagles.Add(c.vertexIndex);
    }

    //private void OnDrawGizmos() {
    //    if (squareGrid != null) {
    //        for (int x = 0; x <  squareGrid.squares.GetLength(0); x++) {
    //            for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
    //                Gizmos.color = (squareGrid.squares[x, y].topLeft.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.position, Vector3.one * 0.4f);

    //                Gizmos.color = (squareGrid.squares[x, y].topRight.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].topRight.position, Vector3.one * 0.4f);

    //                Gizmos.color = (squareGrid.squares[x, y].bottomRight.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].bottomRight.position, Vector3.one * 0.4f);

    //                Gizmos.color = (squareGrid.squares[x, y].bottomLeft.active) ? Color.black : Color.white;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeft.position, Vector3.one * 0.4f);

    //                Gizmos.color = Color.grey;
    //                Gizmos.DrawCube(squareGrid.squares[x, y].centreTop.position, Vector3.one * 0.15f);
    //                Gizmos.DrawCube(squareGrid.squares[x, y].centreRight.position, Vector3.one * 0.15f);
    //                Gizmos.DrawCube(squareGrid.squares[x, y].centreBottom.position, Vector3.one * 0.15f);
    //                Gizmos.DrawCube(squareGrid.squares[x, y].centreLeft.position, Vector3.one * 0.15f);
    //            }
    //        }
    //    }
    //}

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
