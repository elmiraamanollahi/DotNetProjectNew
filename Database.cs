﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace PointOfSaleManagementSys
{
    public class Database
    {
        SqlConnection conn;

        public Database()
        {
            conn = new SqlConnection(@"Server=tcp:mike-jac.database.windows.net,1433;Initial Catalog=POS;
Persist Security Info=False;User ID=dbadmin;Password=Elmira2000;MultipleActiveResultSets=False;
Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            conn.Open();
        }

        public Shopping GetProductbyId(int CategoryId, int ProductId)
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM Products where CategoryId=" + CategoryId + " and productId=" + ProductId, conn))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string name = (string)reader["productName"];
                    decimal price = (decimal)reader["unitprice"];
                    int Id = (int)reader["productId"];
                    Shopping p = new Shopping(Id, name, 1, price, 4, 3, 1);
                    return p;
                }
                return null;
            }
        }

        public List<OrderList> GetAllOrderList(int Id)
        {
            List<OrderList> result = new List<OrderList>();
            using (SqlCommand command = new SqlCommand("SELECT * FROM OrderList where orderId=" + Id, conn))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int orderId = (int)reader["orderId"];
                    int productId = (int)reader["productId"];
                    int quantity = (int)reader["quantity"];
                    decimal unitprice = (decimal)reader["unitprice"];
                    OrderList o = new OrderList(orderId, productId, quantity, unitprice, 0.1m);
                    result.Add(o);
                }
            }
            return result;
        }
        public List<Order> GetAllOrders()
        {
            List<Order> result = new List<Order>();
            using (SqlCommand command = new SqlCommand("SELECT * FROM Orders", conn))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int orderId = (int)reader["orderId"];
                    int empId = (int)reader["empId"];
                    DateTime orderDate = (DateTime)reader["orderDate"];
                    int customerId = (int)reader["customerId"];
                    decimal totalPrice = (decimal)reader["totalPrice"];
                    string paymentMethod = (string)reader["paymentMethod"];
                    int invoiceNr = (int)reader["invoiceNr"];
                    Order ol = new Order(orderId, empId, orderDate, customerId, totalPrice, paymentMethod, invoiceNr);
                    result.Add(ol);
                }
            }
            return result;
        }
        public List<InStock> GetAllProducts()
        {
            List<InStock> result = new List<InStock>();
            try
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM  Products", conn))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int Id = (int)reader["productId"];
                        int categoryId = (int)reader["CategoryId"];
                        string productName = (string)reader["productName"];
                        decimal unitPrice = (decimal)reader["unitPrice"];
                        decimal salePrice = (decimal)reader["PurchasedPrice"];
                        int unitInStock = (int)reader["unitinstock"];
                        int trigger = (int)reader["TriggerLevel"];
                        string vendor = (string)reader["Vendor"];
                        //string vendorAddress = (string)reader["VendorAddress"];
                        DateTime expDate = (DateTime)reader["ExpiryDate"];
                        InStock o = new InStock(Id, categoryId, productName, unitPrice, salePrice, unitInStock, trigger, vendor, expDate.Date);
                        result.Add(o);
                    }
                }
            }

            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
                MessageBox.Show("Error opening database connection: " + e.Message);
                Environment.Exit(1);
            }
            return result;
        }
        public List<Employee> GetAllEmployees()
        {
            List<Employee> result = new List<Employee>();
            using (SqlCommand command = new SqlCommand("SELECT * FROM Employees", conn))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int empId = (int)reader["empId"];
                    string firstName = (string)reader["firstName"];
                    string lastName = (string)reader["lastName"];
                    string userName = (string)reader["userName"];
                    string PSword = (string)reader["PSword"];
                    decimal salary = (decimal) reader["salary"];
                    Employee ep = new Employee(empId, firstName, lastName, userName, PSword,salary);
                    result.Add(ep);
                }
            }
            return result;
        }
        public List<Categories> GetAllCategory()
        {
            List<Categories> result = new List<Categories>();
            using (SqlCommand command = new SqlCommand("SELECT * FROM categories", conn))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = (int)reader["CategoryId"];
                    string name = (string)reader["categoryName"];
                    Categories p = new Categories(id, name);
                    result.Add(p);
                }
            }
            return result;
        }

        public void UpdateOrderList(OrderList o)
        {
            using (SqlCommand cmd = new SqlCommand(
            "UPDATE OrderList SET quantity = @quantity WHERE productId=@productId and orderId=@orderId", conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = o.Quantity;
                cmd.Parameters.Add("@orderId", SqlDbType.Int).Value = o.OrderId;
                cmd.Parameters.Add("@productId", SqlDbType.Int).Value = o.ProductId;
                cmd.ExecuteNonQuery();
            }
        }

        public void AddProduct(InStock ins)
        {
            string sql = "INSERT INTO Products (orderId, productId, quantity, unitPrice) VALUES (@orderId, @productId, @quantity, @unitPrice)";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@productId", SqlDbType.Int).Value = ins.Id;
            cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = ins.Quantity;
            cmd.Parameters.Add("@unitPrice", SqlDbType.Decimal).Value = ins.UnitPrice;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }
        public void AddOrderList(OrderList o)
        {
            string sql = "INSERT INTO Orderlist (orderId, productId, quantity, unitPrice) VALUES (@orderId, @productId, @quantity, @unitPrice)";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@orderId", SqlDbType.Int).Value = o.OrderId;
            cmd.Parameters.Add("@productId", SqlDbType.Int).Value = o.ProductId;
            cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = o.Quantity;
            cmd.Parameters.Add("@unitPrice", SqlDbType.Decimal).Value = o.UnitPrice;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }
        public void AddEmployee(Employee e)
        {
            string sql = "INSERT INTO Employees (FirstName, LastName, Username, PSword, salary) VALUES (@firstName, @lastName, @userName, @pSword,@salary)";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@firstName", SqlDbType.Text).Value = e.FirstName;
            cmd.Parameters.Add("@lastName", SqlDbType.Text).Value = e.LastName;
            cmd.Parameters.Add("@userName", SqlDbType.Text).Value = e.UserName;
            cmd.Parameters.Add("@pSword", SqlDbType.Text).Value = e.PSword;
            cmd.Parameters.Add("@salary", SqlDbType.Money).Value = e.Salary;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }
        public void AddOrder(Order o)
        {
            string sql = "INSERT INTO Orders (orderId, empId, orderDate,customerId, totalPrice, paymentMethod, invoiceNr) VALUES (@orderId, @empId, @orderDate, @customerId, @totalPrice, @paymentMethod, @invoiceNr)";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@orderId", SqlDbType.Int).Value = o.OrderId;
            cmd.Parameters.Add("@empId", SqlDbType.Int).Value = o.EmpId;
            cmd.Parameters.Add("@orderDate", SqlDbType.DateTime).Value = o.OrderDate;
            cmd.Parameters.Add("@customerId", SqlDbType.Int).Value = o.CustomerId;
            cmd.Parameters.Add("@paymentMethod", SqlDbType.Text).Value = o.PaymentMethod;
            cmd.Parameters.Add("@totalPrice", SqlDbType.Money).Value = o.TotalPrice;
            cmd.Parameters.Add("@invoiceNr", SqlDbType.Int).Value = o.InvoiceNr;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }


        public void DeleteOrderListById(int Id)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM orderlist WHERE productId=@Id", conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteOrderById(int Id)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM orders WHERE orderId=@Id", conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteEmployeeById(int Id)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM employees WHERE empId=@Id", conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteProductById(int Id)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM products WHERE productId=@Id", conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeductProductById(int Id, int quantity)
        {
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE products SET unitinstock = unitinstock-@quantity WHERE productId=@productId", conn))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.Add("@productId", SqlDbType.Int).Value = Id;
                cmd.Parameters.Add("@quantity", SqlDbType.Int).Value = quantity;
                cmd.ExecuteNonQuery();
            }
        }
        public Boolean ProductStockById(int Id)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT * from products where productId=" + Id, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int unitinstock = (int)reader["unitinstock"];
                    int triggerlevel = (int)reader["triggerlevel"];
                    if (unitinstock <= triggerlevel)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string ValidPassword(string username, string password)
        {
            //using (SqlCommand command = new SqlCommand("select * from Employees WHERE username='" + username + "' and psword='" + password+"'", conn))
            //   string sql = "SELECT * FROM Employees username=`" + username + "` and psword=`" + password + "`";
            using (SqlCommand command = new SqlCommand("SELECT * FROM Employees where username='" + username + "' and psword='" + password + "'", conn))
            //    using (SqlCommand command = new SqlCommand("SELECT * FROM Employees where username='mikeyu'  and psword='monk6500'", conn))

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    string name = (string)reader["FirstName"];
                    Console.WriteLine(name);
                    return name;
                }
                return "you are wrong!!!";
            }
        }
        public int MaxOrderId()
        {
            int maxOrderId = 0;
            using (SqlCommand command = new SqlCommand("SELECT MAX(orderId) as maxId FROM Orders", conn))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    maxOrderId = (int)reader["maxId"];
                }
            }
            return maxOrderId;
        }

    }
}
