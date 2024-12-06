namespace Lab5.Models;

public class Node
{
    public int Id { get; set; } // Идентификатор узла
    public double X { get; set; } // Позиция по оси X
    public double Y { get; set; } // Позиция по оси Y
    
    public string FillColor { get; set; } = "#1F77B4"; // Исходный цвет
}