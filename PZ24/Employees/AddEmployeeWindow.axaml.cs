using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Employees;

public partial class AddEmployeeWindow : Window
{
    private Database _database = new Database();
    public AddEmployeeWindow()
    {
        InitializeComponent();
        Width = 400;
        Height = 300;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _database.openConnection();
        string sql = "insert into employee (firstnamee, surnamee, phone_number, email ) " +
                     "values (@firstname, @surname, @phone, @email)";
        using (MySqlCommand command = new MySqlCommand(sql, _database.getConnection()))
        {
            command.Parameters.AddWithValue("@firstname", NameTxt.Text);
            command.Parameters.AddWithValue("@surname", SurnameTxt.Text);
            command.Parameters.AddWithValue("@phone", PhoneTxt.Text);
            command.Parameters.AddWithValue("@email", EmailTxt.Text);
            command.ExecuteNonQuery();
            var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Успешно добавлен!", ButtonEnum.Ok);
            var result = box.ShowAsync();
            this.Close();
        }
        _database.closeConnection();
    }
}