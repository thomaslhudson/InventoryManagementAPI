using System.Text.Json;

namespace InventoryManagement.Helpers.JSON
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLower();
    }
}