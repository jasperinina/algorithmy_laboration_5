namespace Lab5.Models;

public class Edge
{
    public Node StartNode { get; set; } // Начальный узел
    public Node EndNode { get; set; } // Конечный узел
    public int Weight { get; set; } // Вес ребра (по желанию)
}