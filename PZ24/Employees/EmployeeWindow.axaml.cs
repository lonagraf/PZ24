using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Employees;

public partial class EmployeeWindow : UserControl
{
    private Database _database = new Database();
    private List<Employee> _employees = new List<Employee>();
    private string _fullTable = "select employee_id, firstnamee, surnamee, phone_number, email from employee;";
    
    public EmployeeWindow()
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
            var currentEmployee = new Employee()
            {
                EmployeeID = reader.GetInt32("employee_id"),
                Name = reader.GetString("firstnamee"),
                Surname = reader.GetString("surnamee"),
                Phone = reader.GetString("phone_number"),
                Email = reader.GetString("email"),
            };
            _employees.Add(currentEmployee);
        }
        _database.closeConnection();
        EmployeeGrid.ItemsSource = _employees;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
        addEmployeeWindow.Show();
    }
    
    public void Delete(int id)
    {
        _database.openConnection();
        string sql = "delete from employee where employee_id = @Id";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }


    private async void DeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Employee selectedEmployee = EmployeeGrid.SelectedItem as Employee;

        if (selectedEmployee != null)
        {
            var warning = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Вы уверены что хотите удалить?", ButtonEnum.YesNo);
            var result = await warning.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                Delete(selectedEmployee.EmployeeID);
                ShowTable(_fullTable);
                var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Успешно удален!", ButtonEnum.Ok);
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

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Employee selectedEmployee = EmployeeGrid.SelectedItem as Employee;
        if (selectedEmployee != null)
        {
            EditEmployeeWindow editEmployeeWindow = new EditEmployeeWindow(selectedEmployee);
            editEmployeeWindow.Show();
            ShowTable(_fullTable);
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите для редактирования", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }
}