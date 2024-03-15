using System;
using System.Data;
using Npgsql;
using static stroymogern.Form1;

namespace stroymogern
{
    public class AuthenticationManager
    {
        private string _connectionString;

        public AuthenticationManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public class User
        {
            public string Username { get; set; }
            public string Role { get; set; }
        }

        public User AuthenticateUser(string username, string password)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                // Проведем аутентификацию пользователя по логину и паролю
                string query = "SELECT id_roles FROM users WHERE логин = @логин AND пароль = @пароль";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@логин", username);
                    command.Parameters.AddWithValue("@пароль", password);

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        int roleId = Convert.ToInt32(result);

                        // Получаем роль пользователя
                        string roleQuery = "SELECT роли FROM roles WHERE id = @roleId";
                        using (NpgsqlCommand roleCommand = new NpgsqlCommand(roleQuery, connection))
                        {
                            roleCommand.Parameters.AddWithValue("@roleId", roleId);

                            object roleResult = roleCommand.ExecuteScalar();

                            if (roleResult != null && roleResult != DBNull.Value)
                            {
                                string roleName = Convert.ToString(roleResult);
                                // Возвращаем экземпляр пользователя с его ролью
                                return new User { Username = username, Role = roleName };
                            }
                        }
                    }
                }
            }

            return null; // Если пользователь не найден или аутентификация не удалась
        }
    }
}
