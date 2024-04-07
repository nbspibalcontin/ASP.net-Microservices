using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using System.Net.Http.Headers;
using WebApplication1.DTO;
using WebApplication1.Exception;
using WebApplication1.HttpServices.Interface;

namespace WebApplication1.HttpServices.Implementation
{
    public class StudentService : IStudent
    {
        private readonly HttpClient _httpClient;

        public StudentService(HttpClient httpClient)
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
            .CircuitBreakerAsync(4, TimeSpan.FromMinutes(5)); 

        public async Task<string> CreateStudent(StudentDto student)
        {
            try
            {
                var formData = new MultipartFormDataContent();

                var properties = typeof(StudentDto).GetProperties();

                foreach (var property in properties)
                {
                    // Get the value of the property
                    var value = property.GetValue(student);

                    // Convert the value to string (or use empty string if null)
                    var stringValue = value?.ToString() ?? "";

                    // Add the value as StringContent to the FormData
                    formData.Add(new StringContent(stringValue), property.Name);
                }

                if (student.File != null)
                {
                    var fileContent = new StreamContent(student.File.OpenReadStream());
                    formData.Add(fileContent, "File", student.File.FileName);
                }

                if (CBPolicy.CircuitState == CircuitState.Open)
                {
                    throw new ApplicationException("Service is not available, Please try again later");
                }

                var response = await CBPolicy.ExecuteAsync(async () => await retryPolicy.ExecuteAsync(async () =>
                await _httpClient.PostAsync("/create-student", formData)));

                if (!response.IsSuccessStatusCode)
                {
                    await HandleNonSuccessResponseAsync(response);
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpStatusCodeException)
            {
                throw;
            }

            catch (ApplicationException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when creating student." + ex);
            }
        }

        public async Task<StudentDto> GetStudent(Guid studentId)
        {
            try
            {

                if (CBPolicy.CircuitState == CircuitState.Open)
                {
                    throw new ApplicationException("Service is not available, Please try again later");
                }

                var response = await CBPolicy.ExecuteAsync(async () => await retryPolicy.ExecuteAsync(async () =>
                await _httpClient.GetAsync($"/getStudentById/{studentId}")));

                if (!response.IsSuccessStatusCode)
                {
                    await HandleNonSuccessResponseAsync(response);
                }

                var student = await response.Content.ReadFromJsonAsync<StudentDto>();

                // Check if student is null and throw an exception if it is
                if (student == null)
                {
                    throw new ApplicationException("Received null response when retrieving student list.");
                }

                return student;
            }
            catch (HttpStatusCodeException)
            {
                throw;
            }

            catch (ApplicationException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when retrieving student." + ex);
            }
        }

        public async Task<List<StudentDto>> ListOfStudent()
        {
            try
            {
                if (CBPolicy.CircuitState == CircuitState.Open)
                {
                    throw new ApplicationException("Service is not available, Please try again later");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJhbGNvbnRpbmViZW5lemVyMDdAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoidXNlciIsImV4cCI6MTcxMjU3ODgxMCwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1MDU1IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDU1In0.wm1gIdBbyAKe6MiZsl4eUFamXrSjs7g58Ki295OzhP4");

                var response = await CBPolicy.ExecuteAsync(async () => await retryPolicy.ExecuteAsync(async () =>
                await _httpClient.GetAsync("/GetAllStudent")));

                if (!response.IsSuccessStatusCode)
                {
                    await HandleNonSuccessResponseAsync(response);
                }

                var studentList = await response.Content.ReadFromJsonAsync<List<StudentDto>>();

                // Check if studentList is null and throw an exception if it is
                if (studentList == null)
                {
                    throw new ApplicationException("Received null response when retrieving student list.");
                }

                return studentList;

            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when retrieving student." + ex);
            }
        }

        public async Task<string> UpdateStudent(StudentDto student)
        {
            try
            {
                var formData = new MultipartFormDataContent();

                var properties = typeof(StudentDto).GetProperties();

                foreach (var property in properties)
                {
                    // Get the value of the property
                    var value = property.GetValue(student);

                    // Convert the value to string (or use empty string if null)
                    var stringValue = value?.ToString() ?? "";

                    // Add the value as StringContent to the FormData
                    formData.Add(new StringContent(stringValue), property.Name);
                }

                if (student.File != null)
                {
                    var fileContent = new StreamContent(student.File.OpenReadStream());
                    formData.Add(fileContent, "File", student.File.FileName);
                }

                if (CBPolicy.CircuitState == CircuitState.Open)
                {
                    throw new ApplicationException("Service is not available, Please try again later");
                }

                var response = await CBPolicy.ExecuteAsync(async () => await retryPolicy.ExecuteAsync(async () =>
                await _httpClient.PutAsync($"/update-student/{student.StudentId}", formData)));

                if (!response.IsSuccessStatusCode)
                {
                    await HandleNonSuccessResponseAsync(response);
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpStatusCodeException)
            {
                throw;
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when retrieving student." + ex);
            }
        }

        public async Task<string> DeleteStudent(Guid studentId)
        {
            try
            {

                if (CBPolicy.CircuitState == CircuitState.Open)
                {
                    throw new ApplicationException("Service is not available, Please try again later");
                }

                var response = await CBPolicy.ExecuteAsync(async () => await retryPolicy.ExecuteAsync(async () =>
                await _httpClient.DeleteAsync($"/delete-student/{studentId}")));

                if (!response.IsSuccessStatusCode)
                {
                    await HandleNonSuccessResponseAsync(response);
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpStatusCodeException)
            {
                throw;
            }

            catch (ApplicationException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Error when retrieving student." + ex);
            }
        }

        public async Task HandleNonSuccessResponseAsync(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var statusCode = response.StatusCode;
            var errorObject = JsonConvert.DeserializeObject<ErrorMessage>(responseBody);

            if ((int)response.StatusCode == 401)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                throw new ApplicationException("You are not Authorized!");
               
            }
            else if ((int)response.StatusCode == 403)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                throw new ApplicationException("You are not allowed to access this API!");
            }
            else if(errorObject != null && !string.IsNullOrEmpty(errorObject.errorMessage))
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
