using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

//I SHOULD HAVE USED PERAMETERS BUT 300+ LINES LATER I DON'T FEEL LIKE CHANGING IT RIGHT NOW
namespace Minu
{
    public class SQLiteHelper
    {
        public SQLiteConnection dbConnection { get; private set; }
        private SQLiteCommand command;

        /// <summary>
        /// Create instance of SQLiteHelper.  Creates a database file if the supplied file is npt already present
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public SQLiteHelper(string connectionString)
        {
            //Create a database file if one does not already exist
            if (!File.Exists(connectionString))
                SQLiteConnection.CreateFile(connectionString);
            //Create connection and open.  Not sure if this is the best way, as it is always open, but reader needs it to be open.  Could use an enumerable but that's effort
            connectionString = "Data source=" + connectionString + ";Version=3;";
            dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
        }

        /// <summary>
        /// Used to make SQL queries that do not return information.  Return the number of rows affected, 0 if error.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>SQL query</returns>
        public int SendNonQuery(string sql)
        {
            try
            {
                command = new SQLiteCommand(sql, dbConnection);
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public object SendScalarQuery(string sql)
        {
            try
            {
                command = new SQLiteCommand(sql, dbConnection);
                return command.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Used to make SQL queries that return information.  Retun the ExecuteReader to handle information coming out.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>SQL query</returns>
        public SQLiteDataReader SendReaderQuery(string sql)
        {
            try
            {
                command = new SQLiteCommand(sql, dbConnection);
                return command.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Close connection to database
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                dbConnection.Close();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Open connection to database
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                dbConnection.Open();
            }
            catch (Exception) { }
        }

        //TODO:
        //SEPARATE BINDS FROM CONSTRUCTION OF SQL QUERIES?  ALLOWS FOR CUSTOM ORDERBY WHERE ETC.  
        //WORK BUT WOULD BE SMOOTH
        //FUNC 1:
        //  CONSTRUCT SQL
        //  EXECUTE COMMAND
        //  RETURN READER
        //FUNC 2:
        //  TAKE CLASS AND READER
        //  BASICALLY THE MAIN BIT OF THE TWO EXISITING FUNCTIONS

        public List<T> BindRecordToClass<T>(string table, string where = null) where T : new()
        {
            string whereState = "";

            if(where != null)
            {
                //pad where with spaces
                whereState = " " + where;
            }
            //Count rows in table
            /*command = new SQLiteCommand("select count(id) from " + table + whereState + ";", dbConnection);
            int rowCount = 0;
            rowCount = Convert.ToInt32(command.ExecuteScalar());*/

            //Get all values from table
            command = new SQLiteCommand("select * from " + table + whereState + ";", dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            return BindReaderToClass<T>(reader);
        }

        /// <summary>provided
        /// Attepmt to bind values in a  class to values from a database table
        /// </summary>
        /// <typeparam name="T">Class to bind to</typeparam>
        /// <param name="table">Table to take values from</param>
        /// <returns></returns>
        public List<T> BindReaderToClass<T>(SQLiteDataReader reader) where T : new()
        {
            //Initialise a class of T.  
            //This means the method can only take classes that are perameterless, which should be fine for models
            List<T> returnClassList = new List<T>();

            //Get properties from class
            PropertyInfo[] props = typeof(T).GetProperties();

            //loop through items in database and compare them to class properties
            while (reader.Read())
            {
                T tempClass = new T();
                //Loop through properties in class
                foreach (PropertyInfo property in props)
                {
                    //Try to get property of class from database.  If the field isn't there, set to null
                    try
                    {
                        if(property.PropertyType == typeof(DateTime))
                        {
                            DateTime date = DateTime.Parse((string)reader[property.Name]);
                            property.SetValue(tempClass, date);
                        }
                        else if(property.PropertyType == typeof(bool) || property.PropertyType == typeof(Boolean))
                        {
                            if(Convert.ToInt32(reader[property.Name]) == 1)
                            {
                                property.SetValue(tempClass, true);
                            }
                            else
                            {
                                property.SetValue(tempClass, false);
                            }
                        }
                        else if(property.PropertyType == typeof(Guid))
                        {
                            property.SetValue(tempClass, Guid.Parse((string)reader[property.Name]));
                        }
                        else
                        {
                            property.SetValue(tempClass, Convert.ChangeType(reader[property.Name], property.PropertyType));
                        }
                    }
                    catch (Exception)
                    {
                        //throw e;
                        property.SetValue(tempClass, null);
                    }
                }
                //Append class to list
                returnClassList.Add(tempClass);
            }
            Console.WriteLine("Done binding");
            return (returnClassList);
        }

        /// <summary>
        /// Insert a class into the database by building an INSERT query.
        /// The class must have identical properties to the table being inserted into.
        /// </summary>
        /// <typeparam name="T">Class type to insert</typeparam>
        /// <param name="data">Object containing data</param>
        /// <param name="table">Name of table to insert into</param>
        /// <returns></returns>
        public int InsertRecord<T>(T data, string table)
        {
            //Set up command string
            string commandStr = "insert into " + table + " (";

            //Get properties from class
            PropertyInfo[] props = typeof(T).GetProperties();

            //Loop through properties
            foreach(PropertyInfo property in props)
            {
                commandStr += property.Name.ToString() + ", ";
            }

            //Trim trailing comma and space off the end of the command string and add values section
            commandStr = commandStr.Remove(commandStr.Length - 2);
            //commandStr.TrimEnd(new char[] { ',', ' ' });
            commandStr += ") values (";

            //Get property values for properties in object and add them to the command string
            foreach(PropertyInfo property in props)
            {
                //thx joosh i can;t debug
                if(property.PropertyType == typeof(string))
                {
                    commandStr += '\'' + property.GetValue(data, null).ToString() + '\'' + ", ";
                }
                else if(property.PropertyType == typeof(DateTime))
                {
                    DateTime dt = (DateTime)property.GetValue(data, null);
                    commandStr += '\'' + dt.ToString("yyyy-MM-dd") + '\'' + ", ";
                }
                else if(property.PropertyType == typeof(bool) || property.PropertyType == typeof(Boolean))
                {
                    //Deal with bools as 0 or 1
                    var val = property.GetValue(data, null);
                    if ((bool)val)
                    {
                        commandStr += "1, ";
                    }
                    else
                    {
                        commandStr += "0, ";
                    }
                }
                else
                {
                    commandStr += property.GetValue(data, null).ToString() + ", ";
                }
            }

            //Trim trailing comma and space off the end of the command string
            commandStr = commandStr.Remove(commandStr.Length - 2);

            //Finish command and execute
            commandStr += ");";
            command = new SQLiteCommand(commandStr, dbConnection);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Create table based off a class.  
        /// Currentyly ONLY WORKS FOR INT, STRING AND DATETIME
        /// </summary>
        /// <typeparam name="T">Datatype to model table from</typeparam>
        /// <param name="name">Name of table to create</param>
        /// <returns></returns>
        public int CreateTable<T>(string name)
        {
            //Set up command string
            string commandStr = "create table " + name + "(";

            //Get properties from class
            PropertyInfo[] props = typeof(T).GetProperties();

            //Get names and types of proerties and add to command string
            foreach (PropertyInfo property in props)
            { 
                if (property.PropertyType == typeof(int))
                {
                    //If the name of the property is a varient of "id" make it the primary key
                    //Clumsy, but eh
                    if(property.Name.ToUpper() == "ID")
                    {
                        commandStr += property.Name + " integer primary key, ";
                    }
                    else
                    {
                        commandStr += property.Name + " integer, ";
                    }
                }
                else if(property.PropertyType == typeof(float))
                {
                    commandStr += property.Name + " real, ";
                }
                else if (property.PropertyType == typeof(int))
                {
                    commandStr += property.Name + " integer, ";
                }
                else
                {
                    commandStr += property.Name + " text, ";
                }
            }
            //Trim trailing comma and space off the end of the command string
            commandStr = commandStr.Remove(commandStr.Length - 2);

            //Finish command and execute
            commandStr += ");";
            command = new SQLiteCommand(commandStr, dbConnection);
            return command.ExecuteNonQuery();
        }
    }
}