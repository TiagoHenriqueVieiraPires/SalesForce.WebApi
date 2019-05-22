using System.Threading.Tasks;
using SalesForce.WebApi.Authorization;
using SalesForce.WebApi.Models;

namespace SalesForce.WebApi.Interfaces
{
    public interface ICrmWebApi
    {
        CrmBaseAuthorization BaseAuthorization { get; }
        Task<string> RetrieveSigleAsync(string table, string externalKey, string id);
        Task<string> RetrieveMultipleAsync(string query);
        Task<string> CreateAsync(string context, string table);
        JobResponse CreateJob(JobRequest jobRequest);
        JobStatusResponse SchedulingJob(string jobId, byte[] archive);
        Task<string> CreateAsync(string table, string externalKey, string context, string id);
        Task<string> UpdateAsync(string context, string table, string id);
        Task<string> UpdateAsync(string table, string externalKey, string context, string id);
        Task<string> DeleteAsync(string table, string id);
        Task<string> UpsertAsync(string table, string externalKey, string context, string id);
        JobStatusResponse BulkCreateAsync(string table, byte[] archive);
        JobStatusResponse BulkUpdateAsync(string table, string externalKey, byte[] archive);
        JobStatusResponse CloseJob(string id);
        JobStatusResponse BulkUpsertAsync(string table, string externalKey, byte[] archive);
        JobStatusResponse UploadArchive(string jobId, byte[] archive);
        JobStatusResponse CheckStatusJob(string id);
        Task<string> GetResultJobSucess(string id);
        Task<string> GetResultJobFailed(string id);
        Task<string> GetResultJobUnprocessed(string id);
    }
}