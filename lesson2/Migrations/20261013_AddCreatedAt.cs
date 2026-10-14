using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lesson2.Migrations
{
    /// <summary>
    /// Вторая миграция: добавление колонки CreatedAt к таблице Tasks.
    /// Существующие задачи не потеряются — у них будет значение NULL или 
    /// по умолчанию (CURRENT_TIMESTAMP из таблицы).
    /// </summary>
    public partial class AddCreatedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Добавляем колонку CreatedAt с дефолтным значением CURRENT_TIMESTAMP
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tasks",
                type: "datetime",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаление колонки при откате миграции
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tasks");
        }
    }
}