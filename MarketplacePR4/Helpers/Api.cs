using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MarketplacePR4.Helpers
{
    // Результат любого запроса к API: успех или нет, код ответа, данные или текст ошибки
    public class ApiResult<T>
    {
        public bool Ok { get; set; }
        public HttpStatusCode Status { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
    }

    public static class Api
    {
        // Один HttpClient на всё приложение. Адрес берём из App.config.
        public static readonly HttpClient Http = new HttpClient
        {
            BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiUrl"] ?? "http://localhost:5009/"),
            Timeout = TimeSpan.FromSeconds(15)
        };

        // После входа кладём токен в заголовок: он будет прикладываться ко всем запросам
        public static void SetToken(string token)
        {
            Http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
        }

        public static async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, object body = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, url);
                if (body != null)
                {
                    var json = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await Http.SendAsync(request);
                var text = await response.Content.ReadAsStringAsync();

                var result = new ApiResult<T> { Ok = response.IsSuccessStatusCode, Status = response.StatusCode };

                if (result.Ok)
                {
                    // Если сервер вернул JSON и нам нужны данные, превращаем его в объект
                    if (!string.IsNullOrWhiteSpace(text) && typeof(T) != typeof(object))
                        result.Data = JsonConvert.DeserializeObject<T>(text);
                }
                else
                {
                    result.Error = ErrorText(text, response.StatusCode);
                }
                return result;
            }
            catch (HttpRequestException)
            {
                return new ApiResult<T> { Ok = false, Error = "Сервер недоступен. Запущен ли API?" };
            }
            catch (TaskCanceledException)
            {
                return new ApiResult<T> { Ok = false, Error = "Сервер слишком долго не отвечает" };
            }
        }

        // Превращает ответ с ошибкой в понятный текст для пользователя
        private static string ErrorText(string text, HttpStatusCode status)
        {
            if (string.IsNullOrWhiteSpace(text))
                return status == HttpStatusCode.Unauthorized
                    ? "Нужно войти в аккаунт"
                    : "Ошибка сервера (" + (int)status + ")";

            text = text.Trim();
            if (text.StartsWith("{"))   // стандартный JSON с описанием ошибки
                return "Некорректный запрос (" + (int)status + ")";

            return text.Trim('"');      
        }
    }
}
