using System.IO;
using Newtonsoft.Json;

namespace Lab5.Models.FileIO;

public class FileService
{
    public static void SaveGraphToFile(Graph graph, string filePath)
    {
        var json = JsonConvert.SerializeObject(graph);
        File.WriteAllText(filePath, json);
    }

    public static Graph LoadGraphFromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Graph>(json);
    }
    
    // Метод для загрузки графа из CSV (матрицы смежности)
    public static Graph LoadGraphFromCsv(string filePath)
    {
        var graph = new Graph();
        var lines = File.ReadAllLines(filePath);

        int nodeCount = lines.Length;
        double centerX = 400; // Центр канваса
        double centerY = 300; // Центр канваса
        double radius = 200; // Радиус окружности

        // Создаём узлы
        for (int i = 0; i < nodeCount; i++)
        {
            double angle = 2 * Math.PI * i / nodeCount; // Угол для равномерного распределения
            var node = new Node
            {
                Id = i + 1,
                X = centerX + radius * Math.Cos(angle),
                Y = centerY + radius * Math.Sin(angle)
            };
            graph.AddNode(node);
        }

        // Создаём рёбра на основе матрицы смежности
        for (int i = 0; i < nodeCount; i++)
        {
            var weights = lines[i].Split(',').Select(int.Parse).ToList();
            for (int j = 0; j < weights.Count; j++)
            {
                if (weights[j] != 0 && i != j) // Если есть ребро и это не самопетля
                {
                    var startNode = graph.Nodes[i];
                    var endNode = graph.Nodes[j];
                    graph.AddEdge(startNode, endNode);
                }
            }
        }

        return graph;
    }
}
