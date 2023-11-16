using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;

namespace PZ24.Marketing;

public partial class CampaignWindow : UserControl
{
    private Database _database = new Database();
    private List<Campaign> _campaigns = new List<Campaign>();
    private string _fullTable = "select * from marketing_campaign;";
    public CampaignWindow()
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
            var currentCampaign = new Campaign()
            {
                CampaignID  = reader.GetInt32("marketing_campaign_id"),
                Name = reader.GetString("campaign_name"),
                Goal = reader.GetString("campaign_goal"),
                StartDate = reader.GetDateTime("start_date").ToString("dd-MM-yyyy"),
                EndDate = reader.GetDateTime("end_date").ToString("dd-MM-yyyy"),
            };
            _campaigns.Add(currentCampaign);
        }
        _database.closeConnection();
        CampaignGrid.ItemsSource = _campaigns;
    }
}