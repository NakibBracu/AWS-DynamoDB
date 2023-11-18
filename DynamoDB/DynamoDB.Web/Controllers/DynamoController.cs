using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Amazon.Runtime.Internal;
using Autofac;
using DynamoDB.Domain.Entities;
using DynamoDB.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DynamoDB.Web.Controllers
{
    public class DynamoController : Controller
    {
        private readonly IConfiguration _configuration;
        ILifetimeScope _scope;
        ILogger<DynamoController> _logger;

        public DynamoController(IConfiguration configuration, ILifetimeScope scope, ILogger<DynamoController> logger)
        {
            _configuration = configuration;
            _scope = scope;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            var model = _scope.Resolve<ItemListModel>();
            string tableName = _configuration["DynamoDbConfig:TableName"];
           

            // Convert the data to the expected model type
            IList<DynamoItem> itemList = await GetDynamoDbItems();

            return View(itemList); // Pass 'itemList' as the model to the view
        }

        public IActionResult AddDataToRow()
        {
            var model = _scope.Resolve<ItemCreateModel>();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddDataToRow(ItemCreateModel model)
        {
            model.ResolveDependency(_scope);

            if (ModelState.IsValid)
            {
                try
                {
                    string tableName = _configuration["DynamoDbConfig:TableName"];
                    model.AddRowWithData(tableName);
                }
                catch (DuplicateNameException ex)
                {
                    _logger.LogError(ex, ex.Message);

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Server Error");


                }
            }

            return RedirectToAction("Index");
        }

    
        
        [Route("Dynamo/UpdateData")]
        public async Task<IActionResult> UpdateData([FromQuery] string name, [FromQuery] int age)
        {
            try
            {
                string tableName = _configuration["DynamoDbConfig:TableName"];
                var model = new ItemUpdateModel(name,age,_scope);
                await model.LoadData(name, age,tableName);

                return View(model);
            }
            catch (ItemNotFoundException ex)
            {
                // Handle the case where the item is not found, for example, by redirecting to an error page.
                return View("ErrorPage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data for update.");
                // Handle the error as needed, e.g., return an error view or a redirect.
                return View("ErrorPage");
            }
        }


        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateItem(ItemUpdateModel model)
        {
            model.ResolveDependency(_scope);

            if (ModelState.IsValid)
            {
                try
                {
                    string tableName = _configuration["DynamoDbConfig:TableName"];
                    model.UpdateRowData(tableName);

                    // Redirect to the success view after a successful update
                    return View("Index");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Server Error");
                    // Handle the error as needed, e.g., show an error view or a redirect to an error page.
                }
            }

            // If there was an issue with the update, return to the same update view.
            return View("UpdateData", model);
        }



        public async Task<IActionResult> GetDynamoTableItems()
        {
            var model = _scope.Resolve<ItemListModel>();
            string tableName = _configuration["DynamoDbConfig:TableName"];
            var data = await model.GetPagedDynamoItemAsync(tableName); // Retrieve the data

            return Json(new { data = data }); // Return it as JSON
        }

        public async Task<IList<DynamoItem>> GetDynamoDbItems()
        {
            var tableName = _configuration["DynamoDbConfig:TableName"];
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
                // You can also return an error view or message here if needed
                return new List<DynamoItem>();
            }
        }


        [Route("Dynamo/DeleteData")]
        public IActionResult DeleteData([FromQuery] string name, [FromQuery] int age)
        {
            var model = _scope.Resolve<ItemListModel>();

            if (ModelState.IsValid)
            {
                try
                {
                    string tableName = _configuration["DynamoDbConfig:TableName"];
                    model.DeleteItem(name,age, tableName);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Server Error");
                }
            }

            return RedirectToAction("Index");
        }


    }
}
