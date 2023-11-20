using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
}