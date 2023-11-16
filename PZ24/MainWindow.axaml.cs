using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PZ24.Employees;
using PZ24.Marketing;
using PZ24.Products;
using PZ24.Service;

namespace PZ24;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Width = 1000;
        Height = 450;
    }

    private void CloseBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void ClientBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.Children.Clear();
        ClientWindow clientWindow = new ClientWindow();
        MainPanel.Children.Add(clientWindow);
    }

    private void ProductBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.Children.Clear();
        ProductWindow productWindow = new ProductWindow();
        MainPanel.Children.Add(productWindow);
    }

    private void EmployeeBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.Children.Clear();
        EmployeeWindow employeeWindow = new EmployeeWindow();
        MainPanel.Children.Add(employeeWindow);
    }

    private void ServiceBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.Children.Clear();
        ServiceWindow serviceWindow = new ServiceWindow();
        MainPanel.Children.Add(serviceWindow);
    }

    private void CampaignBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.Children.Clear();
        CampaignWindow campaignWindow = new CampaignWindow();
        MainPanel.Children.Add(campaignWindow);
    }
}