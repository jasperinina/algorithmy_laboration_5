using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Lab5.Models;
using Lab5.Task_1;
using Microsoft.Win32;

namespace Lab5;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        graph = new Graph();
    }

    private Graph graph;
    private string selectedAlgorithm; // Хранит выбранный алгоритм
    private bool isDraggingNode = false;
    private Node draggedNode;
    private bool isMoveModeActive = false;
    private bool isAddNodeModeActive = false; // Флаг режима добавления узла
    private bool isRemoveNodeModeActive = false; // Флаг режима удаления узлов
    private bool isAddEdgeModeActive = false; // Флаг режима добавления рёбер
    private Node firstNodeForEdge; // Первый выбранный узел для рёбра
    private bool isRemoveEdgeModeActive = false; // Флаг режима удаления рёбер
    private bool isChangeEdgeWeightModeActive = false; // Флаг режима изменения веса рёбра

    private bool isSelectStartNodeActive = false; // Флаг выбора стартовой вершины
    private Node selectedStartNode; // Выбранная стартовая вершина

    private void TaskComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TaskComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            selectedAlgorithm = selectedItem.Content.ToString();
        }
    }

    // Обработчик для кнопки "Запустить алгоритм"
    private async void StartAlgorithmButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(selectedAlgorithm))
        {
            OutputTextBox.Text = "Пожалуйста, выберите алгоритм из списка!";
            return;
        }

        switch (selectedAlgorithm)
        {
            case "Обход графа в ширину":
                if (selectedStartNode == null)
                {
                    EnableStartNodeSelectionMode();
                }
                else
                {
                    await GraphTask_1.VisualizeWeightedBreadthFirstSearch(graph, selectedStartNode, AppendToOutput, HighlightNode, HighlightEdge, 500);
                    ResetModes();
                    selectedStartNode = null;
                }
                break;

            case "Обход графа в глубину":
                if (selectedStartNode == null)
                {
                    EnableStartNodeSelectionMode();
                }
                else
                {
                    await GraphTask_1.VisualizeWeightedDepthFirstSearch(graph, selectedStartNode, AppendToOutput, HighlightNode, HighlightEdge,500);
                    ResetModes();
                    selectedStartNode = null;
                }
                break;

            default:
                OutputTextBox.Text = "Неизвестный алгоритм. Пожалуйста, выберите корректный алгоритм.";
                break;
        }
    }

    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (isSelectStartNodeActive)
        {
            Canvas_MouseDown_SelectStartNode(sender, e); // Режим выбора стартовой вершины
        }
        else if (isMoveModeActive)
        {
            Canvas_MouseDown_MoveNode(sender, e); // Режим перемещения узлов
        }
        else if (isAddNodeModeActive)
        {
            Canvas_MouseDown_AddNode(sender, e); // Режим добавления узлов
        }
        else if (isRemoveNodeModeActive)
        {
            Canvas_MouseDown_RemoveNode(sender, e); // Режим удаления узлов
        }
        else if (isAddEdgeModeActive)
        {
            Canvas_MouseDown_AddEdge(sender, e); // Режим добавления рёбер
        }
        else if (isRemoveEdgeModeActive)
        {
            Canvas_MouseDown_RemoveEdge(sender, e); // Режим удаления рёбер
        }
        else if (isChangeEdgeWeightModeActive)
        {
            Canvas_MouseDown_ChangeEdgeWeight(sender, e); // Режим изменения веса рёбра
        }
    }

    private void ResetModes()
    {
        if (isMoveModeActive) MessageBox.Show("Режим перемещения узлов отключён.");
        if (isAddNodeModeActive) MessageBox.Show("Режим добавления узлов отключён.");
        if (isRemoveNodeModeActive) MessageBox.Show("Режим удаления узлов отключён.");
        if (isAddEdgeModeActive) MessageBox.Show("Режим добавления рёбер отключён.");
        if (isRemoveEdgeModeActive) MessageBox.Show("Режим удаления рёбер отключён.");

        isMoveModeActive = false;
        isAddNodeModeActive = false;
        isRemoveNodeModeActive = false;
        isAddEdgeModeActive = false;
        isRemoveEdgeModeActive = false;
        isSelectStartNodeActive = false; 
        
        ResetGraphColors();
    }
    
    
    
    // Методы для запуска обхода в ширину и глубину
    private void EnableStartNodeSelectionMode()
    {
        ResetModes(); // Сбрасываем другие режимы
        isSelectStartNodeActive = true;
        OutputTextBox.Text = "Кликните на узел, чтобы выбрать стартовую вершину.";
    }

    private async void Canvas_MouseDown_SelectStartNode(object sender, MouseButtonEventArgs e)
    {
        var position = e.GetPosition(GraphCanvas);

        // Находим узел, на который кликнул пользователь
        var node = graph.Nodes.FirstOrDefault(n =>
            position.X >= n.X - 15 && position.X <= n.X + 15 &&
            position.Y >= n.Y - 15 && position.Y <= n.Y + 15);

        if (node != null)
        {
            selectedStartNode = node;
            //OutputTextBox.Text = $"Выбрана стартовая вершина: {node.Id}.";

            // После выбора узла запускаем соответствующий алгоритм
            switch (selectedAlgorithm)
            {
                case "Обход графа в ширину":
                    await GraphTask_1.VisualizeWeightedBreadthFirstSearch(graph, selectedStartNode, AppendToOutput, HighlightNode, HighlightEdge, 500);
                    break;

                case "Обход графа в глубину":
                    await GraphTask_1.VisualizeWeightedDepthFirstSearch(graph, selectedStartNode, AppendToOutput, HighlightNode, HighlightEdge, 500);
                    break;

                default:
                    OutputTextBox.Text = "Неизвестный алгоритм. Пожалуйста, выберите корректный алгоритм.";
                    break;
            }

            ResetModes();
            selectedStartNode = null;
        }
        else
        {
            OutputTextBox.Text = "Пожалуйста, кликните на узел!";
        }
    }
    

    // Методы для визуализации алгоритмов
    private void ResetGraphColors()
    {
        // Сбрасываем цвета узлов
        foreach (var node in graph.Nodes)
        {
            node.FillColor = "#1F77B4"; // Исходный цвет узла (синий)
        }

        // Если требуется сброс цвета рёбер, можно добавить реализацию.
        // Например, если рёбра окрашиваются во время работы алгоритма, их тоже нужно сбрасывать.
        foreach (var edge in graph.Edges)
        {
            // Нет свойства цвета для рёбер, но можно добавить, если нужно визуально их изменять
            // edge.Color = Brushes.Black; // Пример для рёбер, если добавлено свойство цвета
        }

        // Перерисовываем граф
        GraphDrawer.DrawGraph(graph, GraphCanvas);
    }
    
    private void HighlightNode(Node node, string color)
    {
        node.FillColor = color; // Устанавливаем цвет узла
        GraphDrawer.DrawGraph(graph, GraphCanvas); // Перерисовываем граф
    }

    private void ResetHighlightedNode()
    {
        if (firstNodeForEdge != null)
        {
            firstNodeForEdge.FillColor = "#1F77B4"; // Возвращаем исходный цвет
            firstNodeForEdge = null;
            GraphDrawer.DrawGraph(graph, GraphCanvas); // Перерисовываем граф
        }
    }
    
    private void HighlightEdge(Edge edge, string color)
    {
        // Ищем объект Line, связанный с этим ребром
        var line = GraphCanvas.Children
            .OfType<Line>()
            .FirstOrDefault(l => l.Tag == edge);

        if (line != null)
        {
            line.Stroke = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color));
        }
    }

    
    // Кнопки для редактирования графа

    // Обработчик для кнопки "Загрузить граф"
    private void LoadGraphButton_Click(object sender, RoutedEventArgs e)
    {
        // Открытие диалога выбора файла
        var openFileDialog = new OpenFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
            Title = "Открыть файл графа"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;

            try
            {
                // Загружаем граф из выбранного CSV-файла
                graph = FileService.LoadGraphFromCsv(filePath);

                // Отображаем загруженный граф на канвасе
                GraphDrawer.DrawGraph(graph, GraphCanvas);

                // Информируем пользователя об успешной загрузке
                MessageBox.Show("Граф успешно загружен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    // Обработчик для активации режима перемещения
    private void MoveNodeButton_Click(object sender, RoutedEventArgs e)
    {
        ResetModes(); // Сбрасываем все режимы
        isMoveModeActive = true; // Активируем режим перемещения узлов
        MessageBox.Show("Режим перемещения узлов активирован!");
    }


    // Обработчик для начала перетаскивания
    private void Canvas_MouseDown_MoveNode(object sender, MouseButtonEventArgs e)
    {
        if (!isMoveModeActive) return; // Если режим перемещения не активен, выходим

        var position = e.GetPosition(GraphCanvas);

        // Проверяем, был ли клик на узле
        draggedNode = graph.Nodes.FirstOrDefault(n =>
            position.X >= n.X - 15 && position.X <= n.X + 15 &&
            position.Y >= n.Y - 15 && position.Y <= n.Y + 15);

        if (draggedNode != null)
        {
            isDraggingNode = true;

            // Захватываем мышь для отслеживания её движения даже за пределами канваса
            GraphCanvas.CaptureMouse();
        }
    }

    // Обработчик для перемещения узла
    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (!isMoveModeActive) return; // Если режим перемещения не активен, выходим
        if (isDraggingNode && draggedNode != null)
        {
            var position = e.GetPosition(GraphCanvas);
            draggedNode.X = position.X;
            draggedNode.Y = position.Y;

            // Перерисовываем граф
            GraphDrawer.DrawGraph(graph, GraphCanvas);
        }
    }

    // Обработчик для отпускания узла
    private void Canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!isMoveModeActive) return; // Если режим перемещения не активен, выходим
        if (isDraggingNode)
        {
            isDraggingNode = false;
            draggedNode = null; // Обнуляем ссылку на перетаскиваемый узел

            // Отпускаем захват мыши
            GraphCanvas.ReleaseMouseCapture();
        }
    }

    private void AddNodeButton_Click(object sender, RoutedEventArgs e)
    {
        ResetModes(); // Сбрасываем все режимы
        isAddNodeModeActive = true; // Активируем режим добавления узлов
        MessageBox.Show("Режим добавления узлов активирован! Кликните на поле, чтобы добавить узел.");
    }
    
    // Обработчик для добавления узла по клику на канвас
    private void Canvas_MouseDown_AddNode(object sender, MouseButtonEventArgs e)
    {
        if (!isAddNodeModeActive) return;

        var position = e.GetPosition(GraphCanvas);

        var newNode = new Node
        {
            Id = graph.Nodes.Count + 1, // Генерация нового ID
            X = position.X,
            Y = position.Y
        };

        graph.AddNode(newNode); // Добавляем узел в граф
        GraphDrawer.DrawGraph(graph, GraphCanvas); // Перерисовываем граф
    }

    private void RemoveNodeButton_Click(object sender, RoutedEventArgs e)
    {
        ResetModes(); // Сбрасываем все режимы
        isRemoveNodeModeActive = true; // Активируем режим удаления узлов
        MessageBox.Show("Режим удаления узлов активирован! Кликните на узел, чтобы удалить его.");
    }

    // Обработчик для удаления узла по клику
    private void Canvas_MouseDown_RemoveNode(object sender, MouseButtonEventArgs e)
    {
        if (!isRemoveNodeModeActive) return;

        var position = e.GetPosition(GraphCanvas);

        var nodeToRemove = graph.Nodes.FirstOrDefault(n =>
            position.X >= n.X - 15 && position.X <= n.X + 15 &&
            position.Y >= n.Y - 15 && position.Y <= n.Y + 15);

        if (nodeToRemove != null)
        {
            graph.RemoveNode(nodeToRemove); // Удаляем узел из графа
            GraphDrawer.DrawGraph(graph, GraphCanvas); // Перерисовываем граф
        }
    }

    private void AddEdgeButton_Click(object sender, RoutedEventArgs e)
    {
        ResetModes(); // Сбрасываем все режимы
        isAddEdgeModeActive = true; // Активируем режим добавления рёбер
        MessageBox.Show("Режим добавления рёбер активирован! Выберите два узла.");
    }

    // Обработчик для выбора узлов и добавления рёбра
    private void Canvas_MouseDown_AddEdge(object sender, MouseButtonEventArgs e)
    {
        var position = e.GetPosition(GraphCanvas);

        // Находим узел, на который кликнул пользователь
        var selectedNode = graph.Nodes.FirstOrDefault(n =>
            position.X >= n.X - 15 && position.X <= n.X + 15 &&
            position.Y >= n.Y - 15 && position.Y <= n.Y + 15);

        if (selectedNode == null)
        {
            // Если клик был не на узле, ничего не делаем
            return;
        }

        if (firstNodeForEdge == null)
        {
            // Если это первый выбранный узел
            firstNodeForEdge = selectedNode;

            // Подсвечиваем узел
            HighlightNode(selectedNode, "#1F33B4");
        }
        else
        {
            // Если это второй узел
            if (firstNodeForEdge == selectedNode)
            {
                MessageBox.Show("Выбран тот же узел. Выберите другой узел для добавления рёбра.");
                return; // Нельзя соединять узел с самим собой
            }

            // Запрашиваем вес ребра
            string weightInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите вес ребра:",
                "Добавление ребра",
                "0");

            if (!int.TryParse(weightInput, out int weight) || weight <= 0)
            {
                MessageBox.Show("Некорректный вес. Пожалуйста, введите положительное число.");
                return;
            }

            // Добавляем ребро с заданным весом
            graph.Edges.Add(new Edge
            {
                StartNode = firstNodeForEdge,
                EndNode = selectedNode,
                Weight = weight
            });

            // Сбрасываем подсветку узла
            ResetHighlightedNode();

            // Перерисовываем граф
            GraphDrawer.DrawGraph(graph, GraphCanvas);
        }
    }

    private void RemoveEdgeButton_Click(object sender, RoutedEventArgs e)
    {
        ResetModes(); // Сбрасываем все режимы
        isRemoveEdgeModeActive = true; // Активируем режим удаления рёбер
        MessageBox.Show("Режим удаления рёбер активирован! Кликните на ребро, чтобы удалить его.");
    }

    private void Canvas_MouseDown_RemoveEdge(object sender, MouseButtonEventArgs e)
    {
        var position = e.GetPosition(GraphCanvas);

        // Находим ближайшее ребро к точке клика
        var edgeToRemove = graph.Edges.FirstOrDefault(edge =>
        {
            var x1 = edge.StartNode.X;
            var y1 = edge.StartNode.Y;
            var x2 = edge.EndNode.X;
            var y2 = edge.EndNode.Y;

            // Расстояние от точки до линии
            double distance = DistanceFromPointToLine(position.X, position.Y, x1, y1, x2, y2);
            return distance <= 5; // Допустимый порог попадания
        });

        if (edgeToRemove != null)
        {
            // Удаляем ребро
            graph.RemoveEdge(edgeToRemove);
            GraphDrawer.DrawGraph(graph, GraphCanvas); // Перерисовываем граф
            MessageBox.Show($"Ребро между узлами {edgeToRemove.StartNode.Id} и {edgeToRemove.EndNode.Id} удалено.");
        }
        else
        {
            // Если пользователь не попал на ребро
            MessageBox.Show("Вы не попали на ребро. Пожалуйста, кликните ближе к линии.");
        }
    }

    // Метод для вычисления расстояния от точки до линии
    private double DistanceFromPointToLine(double px, double py, double x1, double y1, double x2, double y2)
    {
        double a = px - x1;
        double b = py - y1;
        double c = x2 - x1;
        double d = y2 - y1;

        double dot = a * c + b * d;
        double lenSq = c * c + d * d;
        double param = lenSq != 0 ? dot / lenSq : -1;

        double xx, yy;

        if (param < 0)
        {
            xx = x1;
            yy = y1;
        }
        else if (param > 1)
        {
            xx = x2;
            yy = y2;
        }
        else
        {
            xx = x1 + param * c;
            yy = y1 + param * d;
        }

        double dx = px - xx;
        double dy = py - yy;

        return Math.Sqrt(dx * dx + dy * dy);
    }

    private void Canvas_MouseDown_ChangeEdgeWeight(object sender, MouseButtonEventArgs e)
    {
        var position = e.GetPosition(GraphCanvas);

        // Ищем текстовое поле с весом рёбра
        var clickedElement = GraphCanvas.Children.OfType<TextBlock>()
            .FirstOrDefault(label =>
            {
                var x = Canvas.GetLeft(label);
                var y = Canvas.GetTop(label);
                return position.X >= x && position.X <= x + 30 && position.Y >= y && position.Y <= y + 20;
            });

        if (clickedElement != null)
        {
            var edge = clickedElement.Tag as Edge; // Получаем объект ребра через Tag
            if (edge != null)
            {
                // Запрашиваем новый вес
                string weightInput = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите новый вес ребра:",
                    "Изменение веса ребра",
                    edge.Weight.ToString());

                if (int.TryParse(weightInput, out int newWeight) && newWeight > 0)
                {
                    edge.Weight = newWeight; // Изменяем вес ребра
                    GraphDrawer.DrawGraph(graph, GraphCanvas); // Перерисовываем граф
                }
                else
                {
                    MessageBox.Show("Некорректный ввод. Вес должен быть положительным целым числом.", "Ошибка");
                }
            }
        }
    }

    private void ChangeEdgeWeightButton_Click(object sender, RoutedEventArgs e)
    {
        ResetModes(); // Сбрасываем другие режимы
        isChangeEdgeWeightModeActive = true; // Активируем режим изменения веса рёбра
        MessageBox.Show("Режим изменения веса рёбер активирован! Кликните на вес рёбра, чтобы изменить его.");
    }

    private void ClearCanvasButton_Click(object sender, RoutedEventArgs e)
    {
        ResetModes(); // Сбрасываем все режимы

        // Очищаем все элементы на Canvas
        GraphCanvas.Children.Clear();
        graph.Nodes.Clear();
        graph.Edges.Clear();

        // Очищаем поле вывода
        OutputTextBox.Text = string.Empty;
    }
    
    private void SaveGraphButton_Click(object sender, RoutedEventArgs e)
    {
        ResetModes(); // Сбрасываем все режимы

        var saveFileDialog = new SaveFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
            Title = "Сохранить граф"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                FileService.SaveGraphToCsv(graph, saveFileDialog.FileName);
                MessageBox.Show("Граф успешно сохранён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении графа: {ex.Message}");
            }
        }
    }
    
    private void AppendToOutput(string text)
    {
        OutputTextBox.Text += $"{text}\n\n";
        OutputTextBox.ScrollToEnd(); // Автоматическая прокрутка к последней строке
    }
}