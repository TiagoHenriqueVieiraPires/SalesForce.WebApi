using System.Collections.Generic;
using System.Threading.Tasks;
using SalesForce.WebApi.Authorization;
using SalesForce.WebApi.Models;

namespace SalesForce.WebApi.Interfaces
{
    public interface ICrmWebApi
    {
        CrmBaseAuthorization BaseAuthorization { get; }
        Task<string> CreateAsync(dynamic data, string table);
        Task<string> CreateAsync(string table, string externalKey, dynamic data, string id);
        Task<string> UpdateAsync(string table, string externalKey, dynamic data, string id);
        Task<string> UpdateAsync(dynamic data, string table, string id);
        Task<string> RetrieveSigleAsync(string table, string externalKey, string id);
        Task<string> RetrieveSigleAsync(string table, string id);
        Task<string> RetrieveMultipleAsync(string query);
        Task<string> UpsertAsync(string table, string externalKey, dynamic data, string id);
        Task<JobResponse> CreateJobAsync(JobRequest jobRequest);
        Task<JobStatusResponse> SchedulingJobAsync(string jobId, byte[] archive);
        Task<string> DeleteAsync(string table, string id);
        Task<JobStatusResponse> BulkCreateAsync(string table, byte[] archive);
        Task<JobStatusResponse> BulkUpdateAsync(string table, string externalKey, byte[] archive);
        Task<JobStatusResponse> CloseJobAsync(string id);
        Task<JobStatusResponse> BulkUpsertAsync(string table, string externalKey, byte[] archive);
        Task<JobStatusResponse> UploadArchiveAsync(string jobId, byte[] archive);
        Task<JobStatusResponse> CheckStatusJobAsync(string id);
        Task<string> GetResultJobSucessAsync(string id);
        Task<string> GetResultJobFailedAsync(string id);
        Task<string> GetResultJobUnprocessedAsync(string id);
        Task<List<CompositeResponse>> MultipleRequestAsync(CompositeRequest compositeRequest);
    }
}
