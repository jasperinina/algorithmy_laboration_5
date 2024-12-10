using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Lab5.Models;

namespace Lab5.Task_2
{
    public class MaxTrafficCapacity
    {
        private Graph Graph;

        private int NumberOfVertices;
        private int Start;
        private int End;
        private int[,] MatrixOfGraph;
        public int TrafficCapacity;

        private Action<string> AppendToOutput;
        private Action<Node, string> HighlightNode;
        private Action<Edge, string> HighlightEdge;
        private int Delay;

        private Node StartNode;
        private Node EndNode;

        public MaxTrafficCapacity(Graph graph, Node startNode, Node endNode, Action<string> appendToOutput,
            Action<Node, string> highlightNode, Action<Edge, string> highlightEdge, int delay = 3000)
        {
            Graph = graph;
            NumberOfVertices = graph.Nodes.Count;

            StartNode = startNode;
            EndNode = endNode;
            Start = startNode.Id - 1;
            End = endNode.Id - 1;

            MatrixOfGraph = GetMatrixOfGraph();
            AppendToOutput = appendToOutput;
            HighlightNode = highlightNode;
            HighlightEdge = highlightEdge;
            Delay = delay;
        }

        private int[,] GetMatrixOfGraph()
        {
            //int numberOfVertices = Graph.Nodes.Count; // Получаем количество вершин из списка узлов
            int[,] result = new int[NumberOfVertices, NumberOfVertices];
            foreach (var edge in Graph.Edges)
            {
                int startNode = edge.StartNode.Id - 1;  // Получаем идентификатор начальной вершины
                int endNode = edge.EndNode.Id - 1;        // Получаем идентификатор конечной вершины

                if (startNode >= 0 && startNode < NumberOfVertices && endNode >= 0 && endNode < NumberOfVertices)
                {
                    result[startNode, endNode] = edge.Weight; // Устанавливаем вес ребра в матрице
                    result[endNode, startNode] = edge.Weight;
                }

            }

            return result;
        }


        // Возвращает true, если существует путь из start в end 
        private bool bfs(int[,] rGraph, int start, int end, int[] path)
        {
            // Создаем массив с марками о посещении и заполняем его false
            bool[] visited = new bool[NumberOfVertices];
            for (int i = 0; i < NumberOfVertices; ++i)
            {
                visited[i] = false;
            }

            Queue<int> queue = new Queue<int>();
            queue.Enqueue(start);
            visited[start] = true;
            path[start] = -1;

            // Обход графа
            while (queue.Count != 0)
            {
                int u = queue.Dequeue();

                for (int v = 0; v < NumberOfVertices; v++)
                {
                    if (visited[v] == false && rGraph[u, v] > 0)
                    {
                        if (v == end)
                        {
                            path[v] = u;
                            return true;
                        }

                        queue.Enqueue(v);
                        path[v] = u;
                        visited[v] = true;
                    }
                }
            }

            return false;
        }

        //  Алгоритм Форда-Фалкерсона для поиска максимального потока
        public async Task FordFulkerson()
        {
            int u, v; // вспомогательные переменные

            int[,] rGraph = new int[NumberOfVertices, NumberOfVertices];
            for (u = 0; u < NumberOfVertices; u++)
            {
                for (v = 0; v < NumberOfVertices; v++)
                {
                    rGraph[u, v] = MatrixOfGraph[u, v];
                }
            }

            int[] paths = new int[NumberOfVertices];

            int max_flow = 0; // общая сумма потока

            while (bfs(rGraph, Start, End, paths))
            {
                int path_flow = int.MaxValue; // переменная для минимального потока в конкретном пути
                string outputPath = "Найден путь: ";

                // Говнокод для покраски пути
                for (int i = 0; i < Graph.Nodes.Count; i++) // покрас узлов
                {
                    if (paths.Contains(Graph.Nodes[i].Id - 1) || Graph.Nodes[i].Id - 1 == End)
                    {
                        HighlightNode(Graph.Nodes[i], "#B41F21");
                    }
                }
                for (int i = 0; i < paths.Length - 1; i++) // покрас путей
                {
                    foreach (var e in Graph.Edges)
                    {
                        if (e.StartNode.Id - 1 == paths[i] && e.EndNode.Id - 1 == paths[i + 1])
                        {
                            HighlightEdge(e, "#B41F21");
                        }
                        if (e.StartNode.Id - 1 == paths[paths.Length - 1] && e.EndNode.Id - 1 == End)
                        {
                            HighlightEdge(e, "#B41F21");
                        }
                    }
                }

                outputPath += ((End + 1) + " <- ");

                for (v = End; v != Start; v = paths[v])
                {
                    u = paths[v];
                    path_flow = Math.Min(path_flow, rGraph[u, v]);

                    // Выводим путь
                    outputPath += ((u + 1) + " <- ");
                }

                outputPath = outputPath[0..(outputPath.Length - 3)];
                AppendToOutput(outputPath);

                await Task.Delay(Delay);

                AppendToOutput($"Минимальный поток по этому пути: {path_flow}");

                for (v = End; v != Start; v = paths[v])
                {
                    u = paths[v];
                    rGraph[u, v] -= path_flow;
                    rGraph[v, u] += path_flow;
                }

                AppendToOutput($"Добавляем поток пути в общую сумму: {max_flow} + {path_flow} = {max_flow + path_flow}");
                max_flow += path_flow;
                AppendToOutput($"Теперь максимальный поток = {max_flow}" + "\n");

                // Покрас узлов в обычный цвет
                for (int i = 0; i < Graph.Nodes.Count; i++)
                {
                    if (paths.Contains(Graph.Nodes[i].Id - 1) || Graph.Nodes[i].Id - 1 == End)
                    {
                        HighlightNode(Graph.Nodes[i], "#1F77B4");
                    }
                }

                await Task.Delay(Delay);
            }

            AppendToOutput($"Максимальный поток в сети: {max_flow}");
            TrafficCapacity = max_flow;
        }
    }
}
