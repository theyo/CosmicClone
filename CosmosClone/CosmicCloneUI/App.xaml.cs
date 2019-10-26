﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.


using CosmosCloneCommon.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CosmicCloneUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CosmosDBHelper CosmosHelper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CloneSettings.LoadSettings();
            CosmosHelper = new CosmosDBHelper();
        }
    }
}
