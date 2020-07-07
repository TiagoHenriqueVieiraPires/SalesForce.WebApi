using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SalesForce.WebApi.Authorization;
using SalesForce.WebApi.Interfaces;
using SalesForce.WebApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using SalesForce.WebApi.Config;
using SalesForce.WebApi.Models.ChatterDataTransferObject;
using AutoMapper;
using SalesForce.WebApi.Mappings;

namespace SalesForce.WebApi
{
    public partial class CrmWebApi : ICrmWebApi
    {
        protected HttpClient httpClient;
        private readonly BaseAuthorization _baseAuthorization;
        private readonly IMapper _mapper;
        public readonly Uri ApiUrl;
        public BaseAuthorization BaseAuthorization => _baseAuthorization;
        public CrmWebApi(BaseAuthorization baseAuthorization) : this(baseAuthorization, baseAuthorization.GetSystemUrl()) { }

        public CrmWebApi(BaseAuthorization baseAuthorization, string apiUrl)
        {
            _baseAuthorization = baseAuthorization;
            _baseAuthorization.ConfigHttpClient();
            ApiUrl = new Uri(apiUrl);
            _mapper = new MapperConfiguration(config => config.AddProfile<MappingProfile>()).CreateMapper();
        }

        public async Task<string> CreateAsync(dynamic objectValue, string objectName)
        {
            var context = JsonConvert.SerializeObject(objectValue);
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/sobjects/{objectName}";
            var content = new StringContent(context, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public async Task<string> CreateAsync(string objectName, string externalKeyName, dynamic objectValue, string externalKeyValue)
        {
            return await UpsertAsync(objectName, externalKeyName, objectValue, externalKeyValue);
        }

        public async Task<string> UpdateAsync(dynamic objectValue, string objectName, string id)
        {
            var context = JsonConvert.SerializeObject(objectValue);
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/sobjects/{objectName}/{id}";

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = new StringContent(context, Encoding.UTF8, "application/json")
            };

            var result = await _baseAuthorization.GetHttpCliente().SendAsync(request);

            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public async Task<string> UpdateAsync(string objectName, string externalKeyName, dynamic objectValue, string externalKeyValue)
        {
            return await UpsertAsync(objectName, externalKeyName, objectValue, externalKeyValue);
        }

        public async Task<string> RetrieveMultipleAsync(string query)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/query/?q={query}";
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> RetrieveMultipleNextAsync(string idNexts)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/query/{idNexts}";
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> RetrieveSigleAsync(string objectName, string externalKeyName, string externalKeyValue)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/sobjects/{objectName}/{externalKeyName}/{externalKeyValue}";
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> RetrieveSigleAsync(string objectName, string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/sobjects/{objectName}/{id}";
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpContent> RetrieveFromUrlAsync(string uri)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            uri = uri.Replace("/services/", "services/");
            var url = $"{ApiUrl}{uri}";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return response.Content;
        }

        public async Task<string> UpsertAsync(string objectName, string externalKeyName, dynamic objectValue, string externalKeyValue)
        {
            var context = JsonConvert.SerializeObject(objectValue);
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/sobjects/{objectName}/{externalKeyName}/{externalKeyValue}";

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = new StringContent(context, Encoding.UTF8, "application/json")
            };
            var result = await _baseAuthorization.GetHttpCliente().SendAsync(request);

            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public async Task<string> DeleteAsync(string objectName, string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/sobjects/{objectName}/{id}";
            var result = await httpClient.DeleteAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public async Task<JobStatusResponse> JobBulkUpsertAsync(string objectName, string externalKeyName, byte[] archive)
        {
            var jobRequest = new JobRequest(objectName, "CSV", "upsert", null, externalKeyName);
            var jobResponse = await CreateJobAsync(jobRequest);
            var response = await SchedulingJobAsync(jobResponse.Id, archive);
            return response;
        }

        public async Task<JobStatusResponse> JobBulkCreateAsync(string objectName, byte[] archive)
        {
            var jobRequest = new JobRequest(objectName, "CSV", "insert", "CRLF", null);
            var jobResponse = await CreateJobAsync(jobRequest);
            var response = await SchedulingJobAsync(jobResponse.Id, archive);
            return response;
        }

        public async Task<JobResponse> CreateJobAsync(JobRequest jobRequest)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/jobs/ingest/";
            var context = JsonConvert.SerializeObject(jobRequest);
            var content = new StringContent(context, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<JobResponse>(resultContent);
        }

        public async Task<JobStatusResponse> UploadArchiveAsync(string jobId, byte[] archive)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/jobs/ingest/{jobId}/batches";
            HttpContent content = new ByteArrayContent(archive);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            var result = await httpClient.PutAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<JobStatusResponse>(resultContent);
        }

        public async Task<JobStatusResponse> SchedulingJobAsync(string jobId, byte[] archive)
        {
            var result = await UploadArchiveAsync(jobId, archive);
            return await CloseJobAsync(jobId);
        }

        public async Task<JobStatusResponse> CloseJobAsync(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/jobs/ingest/{id}";

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = new StringContent("{\"state\" : \"UploadComplete\"}", Encoding.UTF8, "application/json")
            };

            var result = await _baseAuthorization.GetHttpCliente().SendAsync(request);

            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<JobStatusResponse>(resultContent);
        }

        public async Task<JobStatusResponse> JobBulkUpdateAsync(string objectName, string externalKeyName, byte[] archive)
        {
            return await JobBulkUpsertAsync(objectName, externalKeyName, archive);
        }

        public async Task<JobStatusResponse> CheckStatusJobAsync(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/jobs/ingest/{id}";
            var result = await httpClient.GetAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<JobStatusResponse>(resultContent);
        }

        public async Task<string> GetResultJobSucessAsync(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/jobs/ingest/{id}/successfulResults";
            var result = await httpClient.GetAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public async Task<string> GetResultJobFailedAsync(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/jobs/ingest/{id}/failedResults";
            var result = await httpClient.GetAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public async Task<string> GetResultJobUnprocessedAsync(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/jobs/ingest/{id}/unprocessedrecords";
            var result = await httpClient.GetAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public async Task<List<CompositeResponse>> MultipleRequestAsync(CompositeRequest compositeRequest)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/composite";
            var context = JsonConvert.SerializeObject(compositeRequest);
            var content = new StringContent(context, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            var json = JsonConvert.DeserializeObject<dynamic>(resultContent);
            return json["compositeResponse"].ToObject<List<CompositeResponse>>();
        }

        private DateTime ComvertToDateTimeUnixTimestamp(TimeSpan time)
        {
            return DateTime.FromOADate(time.TotalSeconds);
        }

        public async Task<string> BulkCreatetAsync(BulkCreateRequest bulkRequest)
        {
            var context = JsonConvert.SerializeObject(bulkRequest.Request);
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/composite/tree/{bulkRequest.Object}";

            var request = new HttpRequestMessage(new HttpMethod("POST"), url)
            {
                Content = new StringContent(context, Encoding.UTF8, "application/json")
            };
            var result = await _baseAuthorization.GetHttpCliente().SendAsync(request);

            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public async Task<List<BulkResult>> MutipleUpdateAsync(BulkUpdateRequest bulkRequest)
        {
            return await BatchRequestAsync(bulkRequest, "PATCH");
        }

        public async Task<string> SendCustomNotificationAsync(CustomNotification notification)
        {
            var context = JsonConvert.SerializeObject(notification);
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/actions/standard/customNotificationAction";

            var request = new HttpRequestMessage(new HttpMethod("POST"), url)
            {
                Content = new StringContent(context, Encoding.UTF8, "application/json")
            };
            var result = await _baseAuthorization.GetHttpCliente().SendAsync(request);

            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public async Task<List<BulkResult>> MutipleCreateAsync(BulkUpdateRequest bulkRequest)
        {
            return await BatchRequestAsync(bulkRequest, "POST");
        }

        public async Task<List<BulkResult>> BatchRequestAsync(BulkUpdateRequest bulkRequest, string method)
        {
            var context = JsonConvert.SerializeObject(bulkRequest);
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/composite/sobjects";

            var request = new HttpRequestMessage(new HttpMethod(method.ToUpper()), url)
            {
                Content = new StringContent(context, Encoding.UTF8, "application/json")
            };
            var result = await _baseAuthorization.GetHttpCliente().SendAsync(request);

            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<List<BulkResult>>(resultContent);
        }

        public async Task<string> MutipleDeleteAsync(BulkDeleteRequest bulkRequest)
        {
            var context = JsonConvert.SerializeObject(bulkRequest);
            httpClient = _baseAuthorization.GetHttpCliente();

            var ids = "";
            foreach (var id in bulkRequest.RecordId)
                ids += string.Concat(id, bulkRequest.RecordId.LastOrDefault() != id ? "," : "");

            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/composite/sobjects?ids={ids}&allOrNone={bulkRequest.AllOrNone}";

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), url)
            {
                Content = new StringContent(context, Encoding.UTF8, "application/json")
            };
            var result = await _baseAuthorization.GetHttpCliente().SendAsync(request);

            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }

        public string UpsertSync(string objectName, string externalKeyName, dynamic objectValue, string externalKeyValue)
        {
            var ret = UpsertAsync(objectName, externalKeyName, objectValue, externalKeyValue).GetAwaiter().GetResult();

            if (string.IsNullOrWhiteSpace(ret))
                ret = JsonConvert.SerializeObject(new UpsertResponse() { Id = null, Success = true });
            return ret;
        }

        public async Task<BatchResults> MultipleCreateChatterAsync(List<ChatterRequest> requests)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v{ApiCofiguration.Version}/chatter/feed-elements/batch";

            var chatters = new BatchCollectionInput()
            {
                Inputs = _mapper.Map<List<RichInput>>(requests)
            };

            var context = JsonConvert.SerializeObject(chatters);
            var content = new StringContent(context, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent} Token Expired In: {ComvertToDateTimeUnixTimestamp(_baseAuthorization.Timeout)}");
            result.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<BatchResults>(resultContent);
        }
    }
}