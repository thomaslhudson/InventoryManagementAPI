using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Data.DataTransferObjects
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Upc { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set; }
        public Guid GroupId { get; set; }
        public GroupDto Group { get; set; }
    }
}
