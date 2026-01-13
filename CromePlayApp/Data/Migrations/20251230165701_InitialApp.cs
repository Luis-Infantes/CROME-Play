using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CromePlayApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CalendarEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CalendarEvents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CalendarEvents",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Campaña dedicada a la Era de apocalipsis, etc. Coste de la partida de 5€ que se abonara el mismo día del evento");

            migrationBuilder.UpdateData(
                table: "CalendarEvents",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Campaña dedicada a la lucha contra los sith, etc. Coste de la partida de 5€ que se abonara el mismo día del evento");

            migrationBuilder.UpdateData(
                table: "CalendarEvents",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Campaña dedicada a la comunidad del anillo, etc. Coste de la partida de 5€ que se abonara el mismo día del evento");

            migrationBuilder.UpdateData(
                table: "CalendarEvents",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Campaña dedicada a la llamada de Cthulhu, etc. Coste de la partida de 5€ que se abonara el mismo día del evento");

            migrationBuilder.UpdateData(
                table: "CalendarEvents",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: "Campaña dedicada a las facciones vampiricas, etc. Coste de la partida de 5€ que se abonara el mismo día del evento");

            migrationBuilder.UpdateData(
                table: "CalendarEvents",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: "Campeonato estandar, etc. Coste de la partida de 5€ que se abonara el mismo día del evento");
        }
    }
}
