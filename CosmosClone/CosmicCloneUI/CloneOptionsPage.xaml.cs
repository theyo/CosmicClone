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
    /// Interaction logic for CloneOptionsPage.xaml
    /// </summary>
    public partial class CloneOptionsPage : Page
    {
        public CloneOptionsPage()
        {
            InitializeComponent();
        }


        public bool TestCloneOptions()
        {
            var result = true;

            CloneSettings.CopyStoredProcedures = SPs.IsChecked.Value;
            CloneSettings.CopyUDFs = UDFs.IsChecked.Value;
            CloneSettings.CopyTriggers = CosmosTriggers.IsChecked.Value;
            CloneSettings.CopyDocuments = Documents.IsChecked.Value;
            CloneSettings.CopyIndexingPolicy = IPs.IsChecked.Value;
            CloneSettings.CopyPartitionKey = PKs.IsChecked.Value;

            if (int.TryParse(OfferThroughput.Text, out int RU))
            {
                CloneSettings.TargetMigrationOfferThroughputRUs = int.Parse(OfferThroughput.Text);
            }
            else
            {
                result = false;
            }

            if (RU < 400)
                result = false;

            if (RU % 100 != 0)
                result = false;

            if (result)
            {
                ConnectionIcon.Source = new BitmapImage(new Uri("/Images/success.png", UriKind.Relative));
                ConnectionTestMsg.Text = "Validation Passed";
            }
            else
            {
                ConnectionIcon.Source = new BitmapImage(new Uri("/Images/fail.png", UriKind.Relative));
                ConnectionTestMsg.Text = "Invalid Throughput provided. Make sure it's a number greater than 400 and multiple of 100.";
            }

            return result;
        }
    }
}
