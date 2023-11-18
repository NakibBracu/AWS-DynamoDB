using Autofac;
using DynamoDB.Application.Services;
using System.ComponentModel.DataAnnotations;

namespace DynamoDB.Web.Models
{
    public class ItemUpdateModel
    {
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Age must be an integer greater than zero")]
        public int Age { get; set; }

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        private IDynamoDBService _dynamoDBService { get; set; }
        public ItemUpdateModel()
        {

        }
        public ItemUpdateModel(IDynamoDBService dynamoDBService)
        {
            _dynamoDBService = dynamoDBService;
        }

        public ItemUpdateModel(string name, int age, ILifetimeScope scope)
        {
            Name = name;
            Age = age;
            _dynamoDBService = scope.Resolve<IDynamoDBService>();
        }

        internal void ResolveDependency(ILifetimeScope scope)
        {
            _dynamoDBService = scope.Resolve<IDynamoDBService>();
        }

        internal async Task LoadData(string name, int age,string _tableName)
        {
            // Load data from your data service and populate the model properties.
            var item = await _dynamoDBService.GetDynamoDbItem(name, age,_tableName);

            if (item != null)
            {
                Name = item.Name;
                Age = item.Age;
                Address = item.Address;
                PhoneNumber = item.PhoneNumber;
            }
            else
            {
                // Handle the case where the item is not found by throwing a custom exception.
                throw new ItemNotFoundException("Item not found");
            }


        }

        internal void UpdateRowData(string tableName)
        {

            _dynamoDBService.UpdateDynamoDbItem(Name, Age, PhoneNumber, Address, tableName);

        }




    }
}
