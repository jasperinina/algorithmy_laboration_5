using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lab5.Task_1;

namespace Lab5;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    // Метод для очистки динамического контента
    private void ClearDynamicContent()
    {
        PageContentControl.Content = null; // Очищаем содержимое контент-контрола
    }
    
    private void Task_1RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        ClearDynamicContent();
        MainFrame.Navigate(new PageTask_1(this));
    }

    /*
    private void Task_2RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        ClearDynamicContent();
        MainFrame.Navigate(new Task_2Page(this));
    }
    
    private void Task_3RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        ClearDynamicContent();
        MainFrame.Navigate(new Task_3Page(this));
    }
    */
}