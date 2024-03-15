using Npgsql;
using System;
using System.Data;

public class OrderManager
{
    private string connectionString;

    public OrderManager(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void PlaceOrder(int userId, int materialId, int quantity)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO orders (user_id, material_id, quantity, order_date) VALUES (@userId, @materialId, @quantity, @orderDate)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@materialId", materialId);
                command.Parameters.AddWithValue("@quantity", quantity);
                command.Parameters.AddWithValue("@orderDate", DateTime.Now);

                command.ExecuteNonQuery();
            }
        }
    }

    public DataTable GetOrdersByUser(int userId)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM orders WHERE user_id = @userId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
    }

    public DataTable GetAllOrders()
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM orders";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
    }
}
