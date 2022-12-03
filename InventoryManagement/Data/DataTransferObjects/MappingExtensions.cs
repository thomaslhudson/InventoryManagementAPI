using InventoryManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data.DataTransferObjects
{
    public static class MappingExtensions
    {
        public static RecordDto ToDto(this Record record)
        {
            if (record != null)
            {
                return new RecordDto
                {
                    Id = record.Id,
                    Month = record.Month,
                    Year = record.Year,
                    Created = record.Created,
                    Started = record.Started,
                    Ended = record.Ended
                };
            }

            return null;
        }

        public static RecordItemDto ToDto(this RecordItem recordItem)
        {
            if (recordItem != null)
            {
                return new RecordItemDto
                {
                    Id = recordItem.Id,
                    Quantity = recordItem.Quantity,
                    DateCreated = recordItem.DateCreated,
                    DateModified = recordItem.DateModified,
                    RecordId = recordItem.Record.Id,
                    RecordMonth = recordItem.Record.Month,
                    RecordYear = recordItem.Record.Year,
                    ProductId = recordItem.Product.Id,
                    ProductName = recordItem.Product.Name,
                    ProductUnitPrice = recordItem.Product.UnitPrice,
                    ProductGroupName = recordItem.Product.Group.Name
                };
            }

            return null;
        }

        public static ProductDto ToDto(this Product product)
        {
            if (product != null)
            {
                return new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Upc = product.Upc,
                    UnitPrice = product.UnitPrice,
                    IsActive = product.IsActive,
                    GroupId = product.GroupId,
                    Group = product.Group.ToDto()
                };
            }

            return null;
        }

        public static GroupDto ToDto(this Group group)
        {
            if (group != null)
            {
                return new GroupDto
                {
                    Id = group.Id,
                    Name = group.Name
                };
            }

            return null;
        }
    }
}