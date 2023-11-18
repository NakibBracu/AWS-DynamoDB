using Autofac;
using DynamoDB.Application.Services;
using System.ComponentModel.DataAnnotations;

namespace DynamoDB.Web.Models
{
    public class ItemCreateModel
    {
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Age must be an integer greater than zero")]
        public int Age { get; set; }

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        private IDynamoDBService _dynamoDBService { get; set; }
        public ItemCreateModel()
        {

        }
        public ItemCreateModel(IDynamoDBService dynamoDBService)
        {
            _dynamoDBService = dynamoDBService;
        }

        internal void ResolveDependency(ILifetimeScope scope)
        {
            _dynamoDBService = scope.Resolve<IDynamoDBService>();
        }

        internal void AddRowWithData(string tableName)
        {

           
            _dynamoDBService.AddRowWithDatainDyanmoDBTable(Name, Age, PhoneNumber, Address, tableName);

        }




    }
}
