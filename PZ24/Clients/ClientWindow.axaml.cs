using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;
using PZ24.Clients;

namespace PZ24;

public partial class ClientWindow : UserControl
{
    private Database _database = new Database();
    private List<Client> _clients = new List<Client>();
    private List<Status> _status = new List<Status>();

    private string _fullTable = "select client_id, firstname, surname, phone_number, email, status_name from client " +
                                "join crm.client_status cs on cs.client_status_id = client.client_status " +
                                "order by client_id;";
    public ClientWindow()
    {
        InitializeComponent();
        ShowTable(_fullTable);
        LoadDataStatusCmb();
    }

    public void ShowTable(string sql)
    {
        _database.openConnection();
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.HasRows && reader.Read())
        {
            var currentClient = new Client()
            {
                ClientID = reader.GetInt32("client_id"),
                Firstname = reader.GetString("firstname"),
                Surname = reader.GetString("surname"),
                PhoneNumber = reader.GetString("phone_number"),
                Email = reader.GetString("email"),
                Status = reader.GetString("status_name")
            };
            _clients.Add(currentClient);
        }
        _database.closeConnection();
        ClientGrid.ItemsSource = _clients;
    }

    private void AddClientBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        AddClientWindow addClientWindow = new AddClientWindow();
        addClientWindow.Show();
    }

    public void Delete(int id)
    {
        _database.openConnection();
        string sql = "delete from client where client_id = @clientId";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@clientId", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }

    private async void DeleteClientBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Client selectedClient = ClientGrid.SelectedItem as Client;

        if (selectedClient != null)
        {
            var warning = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Вы уверены что хотите удалить клиента?", ButtonEnum.YesNo);
            var result = await warning.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                Delete(selectedClient.ClientID);
                ShowTable(_fullTable);
                var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Клиент успешно удален!", ButtonEnum.Ok);
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
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите клиента для удаления", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }

    private void EditClientBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Client selectedClient = ClientGrid.SelectedItem as Client;
        if (selectedClient != null)
        {
            EditClientWindow editClientWindow = new EditClientWindow(selectedClient);
            editClientWindow.Show();
            ShowTable(_fullTable);
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите клиента для редактирования", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
        
    }

    private void LoadDataStatusCmb()
    {
        _database.openConnection();
        string sql = "select status_name from client_status";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            var currentStatus = new Status()
            {
                StatusName = reader.GetString("status_name")
            };
            _status.Add(currentStatus);
        }
        _database.closeConnection();
        var statusCmb = this.Find<ComboBox>("StatusCmb");
        statusCmb.ItemsSource = _status;
    }


    private void StatusCmb_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var statusCmb = (ComboBox)sender;
        var currentStatus = statusCmb.SelectedItem as Status;
        var filteredClients = _clients.Where(x => x.Status == currentStatus.StatusName).ToList();
        ClientGrid.ItemsSource = filteredClients;
    }
}