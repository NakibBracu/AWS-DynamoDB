using DynamoDB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDB.Application.Services
{
    public interface IDynamoDBService
    {
        Task AddRowWithDatainDyanmoDBTable(string name, int age, string? mobile, string? address, string _tableName);
        Task UpdateDynamoDbItem(string name, int age, string? mobile, string? address, string _tableName);

        Task<DynamoItem> GetDynamoDbItem(string name, int age, string tableName);

        Task<IList<DynamoItem>> GetDynamoDbItems(string tableName);

        Task<object> GetDynamoItemsAsync(string tableName);

        Task DeleteDynamoDbItem(string name, int age, string _tableName);
    }
}
