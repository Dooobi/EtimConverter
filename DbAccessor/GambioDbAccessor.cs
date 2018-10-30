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

        public List<GambioProperty> GetProperties()
        {
            MySqlConnection connection = GetConnection();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM properties_description";

            Dictionary<int, GambioProperty> temp = new Dictionary<int, GambioProperty>();

            MySqlDataReader reader;
            reader = command.ExecuteReader();
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

        public void InsertMissingProperties(List<EtimFeature> features)
        {
            List<GambioProperty> properties = GetProperties();

            foreach (EtimFeature feature in features)
            {
                GambioProperty foundProperty = properties.Find(property => property.GermanName == feature.Translations["de-DE"].Description);
                if (foundProperty == null)
                {
                    InsertProperty(feature);
                }
            }
        }

        public void InsertProperty(EtimFeature feature)
        {
            //MySqlConnection connection = GetConnection();

            //MySqlCommand cmd = connection.CreateCommand();
            //cmd.CommandText = "INSERT INTO properties (sort_order) VALUES (1)";
            //cmd.ExecuteNonQuery();

            //long insertedPropertyId = cmd.LastInsertedId;

            //cmd.CommandText = "INSERT INTO properties_description (properties_id, language_id, properties_name, properties_admin_name) VALUES (" + insertedPropertyId + ", 1, '" + feature.Translations["en-GB"].Description + "', '')";
            //cmd.ExecuteNonQuery();
            //cmd.CommandText = "INSERT INTO properties_description (properties_id, language_id, properties_name, properties_admin_name) VALUES (" + insertedPropertyId + ", 2, '" + feature.Translations["de-DE"].Description + "', '')";
            //cmd.ExecuteNonQuery();

            //EtimClass c = null;
            //foreach (c.Features[0].Values[0].Value.)
            //cmd.CommandText = "INSERT INTO properties_values (properties_id, sort_order, value_model, value_price) VALUES (" + insertedPropertyId + ", 1, '', 0)";

            //long insertedPropertyValueId = cmd.LastInsertedId;



            //Console.WriteLine(insertedPropertyId);
            //Console.WriteLine(insertedPropertyDescriptionId);
        }
    }
}
