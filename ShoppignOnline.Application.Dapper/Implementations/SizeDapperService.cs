using Dapper;
using Microsoft.Extensions.Configuration;
using ShoppignOnline.Application.Dapper.Interfaces;
using ShoppingOnline.Application.ECommerce.Products.Dtos;
using ShoppingOnline.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ShoppignOnline.Application.Dapper.Implementations
{
    public class SizeDapperService: ISizeDapperService
    {
        private readonly IConfiguration _configuration;

        public SizeDapperService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IEnumerable<SizeViewModel> GetSizes(int productId)
        {
            using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString(SystemConstants.ConnectionString)))
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@productId", productId);
                try
                {
                    return sqlConnection.Query<SizeViewModel>(@"select c.Id as Id, c.[Name] as [Name] from Sizes as c

                    join ProductQuantities as p

                    on c.Id=p.SizeId and p.Quantity>0 and p.ProductId=@productId

                    group by c.Id, c.[Name]

                    select * from Sizes", dynamicParameters).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
