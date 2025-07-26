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
        public IResult GetAdvertis(string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location) || location[0] != '/')
                    return Results.BadRequest("Заданная локация не корректна !");

                var platforms = _service.FindPlatforms(location).GetAwaiter().GetResult();
                return Results.Ok(platforms);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Ошибка поиска: {ex.Message}");
            }
        }

        /// <summary>
        /// Загрузки рекламных площадок
        /// </summary>
        /// <returns></returns>
        // PUT: api/Request/Edit/5
        [HttpPut]
        public IResult Load(IFormFile file)
        {
            try
            {
                // Проверка наличия файла
                if (file == null || file.Length == 0)
                    return Results.BadRequest("Файл пустой");

                // Проверка размера файла (например, не более 5MB)
                if (file.Length > 5 * 1024 * 1024)
                    return Results.BadRequest("Размер файла превышает - 5MB");

                string content = string.Empty;

                // Чтение содержимого файла
                using (var stream = file.OpenReadStream())
                {
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    content = reader.ReadToEndAsync().GetAwaiter().GetResult();
                }

                // Обработка данных
                return _service.LoadData(content) ? Results.Ok(new { message = "Данные из файла загруженны корректно", fileName = file.FileName }) 
                                                  : Results.BadRequest("Файл не корректный");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Ошибка загрузки: {ex.Message}");
            }
        }
    }
}
