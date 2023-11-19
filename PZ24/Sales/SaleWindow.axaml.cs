using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Sales;

public partial class SaleWindow : UserControl
{
    private Database _database = new Database();
    private List<Sale> _sales = new List<Sale>();

    private string _fullTable =
        "select sale_id, sale_date, cost, product_name, concat(firstname, ' ', surname) as client from sale " +
        "join crm.client c on c.client_id = sale.client " +
        "join crm.product p on p.product_id = sale.product;";
    public SaleWindow()
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
            var currentSale = new Sale()
            {
                SaleID = reader.GetInt32("sale_id"),
                Date = reader.GetDateTime("sale_date").ToString("dd-MM-yyyy"),
                Cost = reader.GetDecimal("cost"),
                Product = reader.GetString("product_name"),
                Client = reader.GetString("client"),
            };
            _sales.Add(currentSale);
        }
        _database.closeConnection();
        SaleGrid.ItemsSource = _sales;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        AddSaleWindow addSaleWindow = new AddSaleWindow();
        addSaleWindow.Show();
    }

    private void Delete(int id)
    {
        _database.openConnection();
        string sql = "delete from sale where sale_id = @id;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }

    private async void DeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Sale selectedSale = SaleGrid.SelectedItem as Sale;

        if (selectedSale != null)
        {
            var warning = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Вы уверены что хотите удалить?", ButtonEnum.YesNo);
            var result = await warning.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                Delete(selectedSale.SaleID);
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
}