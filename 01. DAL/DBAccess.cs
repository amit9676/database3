//using MySql.Data;
//using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data.SqlClient;
using System.Xml;
//using DemoDataProtocol;
namespace DB3
{
    public class DBAccess //:IDAL
    {
        static string connStr = "Server=localhost\\SQLEXPRESS;Trusted_Connection=True";
        SqlConnection conn = new SqlConnection(connStr);
        private void useIceCream()
        {
            string sql = "use Icecream;";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
        private int returnID(SqlCommand cmd)
        {
            object returnObj = cmd.ExecuteScalar();
            int x = -1;
            if (returnObj != null)
            {
                int.TryParse(returnObj.ToString(), out x);
            }
            return x;
        }
        public bool createTables()
        {
            try
            {
                conn.Open();
                string sql = "DROP DATABASE IF EXISTS Icecream;";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                sql = "CREATE DATABASE Icecream;";
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                useIceCream();

                sql = "CREATE TABLE Sales (" +
                    "SaleID int IDENTITY(1,1) PRIMARY KEY," +
                    "Price INT NOT NULL," +
                    "OrderDate DATETIME NOT NULL," +
                    "OrderState BIT NOT NULL);";


                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                sql = "CREATE TABLE Ingridents (" +
                    "IngID int IDENTITY(1,1) PRIMARY KEY," +
                    "Flavor NVARCHAR(45) NOT NULL," +
                    "IngridentType NVARCHAR(45) NOT NULL);";

                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();


                sql = "CREATE TABLE Dishes (" +
                    "DishID int IDENTITY(1,1) PRIMARY KEY," +
                    "IngID int NOT NULL " +
                    "CONSTRAINT FK_Dishes_Ingridents FOREIGN KEY (IngID) REFERENCES Ingridents (IngID)," +
                    "SaleID int NOT NULL " +
                    "CONSTRAINT FK_Dishes_Sales FOREIGN KEY (SaleID) REFERENCES Sales (SaleID)," +
                    "Amount int NOT NULL);";

                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                return true;
                

            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public int insertObject(Object obj)
        {
            try
            {
                conn.Open();

                string sql = "";
                
                useIceCream();
                if (obj is Sale)
                {
                    Sale sale = (Sale)obj;
                    int tOrf = sale.getOrderState() ? 1 : 0;
                    string format = "yyyy-MM-dd HH:mm:ss";
                    sql = "INSERT INTO Sales (Price, OrderDate, OrderState) " +
                    "VALUES (" + sale.getPrice() + ", '" + sale.getOrderDate().ToString(format) + "', " + tOrf + ");"
                    +"SELECT SCOPE_IDENTITY();";
                }

                if (obj is Ingrident)
                {
                    Ingrident ingrident = (Ingrident)obj;
                    sql = "INSERT INTO Ingridents (Flavor, IngridentType) " +
                    "VALUES ('" + ingrident.getFlavor() + "', '" + ingrident.getIngridentType() + "');"
                    + "SELECT SCOPE_IDENTITY();";
                }

                if (obj is Dish)
                {
                    Dish dish = (Dish)obj;
                    sql = "INSERT INTO Dishes (IngID, SaleID, Amount) " +
                    "VALUES (" + dish.getID_ing() + ", " + dish.getID_sale() + ", " + dish.getAmount() + ");"
                    +"SELECT SCOPE_IDENTITY();";
                }

                SqlCommand cmd = new SqlCommand(sql, conn);
                int x = returnID(cmd);
                return x;
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        public Sale getSale(int id)
        {
            try
            {
                conn.Open();

                string sql = "SELECT * FROM Sales where SaleID = " + id;
                SqlCommand cmd = new SqlCommand(sql, conn);
                useIceCream();


                SqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                Sale answer = new Sale(Int32.Parse(rdr["SaleID"].ToString()), Int32.Parse(rdr["Price"].ToString()), DateTime.Parse(rdr["OrderDate"].ToString()), (bool)rdr["OrderState"]);
                return answer;
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
            
        }

        public Dish getDish(int id)
        {
            try
            {
                conn.Open();
                useIceCream();
                string sql = "SELECT * FROM Dishes where DishID = " + id;
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader rdr = cmd.ExecuteReader();

                rdr.Read();
                Object answer = rdr.GetValue(rdr.FieldCount);
                return (Dish)answer;
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
            
        }

        public Ingrident getIngrident(int id)
        {
            conn.Open();
            try
            {
                useIceCream();
                string sql = "SELECT * FROM Ingridents where IngID = " + id;
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                Ingrident answer = new Ingrident(Int32.Parse(rdr["IngID"].ToString()),rdr["Flavor"].ToString(), rdr["IngridentType"].ToString());

                return answer;
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        public int updateIngrident(Ingrident i)
        {
            try
            {
                conn.Open();
                useIceCream();
                string sql = "UPDATE Ingrident SET Flavor = " + i.getFlavor() +
                    ", IngridentType = " + i.getIngridentType() + " where IngID = " + i.getId();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                return i.getId();
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        public int updateSale(Sale s)
        {
            try
            {
                conn.Open();
                useIceCream();
                int tOrf = s.getOrderState() ? 1 : 0;
                string format = "yyyy-MM-dd HH:mm:ss";
                string sql = "UPDATE Sales SET Price = " + s.getPrice() +
                    ", OrderState = " + tOrf + ", OrderDate = '" + s.getOrderDate().ToString(format) +
                    "' where SaleID = " + s.getID() + ";";


                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                return s.getID();
            }
            catch (Exception e)
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
            
        }

        public int updateDish(Dish d)
        {
            try
            {
                conn.Open();
                useIceCream();
                string sql = "UPDATE Dishes SET ID_ing = " + d.getID_ing() +
                    ", ID_sale = " + d.getID_sale() + ", Amount = " + d.getAmount() +
                    " where DishID = " + d.getID();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                return d.getID();
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        public void deleteSale(int id)
        {
            conn.Open();
            try
            {
                useIceCream();
                string sql = "Delete from Sales where SaleID = " + id;
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
        }

        public void deleteIngrident(int id)
        {
            conn.Open();
            try
            {
                useIceCream();
                string sql = "Delete from Ingridents where IngID = " + id;
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
        }

        public void deleteDish(int id)
        {
            
            try
            {
                conn.Open();
                useIceCream();
                string sql = "Delete from Dishes where DishID = " + id;
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
        }

        public ArrayList readAll(string tableName)
        {
            ArrayList all = new ArrayList();

            try
            {
                conn.Open();

                useIceCream();
                string sql = "SELECT * FROM " + tableName;
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Object[] numb = new Object[rdr.FieldCount];
                    rdr.GetValues(numb);
                    all.Add(numb);
                }
                rdr.Close();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally { conn.Close(); }
            return all;
        }


        //managerMode
        public ArrayList dateQuery(DateTime date)
        {
            DateTime d1 = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime d2 = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            ArrayList result = new ArrayList();
            conn.Open();
            try
            {
                useIceCream();
                string format = "yyyy-MM-dd HH:mm:ss";
                string sql = "select * from Sales where OrderDate >= '" + d1.ToString(format) + "' and OrderDate <= '" + d2.ToString(format) + "';";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Object[] numb = new Object[rdr.FieldCount];
                    rdr.GetValues(numb);
                    result.Add(numb);
                }
                rdr.Close();
                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        public ArrayList favoriteIngrident()
        {
            ArrayList result = new ArrayList();
            conn.Open();
            try
            {
                useIceCream();
                string sql = "select * from Dishes join Ingridents on Ingridents.IngID = Dishes.IngID;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Object[] numb = new Object[rdr.FieldCount];
                    rdr.GetValues(numb);
                    result.Add(numb);
                }
                rdr.Close();
                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

    }
}