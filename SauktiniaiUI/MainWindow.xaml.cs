using System;
using System.Collections.Generic;
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

namespace SauktiniaiUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            if (listbox_City.SelectedIndex == -1 || String.IsNullOrEmpty(textbox_FolderPath.Text))
            {
                label_warning.Content = "Nepasirinktas aplankalas arba miestas!";
            }
            else
            {
                button_DownloadJson.IsEnabled = false;
                City city = (City)listbox_City.SelectedItem;
                string filePath = $"{textbox_FolderPath.Text}\\{city}_{DateTime.Now:yyyyMMdd}.json";
                for (int i = 0; i <= 15000; i += 100)
                {
                    string range = $"{i}-{i + 99}";

                    Dispatcher.Invoke(new Action(() => { ReportCurrentRange(range); }), DispatcherPriority.ContextIdle);

                    long fileSizeBefore = Sauktiniai.GetJsonFileSize(filePath);

                    string cmdRequest = Sauktiniai.CmdRequestString(filePath, range, (int)city);
                    Sauktiniai.CmdExecute(cmdRequest);

                    long fileSizeAfter = Sauktiniai.GetJsonFileSize(filePath);

                    //runs until no new data is entering the file
                    if (Math.Abs(fileSizeAfter - fileSizeBefore) <= 10)
                    {
                        break;
                    }
                }
                Sauktiniai.MakeProperJson(filePath, city);

                FillSqlTextbox(filePath);
                label_warning.Content = String.Empty;
                button_DownloadJson.IsEnabled = true;
            }
        }
        private void ReportCurrentRange(string range)
        {
            label_warning.Content = $"Siunčiami numeriai: {range}";
        }
        private void FillSqlTextbox(string filePath)
        {
            textbox_GeneratedSql.Text = $@"
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

/*
SELECT count(*) Skaičius
	,bdate [Gimimo data]
	,cast(count(*) * 1.0 / (
			SELECT count(*)
			FROM [TABLE_NAME]
			) AS DECIMAL(10, 4)) [Pakviestų procentas bendras]
	,sum(iif(info = 'privalomoji karo tarnyba atidėta', 1, 0)) [Tarnyba atidėta]
	,isnull(cast(1.0 * nullif((sum(iif(info = 'privalomoji karo tarnyba atidėta', 1, 0))), 0) / count(*) AS DECIMAL(10, 4)), 0) [Tarnyba atidėta procentas]
FROM [TABLE_NAME]
GROUP BY bdate
ORDER BY [Gimimo data]
 */