using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab5.Models;

public static class GraphDrawer
{
    public static void DrawGraph(Graph graph, Canvas canvas)
    {
        canvas.Children.Clear(); // Очищаем Canvas

        // Рисуем рёбра
        foreach (var edge in graph.Edges)
        {
            // Линия для ребра
            var line = new Line
            {
                X1 = edge.StartNode.X,
                Y1 = edge.StartNode.Y,
                X2 = edge.EndNode.X,
                Y2 = edge.EndNode.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            canvas.Children.Add(line);

            // Подпись ребра (вес)
            var label = new TextBlock
            {
                Text = edge.Weight.ToString(),
                Foreground = Brushes.Red
            };
            Canvas.SetLeft(label, (edge.StartNode.X + edge.EndNode.X) / 2);
            Canvas.SetTop(label, (edge.StartNode.Y + edge.EndNode.Y) / 2);
            canvas.Children.Add(label);
        }

        // Рисуем узлы
        foreach (var node in graph.Nodes)
        {
            // Круг для узла
            var ellipse = new Ellipse
            {
                Width = 30,
                Height = 30,
                Fill = Brushes.Blue
            };
            Canvas.SetLeft(ellipse, node.X - 15); // Центрируем эллипс
            Canvas.SetTop(ellipse, node.Y - 15); // Центрируем эллипс
            canvas.Children.Add(ellipse);

            // Подпись узла (ID)
            var label = new TextBlock
            {
                Text = node.Id.ToString(),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Canvas.SetLeft(label, node.X - 10);
            Canvas.SetTop(label, node.Y - 10);
            canvas.Children.Add(label);
        }
    }
}