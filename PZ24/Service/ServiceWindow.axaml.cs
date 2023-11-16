using System.Collections.Generic;
using System.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
}