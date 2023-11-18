using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDB.Domain.Entities
{
    public class DynamoItem 
    {


        //This name and Age are Partition  Key and Sort Key according to DynamoDB
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Age must be an integer greater than zero")]
        public int Age { get; set; }

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
