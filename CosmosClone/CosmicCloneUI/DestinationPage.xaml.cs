// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
    /// Interaction logic for DestinationPage.xaml
    /// </summary>
    public partial class DestinationPage : Page
    {
        CosmosDBHelper cosmosHelper;
        public DestinationPage()
        {
            InitializeComponent();
            cosmosHelper = new CosmosDBHelper();

            TargetURL.Text = CloneSettings.TargetSettings.EndpointUrl;
            TargetKey.Text = CloneSettings.TargetSettings.AccessKey;
            TargetDB.Text = CloneSettings.TargetSettings.DatabaseName;
            TargetCollection.Text = CloneSettings.TargetSettings.CollectionName;
        }

        private void BtnTestTarget(object sender, RoutedEventArgs e)
        {
            TestDestinationConnection();
        }

        public bool TestDestinationConnection()
        {
            ConnectionTestMsg.Text = "";

            CloneSettings.TargetSettings.EndpointUrl = TargetURL.Text.ToString();
            CloneSettings.TargetSettings.AccessKey = TargetKey.Text.ToString();
            CloneSettings.TargetSettings.DatabaseName = TargetDB.Text.ToString();
            CloneSettings.TargetSettings.CollectionName = TargetCollection.Text.ToString();


            var result = cosmosHelper.TestTargetConnection();
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
