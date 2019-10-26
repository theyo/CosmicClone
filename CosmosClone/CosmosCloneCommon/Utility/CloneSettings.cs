// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using logger = CosmosCloneCommon.Utility.CloneLogger;

namespace CosmosCloneCommon.Utility
{

    public static class CloneSettings
    {
        private const string PriorSessionName = "PriorSession";

        public static bool CopyStoredProcedures { get; set; }
        public static bool CopyUDFs { get; set; }
        public static bool CopyTriggers { get; set; }
        public static bool CopyDocuments { get; set; }
        public static bool CopyIndexingPolicy { get; set; }
        public static bool CopyPartitionKey { get; set; }
        public static int ReadBatchSize { get; set; }
        public static int WriteBatchSize { get; private set; }
        public static bool EnableTextLogging { get; set; }
        public static bool ScrubbingRequired { get; set; }
        public static int SourceOfferThroughputRUs { get; set; }
        public static int TargetMigrationOfferThroughputRUs { get; set; }
        public static int TargetRestOfferThroughputRUs { get; set; }
        public static CosmosCollectionValues SourceSettings { get; private set; }
        public static CosmosCollectionValues TargetSettings { get; private set; }

        public static List<string> AvailableSourceSettings()
        {
            return GetCosmosConfigSection().SourceCosmosDBSettings.OfType<CosmosDBSettingsElement>().Select(e => e.Name).ToList();
        }

        public static List<string> AvailableTargetSettings()
        {
            return GetCosmosConfigSection().TargetCosmosDBSettings.OfType<CosmosDBSettingsElement>().Select(e => e.Name).ToList();
        }

        public static CosmosCollectionValues GetSourceSettings(string name)
        {
            var sourceConfig = GetCosmosConfigSection().SourceCosmosDBSettings.OfType<CosmosDBSettingsElement>().SingleOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (sourceConfig == null)
                throw new Exception($"Source Config with Name: `{name}` not found");

            return CosmosCollectionValues.CreateFromConfiguration(sourceConfig);
        }

        public static CosmosCollectionValues GetTargetSettings(string name)
        {
            var targetConfig = GetCosmosConfigSection().TargetCosmosDBSettings.OfType<CosmosDBSettingsElement>().SingleOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (targetConfig == null)
                throw new Exception($"Target Config with Name: `{name}` not found");

            return CosmosCollectionValues.CreateFromConfiguration(targetConfig);
        }

        public static void SaveSettings(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = PriorSessionName;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["CopyStoredProcedures"].Value = CopyStoredProcedures.ToString();
            config.AppSettings.Settings["CopyUDFs"].Value = CopyUDFs.ToString();
            config.AppSettings.Settings["CopyTriggers"].Value = CopyTriggers.ToString();
            config.AppSettings.Settings["CopyDocuments"].Value = CopyDocuments.ToString();
            config.AppSettings.Settings["CopyIndexingPolicy"].Value = CopyIndexingPolicy.ToString();
            config.AppSettings.Settings["CopyPartitionKey"].Value = CopyPartitionKey.ToString();
            config.AppSettings.Settings["ReadBatchSize"].Value = ReadBatchSize.ToString();
            config.AppSettings.Settings["WriteBatchCount"].Value = WriteBatchSize.ToString();
            config.AppSettings.Settings["EnableTextLogging"].Value = EnableTextLogging.ToString();
            config.AppSettings.Settings["SourceOfferThroughputRUs"].Value = SourceOfferThroughputRUs.ToString();
            config.AppSettings.Settings["TargetMigrationOfferThroughputRUs"].Value = TargetMigrationOfferThroughputRUs.ToString();
            config.AppSettings.Settings["TargetRestOfferThroughputRUs"].Value = TargetRestOfferThroughputRUs.ToString();
            config.AppSettings.Settings["ScrubbingRequired"].Value = ScrubbingRequired.ToString();

            var cosmosConfigs = config.GetSection(CosmosCollectionDBConfigurationSection.SectionName) as CosmosCollectionDBConfigurationSection;
            var sourceConfig = cosmosConfigs.SourceCosmosDBSettings.OfType<CosmosDBSettingsElement>().Single(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            var targetConfig = cosmosConfigs.TargetCosmosDBSettings.OfType<CosmosDBSettingsElement>().Single(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            UpdateSettings(sourceConfig, SourceSettings);
            UpdateSettings(targetConfig, TargetSettings);

            config.Save(ConfigurationSaveMode.Modified);
        }

        public static void LoadSettings(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = PriorSessionName;

            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection(CosmosCollectionDBConfigurationSection.SectionName);

            var appSettings = ConfigurationManager.AppSettings;

            CopyStoredProcedures = bool.Parse(appSettings["CopyStoredProcedures"]);
            CopyUDFs = bool.Parse(appSettings["CopyUDFs"]);
            CopyTriggers = bool.Parse(appSettings["CopyTriggers"]);
            CopyDocuments = bool.Parse(appSettings["CopyDocuments"]);
            CopyIndexingPolicy = bool.Parse(appSettings["CopyIndexingPolicy"]);
            CopyPartitionKey = bool.Parse(appSettings["CopyPartitionKey"]);
            ReadBatchSize = int.Parse(appSettings["ReadBatchSize"].ToString());
            WriteBatchSize = int.Parse(appSettings["WriteBatchCount"]);
            EnableTextLogging = bool.Parse(appSettings["EnableTextLogging"]);
            SourceOfferThroughputRUs = int.Parse(appSettings["SourceOfferThroughputRUs"]);
            TargetMigrationOfferThroughputRUs = int.Parse(appSettings["TargetMigrationOfferThroughputRUs"]);
            TargetRestOfferThroughputRUs = int.Parse(appSettings["TargetRestOfferThroughputRUs"]);
            ScrubbingRequired = bool.Parse(appSettings["ScrubbingRequired"]);

            SourceSettings = GetSourceSettings(name);

            TargetSettings = GetTargetSettings(name);
        }

        private static void UpdateSettings(CosmosDBSettingsElement configSetting, CosmosCollectionValues runSetting)
        {
            configSetting.EndpointUrl = runSetting.EndpointUrl;
            configSetting.AccessKey = runSetting.AccessKey;
            configSetting.DatabaseName = runSetting.DatabaseName;
            configSetting.CollectionName = runSetting.CollectionName;
            configSetting.OfferThroughputRUs = runSetting.OfferThroughputRUs;
        }

        private static CosmosCollectionDBConfigurationSection GetCosmosConfigSection()
        {
            return ConfigurationManager.GetSection(CosmosCollectionDBConfigurationSection.SectionName) as CosmosCollectionDBConfigurationSection;
        }
    }

    public class CosmosCollectionValues
    {
        public string EndpointUrl { get; set; }
        public string AccessKey { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public int OfferThroughputRUs { get; set; }

        public static CosmosCollectionValues CreateFromConfiguration(CosmosDBSettingsElement config)
        {
            return new CosmosCollectionValues()
            {
                EndpointUrl = config.EndpointUrl,
                AccessKey = config.AccessKey,
                DatabaseName = config.DatabaseName,
                CollectionName = config.CollectionName,
                OfferThroughputRUs = config.OfferThroughputRUs

            };
        }
    }
}
