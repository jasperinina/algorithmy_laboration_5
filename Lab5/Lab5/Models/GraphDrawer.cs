using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab5.Models;

public static class GraphDrawer
{
    public static void DrawGraph(Graph graph, Canvas canvas)
    {
        var lightFontFamily = (FontFamily)Application.Current.Resources["LightFontFamily"];
        canvas.Children.Clear(); // Очищаем Canvas

        // Рисуем рёбра
        foreach (var edge in graph.Edges)
        {
            var line = new Line
            {
                X1 = edge.StartNode.X,
                Y1 = edge.StartNode.Y,
                X2 = edge.EndNode.X,
                Y2 = edge.EndNode.Y,
                Stroke = Brushes.Black, // Начальный цвет
                StrokeThickness = 3,
                Tag = edge // Привязка ребра к линии
            };
            canvas.Children.Add(line);

            // Подпись ребра (вес)
            var label = new TextBlock
            {
                Text = edge.Weight.ToString(),
                Foreground = Brushes.Black,
                FontSize = 14,
                FontFamily = lightFontFamily,
                Tag = edge // Привязка веса к ребру
            };

            // Устанавливаем положение текста
            Canvas.SetLeft(label, (edge.StartNode.X + edge.EndNode.X) / 2 - 10);
            Canvas.SetTop(label, (edge.StartNode.Y + edge.EndNode.Y) / 2 - 10);
            canvas.Children.Add(label);
        }

        // Рисуем узлы
        foreach (var node in graph.Nodes)
        {
            double ellipseDiameter = 50;
            double ellipseRadius = ellipseDiameter / 2;

            var ellipse = new Ellipse
            {
                Width = ellipseDiameter,
                Height = ellipseDiameter,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(node.FillColor))
            };
            Canvas.SetLeft(ellipse, node.X - ellipseRadius);
            Canvas.SetTop(ellipse, node.Y - ellipseRadius);
            canvas.Children.Add(ellipse);

            var label = new TextBlock
            {
                Text = node.Id.ToString(),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                FontFamily = lightFontFamily
            };

            var formattedText = new System.Windows.Media.FormattedText(
                label.Text,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch),
                label.FontSize,
                Brushes.Black,
                new System.Windows.Media.NumberSubstitution(),
                1);

            double textWidth = formattedText.Width;
            double textHeight = formattedText.Height;

            Canvas.SetLeft(label, node.X - textWidth / 2);
            Canvas.SetTop(label, node.Y - textHeight / 2);
            canvas.Children.Add(label);
        }
    }
}