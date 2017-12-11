using Farsaga.Config;
using Farsaga.GameClasses.PlayerClasses;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarsagaServer
{
    static class DatabaseHandler
    {

        public static bool PromoteUser(string name, int role)
        {
            try
            {
                int pk = GetPK(name);
                string promote = String.Format("UPDATE characterInformation SET role={0} WHERE fk = {1}", role, pk);
                string connectionString = ConnectionInfo.DATABASESTRING;
                MySqlConnection connection = new MySqlConnection(connectionString);
                MySqlCommand promoter = new MySqlCommand(promote, connection);
                connection.Open();
                promoter.ExecuteNonQuery();
                connection.Close();
                return true;
            } catch (Exception e)
            {
                return false;
            }
        }

        public static bool DemoteUser(string name, int role)
        {
            try
            {
                int pk = GetPK(name);
                string promote = String.Format("UPDATE characterInformation SET role={0} WHERE fk = {1}", role, pk);
                string connectionString = ConnectionInfo.DATABASESTRING;
                MySqlConnection connection = new MySqlConnection(connectionString);
                MySqlCommand promoter = new MySqlCommand(promote, connection);
                connection.Open();
                promoter.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static int GetPK(string name)
        {
            string pkCheck = String.Format("SELECT * FROM userAccounts WHERE username = '{0}'", name);
            //string characterCheck = "SELECT * FROM  [dbo].[characters] WHERE pk = '@pk'";
            string connectionString = ConnectionInfo.DATABASESTRING;

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand pkChecker = new MySqlCommand(pkCheck, connection);

            //SqlCommand characterChecker = new SqlCommand(characterCheck, connection);
            //characterChecker.Parameters.AddWithValue("@pk", "Your-Parm-Value");

            connection.Open();
            int count = int.Parse(pkChecker.ExecuteScalar().ToString());
            connection.Close();
            if (count > 0)
            {
                connection.Open();
                MySqlDataReader reader = pkChecker.ExecuteReader();
                try
                {
                    int pk;
                    string username;

                    while (reader.Read())
                    {
                        
                        pk = int.Parse(reader["pk"].ToString());
                        connection.Close();
                        return pk;
                    }
                } catch(Exception e)
                {
                    
                }
                connection.Close();
                return -1;
            }

            connection.Close();
            return - 1;
        }

        public static bool checkForUser(string name)
        {
            string pkCheck = String.Format("SELECT * FROM userAccounts WHERE username = '{0}'", name);
            //string characterCheck = "SELECT * FROM  [dbo].[characters] WHERE pk = '@pk'";
            string connectionString = ConnectionInfo.DATABASESTRING;

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand pkChecker = new MySqlCommand(pkCheck, connection);

            //SqlCommand characterChecker = new SqlCommand(characterCheck, connection);
            //characterChecker.Parameters.AddWithValue("@pk", "Your-Parm-Value");

            connection.Open();
            int count = int.Parse(pkChecker.ExecuteScalar().ToString());
            connection.Close();
            if (count > 0)
            {
                connection.Open();
                MySqlDataReader reader = pkChecker.ExecuteReader();
                try
                {
                    int pk;
                    string username;

                    while (reader.Read())
                    {
                        pk = int.Parse(reader["pk"].ToString());
                        username = (string)reader["username"];
                    }
                    reader.Close();
                    connection.Close();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                reader.Close();
            }
            
            connection.Close();
            return false;
        }

        public static void SavePlayer(Player player)
        {
            int pk = GetPK(player.getName());
            int x = player.GetX();
            int y = player.GetY();
            string update = String.Format("UPDATE characterInformation SET xposition={1}, yposition={2} WHERE fk = {0}", pk, player.GetX(), player.GetY());
            MySqlConnection connection = new MySqlConnection(ConnectionInfo.DATABASESTRING);
            MySqlCommand databaseGet = new MySqlCommand(update, connection);
            connection.Open();
            databaseGet.ExecuteNonQuery();
            connection.Close();
        }

        public static Player GetPlayer(string name, string playerClass)
        {
            Player player;

            int playerPK = GetPK(name);
            int x = 0;
            int y = 0;
            int id = 0;
            string pClass = playerClass;
            int pRole = 0;

            string userGetter = String.Format("SELECT * FROM characterInformation WHERE fk = {0}", playerPK);
            //string characterCheck = "SELECT * FROM  [dbo].[characters] WHERE pk = '@pk'";
            string connectionString = ConnectionInfo.DATABASESTRING;

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand databaseGet = new MySqlCommand(userGetter, connection);

            connection.Open();

            string pk;
            int count;
            object obj = databaseGet.ExecuteScalar();

            if(obj != null)
            {
                count = int.Parse(databaseGet.ExecuteScalar().ToString());
            } else
            {
                count = 0;
            }

            connection.Close();
            if (count > 0)
            {
                connection.Open();
                databaseGet = new MySqlCommand(userGetter, connection);
                MySqlDataReader reader = databaseGet.ExecuteReader();
                while(reader.Read())
                {
                    x = reader.GetInt32("xposition");
                    y = reader.GetInt32("yposition");
                    pClass = reader.GetString("selectedchar");
                    pRole = reader.GetInt32("role");
                }
            } else {
                if (pClass != CharacterSelections.NONE)
                {
                    connection.Open();
                    x = 0;
                    y = 0;
                    string addUserToCharacters = String.Format("INSERT INTO characterInformation (fk, xposition, yposition, selectedchar, role) VALUES ({0}, {1}, {2}, '{3}', {4})", playerPK, x, y, pClass, ServerRoles.PLAYER);
                    MySqlCommand addCharacter = new MySqlCommand(addUserToCharacters, connection);
                    addCharacter.ExecuteNonQuery();
                    connection.Close();
                }
            }
            
            connection.Close();
            player = new Player(0, name, x, y, false, null, pClass, pRole, null);
            return player;
        }
    }
}
