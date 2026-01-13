using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CromePlayApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewInitialApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BackgroundClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeBackgroundFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clubes",
                columns: table => new
                {
                    ClubId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClubName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClubEmail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClubDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubes", x => x.ClubId);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MemberName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberPhone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_Members_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PendingUsers",
                columns: table => new
                {
                    PendingUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PendingUserEmail = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingUsers", x => x.PendingUserId);
                    table.ForeignKey(
                        name: "FK_PendingUsers_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClubId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Games_Clubes_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubes",
                        principalColumn: "ClubId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AvailablePlace = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    EnrollmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    GameId1 = table.Column<int>(type: "int", nullable: true),
                    MemberId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.EnrollmentId);
                    table.ForeignKey(
                        name: "FK_Enrollments_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Games_GameId1",
                        column: x => x.GameId1,
                        principalTable: "Games",
                        principalColumn: "GameId");
                    table.ForeignKey(
                        name: "FK_Enrollments_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Members_MemberId1",
                        column: x => x.MemberId1,
                        principalTable: "Members",
                        principalColumn: "MemberId");
                });

            migrationBuilder.InsertData(
                table: "Clubes",
                columns: new[] { "ClubId", "ClubDescription", "ClubEmail", "ClubName" },
                values: new object[] { 1, "Adéntrate en un mundo donde la magia y la estrategia se entrelazan. Forja tu destino como héroe, invocando criaturas legendarias, lanzando poderosos hechizos y desafiando a tus rivales en épicas batallas. Cada carta es una historia, cada jugada, una leyenda.", "CardGames@Contact.com", "Juegos de cartas" });

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "MemberId", "IdentityUserId", "MemberAddress", "MemberEmail", "MemberName", "MemberPhone" },
                values: new object[,]
                {
                    { 1, null, "Calle Gran Vía 12", "SophieLeF@Contact.com", "Sophie Lefèvre", "+34 612 345 678" },
                    { 2, null, "Avenida Diagonal 45", "AlejandroG@Contact.com", "Alejandro García", "+34 622 987 654" },
                    { 3, null, "Calle Larios 8", "JulienM@Contact.com", "Julien Moreau", "+34 633 456 789" },
                    { 4, null, "Paseo de la Castellana 221", "DavidW@Contact.com", "David Wilson", "+34 644 321 987" },
                    { 5, null, "Calle Mayor 10", "LukasM@Contact.com", "Lukas Müller", "+34 655 789 123" }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "GameId", "ClubId", "GameDescription", "GameName" },
                values: new object[,]
                {
                    { 1, 1, "¡Entra en el universo Marvel y conviértete en tu héroe favorito! Marvel Champions es un juego cooperativo de cartas en el que tú y tus amigos uniréis fuerzas para enfrentarse a villanos icónicos y detener sus planes malvados. Cada jugador elige un héroe con habilidades únicas y construye su mazo para adaptarse a diferentes desafíos. Con partidas dinámicas, expansiones temáticas y una experiencia inmersiva, este juego te hará sentir parte de la acción. ¿Estás listo para salvar el mundo?", "Marvel Champions" },
                    { 2, 1, "¡Explora una galaxia muy, muy lejana en este juego de cartas coleccionable lleno de acción! Star Wars: Unlimited te permite construir tu propio mazo y enfrentarte en duelos estratégicos con héroes, villanos y naves icónicas de la saga. Con reglas dinámicas, ilustraciones espectaculares y expansiones continuas, cada partida es una nueva aventura. ¿Tienes lo necesario para liderar la batalla y cambiar el destino de la galaxia?", "Star Wars (Unlimited)" },
                    { 3, 1, "Embárcate en una épica aventura por la Tierra Media. Este juego cooperativo te permite liderar a héroes legendarios en misiones peligrosas contra las fuerzas oscuras de Sauron. Con mazos personalizables, escenarios desafiantes y una narrativa envolvente, The Lord of the Rings: El Juego de Cartas es la experiencia definitiva para los fans de Tolkien.", "The Lord of the Rings" },
                    { 4, 1, "Sumérgete en los oscuros mitos de Lovecraft con este juego cooperativo narrativo. En Arkham Horror: El Juego de Cartas, asumirás el papel de un investigador que debe resolver misterios inquietantes y enfrentarse a horrores sobrenaturales. Cada decisión cuenta: construye tu mazo, explora escenarios llenos de tensión y lucha por mantener la cordura mientras desentrañas secretos prohibidos.", "Arkham Horror" },
                    { 5, 1, "En Vampire: The Eternal Struggle no eres un simple jugador: eres un Matusalén, un vampiro ancestral que mueve los hilos en la oscuridad. Recluta a tu cripta, conspira, negocia y traiciona para dominar la noche. Cada decisión cuenta, cada alianza puede romperse, y solo el más astuto sobrevivirá.\r\n¿Controlarás la sangre… o serás drenado por tus rivales? La lucha nunca termina. ¿Estás listo para jugar?", "vampire vtes" },
                    { 6, 1, "En Magic: The Gathering te conviertes en un planeswalker, un poderoso hechicero capaz de viajar entre mundos y desatar fuerzas inimaginables. Invoca criaturas legendarias, lanza conjuros devastadores y construye estrategias únicas para derrotar a tus rivales. Cada partida es un duelo épico donde la inteligencia y la imaginación deciden el destino.", "MAGIC. The gathering" }
                });

            migrationBuilder.InsertData(
                table: "CalendarEvents",
                columns: new[] { "Id", "AvailablePlace", "CreatedAt", "Description", "End", "GameId", "OwnerUserId", "Start", "Title", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Campaña dedicada a la Era de apocalipsis, etc. Coste de la partida de 5€ que se abonara el mismo día del evento", new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "MasterClub", new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Evento de Marvel Champions", "CardGames", null },
                    { 2, 18, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Campaña dedicada a la lucha contra los sith, etc. Coste de la partida de 5€ que se abonara el mismo día del evento", new DateTime(2026, 4, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "MasterClub", new DateTime(2026, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Evento de Star Wars (Unlimited)", "CardGames", null },
                    { 3, 14, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Campaña dedicada a la comunidad del anillo, etc. Coste de la partida de 5€ que se abonara el mismo día del evento", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "MasterClub", new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Evento de The Lord of the Rings", "CardGames", null },
                    { 4, 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Campaña dedicada a la llamada de Cthulhu, etc. Coste de la partida de 5€ que se abonara el mismo día del evento", new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "MasterClub", new DateTime(2026, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Evento de Arkham Horror", "CardGames", null },
                    { 5, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Campaña dedicada a las facciones vampiricas, etc. Coste de la partida de 5€ que se abonara el mismo día del evento", new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "MasterClub", new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Evento de vampire vtes", "CardGames", null },
                    { 6, 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Campeonato estandar, etc. Coste de la partida de 5€ que se abonara el mismo día del evento", new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, "MasterClub", new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Evento de MAGIC. The gathering", "CardGames", null }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "EnrollmentId", "GameId", "GameId1", "MemberId", "MemberId1" },
                values: new object[,]
                {
                    { 1, 1, null, 1, null },
                    { 2, 1, null, 2, null },
                    { 3, 1, null, 3, null },
                    { 4, 1, null, 4, null },
                    { 5, 1, null, 5, null },
                    { 6, 2, null, 1, null },
                    { 7, 2, null, 2, null },
                    { 8, 2, null, 3, null },
                    { 9, 2, null, 4, null },
                    { 11, 3, null, 1, null },
                    { 12, 3, null, 3, null },
                    { 13, 3, null, 4, null },
                    { 14, 4, null, 2, null },
                    { 15, 4, null, 3, null },
                    { 16, 4, null, 4, null },
                    { 17, 4, null, 5, null },
                    { 18, 5, null, 2, null },
                    { 19, 5, null, 3, null },
                    { 20, 5, null, 4, null },
                    { 21, 6, null, 2, null },
                    { 22, 6, null, 3, null },
                    { 23, 6, null, 4, null },
                    { 24, 6, null, 5, null },
                    { 25, 6, null, 1, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_GameId",
                table: "CalendarEvents",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Clubes_ClubEmail",
                table: "Clubes",
                column: "ClubEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_GameId",
                table: "Enrollments",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_GameId1",
                table: "Enrollments",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_MemberId",
                table: "Enrollments",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_MemberId1",
                table: "Enrollments",
                column: "MemberId1");

            migrationBuilder.CreateIndex(
                name: "IX_Games_ClubId",
                table: "Games",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_IdentityUserId",
                table: "Members",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingUsers_IdentityUserId",
                table: "PendingUsers",
                column: "IdentityUserId",
                unique: true,
                filter: "[IdentityUserId] IS NOT NULL AND [IdentityUserId] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_PendingUsers_PendingUserEmail",
                table: "PendingUsers",
                column: "PendingUserEmail",
                unique: true,
                filter: "[PendingUserEmail] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "PendingUsers");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Clubes");
        }
    }
}
