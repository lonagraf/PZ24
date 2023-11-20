using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Service;

public partial class AddServiceWindow : Window
{
    private Database _database = new Database();
    public AddServiceWindow()
    {
        InitializeComponent();
        LoadDataTypeCmb();
        LoadDataStatusCmb();
        LoadDataClientCmb();
        LoadDataEmployeeCmb();
        Width = 400;
        Height = 300;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _database.openConnection();
        string sql =
            "insert into client_service (appeal_date, appeal_type, appeal_status, problem_description, client, employee) " +
            "values (@Date, @Type, @Status, @Desc, @Client, @Employee)";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@Date", DateTxt.Text);
        int selectedTypeId = GetSelectedTypeId(TypeCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@Type", selectedTypeId);
        int selectedStatusId = GetSelectedStatusId(StatusCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@Status", selectedStatusId);
        command.Parameters.AddWithValue("@Desc", DescTxt.Text);
        int selectedClientId = GetSelectedClientId(ClientCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@Client", selectedClientId);
        int selectedEmployeeId = GetSelectedEmployeeId(EmployeeCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@Employee", selectedEmployeeId);
        command.ExecuteNonQuery();
        var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Успешно добавленo!", ButtonEnum.Ok);
        var result = box.ShowAsync();
        this.Close();
    }

    private void LoadDataTypeCmb()
    {
        _database.openConnection();
        string sql = "select type_name from appeal_type;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                TypeCmb.Items.Add(reader["type_name"].ToString());
            }
        }
        _database.closeConnection();
    }
    
    private int GetSelectedTypeId(string selectedType)
    {
        _database.openConnection();
        string sql = "select appeal_type_id from appeal_type where type_name = @selectedType";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedType", selectedType);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
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
    
    private int GetSelectedStatusId(string selectedStatus)
    {
        _database.openConnection();
        string sql = "select appeal_status_id from appeal_status where status_name = @selectedStatus";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedStatus", selectedStatus);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
    
    private void LoadDataClientCmb()
    {
        _database.openConnection();
        string sql = "select concat(firstname, ' ', surname) as client from client;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                ClientCmb.Items.Add(reader["client"].ToString());
            }
        }
        _database.closeConnection();
    }
    
    private int GetSelectedClientId(string selectedClient)
    {
        _database.openConnection();
        string sql = "select client_id from client where concat(firstname, ' ', surname) = @selectedClient";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedClient", selectedClient);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
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