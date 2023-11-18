using System.Runtime.Serialization;

namespace DynamoDB.Web.Models
{
    [Serializable]
    internal class ItemNotFoundException : Exception
    {
        public ItemNotFoundException()
        {
        }

        public ItemNotFoundException(string? message) : base(message)
        {
        }

     
    }
}