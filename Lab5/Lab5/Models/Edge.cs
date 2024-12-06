namespace Lab5.Models;

using Newtonsoft.Json;

public class Edge
{
    public int StartNodeId { get; set; } // Идентификатор начального узла
    public int EndNodeId { get; set; } // Идентификатор конечного узла
    public int Weight { get; set; } // Вес ребра

    [JsonIgnore]
    public Node StartNode { get; set; }

    [JsonIgnore]
    public Node EndNode { get; set; }
}