using Azure;
using Frontend.DTO;
using Frontend.Exception;
using Frontend.HttpServices.Interface;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Net;

namespace Frontend.HttpServices.Implementation
{
    public class StudentServices : IStudent
    {
        private readonly HttpClient _httpClient;

        public StudentServices(HttpClient httpClient)
        {

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:1000/");
        }

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

                var response = await _httpClient.PostAsync("/create-student", formData);

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
                var response = await _httpClient.GetAsync($"/getStudentById/{studentId}");

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
                var response = await _httpClient.GetAsync("/GetAllStudent");

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
            catch (HttpStatusCodeException)
            {
                throw;
            }
            catch (BadGatewayException)
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

                var response = await _httpClient.PutAsync($"/update-student/{student.StudentId}", formData);

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

                var response = await _httpClient.DeleteAsync($"/delete-student/{studentId}");

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

            if (response.StatusCode == HttpStatusCode.BadGateway) // Bad Gateway
            {
                throw new BadGatewayException("The server encountered a problem while receiving a response from an upstream server.");
            }

            // Deserialize the response body to extract the errorMessage
            var errorObject = JsonConvert.DeserializeObject<ErrorMessage>(responseBody);
            if (errorObject != null && !string.IsNullOrEmpty(errorObject.errorMessage))
            {
                Console.WriteLine($"HTTP Error: {statusCode}, Error Message: {errorObject.errorMessage}");
                response.StatusCode = statusCode;
                throw new ApplicationException(errorObject.errorMessage);
            }
            else
            {
                Console.WriteLine($"HTTP Error: {statusCode}, No error message provided by the service.");
                throw new ApplicationException(errorObject!.errorMessage);
            }
        }
    }
}
