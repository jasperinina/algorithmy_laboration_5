using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Lab5.Models;

public class FileService
{
    public static void SaveGraphToFile(Graph graph, string filePath)
    {
        // Обновляем идентификаторы узлов в рёбрах
        foreach (var edge in graph.Edges)
        {
            edge.StartNodeId = edge.StartNode.Id;
            edge.EndNodeId = edge.EndNode.Id;
        }

        var json = JsonConvert.SerializeObject(graph, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public static Graph LoadGraphFromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var graph = JsonConvert.DeserializeObject<Graph>(json);

        // Восстанавливаем ссылки на объекты Node
        foreach (var edge in graph.Edges)
        {
            edge.StartNode = graph.Nodes.FirstOrDefault(n => n.Id == edge.StartNodeId);
            edge.EndNode = graph.Nodes.FirstOrDefault(n => n.Id == edge.EndNodeId);
        }

        return graph;
    }
    
    // Метод для загрузки графа из CSV (матрицы смежности)
    public static Graph LoadGraphFromCsv(string filePath)
    {
        var graph = new Graph();
        var lines = File.ReadAllLines(filePath);

        int nodeCount = lines.Length;

        // Извлекаем координаты узлов и создаём узлы
        for (int i = 0; i < nodeCount; i++)
        {
            var columns = lines[i].Split(';');

            // Получаем координаты узла
            double x = double.Parse(columns[columns.Length - 2].Replace(',', '.'), CultureInfo.InvariantCulture);  // Координата X
            double y = double.Parse(columns[columns.Length - 1].Replace(',', '.'), CultureInfo.InvariantCulture);  // Координата Y

            var node = new Node
            {
                Id = i + 1,  // Узлы нумеруются с 1
                X = x,
                Y = y
            };

            graph.AddNode(node);
        }

        // Загружаем рёбра и их веса из матрицы смежности
        for (int i = 0; i < nodeCount; i++)
        {
            var columns = lines[i].Split(';');

            for (int j = 0; j < nodeCount; j++)
            {
                // Пропускаем последние два столбца, так как это координаты
                if (int.TryParse(columns[j], out int weight) && weight > 0 && i != j) // Проверка на положительный вес
                {
                    var startNode = graph.Nodes[i];
                    var endNode = graph.Nodes[j];

                    // Используем перегруженный метод AddEdge с весом
                    graph.AddEdge(startNode, endNode, weight);
                }
            }
        }

        return graph;
    }
    
    public static string ConvertGraphToCsv(Graph graph)
    {
        int nodeCount = graph.Nodes.Count;

        // Матрица смежности (2D массив для весов рёбер)
        double[,] adjacencyMatrix = new double[nodeCount, nodeCount];

        // Заполняем матрицу смежности
        foreach (var edge in graph.Edges)
        {
            int startIndex = edge.StartNode.Id - 1; // Индекс узла (ID начинается с 1, а индексация в массиве с 0)
            int endIndex = edge.EndNode.Id - 1;

            adjacencyMatrix[startIndex, endIndex] = edge.Weight; // Присваиваем вес ребра
            adjacencyMatrix[endIndex, startIndex] = edge.Weight; // Так как граф неориентированный
        }

        // Строим CSV-строку
        var csvBuilder = new StringBuilder();

        for (int i = 0; i < nodeCount; i++)
        {
            // Добавляем веса рёбер для текущего узла
            for (int j = 0; j < nodeCount; j++)
            {
                csvBuilder.Append(adjacencyMatrix[i, j]);
                if (j < nodeCount - 1)
                    csvBuilder.Append(';');
            }

            // Добавляем координаты текущего узла (x, y)
            var node = graph.Nodes[i];
            csvBuilder.Append($";{node.X};{node.Y}");
        
            csvBuilder.AppendLine(); // Переход на новую строку
        }

        return csvBuilder.ToString();
    }

    public static void SaveGraphToCsv(Graph graph, string filePath)
    {
        string csvContent = ConvertGraphToCsv(graph);
        File.WriteAllText(filePath, csvContent);
    }
}
