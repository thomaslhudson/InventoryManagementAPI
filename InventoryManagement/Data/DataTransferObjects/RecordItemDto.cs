using InventoryManagement.Data.Models;

namespace InventoryManagement.Data.DataTransferObjects
{
    public class RecordItemDto
    {
        public Guid Id { get; set; }

        public decimal Quantity { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public Guid RecordId { get; set; }

        public byte RecordMonth { get; set; }

        public short RecordYear { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ProductUnitPrice { get; set; }

        public string ProductGroupName { get; set; }
    }
}
