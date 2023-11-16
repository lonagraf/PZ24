using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;

namespace PZ24.Products;

public partial class ProductWindow : UserControl
{
    private Database _database = new Database();
    private List<Product> _products = new List<Product>();
    private List<Category> _categories = new List<Category>();

    private string _fullTable =
        "select product_id, product_name, product_description, price, category_name from product " +
        "join crm.product_category pc on pc.product_category_id = product.product_category;";
    public ProductWindow()
    {
        InitializeComponent();
        ShowTable(_fullTable);
        LoadDataCategoryCmb();
    }

    private void ShowTable(string sql)
    {
        _database.openConnection();
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.HasRows && reader.Read())
        {
            var currentProduct = new Product()
            {
                ProductID = reader.GetInt32("product_id"),
                ProductName = reader.GetString("product_name"),
                ProductDesc = reader.GetString("product_description"),
                Price = reader.GetDecimal("price"),
                Category = reader.GetString("category_name")
            };
            _products.Add(currentProduct);
        }
        _database.closeConnection();
        ProductGrid.ItemsSource = _products;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        AddProductWindow addProductWindow = new AddProductWindow();
        addProductWindow.Show();
    }

    private void Delete(int id)
    {
        _database.openConnection();
        string sql = "delete from product where product_id = @id;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }

    private async void DeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Product selectedProduct = ProductGrid.SelectedItem as Product;

        if (selectedProduct != null)
        {
            var warning = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Вы уверены что хотите удалить продукт?", ButtonEnum.YesNo);
            var result = await warning.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                Delete(selectedProduct.ProductID);
                ShowTable(_fullTable);
                var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Продукт успешно удален!", ButtonEnum.Ok);
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
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите продукт для удаления", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Product selectedProduct = ProductGrid.SelectedItem as Product;

        if (selectedProduct != null)
        {
            EditProductWindow editProductWindow = new EditProductWindow(selectedProduct);
            editProductWindow.Show();
            ShowTable(_fullTable);
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите продукт для редактирования", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }

    private void CategoryBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        CategoryWindow categoryWindow = new CategoryWindow();
        categoryWindow.Show();
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
                var currentCategory = new Category
                {
                    Name = reader.GetString("category_name")
                };
                _categories.Add(currentCategory);
            }
        }
        _database.closeConnection();
        var categoryCmb = this.Find<ComboBox>("CategoryCmb");
        categoryCmb.ItemsSource = _categories;
    }
    
    private void CategoryCmb_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var categoryCmb = (ComboBox)sender;
        var currentCategory = categoryCmb.SelectedItem as Category;
        var filteredProducts = _products.Where(x => x.Category == currentCategory.Name).ToList();
        ProductGrid.ItemsSource = filteredProducts;
    }
}