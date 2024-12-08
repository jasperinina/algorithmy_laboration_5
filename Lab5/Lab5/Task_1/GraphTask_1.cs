using Lab5.Models;

namespace Lab5.Task_1;

public static class GraphTask_1
{
    // Метод для выполнения обхода графа в ширину
    public static async Task VisualizeWeightedBreadthFirstSearch(Graph graph, Node startNode, Action<string> appendToOutput, 
        Action<Node, string> highlightNode, Action<Edge, string> highlightEdge, int delay = 500)
    {
        var visited = new HashSet<int>();
        var queue = new PriorityQueue<Node, int>(); // Очередь с приоритетами (узел, вес)
        var traversalOrder = new List<int>(); // Список для хранения порядка обхода

        queue.Enqueue(startNode, 0);

        appendToOutput($"\n\nВыбран алгоритм: Обход графа в ширину.");
        appendToOutput($"Стартовая вершина: {startNode.Id}");

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue(); // Извлекаем узел с минимальным весом

            if (visited.Contains(currentNode.Id))
            {
                appendToOutput($"Узел {currentNode.Id} уже посещён. Пропускаем.");
                continue;
            }

            // Подсвечиваем текущий узел
            appendToOutput($"Посещаем узел {currentNode.Id}.");
            highlightNode(currentNode, "#B41F21"); // Цвет: красный
            await Task.Delay(delay); // Задержка для визуализации

            visited.Add(currentNode.Id); // Помечаем узел как посещённый
            traversalOrder.Add(currentNode.Id); // Добавляем узел в результат обхода

            // Закрашиваем узел как завершённый
            appendToOutput($"Посетили узел {currentNode.Id}.");
            highlightNode(currentNode, "#1FB43A"); // Цвет: зеленый
            await Task.Delay(delay); // Задержка для визуализации

            // Обрабатываем соседей текущего узла
            var neighbors = graph.Edges
                .Where(edge => edge.StartNode == currentNode && !visited.Contains(edge.EndNode.Id))
                .Select(edge => new { Neighbor = edge.EndNode, Weight = edge.Weight, Edge = edge })
                .Union(graph.Edges
                    .Where(edge => edge.EndNode == currentNode && !visited.Contains(edge.StartNode.Id))
                    .Select(edge => new { Neighbor = edge.StartNode, Weight = edge.Weight, Edge = edge }));

            foreach (var neighbor in neighbors)
            {
                // Подсвечиваем ребро
                appendToOutput($"Переход по ребру к узлу {neighbor.Neighbor.Id} (вес ребра: {neighbor.Weight}).");
                highlightEdge(neighbor.Edge, "#B41F21"); // Цвет: красный
                await Task.Delay(delay); // Задержка для визуализации

                // Добавляем узел в очередь
                queue.Enqueue(neighbor.Neighbor, neighbor.Weight);

                // Возвращаем цвет ребра в исходное состояние
                highlightEdge(neighbor.Edge, "#000000"); // Чёрный цвет
                await Task.Delay(delay); // Задержка для визуализации
            }
        }

        appendToOutput("Обход графа завершён!");
        appendToOutput($"Результат обхода: {string.Join(" -> ", traversalOrder)}");
    }

    // Метод для выполнения обхода графа в глубину
    public static async Task VisualizeWeightedDepthFirstSearch(Graph graph, Node startNode,
        Action<string> appendToOutput, Action<Node, string> highlightNode,
        Action<Edge, string> highlightEdge, int delay = 500)
    {
        var visited = new HashSet<int>(); // Для отслеживания посещённых узлов
        var traversalOrder = new List<int>(); // Для хранения порядка обхода
        
        appendToOutput($"\n\nВыбран алгоритм: Обход графа в глубину.");
        appendToOutput($"Стартовая вершина: {startNode.Id}");

        // Запускаем рекурсивный DFS
        await DepthFirstSearchRecursive(graph, startNode, visited, traversalOrder, appendToOutput, highlightNode, highlightEdge, delay);

        // Выводим итоговый результат
        appendToOutput("Обход графа завершён!");
        appendToOutput($"Результат обхода: {string.Join(" -> ", traversalOrder)}");
    }

    private static async Task DepthFirstSearchRecursive(Graph graph, Node currentNode, HashSet<int> visited,
        List<int> traversalOrder, Action<string> appendToOutput,
        Action<Node, string> highlightNode, Action<Edge, string> highlightEdge, int delay)
    {
        // Если узел уже посещён, выходим
        if (visited.Contains(currentNode.Id))
        {
            appendToOutput($"Узел {currentNode.Id} уже посещён. Пропускаем.");
            return;
        }

        // Отмечаем текущий узел как посещённый
        visited.Add(currentNode.Id);
        traversalOrder.Add(currentNode.Id);

        // Подсвечиваем текущий узел
        appendToOutput($"Посещаем узел {currentNode.Id}.");
        highlightNode(currentNode, "#B41F21"); // Красный цвет
        await Task.Delay(delay);

        // Закрашиваем узел как завершённый
        highlightNode(currentNode, "#1FB43A"); // Зеленый цвет
        await Task.Delay(delay);

        // Получаем соседей, отсортированных по весу рёбер
        var neighbors = graph.Edges
            .Where(edge => edge.StartNode == currentNode && !visited.Contains(edge.EndNode.Id))
            .Select(edge => new { Neighbor = edge.EndNode, Weight = edge.Weight, Edge = edge })
            .Union(graph.Edges
                .Where(edge => edge.EndNode == currentNode && !visited.Contains(edge.StartNode.Id))
                .Select(edge => new { Neighbor = edge.StartNode, Weight = edge.Weight, Edge = edge }))
            .OrderBy(n => n.Weight); // Сортировка соседей по весу рёбер

        // Рекурсивно обходим всех соседей
        foreach (var neighbor in neighbors)
        {
            appendToOutput($"Переход по ребру к узлу {neighbor.Neighbor.Id} (вес ребра: {neighbor.Weight}).");

            // Подсвечиваем ребро
            highlightEdge(neighbor.Edge, "#B41F21"); // Красный цвет
            await Task.Delay(delay);

            await DepthFirstSearchRecursive(graph, neighbor.Neighbor, visited, traversalOrder, appendToOutput,
                highlightNode, highlightEdge, delay);

            // Возвращаем ребро к исходному цвету
            highlightEdge(neighbor.Edge, "#000000"); // Чёрный цвет (или стандартный)
        }
    }
}