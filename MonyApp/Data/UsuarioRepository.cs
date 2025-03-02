using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MonyApp.Models;

namespace MonyApp.Data
{
    public class UsuarioRepository
    {
        private readonly MySqlDatabase _database;

        public UsuarioRepository(MySqlDatabase database)
        {
            _database = database;
        }

        public async Task<bool> ExisteEmail(string email)
        {
            using (var connection = _database.GetConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT COUNT(*) FROM usuarios WHERE email = @Email";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using (var connection = _database.GetConnection())
            {
                await connection.OpenAsync();

                var query = @"INSERT INTO usuarios 
                          (nombre_completo, email, telefono, password, fecha_registro, activo) 
                          VALUES (@NombreCompleto, @Email, @Telefono, @Password, @FechaRegistro, @Activo);
                          SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreCompleto", usuario.NombreCompleto);
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@Telefono", usuario.Telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Password", usuario.Password);
                    command.Parameters.AddWithValue("@FechaRegistro", usuario.FechaRegistro);
                    command.Parameters.AddWithValue("@Activo", usuario.Activo);

                    var id = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return id;
                }
            }
        }

        public async Task<Usuario> ObtenerUsuarioPorId(int id)
        {
            using (var connection = _database.GetConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM usuarios WHERE id = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Usuario
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                NombreCompleto = reader["nombre_completo"].ToString(),
                                Email = reader["email"].ToString(),
                                Telefono = reader["telefono"] == DBNull.Value ? null : reader["telefono"].ToString(),
                                Password = reader["password"].ToString(),
                                FechaRegistro = Convert.ToDateTime(reader["fecha_registro"]),
                                Activo = Convert.ToBoolean(reader["activo"])
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public async Task<Usuario> ObtenerUsuarioPorEmail(string email)
        {
            using (var connection = _database.GetConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM usuarios WHERE email = @Email";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Usuario
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                NombreCompleto = reader["nombre_completo"].ToString(),
                                Email = reader["email"].ToString(),
                                Telefono = reader["telefono"] == DBNull.Value ? null : reader["telefono"].ToString(),
                                Password = reader["password"].ToString(),
                                FechaRegistro = Convert.ToDateTime(reader["fecha_registro"]),
                                Activo = Convert.ToBoolean(reader["activo"])
                            };
                        }
                        return null;
                    }
                }
            }
        }
    }
}