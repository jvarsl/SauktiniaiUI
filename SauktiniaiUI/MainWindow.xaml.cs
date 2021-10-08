using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using SauktiniaiUI.Models;

namespace SauktiniaiUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApiHelper.InitializeApiClient();
        }

        private void button_SelectPath_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textbox_FolderPath.Text = dialog.FileName;
            }
        }
        private async void button_DownloadJson_Click(object sender, RoutedEventArgs e)
        {
            if (listbox_City.SelectedIndex == -1 || string.IsNullOrEmpty(textbox_FolderPath.Text))
            {
                label_warning.Content = "Nepasirinktas aplankalas arba miestas";
                return;
            }

            button_DownloadJson.IsEnabled = false;
            City city = (City)listbox_City.SelectedItem;
            string filePath = $"{textbox_FolderPath.Text}\\{city}_{DateTime.Now:yyyyMMdd}.json";

            // the website uses 0-99 ranges for each page increasing by 100
            // once the range is over the possible entries nothing will get returned and loop will exit-break
            var wholeEnlistedPeopleList = new List<EnlistedPerson>();
            for (int rangeNumber = 0; ; rangeNumber += 100)
            {
                var range = $"{rangeNumber}-{rangeNumber + 99}";

                ReportCurrentRange(range);

                var newEntries = await MilitantsProcessor.GetMilitantsPageDataAsync(city, range);
                wholeEnlistedPeopleList.AddRange(newEntries);

                if (!newEntries.Any())
                {
                    break;
                }
            }

            wholeEnlistedPeopleList.ForEach(x => x.RegionNumber = city);
            var json = JsonConvert.SerializeObject(wholeEnlistedPeopleList);
            File.WriteAllText(filePath, json, Encoding.Unicode);

            FillSqlTextbox(filePath);
            label_warning.Content = string.Empty;
            button_DownloadJson.IsEnabled = true;
        }

        private void ReportCurrentRange(string range)
        {
            label_warning.Content = $"Siunčiami numeriai: {range}";
        }

        private void FillSqlTextbox(string filePath)
        {
            textbox_GeneratedSql.Text =
                  $@"
                    DECLARE @JSON NVARCHAR(MAX)

                    SELECT @JSON = BulkColumn
                    FROM OPENROWSET(BULK '{filePath}', SINGLE_NCLOB) AS j

                    SELECT *
                    -- INTO [YOUR_NEW_TABLE_NAME]
                    FROM OPENJSON(@json) WITH (
		                     [pos] INT
		                    ,[number] VARCHAR(100)
		                    ,[name] VARCHAR(100)
		                    ,[lastname] VARCHAR(100)
		                    ,[bdate] SMALLINT
		                    ,[department] VARCHAR(50)
		                    ,[info] VARCHAR(max)
		                    ,[date] DATE
		                    ,[region] VARCHAR(50)
		                    ,[regionNo] SMALLINT
		                    );";
        }
    }
}
