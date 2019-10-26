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
        public static class Constants
        {
            public const string PriorSessionName = "PriorSession";
            public const string CopyStoredProcedures = "CopyStoredProcedures";
            public const string CopyUDFs = "CopyUDFs";
            public const string CopyTriggers = "CopyTriggers";
            public const string CopyDocuments = "CopyDocuments";
            public const string CopyIndexingPolicy = "CopyIndexingPolicy";
            public const string CopyPartitionKey = "CopyPartitionKey";
            public const string ReadBatchSize = "ReadBatchSize";
            public const string WriteBatchCount = "WriteBatchCount";
            public const string EnableTextLogging = "EnableTextLogging";
            public const string SourceOfferThroughputRUs = "SourceOfferThroughputRUs";
            public const string TargetMigrationOfferThroughputRUs = "TargetMigrationOfferThroughputRUs";
            public const string TargetRestOfferThroughputRUs = "TargetRestOfferThroughputRUs";
            public const string ScrubbingRequired = "ScrubbingRequired";
        }

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
                name = Constants.PriorSessionName;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[Constants.CopyStoredProcedures].Value = CopyStoredProcedures.ToString();
            config.AppSettings.Settings[Constants.CopyUDFs].Value = CopyUDFs.ToString();
            config.AppSettings.Settings[Constants.CopyTriggers].Value = CopyTriggers.ToString();
            config.AppSettings.Settings[Constants.CopyDocuments].Value = CopyDocuments.ToString();
            config.AppSettings.Settings[Constants.CopyIndexingPolicy].Value = CopyIndexingPolicy.ToString();
            config.AppSettings.Settings[Constants.CopyPartitionKey].Value = CopyPartitionKey.ToString();
            config.AppSettings.Settings[Constants.ReadBatchSize].Value = ReadBatchSize.ToString();
            config.AppSettings.Settings[Constants.WriteBatchCount].Value = WriteBatchSize.ToString();
            config.AppSettings.Settings[Constants.EnableTextLogging].Value = EnableTextLogging.ToString();
            config.AppSettings.Settings[Constants.SourceOfferThroughputRUs].Value = SourceOfferThroughputRUs.ToString();
            config.AppSettings.Settings[Constants.TargetMigrationOfferThroughputRUs].Value = TargetMigrationOfferThroughputRUs.ToString();
            config.AppSettings.Settings[Constants.TargetRestOfferThroughputRUs].Value = TargetRestOfferThroughputRUs.ToString();
            config.AppSettings.Settings[Constants.ScrubbingRequired].Value = ScrubbingRequired.ToString();

            var cosmosConfigs = config.GetSection(CosmosCollectionDBConfigurationSection.SectionName) as CosmosCollectionDBConfigurationSection;
            var sourceConfig = cosmosConfigs.SourceCosmosDBSettings.OfType<CosmosDBSettingsElement>().Single(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            var targetConfig = cosmosConfigs.TargetCosmosDBSettings.OfType<CosmosDBSettingsElement>().Single(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            UpdateCosmosDBSettings(sourceConfig, SourceSettings);
            UpdateCosmosDBSettings(targetConfig, TargetSettings);

            config.Save(ConfigurationSaveMode.Modified);
        }

        public static void LoadSettings(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = Constants.PriorSessionName;

            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection(CosmosCollectionDBConfigurationSection.SectionName);

            var appSettings = ConfigurationManager.AppSettings;

            CopyStoredProcedures = bool.Parse(appSettings[Constants.CopyStoredProcedures]);
            CopyUDFs = bool.Parse(appSettings[Constants.CopyUDFs]);
            CopyTriggers = bool.Parse(appSettings[Constants.CopyTriggers]);
            CopyDocuments = bool.Parse(appSettings[Constants.CopyDocuments]);
            CopyIndexingPolicy = bool.Parse(appSettings[Constants.CopyIndexingPolicy]);
            CopyPartitionKey = bool.Parse(appSettings[Constants.CopyPartitionKey]);
            ReadBatchSize = int.Parse(appSettings[Constants.ReadBatchSize].ToString());
            WriteBatchSize = int.Parse(appSettings[Constants.WriteBatchCount]);
            EnableTextLogging = bool.Parse(appSettings[Constants.EnableTextLogging]);
            SourceOfferThroughputRUs = int.Parse(appSettings[Constants.SourceOfferThroughputRUs]);
            TargetMigrationOfferThroughputRUs = int.Parse(appSettings[Constants.TargetMigrationOfferThroughputRUs]);
            TargetRestOfferThroughputRUs = int.Parse(appSettings[Constants.TargetRestOfferThroughputRUs]);
            ScrubbingRequired = bool.Parse(appSettings[Constants.ScrubbingRequired]);

            SourceSettings = GetSourceSettings(name);

            TargetSettings = GetTargetSettings(name);
        }

        private static void UpdateCosmosDBSettings(CosmosDBSettingsElement configSetting, CosmosCollectionValues runSetting)
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
