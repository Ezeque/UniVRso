using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public LayerMask obstacleLayer; 
    public float gridSize = 1f;     

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start);
        Node endNode = new Node(end);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            openList.Sort((a, b) => a.F.CompareTo(b.F));
            Node currentNode = openList[0];

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.Position == endNode.Position)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedList.Contains(neighbor) || IsObstacle(neighbor.Position))
                    continue;

                float newCost = currentNode.G + Vector3.Distance(currentNode.Position, neighbor.Position);
                if (newCost < neighbor.G || !openList.Contains(neighbor))
                {
                    neighbor.G = newCost;
                    neighbor.H = Vector3.Distance(neighbor.Position, endNode.Position);
                    neighbor.Parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return new List<Vector3>(); 
    }

    private bool IsObstacle(Vector3 position)
    {
        return Physics.CheckSphere(position, gridSize / 2, obstacleLayer);
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        
        foreach (Vector3 direction in directions)
        {
            Vector3 neighborPosition = node.Position + direction * gridSize;
            neighbors.Add(new Node(neighborPosition));
        }

        return neighbors;
    }

    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }
}

public class Node
{
    public Vector3 Position;
    public float G; 
    public float H; 
    public float F => G + H; 
    public Node Parent;

    public Node(Vector3 position)
    {
        Position = position;
        G = 0;
        H = 0;
        Parent = null;
    }
}
