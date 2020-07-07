using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SalesForce.WebApi.Authorization;
using SalesForce.WebApi.Models;
using SalesForce.WebApi.Models.ChatterDataTransferObject;

namespace SalesForce.WebApi.Interfaces
{
    public interface ICrmWebApi
    {
        BaseAuthorization BaseAuthorization { get; }
        Task<string> CreateAsync(dynamic objectValue, string objectName);
        Task<string> CreateAsync(string objectName, string externalKeyName, dynamic objectValue, string externalKeyValue);
        Task<string> UpdateAsync(string objectName, string externalKeyName, dynamic objectValue, string externalKeyValue);
        Task<string> UpdateAsync(dynamic objectValue, string objectName, string id);
        Task<string> RetrieveSigleAsync(string objectName, string externalKeyName, string externalKeyValue);
        Task<string> RetrieveSigleAsync(string objectName, string id);
        Task<string> RetrieveMultipleAsync(string query);
        Task<string> UpsertAsync(string objectName, string externalKeyName, dynamic objectValue, string externalKeyValue);
        string UpsertSync(string objectName, string externalKeyName, dynamic objectValue, string externalKeyValue);
        Task<JobResponse> CreateJobAsync(JobRequest jobRequest);
        Task<JobStatusResponse> SchedulingJobAsync(string jobId, byte[] archive);
        Task<string> DeleteAsync(string objectName, string id);
        Task<JobStatusResponse> JobBulkCreateAsync(string objectName, byte[] archive);
        Task<JobStatusResponse> JobBulkUpdateAsync(string objectName, string externalKeyName, byte[] archive);
        Task<JobStatusResponse> JobBulkUpsertAsync(string objectName, string externalKeyName, byte[] archive);
        Task<JobStatusResponse> CloseJobAsync(string id);
        Task<JobStatusResponse> UploadArchiveAsync(string jobId, byte[] archive);
        Task<JobStatusResponse> CheckStatusJobAsync(string id);
        Task<string> GetResultJobSucessAsync(string id);
        Task<string> GetResultJobFailedAsync(string id);
        Task<string> GetResultJobUnprocessedAsync(string id);
        Task<List<CompositeResponse>> MultipleRequestAsync(CompositeRequest compositeRequest);
        Task<HttpContent> RetrieveFromUrlAsync(string uri);

        /// <summary>
        /// Reference: https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/requests_composite_sobject_tree.htm?search_text=tree
        /// Limit Request: 200 records
        /// </summary>        
        Task<string> BulkCreatetAsync(BulkCreateRequest bulkRequest);
        Task<string> SendCustomNotificationAsync(CustomNotification notification);

        /// <summary>
        /// Reference: https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/resources_composite_sobjects_collections_update.htm
        /// Limit Request: 200 records
        /// Request:
        ///         {
        ///               "allOrNone" : false,
        ///               "records" : [{
        ///                   "attributes" : {"type" : "Account"},
        ///                   "id" : "001xx000003DGb2AAG",
        ///                   "NumberOfEmployees" : 27000
        ///              },{
        ///                  "attributes" : {"type" : "Contact"},
        ///                  "id" : "003xx000004TmiQAAS",
        ///                  "Title" : "Lead Engineer"
        ///             }]
        ///         }
        /// Response:
        ///         [
        ///               {
        ///                   "id" : "001RM000003oCprYAE",
        ///                    "success" : true,
        ///                    "errors" : [ ]
        ///                },
        ///                {
        ///                    "id" : "003RM0000068og4YAA",
        ///                    "success" : true,
        ///                    "errors" : [ ]
        ///                }
        ///            ] 
        /// </summary>
        Task<List<BulkResult>> MutipleUpdateAsync(BulkUpdateRequest bulkRequest);

        /// <summary>
        /// Reference: https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/resources_composite_sobjects_collections_create.htm
        /// Limit Request: 200 records
        /// Request:
        ///         {
        ///               "allOrNone" : false,
        ///               "records" : [{
        ///                   "attributes" : {"type" : "Account"},
        ///                   "id" : "001xx000003DGb2AAG",
        ///                   "NumberOfEmployees" : 27000
        ///              },{
        ///                  "attributes" : {"type" : "Contact"},
        ///                  "id" : "003xx000004TmiQAAS",
        ///                  "Title" : "Lead Engineer"
        ///             }]
        ///         }
        /// Response:
        ///         [
        ///               {
        ///                   "id" : "001RM000003oCprYAE",
        ///                    "success" : true,
        ///                    "errors" : [ ]
        ///                },
        ///                {
        ///                    "id" : "003RM0000068og4YAA",
        ///                    "success" : true,
        ///                    "errors" : [ ]
        ///                }
        ///            ] 
        /// </summary>
        Task<List<BulkResult>> MutipleCreateAsync(BulkUpdateRequest bulkRequest);
        Task<List<BulkResult>> BatchRequestAsync(BulkUpdateRequest bulkRequest, string method);
        Task<string> MutipleDeleteAsync(BulkDeleteRequest bulkRequest);
        Task<string> RetrieveMultipleNextAsync(string idNexts);

        /// <summary>Cria até 500 postages de feed elements na seção de chatters vinculando à um usuário, grupo ou registro (account por exemplo).</summary>
        /// <remarks> 
        /// Reference: https://developer.salesforce.com/docs/atlas.en-us.chatterapi.meta/chatterapi/connect_resources_feed_element_batch_post.htm
        /// Limit: 500 feed elements
        /// Request:
        ///    {
        ///        "inputs":[
        ///            {
        ///                "richInput": {
        ///                    "feedElementType" : "FeedItem",
        ///                    "subjectId" : "001M000001EQdwFIAT",
        ///                    "visibility": "InternalUsers",
        ///                    "body" : {
        ///                        "messageSegments" : [
        ///                            {
        ///                                "type" : "Text",
        ///                                "text" : "Post created by chatter/feed-elements/batch"
        ///                            }
        ///                        ]
        ///                    }
        ///                }
        ///            }
        ///        ]
        ///    }
        /// Response:
        ///    {
        ///        "hasErrors": false,
        ///        "results": [
        ///            {
        ///                "result": {
        ///                    "id": "0D5M000000dkOUfKAM",
        ///                    "parent": {
        ///                        "id": "001M000001EQdwFIAT",
        ///                        "type": "Account"
        ///                    },
        ///                },
        ///                "statusCode": 201
        ///            }
        ///        ]
        ///    }
        /// </remarks>
        /// <param name="requests">Lista de chatters contendo o texto da mensagem que deseja postar e o item que deseja vincular ao chatter a ser criado: id do usuário, grupo, registro ou texto 'me' para indicar o usuário do contexto atual.</param>
        /// <returns> Retorna o resultado da inserção em lote para cada solicitação de criação de chatter com seus respectivos código de status da requisição HTTP.</returns>
        Task<BatchResults> MultipleCreateChatterAsync(List<ChatterRequest> requests);
    }
}