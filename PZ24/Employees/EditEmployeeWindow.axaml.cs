using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Employees;

public partial class EditEmployeeWindow : Window
{
    private Database _database = new Database();
    private Employee _employee;
    public EditEmployeeWindow(Employee employee)
    {
        InitializeComponent();
        _employee = employee;
        NameTxt.Text = _employee.Name;
        SurnameTxt.Text = _employee.Surname;
        PhoneTxt.Text = _employee.Phone;
        EmailTxt.Text = _employee.Email;
        Width = 400;
        Height = 300;
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        int id = _employee.EmployeeID;
        _database.openConnection();
        string sql =
            "update employee set firstnamee = @name, surnamee = @surname, phone_number = @number, email = @email where employee_id = @id;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@name", NameTxt.Text);
        command.Parameters.AddWithValue("@surname", SurnameTxt.Text);
        command.Parameters.AddWithValue("@number", PhoneTxt.Text);
        command.Parameters.AddWithValue("@email", EmailTxt.Text);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Данные успешно изменены", ButtonEnum.Ok);
        var result = box.ShowAsync();
        this.Close();
    }
}