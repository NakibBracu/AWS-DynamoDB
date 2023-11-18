using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DynamoDB.Application.Services;
using DynamoDB.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DynamoDB.Infrastructure.Services
{
    public class DynamoDBService : IDynamoDBService
    {

        private IConfiguration _configuration;

        private readonly ILogger _logger;
        

        public DynamoDBService(IConfiguration configuration, ILogger logger)
        {

            _configuration = configuration;
            _logger = logger;

        }


        public async Task AddRowWithDatainDyanmoDBTable(string name, int age, string? mobile, string address, string _tableName)
        {
            try
            {
                string tableName = _tableName;

                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                };

                using (var client = new AmazonDynamoDBClient(config))
                {
                    var context = new DynamoDBContext(client);

                    var item = new Dictionary<string, AttributeValue>
                {
                    { "Name", new AttributeValue { S = name } },
                    { "Age", new AttributeValue { N = age.ToString() } }
                };

                    if (!string.IsNullOrEmpty(mobile))
                    {
                        item.Add("Mobile", new AttributeValue { N = mobile });
                    }

                    if (!string.IsNullOrEmpty(address))
                    {
                        item.Add("Address", new AttributeValue { S = address });
                    }

                    var request = new PutItemRequest
                    {
                        TableName = tableName,
                        Item = item
                    };

                    await client.PutItemAsync(request);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred: {ex.Message}");
            }
        }



        public async Task UpdateDynamoDbItem(string name, int age, string? mobile, string? address, string _tableName)
        {
            try
            {
                string tableName = _tableName;

                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                };

                using (var client = new AmazonDynamoDBClient(config))
                {
                    var context = new DynamoDBContext(client);

                    // Create a key for the item to update
                    var key = new Dictionary<string, AttributeValue>
            {
                { "Name", new AttributeValue { S = name } },
                { "Age", new AttributeValue { N = age.ToString() } }
            };

                    // Create an update expression to modify the "Mobile" and "Address" attributes
                    var updateExpression = "SET";

                    var values = new Dictionary<string, AttributeValue>();

                    if (!string.IsNullOrEmpty(mobile))
                    {
                        updateExpression += " Mobile = :newMobile,";
                        values.Add(":newMobile", new AttributeValue { N = mobile });
                    }

                    if (!string.IsNullOrEmpty(address))
                    {
                        updateExpression += " Address = :newAddress,";
                        values.Add(":newAddress", new AttributeValue { S = address });
                    }

                    // Remove the trailing comma, if any
                    updateExpression = updateExpression.TrimEnd(',');

                    var request = new UpdateItemRequest
                    {
                        TableName = tableName,
                        Key = key,
                        UpdateExpression = updateExpression,
                        ExpressionAttributeValues = values
                    };

                    // Update the "Mobile" and "Address" attributes for the item with the specified "Name" and "Age"
                    await client.UpdateItemAsync(request);
                }

                _logger.Information("Item Updated Successfully!");
            }
            catch (Exception ex)
            {
                _logger.Error("Item Update Failed!", ex);
            }
        }

        public async Task<DynamoItem> GetDynamoDbItem(string name, int age, string _tableName)
        {
            try
            {
                string tableName = _tableName;

                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                };

                using (var client = new AmazonDynamoDBClient(config))
                {
                    var context = new DynamoDBContext(client);

                    var key = new Dictionary<string, AttributeValue>
            {
                { "Name", new AttributeValue { S = name } },
                { "Age", new AttributeValue { N = age.ToString() } }
            };

                    var request = new GetItemRequest
                    {
                        TableName = tableName,
                        Key = key
                    };

                    var response = await client.GetItemAsync(request);

                    if (response.IsItemSet)
                    {
                        DynamoItem item = new DynamoItem
                        {
                            Name = name,
                            Age = age,
                        };

                        if (response.Item.ContainsKey("Mobile"))
                        {
                            item.PhoneNumber = response.Item["Mobile"].N;
                        }
                        else
                        {
                            // Handle the case where the "Mobile" attribute is missing, e.g., set it to a default value.
                            item.PhoneNumber = "";
                        }

                        if (response.Item.ContainsKey("Address"))
                        {
                            item.Address = response.Item["Address"].S;
                        }
                        else
                        {
                            // Handle the case where the "Address" attribute is missing, e.g., set it to a default value.
                            item.Address = "";
                        }

                        return item;
                    }
                    else
                    {
                        // Handle the case where the item doesn't exist
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors as needed
                _logger.Error("Item Retrieval Failed!", ex);
                return null;
            }
        }


        public async Task<IList<DynamoItem>> GetDynamoDbItems(string tableName)
        {
            try
            {
                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                };

                using (var client = new AmazonDynamoDBClient(config))
                {
                    var context = new DynamoDBContext(client);

                    var scanRequest = new ScanRequest
                    {
                        TableName = tableName
                    };

                    var scanResponse = await client.ScanAsync(scanRequest);

                    if (scanResponse.Items.Count > 0)
                    {
                        var items = new List<DynamoItem>();

                        foreach (var item in scanResponse.Items)
                        {
                            DynamoItem dynamoItem = new DynamoItem
                            {
                                Name = item["Name"].S,
                                Age = int.Parse(item["Age"].N)
                            };

                            if (item.ContainsKey("Mobile"))
                            {
                                dynamoItem.PhoneNumber = item["Mobile"].S;
                            }
                            else
                            {
                                dynamoItem.PhoneNumber = "";
                            }

                            if (item.ContainsKey("Address"))
                            {
                                dynamoItem.Address = item["Address"].S;
                            }
                            else
                            {
                                dynamoItem.Address = "";
                            }

                            items.Add(dynamoItem);
                        }

                        return items;
                    }
                    else
                    {
                        // Handle the case where no items were found
                        return new List<DynamoItem>();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or log errors as needed
                _logger.Error("Item Retrieval Failed!", ex);
                return new List<DynamoItem>();
            }
        }


        public async Task<object> GetDynamoItemsAsync(string tableName)
        {
            try
            {
                string awsRegion = "us-east-1"; // Change to your desired region

                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsRegion)
                };

                using (var client = new AmazonDynamoDBClient(config))
                {
                    var context = new DynamoDBContext(client);

                    var scanRequest = new ScanRequest
                    {
                        TableName = tableName
                    };

                    var scanResponse = await client.ScanAsync(scanRequest);

                    // Convert the scan response to an object (you can create a custom class to represent the data)
                    // For simplicity, we'll use a dynamic object here
                    var data = new
                    {
                        Items = scanResponse.Items
                    };

                    return data;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return an error response
                return ex;
            }
        }


        public async Task DeleteDynamoDbItem(string name, int age, string _tableName)
        {
            try
            {
                string tableName = _tableName;

                var config = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.USEast1
                };

                using (var client = new AmazonDynamoDBClient(config))
                {
                    var request = new DeleteItemRequest
                    {
                        TableName = tableName,
                        Key = new Dictionary<string, AttributeValue>
                {
                    { "Name", new AttributeValue { S = name } }, // Assuming "Name" is a string
                    { "Age", new AttributeValue { N = age.ToString() } } // Assuming "Age" is a number
                }
                    };

                    await client.DeleteItemAsync(request);
                }

                _logger.Information("item deleted Successfully!");
            }
            catch (Exception ex)
            {
                _logger.Information("item Delete Failed!", ex);
            }
        }


    }
}
