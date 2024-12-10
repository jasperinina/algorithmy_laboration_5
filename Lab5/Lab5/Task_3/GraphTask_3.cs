using Lab5.Models;

namespace Lab5.Task_3;

public static class GraphTask_3
{
    public static async Task VisualizePrim(Graph graph, Node startNode,
    Action<string> appendToOutput, Action<Node, string> highlightNode,
    Action<Edge, string> highlightEdge, int delay = 500)
    {
        var visited = new HashSet<int>(); // Для отслеживания посещённых узлов
        var treeEdges = new List<Edge>(); // Для хранения рёбер дерева
        var priorityQueue = new PriorityQueue<(Node, Edge), int>(); // Очередь с приоритетами (узел, ребро, вес)

        appendToOutput($"\n\nВыбран алгоритм: Построение минимального остовного дерева.");
        // Инициализация: начинаем с стартового узла
        visited.Add(startNode.Id);
        appendToOutput($"Стартовая вершина: {startNode.Id}");

        // Добавляем все рёбра, исходящие из стартового узла, в очередь с приоритетами
        foreach (var edge in graph.Edges.Where(e => e.StartNode == startNode || e.EndNode == startNode))
        {
            var neighbor = edge.StartNode == startNode ? edge.EndNode : edge.StartNode;
            priorityQueue.Enqueue((neighbor, edge), edge.Weight);
            highlightEdge(edge, "#B41F21"); // Цвет: красный
            appendToOutput($"Отмечаем возможное минимальное ребро {edge.StartNode.Id} - {edge.EndNode.Id} (вес: {edge.Weight})");
            await Task.Delay(delay); // Задержка для визуализации
        }

        // Закрашиваем узел как завершённый
        appendToOutput($"Все рёбра узла {startNode.Id} отмечены.");
        highlightNode(startNode, "#1FB43A"); // Цвет: зеленый
        await Task.Delay(delay); // Задержка для визуализации

        while (priorityQueue.Count > 0)
        {
            var (currentNode, currentEdge) = priorityQueue.Dequeue(); // Извлекаем ребро с минимальным весом

            if (visited.Contains(currentNode.Id))
            {
                continue; // Если узел уже посещён, пропускаем
            }

            // Подсвечиваем текущее ребро
            appendToOutput($"Добавляем минимальное из рёбер {currentEdge.StartNode.Id} - {currentEdge.EndNode.Id} (вес: {currentEdge.Weight}) в остовное дерево.");
            highlightEdge(currentEdge, "#1FB43A"); // Цвет: зеленый
            await Task.Delay(delay); // Задержка для визуализации

            // Добавляем ребро в дерево
            treeEdges.Add(currentEdge);

            // Подсвечиваем текущий узел
            appendToOutput($"Переходим по этому ребру к узлу {currentNode.Id}.");
            highlightNode(currentNode, "#B41F21"); // Цвет: красный
            await Task.Delay(delay); // Задержка для визуализации

            // Помечаем узел как посещённый
            visited.Add(currentNode.Id);

            // Добавляем все рёбра, исходящие из текущего узла, в очередь с приоритетами
            foreach (var edge in graph.Edges.Where(e => (e.StartNode == currentNode || e.EndNode == currentNode) && !visited.Contains(e.StartNode == currentNode ? e.EndNode.Id : e.StartNode.Id)))
            {
                var neighbor = edge.StartNode == currentNode ? edge.EndNode : edge.StartNode;
                priorityQueue.Enqueue((neighbor, edge), edge.Weight);
                highlightEdge(edge, "#B41F21"); // Цвет: красный
                appendToOutput($"Отмечаем возможное минимальное ребро {edge.StartNode.Id} - {edge.EndNode.Id} (вес: {edge.Weight})");
                await Task.Delay(delay); // Задержка для визуализации
            }

            // Закрашиваем узел как завершённый
            appendToOutput($"Все рёбра узла {currentNode.Id} отмечены.");
            highlightNode(currentNode, "#1FB43A"); // Цвет: зеленый
            await Task.Delay(delay); // Задержка для визуализации
        }
        appendToOutput("Минимальное остовное дерево построено!");
    }
}
