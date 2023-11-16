using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Products;

public partial class AddProductWindow : Window
{
    private Database _database = new Database();
    public AddProductWindow()
    {
        InitializeComponent();
        LoadDataCategoryCmb();
        Width = 400;
        Height = 300;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _database.openConnection();
        string sql = "insert into product (product_name, product_description, price, product_category) " +
                     "values (@name, @desc, @price, @category);";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@name", NameTxt.Text);
        command.Parameters.AddWithValue("desc", DescTxt.Text);
        command.Parameters.AddWithValue("@price", PriceTxt.Text);
        int selectedCategoryId = GetSelectedCategoryId(CategoryCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@category", selectedCategoryId);
        command.ExecuteNonQuery();
        _database.closeConnection();
        var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Продукт успешно добавлен", ButtonEnum.Ok);
        var result = box.ShowAsync();
        this.Close();
    }

    private void LoadDataCategoryCmb()
    {
        _database.openConnection();
        string sql = "select category_name from product_category;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                CategoryCmb.Items.Add(reader["category_name"].ToString());
            }
        }
        _database.closeConnection();
    }

    private int GetSelectedCategoryId(string selectedCategory)
    {
        _database.openConnection();
        string sql = "select product_category_id from product_category where category_name = @selectedCategory;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@selectedCategory", selectedCategory);
        int selectedId = Convert.ToInt32(command.ExecuteScalar());
        return selectedId;
    }
}