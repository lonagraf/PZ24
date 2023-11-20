using System.Collections.Generic;
using System.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;
using PZ24.Employees;

namespace PZ24.Service;

public partial class ServiceWindow : UserControl
{
    private Database _database = new Database();
    private List<Service> _services = new List<Service>();

    private string _fullTable =
        "select client_service_id, appeal_date, type_name, status_name, problem_description, concat(firstname, ' ', surname) as client, concat(firstnamee, ' ', surnamee) as employee from client_service " +
        "join crm.appeal_type a on a.appeal_type_id = client_service.appeal_type " +
        "join crm.appeal_status  on client_service.appeal_status = appeal_status.appeal_status_id " +
        "join crm.client c on client_service.client = c.client_id " +
        "join crm.employee e on e.employee_id = client_service.employee;";
    
    public ServiceWindow()
    {
        InitializeComponent();
        ShowTable(_fullTable);
    }
    
    public void ShowTable(string sql)
    {
        _database.openConnection();
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.HasRows && reader.Read())
        {
            var currentService = new Service()
            {
                ServiceID = reader.GetInt32("client_service_id"),
                Date = reader.GetDateTime("appeal_date").ToString("dd-MM-yyyy"),
                Type = reader.GetString("type_name"),
                Status = reader.GetString("status_name"),
                Desc = reader.GetString("problem_description"),
                Client = reader.GetString("client"),
                Employee = reader.GetString("employee")
            };
            _services.Add(currentService);
        }
        _database.closeConnection();
        ServiceGrid.ItemsSource = _services;
    }

    private void AddClientBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        AddServiceWindow addServiceWindow = new AddServiceWindow();
        addServiceWindow.Show();
    }

    private void Delete(int id)
    {
        _database.openConnection();
        string sql = "delete from client_service where client_service_id = @id";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }

    private async void DeleteClientBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Service selectedService = ServiceGrid.SelectedItem as Service;

        if (selectedService != null)
        {
            var warning = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Вы уверены что хотите удалить?", ButtonEnum.YesNo);
            var result = await warning.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                Delete(selectedService.ServiceID);
                ShowTable(_fullTable);
                var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Yспешно удален!", ButtonEnum.Ok);
                var successResult = box.ShowAsync();
            }
            else
            {
                var cancelBox = MessageBoxManager.GetMessageBoxStandard("Отмена", "Операция удаления отменена", ButtonEnum.Ok);
                var cancelResult = cancelBox.ShowAsync();
            }
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите для удаления", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }

    private void EditClientBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Service selectedService = ServiceGrid.SelectedItem as Service;

        if (selectedService != null)
        {
            EditServiceWindow editServiceWindow = new EditServiceWindow(selectedService);
            editServiceWindow.Show();
            ShowTable(_fullTable);
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите для редактирования", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }
}