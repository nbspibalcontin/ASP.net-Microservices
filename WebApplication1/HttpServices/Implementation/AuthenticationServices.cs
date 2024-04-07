using Polly.CircuitBreaker;
using Polly.Retry;
using Polly;
using WebApplication1.HttpServices.Interface;
using WebApplication1.DTO;
using Newtonsoft.Json;

namespace WebApplication1.HttpServices.Implementation
{
    public class AuthenticationServices : IAuthentication
    {
        private readonly HttpClient _httpClient;

        public AuthenticationServices(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:1000/");
        }

        private static readonly AsyncRetryPolicy<HttpResponseMessage> retryPolicy =
           Policy.HandleResult<HttpResponseMessage>(resp =>
           (int)resp.StatusCode >= 500)
           .WaitAndRetryAsync(4, retryAttempt =>
           {
               Console.WriteLine($"Attempt {retryAttempt} - Retrying due to error");
               return TimeSpan.FromSeconds(5 + retryAttempt);
           });

        private static readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> CBPolicy =
            Policy.HandleResult<HttpResponseMessage>(
                resp => (int)resp.StatusCode >= 500)
            .AdvancedCircuitBreakerAsync(
                0.5, //50%
                TimeSpan.FromMinutes(1), // Duration to measure failure
                10, //Request nb
                TimeSpan.FromMinutes(2)); // Duration break

        public async Task<string> AuthenticationLogin(LoginDto loginDto)
        {
            try
            {
                var formData = new MultipartFormDataContent();

                var properties = typeof(LoginDto).GetProperties();

                foreach (var property in properties)
                {
                    // Get the value of the property
                    var value = property.GetValue(loginDto);

                    // Convert the value to string (or use empty string if null)
                    var stringValue = value?.ToString() ?? "";

                    // Add the value as StringContent to the FormData
                    formData.Add(new StringContent(stringValue), property.Name);
                }

                if (CBPolicy.CircuitState == CircuitState.Open)
                {
                    throw new ApplicationException("Service is not available, Please try again later");
                }

                var response = await CBPolicy.ExecuteAsync(async () => await retryPolicy.ExecuteAsync(async () =>
                await _httpClient.PostAsync("/authentication/login", formData)));

                if (!response.IsSuccessStatusCode)
                {
                    await HandleNonSuccessResponseAsync(response);
                }

                return await response.Content.ReadAsStringAsync();

            }
            catch (ApplicationException)
            {
                throw;
            }
        }

        public async Task HandleNonSuccessResponseAsync(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            var errorObject = JsonConvert.DeserializeObject<ErrorMessage>(responseBody);

            // Deserialize the response body to extract the errorMessage
            if (errorObject != null && !string.IsNullOrEmpty(errorObject.errorMessage))
            {
                Console.WriteLine($"HTTP Error: {statusCode}, Error Message: {errorObject.errorMessage}");
                response.StatusCode = statusCode;
                throw new ApplicationException(errorObject.errorMessage);
            }
            else
            {
                throw new ApplicationException("Something error in the service.");
            }
        }
    }
}
