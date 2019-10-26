using CosmosCloneCommon.Utility;
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

namespace CosmicCloneUI
{
    /// <summary>
    /// Interaction logic for SourcePage.xaml
    /// </summary>
    public partial class SourcePage : Page
    {
        CosmosDBHelper cosmosHelper;
        public SourcePage()
        {
            InitializeComponent();
            cosmosHelper = new CosmosDBHelper();

            SourceURL.Text = CloneSettings.SourceSettings.EndpointUrl;
            SourceKey.Text = CloneSettings.SourceSettings.AccessKey;
            SourceDB.Text = CloneSettings.SourceSettings.DatabaseName;
            SourceCollection.Text = CloneSettings.SourceSettings.CollectionName;
        }

        private void BtnTestSource(object sender, RoutedEventArgs e)
        {
            TestSourceConnection();
        }

        public bool TestSourceConnection()
        {
            ConnectionTestMsg.Text = "";

            CloneSettings.SourceSettings.EndpointUrl = SourceURL.Text.ToString();
            CloneSettings.SourceSettings.AccessKey = SourceKey.Text.ToString();
            CloneSettings.SourceSettings.DatabaseName = SourceDB.Text.ToString();
            CloneSettings.SourceSettings.CollectionName = SourceCollection.Text.ToString();

            var result = cosmosHelper.TestSourceConnection();
            if (result.IsSuccess)
            {
                ConnectionIcon.Source = new BitmapImage(new Uri("/Images/success.png", UriKind.Relative));
                ConnectionTestMsg.Text = "Validation Passed";
            }
            else
            {
                ConnectionIcon.Source = new BitmapImage(new Uri("/Images/fail.png", UriKind.Relative));
                ConnectionTestMsg.Text = result.Message;
            }
            return result.IsSuccess;
        }
    }
}
