using Exercise4.Models;
using Exercise4.Models.DTO;
using Exercise4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Exercise4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly IAnimalRepository _animalRepository;

        public AnimalsController(IConfiguration configuration, IAnimalRepository animalRepository)
        {
            _configuration = configuration;
            _animalRepository = animalRepository;
        }

        // GET: api/animals
        [HttpGet]
        public async Task<IActionResult> GetAll(String? orderBy)
        {
            HashSet<string> orderByValues = new HashSet<string> { "name", "description", "category", "area" };
            orderBy ??= "name";
            if (!orderByValues.Contains(orderBy))
            {
                orderBy = "name";
            }

            List<Animal> list = await _animalRepository.GetAll(orderBy);

            if (list == null)
            {
                return Ok("");
            }
            return Ok(list);
        }

        // POST: api/animals
        [HttpPost]
        public async Task<IActionResult> Add(AddAnimalDTO animal)
        {
            if (await _animalRepository.Exists(animal.Id))
            {
                return Conflict();
            }

            await _animalRepository.Add(animal);

            return Created($"api/animals/{animal.Id}", animal);
        }

        // PUT: api/animals/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AddAnimalDTO animal)
        {
            if (!await _animalRepository.Exists(animal.Id))
            {
                return NotFound();
            }

            await _animalRepository.Update(animal);

            return Ok(animal);
        }

        // DELETE: api/animals{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _animalRepository.Exists(id))
            {
                return NotFound();
            }

            await _animalRepository.Delete(id);

            if (!await _animalRepository.Exists(id))
            {
                return Ok("Animal removed succesfully");
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

    }
}
