using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public SquareGrid _SquareGrid;
    
    public void GenerateMesh(int[,] map , float squareSize)
    {
        _SquareGrid = new SquareGrid(map, squareSize);
    }

    private void OnDrawGizmos()
    {
        if (_SquareGrid != null)
        {
            for (int x = 0; x < _SquareGrid.Squares.GetLength(0); x++)
            {
                for (int y = 0; y < _SquareGrid.Squares.GetLength(1); y++)
                {
                    Gizmos.color = (_SquareGrid.Squares[x, y].TopLeft.Active) ? Color.black : Color.white;
                    Gizmos.DrawCube(_SquareGrid.Squares[x, y].TopLeft.Position, Vector3.one * .4f);

                    Gizmos.color = (_SquareGrid.Squares[x, y].TopRight.Active) ? Color.black : Color.white;
                    Gizmos.DrawCube(_SquareGrid.Squares[x, y].TopRight.Position, Vector3.one * .4f);

                    Gizmos.color = (_SquareGrid.Squares[x, y].BottomRight.Active) ? Color.black : Color.white;
                    Gizmos.DrawCube(_SquareGrid.Squares[x, y].BottomRight.Position, Vector3.one * .4f);

                    Gizmos.color = (_SquareGrid.Squares[x, y].BottomLeft.Active) ? Color.black : Color.white;
                    Gizmos.DrawCube(_SquareGrid.Squares[x, y].BottomLeft.Position, Vector3.one * .4f);


                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(_SquareGrid.Squares[x, y].CenterTop.Position, Vector3.one * .15f);
                    Gizmos.DrawCube(_SquareGrid.Squares[x, y].CenterRight.Position, Vector3.one * .15f);
                    Gizmos.DrawCube(_SquareGrid.Squares[x, y].CenterBottom.Position, Vector3.one * .15f);
                    Gizmos.DrawCube(_SquareGrid.Squares[x, y].CenterLeft.Position, Vector3.one * .15f);
                }
            }
        }
    }

    public class SquareGrid
    {
        public Square[,] Squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];
            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0,
                        -mapWidth / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            }
            Squares = new Square[nodeCountX - 1, nodeCountY - 1];
            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    Squares[x,y] =  new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }
    }

    public class Square
    {
        public ControlNode TopLeft, TopRight, BottomRight, BottomLeft;
        public Node CenterTop, CenterRight, CenterBottom, CenterLeft;

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;

            CenterTop = topLeft.Right;
            CenterRight = bottomRight.Above;
            CenterBottom = bottomLeft.Right;
            CenterLeft = bottomLeft.Above;
        }
    }

    public class Node
    {
        public Vector3 Position;
        public int VertexIndex = -1;

        public Node(Vector3 pos)
        {
            Position = pos;
        }
    }

    public class ControlNode : Node
    {
        public bool Active;
        public Node Above, Right;

        public ControlNode(Vector3 pos, bool active, float squareSize) : base(pos)
        {
            Active = active;
            Above = new Node(Position + Vector3.forward * squareSize / 2f);
            Right = new Node(Position + Vector3.right * squareSize / 2f);
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
