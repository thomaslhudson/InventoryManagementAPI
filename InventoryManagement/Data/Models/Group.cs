namespace InventoryManagement.Data.Models
{
    public class Group
    {
        public Guid Id { get; private set; }

        public string Name { get; set; }

        public IList<Product> Products { get; set; }
    }
}
