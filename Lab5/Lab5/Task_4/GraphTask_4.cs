using Lab5.Models;
using System.Windows.Controls;

namespace Lab5.Task_4;

public static class GraphTask_4
{
    public static async Task VisualizeMinWay(Canvas canva, Graph graph, Node startNode, Node endNode,
    Action<string> appendToOutput, Action<Node, string> highlightNode,
    Action<Edge, string> highlightEdge, Action<Canvas, Node, string> updateDistance, int delay = 500)
    {
        var distances = new Dictionary<int, int>(); // Хранит расстояния до каждого узла
        var previousNodes = new Dictionary<int, Node>(); // Хранит предыдущие узлы для каждого узла
        var priorityQueue = new PriorityQueue<Node, int>(); // Очередь с приоритетами (узел, расстояние)
        var visited = new HashSet<int>(); // Для отслеживания посещённых узлов

        appendToOutput($"\n\nВыбран алгоритм: Поиск кратчайшего пути.");
        appendToOutput($"Стартовая вершина: {startNode.Id}, Конечная вершина: {endNode.Id}");

        // Инициализация расстояний
        foreach (var node in graph.Nodes)
        {
            distances[node.Id] = int.MaxValue; // Изначально расстояние до всех узлов равно бесконечности
        }
        GraphDrawer.DrawDistance(graph, canva);
        distances[startNode.Id] = 0; // Расстояние до стартового узла равно 0
        updateDistance(canva, startNode, distances[startNode.Id].ToString()); //почему-то не работает

        priorityQueue.Enqueue(startNode, 0); 

        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue.Dequeue(); // Извлекаем узел с минимальным расстоянием

            if (visited.Contains(currentNode.Id))
            {
                continue;
            }

            // Подсвечиваем текущий узел
            appendToOutput($"Посещаем узел {currentNode.Id}.");
            highlightNode(currentNode, "#B41F21"); // Цвет: красный
            await Task.Delay(delay); // Задержка для визуализации

            visited.Add(currentNode.Id); // Помечаем узел как посещённый

            // Закрашиваем узел как завершённый
            appendToOutput($"Посетили узел {currentNode.Id}.");
            highlightNode(currentNode, "#1FB43A"); // Цвет: зеленый
            await Task.Delay(delay); // Задержка для визуализации

            // Если текущий узел - конечный, то путь найден
            if (currentNode.Id == endNode.Id)
            {
                appendToOutput("Путь найден!");
                break;
            }

            // Обрабатываем соседей текущего узла
            var neighbors = graph.Edges
                .Where(edge => edge.StartNode == currentNode && !visited.Contains(edge.EndNode.Id))
                .Select(edge => new { Neighbor = edge.EndNode, Weight = edge.Weight, Edge = edge })
                .Union(graph.Edges
                    .Where(edge => edge.EndNode == currentNode && !visited.Contains(edge.StartNode.Id))
                    .Select(edge => new { Neighbor = edge.StartNode, Weight = edge.Weight, Edge = edge }));

            foreach (var neighbor in neighbors)
            {
                var distanceThroughCurrent = distances[currentNode.Id] + neighbor.Weight;

                if (distanceThroughCurrent < distances[neighbor.Neighbor.Id])
                {
                    distances[neighbor.Neighbor.Id] = distanceThroughCurrent;
                    appendToOutput($"Обновляем расстояние до точки {neighbor.Neighbor.Id}");
                    updateDistance(canva, neighbor.Neighbor, distanceThroughCurrent.ToString());
                    previousNodes[neighbor.Neighbor.Id] = currentNode;

                    // Подсвечиваем ребро
                    highlightEdge(neighbor.Edge, "#B41F21"); // Цвет: красный
                    await Task.Delay(delay); // Задержка для визуализации

                    // Возвращаем цвет ребра в исходное состояние
                    highlightEdge(neighbor.Edge, "#000000"); // Чёрный цвет
                    await Task.Delay(delay); // Задержка для визуализации

                    // Добавляем узел в очередь с новым расстоянием
                    priorityQueue.Enqueue(neighbor.Neighbor, distanceThroughCurrent);
                }
                else
                {
                    appendToOutput($"Текущее растояние {distanceThroughCurrent} до точки {neighbor.Neighbor.Id} > минимального растояния {distances[neighbor.Neighbor.Id]} до этой же точки.");
                }
            }
        }

        // Восстановление пути
        var path = new List<int>();
        var current = endNode;
        while (current != null)
        {
            path.Insert(0, current.Id);
            current = previousNodes.ContainsKey(current.Id) ? previousNodes[current.Id] : null;
        }

        appendToOutput("Обход графа завершён!");
        appendToOutput($"Кратчайший путь: {string.Join(" -> ", path)}");
        appendToOutput($"Вес кратчайшего пути: {distances[endNode.Id]}");
    }
}
