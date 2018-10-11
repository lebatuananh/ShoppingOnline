using System;
using ShoppingOnline.Application.Systems.Functions.Dtos;
using ShoppingOnline.Application.Systems.Roles.Dtos;

namespace ShoppingOnline.Application.Systems.Permissions.Dtos
{
    public class PermissionViewModel
    {
        public int Id { get; set; }

        public Guid RoleId { get; set; }

        public string FunctionId { get; set; }

        public bool CanCreate { set; get; }

        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }

        public bool CanDelete { set; get; }

        public AppRoleViewModel AppRole { get; set; }

        public FunctionViewModel Function { get; set; }
    }
}