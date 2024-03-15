using Npgsql;
using System;
using System.Data;

public class MaterialManager
{
    private string connectionString;

    public MaterialManager(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void AddMaterial(string name, decimal price, string articul, int quantity, string photo)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO tovar (наименование, цена, артикул, количество, фото) VALUES (@наименование, @цена, @артикул, @количество, @фото)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@наименование", name);
                command.Parameters.AddWithValue("@цена", price);
                command.Parameters.AddWithValue("@артикул", articul);
                command.Parameters.AddWithValue("@количество", quantity);
                command.Parameters.AddWithValue("@фото", photo);

                command.ExecuteNonQuery();
            }
        }
    }

    public void UpdateMaterial(int id, string name, decimal price, string articul, int quantity, string photo)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "UPDATE tovar SET наименование = @наименование, цена = @цена, артикул = @артикул, количество = @количество, фото = @фото WHERE id = @id";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@наименование", name);
                command.Parameters.AddWithValue("@цена", price);
                command.Parameters.AddWithValue("@артикул", articul);
                command.Parameters.AddWithValue("@количество", quantity);
                command.Parameters.AddWithValue("@фото", photo);

                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteMaterial(int id)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            string query = "DELETE FROM tovar WHERE id = @id";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }
    }
}
