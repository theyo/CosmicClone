using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosCloneCommon.Utility
{
    public class CosmosCollectionDBConfigurationSection : ConfigurationSection
    {
        public const string SectionName = "cosmosCollectionDBConfigurationSection";

        private const string SourceCosmosDBSettingsName = "sourceCosmosDBSettings";
        private const string TargetCosmosDBSettingsName = "targetCosmosDBSettings";

        [ConfigurationProperty(SourceCosmosDBSettingsName)]
        [ConfigurationCollection(typeof(CosmosDBSettingsCollection), AddItemName = "add")]
        public CosmosDBSettingsCollection SourceCosmosDBSettings { get { return (CosmosDBSettingsCollection)base[SourceCosmosDBSettingsName]; } }

        [ConfigurationProperty(TargetCosmosDBSettingsName)]
        [ConfigurationCollection(typeof(CosmosDBSettingsCollection), AddItemName = "add")]
        public CosmosDBSettingsCollection TargetCosmosDBSettings { get { return (CosmosDBSettingsCollection)base[TargetCosmosDBSettingsName]; } }
    }

    public class CosmosDBSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CosmosDBSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CosmosDBSettingsElement)element).Name;
        }
    }

    public class CosmosDBSettingsElement : ConfigurationElement
    {

        [ConfigurationProperty(nameof(Name), IsRequired = false)]
        public string Name
        {
            get { return (string)this[nameof(Name)]; }
            set { this[nameof(Name)] = value; }
        }

        [ConfigurationProperty(nameof(EndpointUrl), IsRequired = false)]
        public string EndpointUrl
        {
            get { return (string)this[nameof(EndpointUrl)]; }
            set { this[nameof(EndpointUrl)] = value; }
        }

        [ConfigurationProperty(nameof(AccessKey), IsRequired = false)]
        public string AccessKey
        {
            get { return (string)this[nameof(AccessKey)]; }
            set { this[nameof(AccessKey)] = value; }
        }

        [ConfigurationProperty(nameof(DatabaseName), IsRequired = false)]
        public string DatabaseName
        {
            get { return (string)this[nameof(DatabaseName)]; }
            set { this[nameof(DatabaseName)] = value; }
        }

        [ConfigurationProperty(nameof(CollectionName), IsRequired = false)]
        public string CollectionName
        {
            get { return (string)this[nameof(CollectionName)]; }
            set { this[nameof(CollectionName)] = value; }
        }

        [ConfigurationProperty(nameof(OfferThroughputRUs), IsRequired = false)]
        public int OfferThroughputRUs
        {
            get { return (int)(this[nameof(OfferThroughputRUs)] ?? 400); }
            set { this[nameof(OfferThroughputRUs)] = value; }
        }

        [ConfigurationProperty(nameof(ReadDelayBetweenRequestsInMs), IsRequired = false)]
        public int ReadDelayBetweenRequestsInMs
        {
            get { return (int)(this[nameof(ReadDelayBetweenRequestsInMs)] ?? 2000); }
            set { this[nameof(ReadDelayBetweenRequestsInMs)] = value; }
        }

        [ConfigurationProperty(nameof(ConnString), IsRequired = false)]
        public string ConnString
        {
            get { return (string)this[nameof(ConnString)]; }
            set { this[nameof(ConnString)] = value; }
        }

        [ConfigurationProperty(nameof(IsFixedCollection), IsRequired = false)]
        public bool IsFixedCollection
        {
            get { return (bool)(this[nameof(IsFixedCollection)] ?? false); }
            set { this[nameof(IsFixedCollection)] = value; }
        }
    }
}
