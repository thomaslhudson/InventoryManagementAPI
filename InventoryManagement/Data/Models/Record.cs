namespace InventoryManagement.Data.Models
{
    public class Record
    {
        public Guid Id { get; private set; }

        public byte Month { get; set; }

        public short Year { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Ended { get; set; }

        public IList<RecordItem> RecordItems { get; set; }
    }
}

