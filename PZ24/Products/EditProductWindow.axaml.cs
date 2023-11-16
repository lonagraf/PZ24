using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Products;

public partial class EditProductWindow : Window
{
    private Database _database = new Database();
    private Product _product;
    public EditProductWindow(Product product)
    {
        InitializeComponent();
        _product = product;
        NameTxt.Text = _product.ProductName;
        DescTxt.Text = _product.ProductDesc;
        PriceTxt.Text = _product.Price.ToString();
        CategoryCmb.SelectedItem = _product.Category;
        Width = 400;
        Height = 300;
        LoadDataCategoryCmb();
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        int id = _product.ProductID;
        _database.openConnection();
        string sql =
            "update product set product_name = @name, product_description = @desc, price = @price, product_category = @category where " +
            "product_id = @id;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@name", NameTxt.Text);
        command.Parameters.AddWithValue("@desc", DescTxt.Text);
        command.Parameters.AddWithValue("@price", PriceTxt.Text);
        int selectedCategoryId = GetSelectedCategoryId(CategoryCmb.SelectedItem.ToString());
        command.Parameters.AddWithValue("@category", selectedCategoryId);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
        var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Изменения успешно сохранены", ButtonEnum.Ok);
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