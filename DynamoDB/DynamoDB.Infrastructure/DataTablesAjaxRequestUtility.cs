using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDB.Infrastructure
{
    public class DataTablesAjaxRequestUtility
    {
        //This class is written for holding http parameter from DataTable
        private HttpRequest _request;

        public int Start
        {
            get
            {
                return int.Parse(RequestData.Where(x => x.Key == "start")
                    .FirstOrDefault().Value);
            }
        }
        public int Length
        {
            get
            {
                return int.Parse(RequestData.Where(x => x.Key == "length")
                    .FirstOrDefault().Value);
            }
        }

        public string SearchText
        {
            get
            {
                return RequestData.Where(x => x.Key == "search[value]")
                    .FirstOrDefault().Value;
            }
        }

        public DataTablesAjaxRequestUtility(HttpRequest request)
        {
            _request = request;
        }

        public int PageIndex
        {
            get
            {
                if (Length > 0)
                    return (Start / Length) + 1;
                else
                    return 1;
            }
        }

        public int PageSize
        {
            get
            {
                if (Length == 0)
                    return 10;
                else
                    return Length;
            }
        }

        private IEnumerable<KeyValuePair<string, StringValues>> RequestData
        {
            get
            {
                var method = _request.Method.ToLower();
                if (method == "get")
                    return _request.Query;
                else if (method == "post")
                    return _request.Form;
                else
                    throw new InvalidOperationException("Http method not supported, use get or post");
            }
        }

        public static object EmptyResult
        {
            get
            {
                return new
                {
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = (new string[] { }).ToArray()
                };
            }
        }

        public string GetSortText(string[] columnNames)
        {
            var sortText = new StringBuilder();
            for (var i = 0; i < columnNames.Length; i++)
            {
                if (RequestData.Any(x => x.Key == $"order[{i}][column]"))
                {
                    if (sortText.Length > 0)
                        sortText.Append(",");

                    var columnValue = RequestData.Where(x => x.Key == $"order[{i}][column]").FirstOrDefault();
                    var directionValue = RequestData.Where(x => x.Key == $"order[{i}][dir]").FirstOrDefault();

                    var column = int.Parse(columnValue.Value.ToArray()[0]);
                    var direction = directionValue.Value.ToArray()[0];
                    var sortDirection = $"{columnNames[column]} {(direction == "asc" ? "asc" : "desc")}";
                    sortText.Append(sortDirection);
                }
            }
            return sortText.ToString();
        }
    }
}
