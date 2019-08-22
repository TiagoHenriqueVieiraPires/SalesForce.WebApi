using System;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SalesForce.WebApi.Authorization;
using SalesForce.WebApi.Interfaces;
using SalesForce.WebApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SalesForce.WebApi
{
    public partial class CrmWebApi : ICrmWebApi
    {
        protected HttpClient httpClient;
        private readonly CrmBaseAuthorization _baseAuthorization;
        public readonly Uri ApiUrl;
        public CrmBaseAuthorization BaseAuthorization => _baseAuthorization;
        public CrmWebApi(CrmBaseAuthorization baseAuthorization) : this(baseAuthorization, baseAuthorization.GetSystemUrl()) { }

        public CrmWebApi(CrmBaseAuthorization baseAuthorization, string apiUrl)
        {
            _baseAuthorization = baseAuthorization;
            _baseAuthorization.ConfigHttpClient();
            ApiUrl = new Uri(apiUrl);
        }
        public async Task<string> CreateAsync(dynamic data, string table)
        {
            var context = JsonConvert.SerializeObject(data);
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}";
            var content = new StringContent(context, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }
        public async Task<string> CreateAsync(string table, string externalKey, dynamic data, string id)
        {
            return await UpsertAsync(table, externalKey, data, id);
        }
        public async Task<string> UpdateAsync(dynamic data, string table, string id)
        {
            var context = JsonConvert.SerializeObject(data);
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}/{id}";
            var content = new StringContent(context, Encoding.UTF8, "application/json");
            var result = await httpClient.PatchAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }
        public async Task<string> UpdateAsync(string table, string externalKey, dynamic data, string id)
        {
            return await UpsertAsync(table, externalKey, data, id);
        }
        public async Task<string> RetrieveMultipleAsync(string query)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/query/?q={query}";
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> RetrieveSigleAsync(string table, string externalKey, string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}/{externalKey}/{id}";
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> RetrieveSigleAsync(string table, string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}/{id}";
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> UpsertAsync(string table, string externalKey, dynamic data, string id)
        {
            var context = JsonConvert.SerializeObject(data);
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}/{externalKey}/{id}";
            var content = new StringContent(context, Encoding.UTF8, "application/json");
            var result = await httpClient.PatchAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }
        public async Task<string> DeleteAsync(string table, string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}/{id}";
            var result = await httpClient.DeleteAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }
        public async Task<JobStatusResponse> BulkUpsertAsync(string table, string externalKey, byte[] archive)
        {
            var jobRequest = new JobRequest(table, "CSV", "upsert", null, "Documento__c");
            var jobResponse = await CreateJobAsync(jobRequest);
            var response = await SchedulingJobAsync(jobResponse.Id, archive);
            return response;
        }
        public async Task<JobStatusResponse> BulkCreateAsync(string table, byte[] archive)
        {
            var jobRequest = new JobRequest(table, "CSV", "insert", "CRLF", null);
            var jobResponse = await CreateJobAsync(jobRequest);
            var response = await SchedulingJobAsync(jobResponse.Id, archive);
            return response;
        }
        public async Task<JobResponse> CreateJobAsync(JobRequest jobRequest)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/";
            var context = JsonConvert.SerializeObject(jobRequest);
            var content = new StringContent(context, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<JobResponse>(resultContent);
        }
        public async Task<JobStatusResponse> UploadArchiveAsync(string jobId, byte[] archive)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{jobId}/batches";
            HttpContent content = new ByteArrayContent(archive);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            var result = await httpClient.PutAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
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
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}";
            var content = new StringContent("{\"state\" : \"UploadComplete\"}", Encoding.UTF8, "application/json");
            var result = await httpClient.PatchAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<JobStatusResponse>(resultContent);
        }
        public async Task<JobStatusResponse> BulkUpdateAsync(string table, string externalKey, byte[] archive)
        {
            return await BulkUpsertAsync(table, externalKey, archive);
        }
        public async Task<JobStatusResponse> CheckStatusJobAsync(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}";
            var result = await httpClient.GetAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<JobStatusResponse>(resultContent);
        }
        public async Task<string> GetResultJobSucessAsync(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}/successfulResults";
            var result = await httpClient.GetAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }
        public async Task<string> GetResultJobFailedAsync(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}/failedResults";
            var result =await  httpClient.GetAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }
        public async Task<string> GetResultJobUnprocessedAsync(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}/unprocessedrecords";
            var result = await httpClient.GetAsync(url);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            return resultContent;
        }
        public async Task<List<CompositeResponse>> MultipleRequestAsync(CompositeRequest compositeRequest)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/composite";
            var context = JsonConvert.SerializeObject(compositeRequest);
            var content = new StringContent(context, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url, content);
            var resultContent = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent}");
            result.EnsureSuccessStatusCode();
            var json = JsonConvert.DeserializeObject<dynamic>(resultContent);
            return json["compositeResponse"].ToObject<List<CompositeResponse>>();
        }
    }
}
