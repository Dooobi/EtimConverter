using BmecatDatasourceReader.Model;
using EtimDatasourceReader;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessor
{
    public class GambioDbAccessor
    {
        private string server;
        private string dbName;
        private string user;
        private string password;

        private MySqlConnection _connection = null;

        public GambioDbAccessor(string server, string dbName, string user, string password)
        {
            this.server = server;
            this.dbName = dbName;
            this.user = user;
            this.password = password;
        }

        private MySqlConnection GetConnection()
        {
            if (_connection == null)
            {
                MySqlConnectionStringBuilder bld = new MySqlConnectionStringBuilder();
                bld.Server = server;
                bld.Database = dbName;
                bld.UserID = user;
                bld.Password = password;
                _connection = new MySqlConnection(bld.GetConnectionString(true));
                _connection.Open();
            }
            return _connection;
        }

        public GambioProperty GetPropertyById(long propertyId)
        {
            MySqlConnection connection = GetConnection();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM properties_description WHERE properties_id = " + propertyId;

            GambioProperty property = null;

            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (property == null)
                {
                    property = new GambioProperty();
                    property.PropertyId = propertyId;
                }

                int languageId = Convert.ToInt32(reader["language_id"]);
                string propertyName = Convert.ToString(reader["properties_name"]);
                string propertyAdminName = Convert.ToString(reader["properties_admin_name"]);

                switch (languageId)
                {
                    case 1:
                        property.EnglishName = propertyName;
                        property.EnglishAdminName = propertyAdminName;
                        break;
                    case 2:
                        property.GermanName = propertyName;
                        property.GermanAdminName = propertyAdminName;
                        break;
                    default:
                        Console.WriteLine("Unknown languageId (" + languageId + ") in properties_description table.");
                        break;
                }
            }
            reader.Close();
            
            return property;
        }

        public List<GambioProperty> GetProperties()
        {
            MySqlConnection connection = GetConnection();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM properties_description";

            Dictionary<int, GambioProperty> temp = new Dictionary<int, GambioProperty>();
            
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int propertyId = Convert.ToInt32(reader["properties_id"]);
                int languageId = Convert.ToInt32(reader["language_id"]);
                string propertyName = Convert.ToString(reader["properties_name"]);
                string propertyAdminName = Convert.ToString(reader["properties_admin_name"]);

                if (!temp.ContainsKey(propertyId))
                {
                    GambioProperty prop = new GambioProperty();
                    prop.PropertyId = propertyId;
                    temp[propertyId] = prop;
                }

                GambioProperty property = temp[propertyId];

                switch (languageId)
                {
                    case 1:
                        property.EnglishName = propertyName;
                        property.EnglishAdminName = propertyAdminName;
                        break;
                    case 2:
                        property.GermanName = propertyName;
                        property.GermanAdminName = propertyAdminName;
                        break;
                    default:
                        Console.WriteLine("Unknown languageId (" + languageId + ") in properties_description table.");
                        break;
                }
            }
            reader.Close();

            List<GambioProperty> properties = new List<GambioProperty>();
            foreach (GambioProperty prop in temp.Values)
            {
                properties.Add(prop);
            }
            return properties;
        }

        public List<GambioPropertyValue> GetPropertyValues(GambioProperty property)
        {
            MySqlConnection connection = GetConnection();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM properties_values_description";

            Dictionary<int, GambioPropertyValue> temp = new Dictionary<int, GambioPropertyValue>();
            
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int propertyValueId = Convert.ToInt32(reader["properties_values_id"]);
                int languageId = Convert.ToInt32(reader["language_id"]);
                string propertyValueName = Convert.ToString(reader["values_name"]);

                if (!temp.ContainsKey(propertyValueId))
                {
                    GambioPropertyValue propValue = new GambioPropertyValue();
                    propValue.PropertyId = property.PropertyId;
                    propValue.PropertyValueId = propertyValueId;
                    temp[propertyValueId] = propValue;
                }

                GambioPropertyValue propertyValue = temp[propertyValueId];

                switch (languageId)
                {
                    case 1:
                        propertyValue.EnglishName = propertyValueName;
                        break;
                    case 2:
                        propertyValue.GermanName = propertyValueName;
                        break;
                    default:
                        Console.WriteLine("Unknown languageId (" + languageId + ") in properties_values_description table.");
                        break;
                }
            }
            reader.Close();

            List<GambioPropertyValue> propertyValues = new List<GambioPropertyValue>();
            foreach (GambioPropertyValue propValue in temp.Values)
            {
                propertyValues.Add(propValue);
            }
            return propertyValues;
        }

        public void InsertMissingPropertiesAndValues(Dictionary<EtimFeature, List<ProductFeature>> featuresWithPossibleValues)
        {
            List<GambioProperty> properties = GetProperties();

            foreach (KeyValuePair<EtimFeature, List<ProductFeature>> featureWithPossibleValues in featuresWithPossibleValues)
            {
                EtimFeature feature = featureWithPossibleValues.Key;
                List<ProductFeature> possibleValues = featureWithPossibleValues.Value;

                GambioProperty foundProperty = properties.Find(property => property.GermanName == feature.Translations["de-DE"].Description);
                if (foundProperty == null)
                {
                    foundProperty = InsertProperty(feature);
                }
                
                InsertMissingPropertyValues(foundProperty, possibleValues);
            }
        }

        public void InsertMissingPropertyValues(GambioProperty property, List<ProductFeature> possibleValues)
        {
            List<GambioPropertyValue> values = GetPropertyValues(property);

            foreach (ProductFeature possibleValue in possibleValues)
            {
                GambioPropertyValue foundPropertyValue = values.Find(value => value.GermanName == possibleValue.ToPropertyValue());
                if (foundPropertyValue == null)
                {
                    InsertPropertyValue(property, possibleValue);
                }
            }
        }

        public GambioProperty InsertProperty(EtimFeature feature)
        {
            MySqlConnection connection = GetConnection();

            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO properties (sort_order) VALUES (1)";
            cmd.ExecuteNonQuery();

            long insertedPropertyId = cmd.LastInsertedId;

            cmd.CommandText = "INSERT INTO properties_description (properties_id, language_id, properties_name, properties_admin_name) VALUES (" + insertedPropertyId + ", 1, '" + feature.Translations["en-GB"].Description + "', '')";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO properties_description (properties_id, language_id, properties_name, properties_admin_name) VALUES (" + insertedPropertyId + ", 2, '" + feature.Translations["de-DE"].Description + "', '')";
            cmd.ExecuteNonQuery();
            
            return GetPropertyById(insertedPropertyId);
        }

        public void InsertPropertyValue(GambioProperty property, ProductFeature value)
        {
            MySqlConnection connection = GetConnection();
            MySqlCommand cmd = connection.CreateCommand();

            cmd.CommandText = "INSERT INTO properties_values (properties_id, sort_order, value_model, value_price) VALUES (" + property.PropertyId + ", 1, '', 0)";
            cmd.ExecuteNonQuery();

            long insertedPropertyValueId = cmd.LastInsertedId;

            cmd.CommandText = "INSERT INTO properties_values_description (properties_values_id, language_id, values_name) VALUES (" + insertedPropertyValueId + ", 1, '" + value.ToPropertyValue() + "')";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO properties_values_description (properties_values_id, language_id, values_name) VALUES (" + insertedPropertyValueId + ", 2, '" + value.ToPropertyValue() + "')";
            cmd.ExecuteNonQuery();
        }
    }
}
