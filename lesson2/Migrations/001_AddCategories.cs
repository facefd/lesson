using Microsoft.EntityFrameworkCore.Migrations;

namespace lesson2.Migrations;

public partial class AddCategories : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
            });

        migrationBuilder.AddColumn<int>(
            name: "CategoryId",
            table: "Tasks",
            type: "INTEGER",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Tasks_CategoryId",
            table: "Tasks",
            column: "CategoryId");

        migrationBuilder.AddForeignKey(
            name: "FK_Tasks_Categories_CategoryId",
            table: "Tasks",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "Id");

        migrationBuilder.Sql(@"
            INSERT INTO [Categories] ([Id], [Name]) VALUES (1, 'Работа');
            INSERT INTO [Categories] ([Id], [Name]) VALUES (2, 'Учёба');
            INSERT INTO [Categories] ([Id], [Name]) VALUES (3, 'Личное');
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Tasks_Categories_CategoryId", table: "Tasks");
        migrationBuilder.DropIndex(name: "IX_Tasks_CategoryId", table: "Tasks");
        migrationBuilder.DropColumn(name: "CategoryId", table: "Tasks");
        migrationBuilder.DropTable(name: "Categories");
    }
}
