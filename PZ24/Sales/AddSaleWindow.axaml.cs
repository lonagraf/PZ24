using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;
using PZ24.Products;

namespace PZ24.Sales;

public partial class AddSaleWindow : Window
{
    private Database _database = new Database();
    public AddSaleWindow()
    {
        InitializeComponent();
        Width = 400;
        Height = 300;
        LoadDataProductCmb();
        LoadDataClientCmb();
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _database.openConnection();
        string sql = "insert into sale (sale_date, cost, product, client) values (@date, @cost, @product, @client);";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@date", DateTxt.Text);
        command.Parameters.AddWithValue("@cost", CostTxt.Text);
        int selectedProductId = GetSelectedProductId(ProductCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@product", selectedProductId);
        int selectedClientId = GetSelectedClientId(ClientCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@client", selectedClientId);
        command.ExecuteNonQuery();
        var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Данные успешно добавлены", ButtonEnum.Ok);
        var result = box.ShowAsync();
        this.Close();
    }

    private void LoadDataProductCmb()
    {
        _database.openConnection();
        string sql = "select product_name from product;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            ProductCmb.Items.Add(reader["product_name"]).ToString();
        }
        _database.closeConnection();
    }

    private int GetSelectedProductId(string selectedProduct)
    {
        _database.openConnection();
        string sql = "select product_id from product where product_name = @selectedProduct;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedProduct", selectedProduct);
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
}