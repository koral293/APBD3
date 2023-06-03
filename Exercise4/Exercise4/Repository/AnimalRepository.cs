using Exercise4.Models;
using Exercise4.Models.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Exercise4.Repository
{

    public interface IAnimalRepository
    {
        Task<bool> Exists(int id);
        Task<List<Animal>> GetAll(String orderBy);
        Task Add(AddAnimalDTO animalDTO);
        Task Update(AddAnimalDTO addAnimalDTO);
        Task Delete(int id);
    }
    public class AnimalRepository : IAnimalRepository
    {
        private readonly IConfiguration _configuration;

        public AnimalRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ICollection<Animal>> GetAll(String orderBy)
        {
            List<Animal> list = new List<Animal>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                string query = $"SELECT * FROM Animal ORDER BY {orderBy}";
                SqlCommand command = conn.CreateCommand();
                command.CommandText = query;
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new Animal
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetValue(2) == DBNull.Value ? null : reader.GetString(2),
                            Category = reader.GetString(3),
                            Area = reader.GetString(4)
                        });
                    }
                }
                return list;
            }
        }

        public async Task Add(AddAnimalDTO animalDTO)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                string query = "INSERT INTO Animal (id, name, description, category, area) values (@1, @2, @3, @4, @5)";
                SqlCommand command = conn.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@1", animalDTO.Id);
                command.Parameters.AddWithValue("@2", animalDTO.Name);
                command.Parameters.AddWithValue("@3", animalDTO.Description == null ? DBNull.Value : animalDTO.Description);
                command.Parameters.AddWithValue("@4", animalDTO.Category);
                command.Parameters.AddWithValue("@5", animalDTO.Area);
                await conn.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Update(AddAnimalDTO addAnimalDTO)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                string query = "Update Animal SET name = @2, description = @3, category = @4, area = @5 WHERE id = @1";
                SqlCommand command = conn.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@1", addAnimalDTO.Id);
                command.Parameters.AddWithValue("@2", addAnimalDTO.Name);
                command.Parameters.AddWithValue("@3", addAnimalDTO.Description);
                command.Parameters.AddWithValue("@4", addAnimalDTO.Category);
                command.Parameters.AddWithValue("@5", addAnimalDTO.Area);
                await conn.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                string query = "DELETE FROM Animal WHERE id = @1";
                SqlCommand command = conn.CreateCommand();
                command.CommandText = query;
                command.Parameters.AddWithValue("@1", id);
                await conn.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> Exists(int id)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                SqlCommand command = conn.CreateCommand();
                command.CommandText = $"SELECT id FROM Animal WHERE id = @1";
                command.Parameters.AddWithValue("@1", id);
                await conn.OpenAsync();
                if (await command.ExecuteScalarAsync() is not null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
