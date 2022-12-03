using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

#nullable disable

namespace InventoryManagement.Data.Migrations
{
    public partial class spGroupTotalsByRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            StringBuilder spText = new StringBuilder();
            spText.AppendLine("CREATE PROCEDURE [dbo].[spGroupTotalsByRecord] ");
            spText.AppendLine("@RecordId uniqueidentifier");
            spText.AppendLine("AS");
            spText.AppendLine("\tSET NOCOUNT ON;");
            spText.AppendLine("SELECT\t[SubQuery].[GroupName], SUM([SubQuery].[Cost]) AS [Total] ");
            spText.AppendLine("FROM\t(SELECT	CAST(SUM([UnitPrice]*[Quantity]) AS decimal(9, 2)) AS [Cost],[Group].[Name] AS [GroupName] ");
            spText.AppendLine("\t\tFROM [Record] INNER JOIN ");
            spText.AppendLine("\t\t[RecordItem] ON [Record].[Id] = [RecordItem].[RecordId] INNER JOIN ");
            spText.AppendLine("\t\t[Product] ON [RecordItem].[ProductId] = [Product].[Id] INNER JOIN ");
            spText.AppendLine("\t\t[Group] ON [Product].[GroupId] = [Group].[Id] ");
            spText.AppendLine("\t\tWHERE	[Record].[Id] = @RecordId ");
            spText.AppendLine("\t\tGROUP BY [Group].[Name])");
            spText.AppendLine("AS [SubQuery] ");
            spText.AppendLine("GROUP BY ROLLUP([GroupName])");

            migrationBuilder.Sql(spText.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROC spGroupTotalsByRecord";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}
