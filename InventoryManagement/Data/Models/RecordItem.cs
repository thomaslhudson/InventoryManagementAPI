namespace InventoryManagement.Data.Models
{
    public class RecordItem
    {
        public Guid Id { get; private set; }

        public decimal Quantity { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public Guid RecordId { get; set; }

        public Record Record { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; }
    }
}
