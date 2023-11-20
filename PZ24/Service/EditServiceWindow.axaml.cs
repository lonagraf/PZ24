using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Service;

public partial class EditServiceWindow : Window
{
    private Database _database = new Database();
    private Service _service;
    public EditServiceWindow(Service service)
    {
        InitializeComponent();
        _service = service;
        StatusCmb.SelectedItem = _service.Status;
        EmployeeCmb.SelectedItem = _service.Employee;
        LoadDataStatusCmb();
        LoadDataEmployeeCmb();
        Width = 400;
        Height = 300;
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        int id = _service.ServiceID;
        _database.openConnection();
        string sql =
            "update client_service set appeal_status = @status, employee = @employee where client_service_id = @id";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        int selectedStatusId = GetSelectedStatusId(StatusCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@status", selectedStatusId);
        int selectedEmployeeId = GetSelectedEmployeeId(EmployeeCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@employee", selectedEmployeeId);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Данные успешно изменены", ButtonEnum.Ok);
        var result = box.ShowAsync();
        this.Close();
    }

    private void LoadDataStatusCmb()
    {
        _database.openConnection();
        string sql = "select status_name from appeal_status;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                StatusCmb.Items.Add(reader["status_name"].ToString());
            }
        }
        _database.closeConnection();
    }

    private void LoadDataEmployeeCmb()
    {
        _database.openConnection();
        string sql = "select concat(firstnamee, ' ', surnamee) as employee from employee;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                EmployeeCmb.Items.Add(reader["employee"].ToString());
            }
        }
        _database.closeConnection();
    }
    
    private int GetSelectedStatusId(string selectedStatus)
    {
        _database.openConnection();
        string sql = "select appeal_status_id from appeal_status where status_name = @selectedStatus";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedStatus", selectedStatus);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
    
    private int GetSelectedEmployeeId(string selectedEmployee)
    {
        _database.openConnection();
        string sql = "select employee_id from employee where concat(firstnamee, ' ', surnamee) = @selectedEmployee";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedEmployee", selectedEmployee);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
}