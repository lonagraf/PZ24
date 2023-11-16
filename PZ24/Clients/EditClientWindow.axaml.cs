using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Clients;

public partial class EditClientWindow : Window
{
    private Database _database = new Database();
    private Client _client;
    public EditClientWindow(Client client)
    {
        InitializeComponent();
        _client = client;
        NameTxt.Text = _client.Firstname;
        SurnameTxt.Text = _client.Surname;
        PhoneTxt.Text = _client.PhoneNumber;
        EmailTxt.Text = _client.Email;
        StatusCmb.SelectedItem = _client.Status;
        LoadDataStatusCmb();
        Height = 400;
        Width = 350;
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        int id = _client.ClientID;
        _database.openConnection();
        string sql =
            "update client set firstname = @name, surname = @surname, phone_number = @number, email = @email, client_status = @status where client_id = @id;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@name", NameTxt.Text);
        command.Parameters.AddWithValue("@surname", SurnameTxt.Text);
        command.Parameters.AddWithValue("@number", PhoneTxt.Text);
        command.Parameters.AddWithValue("@email", EmailTxt.Text);
        int selectedStatusId = GetSelectedStatusId(StatusCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("status", selectedStatusId);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Данные успешно изменены", ButtonEnum.Ok);
        var result = box.ShowAsync();
        this.Close();
    }

    private void LoadDataStatusCmb()
    {
        _database.openConnection();
        string sql = "select status_name from client_status;";
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
        string sql = "select client_status_id from client_status where status_name = @selectedStatus";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedStatus", selectedStatus);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
}