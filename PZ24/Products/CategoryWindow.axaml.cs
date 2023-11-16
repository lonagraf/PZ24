using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MySql.Data.MySqlClient;
using PZ24;
using PZ24.Products;

namespace PZ24.Products;
public partial class CategoryWindow : Window
{
    private Database _database = new Database();
    private List<Category> _categories = new List<Category>();
    private string _fullTable = "select product_category_id, category_name from product_category;";
    public CategoryWindow()
    {
        InitializeComponent();
        ShowTable(_fullTable);
        Width = 300;
        Height = 300;
    }

    private void ShowTable(string sql)
    {
        _database.openConnection();
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.HasRows && reader.Read())
        {
            var currentCategory = new Category()
            {
                CategoryID = reader.GetInt32("product_category_id"),
                Name = reader.GetString("category_name")
            };
            _categories.Add(currentCategory);
        }
        _database.closeConnection();
        CategoryGrid.ItemsSource = _categories;
    }

    private void AddBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (NameTxt.Text != null) 
        {
            _database.openConnection();
            string sql = "insert into product_category (category_name) values (@category);";
            MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
            command.Parameters.AddWithValue("@category", NameTxt.Text);
            command.ExecuteNonQuery();
            var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Успешно добавлено!", ButtonEnum.Ok);
            var result = box.ShowAsync();
            _database.closeConnection();
        }
        else
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Введите название для добавления!", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }

    private void Delete(int id)
    {
        _database.openConnection();
        string sql = "delete from product_category where product_category_id = @id;";
        MySqlCommand command = new MySqlCommand(sql, _database.getConnection());
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        _database.closeConnection();
    }

    private async void DeleteBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Category selectedCategory = CategoryGrid.SelectedItem as Category;

        if (selectedCategory != null)
        {
            var warning = MessageBoxManager.GetMessageBoxStandard("Предупреждение", "Вы уверены что хотите удалить категорию?", ButtonEnum.YesNo);
            var result = await warning.ShowAsync();
            if (result == ButtonResult.Yes)
            {
                Delete(selectedCategory.CategoryID);
                ShowTable(_fullTable);
                var box = MessageBoxManager.GetMessageBoxStandard("Успешно", "Категория успешно удалена!", ButtonEnum.Ok);
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
            var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите категорию для удаления", ButtonEnum.Ok);
            var result = box.ShowAsync();
        }
    }
}