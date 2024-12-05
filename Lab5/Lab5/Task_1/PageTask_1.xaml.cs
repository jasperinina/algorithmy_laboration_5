using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lab5.Models;
using Lab5.Models.FileIO;
using Microsoft.Win32;

namespace Lab5.Task_1;

public partial class PageTask_1 : Page
{
    private MainWindow _mainWindow;
    private StackPanel dynamicPanel;
    
    private TextBox FilePathTextBox;
    
    private Graph graph;
    private bool isDraggingNode = false;
    private Node draggedNode;

    public PageTask_1(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        AddDynamicControls();
        graph = new Graph();
    }

    private void AddDynamicControls()
    {
        dynamicPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Left
        };
        
        // Текстовое поле для ввода пути к файлу
        FilePathTextBox = new TextBox
        {
            Width = 360,
            Margin = new Thickness(0, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            //Style = (Style)_mainWindow.FindResource("RoundedTextBoxStyle")
        };
        
        // Кнопка для открытия диалога выбора файла
        Button browseFileButton = new Button
        {
            Content = "Обзор",
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 20, 0, 8),
            //Style = (Style)_mainWindow.FindResource("OverwriteFileButtonStyle")
        };
        browseFileButton.Click += (s, e) => BrowseButton_Click(this, e);
        
        // Кнопка для открытия диалога выбора файла
        Button LoadGraphButton = new Button
        {
            Content = "Загрузить граф",
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 20, 0, 8),
            //Style = (Style)_mainWindow.FindResource("OverwriteFileButtonStyle")
        };
        LoadGraphButton.Click += (s, e) => LoadGraphButton_Click(this, e);
        
        dynamicPanel.Children.Add(FilePathTextBox);
        dynamicPanel.Children.Add(browseFileButton);
        dynamicPanel.Children.Add(LoadGraphButton);
        
        _mainWindow.PageContentControl.Content = dynamicPanel;
    }
    
    // Обработчик для кнопки "Обзор"
    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        // Открытие диалога выбора файла
        var openFileDialog = new OpenFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
            Title = "Открыть файл графа"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            // Записываем выбранный путь в TextBox
            FilePathTextBox.Text = openFileDialog.FileName;
        }
    }
    
    // Обработчик для кнопки "Загрузить граф"
    private void LoadGraphButton_Click(object sender, RoutedEventArgs e)
    {
        string filePath = FilePathTextBox.Text;

        if (string.IsNullOrWhiteSpace(filePath))
        {
            MessageBox.Show("Пожалуйста, выберите файл для загрузки.");
            return;
        }

        try
        {
            // Загружаем граф из выбранного CSV-файла
            graph = FileService.LoadGraphFromCsv(filePath);
                
            // Отображаем загруженный граф на канвасе
            GraphDrawer.DrawGraph(graph, GraphCanvas);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}");
        }
    }

    // Обработчик для начала перетаскивания
    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var position = e.GetPosition(GraphCanvas);
        Console.WriteLine("MouseDown: " + position); // Логирование для отладки

        // Проверяем, был ли клик на узле
        draggedNode = graph.Nodes.FirstOrDefault(n =>
            position.X >= n.X - 15 && position.X <= n.X + 15 &&
            position.Y >= n.Y - 15 && position.Y <= n.Y + 15);

        if (draggedNode != null)
        {
            isDraggingNode = true;
            Console.WriteLine("Node picked: " + draggedNode.Id); // Логирование для отладки

            // Захватываем мышь для отслеживания её движения даже за пределами канваса
            GraphCanvas.CaptureMouse();
        }
    }

    // Обработчик для перемещения узла
    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDraggingNode && draggedNode != null)
        {
            var position = e.GetPosition(GraphCanvas);
            draggedNode.X = position.X;
            draggedNode.Y = position.Y;

            Console.WriteLine("MouseMove: " + position); // Логирование для отладки

            // Перерисовываем граф
            GraphDrawer.DrawGraph(graph, GraphCanvas);
        }
    }

    // Обработчик для отпускания узла (PreviewMouseUp вместо MouseUp)
    private void Canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (isDraggingNode)
        {
            isDraggingNode = false;
            Console.WriteLine("MouseUp: Node released"); // Логирование для отладки
            draggedNode = null;  // Обнуляем ссылку на перетаскиваемый узел

            // Отпускаем захват мыши
            GraphCanvas.ReleaseMouseCapture();
        }
    }
}