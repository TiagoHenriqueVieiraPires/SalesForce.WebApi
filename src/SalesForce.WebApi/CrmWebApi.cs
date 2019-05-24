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
        public Task<string> CreateAsync(string context, string table)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}";

            var content = new StringContent(context, Encoding.UTF8, "application/json");

            var result = httpClient.PostAsync(url, content).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return resultContent;
        }
        public Task<string> CreateAsync(string table, string externalKey, string context, string id)
        {
            return UpsertAsync(table, externalKey, context, id);
        }
        public Task<string> UpdateAsync(string context, string table, string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}/{id}";

            var content = new StringContent(context, Encoding.UTF8, "application/json");

            var result = httpClient.PatchAsync(url, content).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return resultContent;
        }
        public Task<string> UpdateAsync(string table, string externalKey, string context, string id)
        {
            return UpsertAsync(table, externalKey, context, id);
        }
        public Task<string> RetrieveMultipleAsync(string query)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/query/?q={query}";

            var response = httpClient.GetAsync(url).GetAwaiter().GetResult();

            return response.Content.ReadAsStringAsync();
        }
        public Task<string> UpsertAsync(string table, string externalKey, string context, string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}/{externalKey}/{id}";

            var content = new StringContent(context, Encoding.UTF8, "application/json");

            var result = httpClient.PatchAsync(url, content).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return resultContent;
        }
        public Task<string> DeleteAsync(string table, string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}/{id}";

            var result = httpClient.DeleteAsync(url).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return resultContent;
        }
        public JobStatusResponse BulkUpsertAsync(string table, string externalKey, byte[] archive)
        {
            var jobRequest = new JobRequest(table, "CSV", "upsert", null, "Documento__c");

            var jobResponse = CreateJob(jobRequest);

            var response = SchedulingJob(jobResponse.Id, archive);

            return response;
        }
        public JobStatusResponse BulkCreateAsync(string table, byte[] archive)
        {
            var jobRequest = new JobRequest(table, "CSV", "insert", "CRLF", null);

            var jobResponse = CreateJob(jobRequest);

            var response = SchedulingJob(jobResponse.Id, archive);

            return response;
        }

        public Task<string> RetrieveSigleAsync(string table, string externalKey, string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/sobjects/{table}/{externalKey}/{id}";

            var response = httpClient.GetAsync(url).GetAwaiter().GetResult();

            return response.Content.ReadAsStringAsync();
        }

        public JobResponse CreateJob(JobRequest jobRequest)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/";

            var context = JsonConvert.SerializeObject(jobRequest);

            var content = new StringContent(context, Encoding.UTF8, "application/json");

            var result = httpClient.PostAsync(url, content).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<JobResponse>(resultContent.GetAwaiter().GetResult());
        }

        public JobStatusResponse UploadArchive(string jobId, byte[] archive)
        {
            httpClient = _baseAuthorization.GetHttpCliente();

            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{jobId}/batches";

            HttpContent content = new ByteArrayContent(archive);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

            var result = httpClient.PutAsync(url, content).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();

            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<JobStatusResponse>(resultContent.GetAwaiter().GetResult());
        }

        public JobStatusResponse SchedulingJob(string jobId, byte[] archive)
        {
            var result = UploadArchive(jobId, archive);
            return CloseJob(jobId);
        }

        public JobStatusResponse CloseJob(string id)
        {

            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}";

            var content = new StringContent("{\"state\" : \"UploadComplete\"}", Encoding.UTF8, "application/json");
            var result = httpClient.PatchAsync(url, content).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<JobStatusResponse>(resultContent.GetAwaiter().GetResult());
        }

        public JobStatusResponse BulkUpdateAsync(string table, string externalKey, byte[] archive)
        {
            return BulkUpsertAsync(table, externalKey, archive);
        }

        public JobStatusResponse CheckStatusJob(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}";

            var result = httpClient.GetAsync(url).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<JobStatusResponse>(resultContent.GetAwaiter().GetResult());
        }

        public Task<string> GetResultJobSucess(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}/successfulResults";

            var result = httpClient.GetAsync(url).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return resultContent;
        }

        public Task<string> GetResultJobFailed(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}/failedResults";

            var result = httpClient.GetAsync(url).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return resultContent;
        }

        public Task<string> GetResultJobUnprocessed(string id)
        {
            httpClient = _baseAuthorization.GetHttpCliente();
            var url = $"{ApiUrl}services/data/v45.0/jobs/ingest/{id}/unprocessedrecords";

            var result = httpClient.GetAsync(url).GetAwaiter().GetResult();

            var resultContent = result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"Error=>> {resultContent.GetAwaiter().GetResult()}");

            result.EnsureSuccessStatusCode();

            return resultContent;
        }

    }
}
