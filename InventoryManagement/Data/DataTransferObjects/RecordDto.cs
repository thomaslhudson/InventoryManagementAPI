namespace InventoryManagement.Data.DataTransferObjects
{
    public class RecordDto
    {
        public Guid Id { get; set; }

        public byte Month { get; set; }

        public short Year { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Started { get; set; }

        public DateTime? Ended { get; set; }
    }
}
