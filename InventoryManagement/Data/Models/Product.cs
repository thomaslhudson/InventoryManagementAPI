namespace InventoryManagement.Data.Models
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public string Upc { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsActive { get; set;}
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public IList<RecordItem> RecordItems { get; set; }
    }
}
