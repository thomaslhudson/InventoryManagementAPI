using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

#nullable disable

namespace InventoryManagement.Data.Migrations
{
    public partial class spGroupTotalsByRecordSubset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            StringBuilder spText = new StringBuilder();
            spText.AppendLine("CREATE PROCEDURE [dbo].[spGroupTotalsByRecordSubset]");
            spText.AppendLine("@RecordId uniqueidentifier,");
            spText.AppendLine("@List nvarchar(max)");
            spText.AppendLine("AS");
            spText.AppendLine("SET NOCOUNT ON;");
            spText.AppendLine("SELECT\t\t[Group].[Name],");
            spText.AppendLine("\t\t\tCAST(SUM([UnitPrice]*[Quantity]) as decimal(9, 2))  AS [Total]");
            spText.AppendLine("FROM\t\t[Record] INNER JOIN");
            spText.AppendLine("\t\t\t[RecordItem] ON [Record].[Id] = [RecordItem].[RecordId] INNER JOIN");
            spText.AppendLine("\t\t\t[Product] ON [RecordItem].[ProductId] = [Product].[Id] INNER JOIN");
            spText.AppendLine("\t\t\t[Group] ON [Product].[GroupId] = [Group].[Id]");
            spText.AppendLine("WHERE\t\t[Record].[Id] = @RecordId AND");
            spText.AppendLine("\t\t\t[Group].[Name] IN(SELECT * FROM STRING_SPLIT(@List, ','))");
            spText.AppendLine("GROUP BY\t[Group].[Name]");
            spText.AppendLine("ORDER BY\t[Group].[Name]");

            migrationBuilder.Sql(spText.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROC spGroupTotalsByRecordSubset";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}
