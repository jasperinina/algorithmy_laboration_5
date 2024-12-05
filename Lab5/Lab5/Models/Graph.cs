namespace Lab5.Models;

public class Graph
{
    public List<Node> Nodes { get; set; } = new List<Node>();
    public List<Edge> Edges { get; set; } = new List<Edge>();

    // Методы для добавления и удаления узлов
    public void AddNode(Node node)
    {
        Nodes.Add(node);
    }

    public void RemoveNode(Node node)
    {
        Nodes.Remove(node);
        Edges.RemoveAll(e => e.StartNode == node || e.EndNode == node); // Удаляем рёбра, связанные с этим узлом
    }

    // Методы для добавления и удаления рёбер
    public void AddEdge(Node start, Node end)
    {
        Edges.Add(new Edge { StartNode = start, EndNode = end });
    }

    public void RemoveEdge(Edge edge)
    {
        Edges.Remove(edge);
    }
}