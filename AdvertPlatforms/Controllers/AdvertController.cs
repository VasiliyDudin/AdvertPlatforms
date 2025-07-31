using AdvertPlatforms.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace AdvertPlatforms.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdvertController : Controller
    {
        private AdvertisingService _service;
        public AdvertController([FromServices] AdvertisingService service) => _service = service;

        /// <summary>
        /// Получение списка рекламных площадок для заданной локации
        /// </summary>
        /// <returns></returns>
        // GET api/Request/GetAdvertis
        [HttpGet]
        public async Task<ActionResult<List<string>>> GetAdvertis(string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location) || location[0] != '/')
                    return BadRequest("Заданная локация не корректна !");

                var platforms = await _service.FindPlatformsAsync(location);//_service.FindPlatforms(location).GetAwaiter().GetResult();
                return Ok(platforms);
            }
            catch (Exception ex)
            {
                return Problem($"Ошибка поиска: {ex.Message}");
            }
        }

        /// <summary>
        /// Загрузки рекламных площадок
        /// </summary>
        /// <returns></returns>
        // PUT: api/Request/Edit/5
        [HttpPut]
        public IActionResult Load(IFormFile file)
        {
            try
            {
                // Проверка наличия файла
                if (file == null || file.Length == 0)
                    return BadRequest("Файл пустой");

                // Проверка размера файла (например, не более 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return BadRequest("Размер файла превышает - 5MB");

                string content = string.Empty;

                // Чтение содержимого файла
                using (var stream = file.OpenReadStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    content = reader.ReadToEndAsync().GetAwaiter().GetResult();
                }

                // Обработка данных
                return _service.LoadData(content) ? Ok(new { message = "Данные из файла загруженны корректно", fileName = file.FileName })
                                                  : BadRequest("Файл не корректный");
            }
            catch (Exception ex)
            {
                return Problem($"Ошибка загрузки: {ex.Message}");
            }
        }
    }
}
