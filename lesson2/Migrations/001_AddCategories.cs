using Microsoft.EntityFrameworkCore.Migrations;

namespace lesson2.Migrations;

// Задача 3: Миграция AddCategories
// Вручную создана для демонстрации структуры миграции EF Core со связями
public partial class AddCategories : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Таблица Categories
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

        // Добавление Column CategoryId к Tasks
        migrationBuilder.AddColumn<int>(
            name: "CategoryId",
            table: "Tasks",
            type: "INTEGER",
            nullable: true);

        // Foreign key constraint
        migrationBuilder.CreateIndex(
            name: "IX_Tasks_CategoryId",
            table: "Tasks",
            column: "CategoryId");

        // Foreign key relationship
        migrationBuilder.AddForeignKey(
            name: "FK_Tasks_Categories_CategoryId",
            table: "Tasks",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "Id");

        // Seed данные через SQL (аналог HasData)
        migrationBuilder.Sql(@"
            INSERT INTO [Categories] ([Id], [Name]) VALUES (1, 'Работа');
            INSERT INTO [Categories] ([Id], [Name]) VALUES (2, 'Учёба');
            INSERT INTO [Categories] ([Id], [Name]) VALUES (3, 'Личное');
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop FK and Index
        migrationBuilder.DropForeignKey(
            name: "FK_Tasks_Categories_CategoryId",
            table: "Tasks");

        migrationBuilder.DropIndex(
            name: "IX_Tasks_CategoryId",
            table: "Tasks");

        // Drop column
        migrationBuilder.DropColumn(
            name: "CategoryId",
            table: "Tasks");

        // Drop table
        migrationBuilder.DropTable(
            name: "Categories");
    }
}
