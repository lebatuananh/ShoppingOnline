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
    public class ColorDapperService : IColorDapperService
    {
        private readonly IConfiguration _configuration;

        public ColorDapperService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public IEnumerable<ColorViewModel> GetColors(int productId)
        {
            using (var sqlConnection =
                new SqlConnection(_configuration.GetConnectionString(SystemConstants.ConnectionString)))
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@productId", productId);
                try
                {
                    return sqlConnection.Query<ColorViewModel>(
                        @"select c.Id as Id, c.[Code] as Code, c.[Name] as [Name] from Colors as c
                    join ProductQuantities as p
                    on c.Id=p.ColorId and p.Quantity>0 and p.ProductId=@productId
                    group by c.Id, c.Code, c.[Name]", dynamicParameters).ToList();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}