using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using DynamoDB.Application.Services;
using System.Web;
using DynamoDB.Domain.Entities;

namespace DynamoDB.Web.Models
{
    public class ItemListModel
    {
        private readonly IDynamoDBService _dynamoDBService;

        public ItemListModel()
        {

        }
        public ItemListModel(IDynamoDBService dynamoDBService)
        {
            _dynamoDBService = dynamoDBService;
        }

        public async Task<object> GetPagedDynamoItemAsync(string tableName)
        {
            try
            {
             var result =  await _dynamoDBService.GetDynamoItemsAsync(tableName);
             return result;
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                return ex;
            }
        }

        internal void DeleteItem(string name, int age, string tableName)
        {
            _dynamoDBService.DeleteDynamoDbItem(name,age,tableName);
        }
    }
}
