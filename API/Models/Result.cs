using Microsoft.AspNetCore.Mvc;

namespace API.Models
{
    public class Result<T>
    {
        public ResultStatus Status { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }

    }

    public enum ResultStatus
    {
        /// <summary>
        /// (200 OK). Принимает (result.Data)  
        /// </summary>
        Success,
        /// <summary>
        /// (404 Not Found). Принимает (result.Message)
        /// </summary>
        NotFound,
        /// <summary>
        /// (400 Bad Request). Принимает (result.Message)
        /// </summary>
        InvalidData,
        /// <summary>
        /// (409 Conflict). Принимает (result.Message)
        /// </summary>
        Conflict,
        /// <summary>
        /// (201 Created). Принимает (result.Data)
        /// </summary>
        Created
    }

    /// <summary>
    /// Класс с методом расширения. Нужен, для того чтобы класс Result можно было использовать отдельно от using Microsoft.AspNetCore.Mvc;
    /// Допустим обрабатывать запрос в WPF без подключения не нужной библиотеки 
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Сам метод расширения о нем можно почитать тут https://gamma.app/docs/22--8i7ywvaxnbiqwes?mode=doc
        /// </summary>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Возвращает <see cref="IActionResult"/> для ответа на запрос пользователя</returns>
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result.Status switch
            {
                ResultStatus.Success => new OkObjectResult(result.Data),
                ResultStatus.NotFound => new NotFoundObjectResult(result.Message),
                ResultStatus.InvalidData => new BadRequestObjectResult(result.Message),
                ResultStatus.Conflict => new ConflictObjectResult(result.Message),
                ResultStatus.Created => new ObjectResult(result.Data) { StatusCode = StatusCodes.Status201Created },
                _ => new StatusCodeResult(500)
            };
        }
    }
}
