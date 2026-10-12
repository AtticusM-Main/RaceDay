using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RaceDay.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_Role",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganiserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Location = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    EventType = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.EventId);
                    table.CheckConstraint("CK_Event_EventType", "[EventType] IN ('Run', 'Walk', 'Cycle')");
                    table.ForeignKey(
                        name: "FK_Event_Organiser",
                        column: x => x.OrganiserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Distance = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    MaxParticipants = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Category_Event",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrolment",
                columns: table => new
                {
                    EnrolmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    EnrolmentDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrolment", x => x.EnrolmentId);
                    table.ForeignKey(
                        name: "FK_Enrolment_Category",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrolment_Participant",
                        column: x => x.ParticipantId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Result",
                columns: table => new
                {
                    ResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrolmentId = table.Column<int>(type: "int", nullable: false),
                    FinishTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Result", x => x.ResultId);
                    table.ForeignKey(
                        name: "FK_Result_Enrolment",
                        column: x => x.EnrolmentId,
                        principalTable: "Enrolment",
                        principalColumn: "EnrolmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "Organiser" },
                    { 2, "Participant" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "CreatedAt", "Email", "FullName", "PasswordHash", "RoleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "naledi.khumalo@raceday.co.za", "Naledi Khumalo", "AQAAAAIAAYagAAAAEDk+C1J4uROV2oSQmmBgDXzS+KwwS1h7rw0GMemaoQuaC9GsEYpFwcxFy9MFc7WF6Q==", 1 },
                    { 2, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "johan.botha@raceday.co.za", "Johan Botha", "AQAAAAIAAYagAAAAEKsDkXCS6dxTig6+ORCDmGYbgYcXkdhq20qmul9Ec/bqZGaUTC7SKoXW4J/HFtd9AQ==", 1 },
                    { 3, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "thandiwe.nkosi@example.com", "Thandiwe Nkosi", "AQAAAAIAAYagAAAAEFJWFVSDlBvwSX8dWVV+BOdNGP+yvWuWxVktQdOLPIvy3ZqtHaJPdUjLF5fS1s+pYw==", 2 },
                    { 4, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "ryan.pillay@example.com", "Ryan Pillay", "AQAAAAIAAYagAAAAEN+iUFzbRO5j8njgO0zx1VXqSRMjdpVeCA5/W0SCnBLQq+hEAeumNHfVZ1vv4AQwVw==", 2 }
                });

            migrationBuilder.InsertData(
                table: "Event",
                columns: new[] { "EventId", "CreatedAt", "Description", "Distance", "EventDate", "EventType", "Location", "Name", "OrganiserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Annual road running event through the city bowl.", 10.00m, new DateTime(2026, 10, 18, 7, 0, 0, 0, DateTimeKind.Unspecified), "Run", "Cape Town, South Africa", "Cape Town Spring Run", 1 },
                    { 2, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Coastal race along the Golden Mile.", 21.10m, new DateTime(2026, 11, 8, 6, 30, 0, 0, DateTimeKind.Unspecified), "Run", "Durban, South Africa", "Durban Beachfront Race", 1 },
                    { 3, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "Off-road trail run through the Northern suburbs.", 15.00m, new DateTime(2026, 11, 22, 6, 0, 0, 0, DateTimeKind.Unspecified), "Run", "Johannesburg, South Africa", "Joburg Trail Challenge", 2 }
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "CategoryId", "Distance", "EventId", "MaxParticipants", "Name" },
                values: new object[,]
                {
                    { 1, 5.00m, 1, 500, "5km" },
                    { 2, 10.00m, 1, 300, "10km" },
                    { 3, 5.00m, 2, 400, "5km" },
                    { 4, 21.10m, 2, 150, "21km" },
                    { 5, 15.00m, 3, 200, "15km" }
                });

            migrationBuilder.InsertData(
                table: "Enrolment",
                columns: new[] { "EnrolmentId", "CategoryId", "EnrolmentDate", "ParticipantId" },
                values: new object[,]
                {
                    { 1, 2, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 2, 2, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 3, 4, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), 3 }
                });

            migrationBuilder.InsertData(
                table: "Result",
                columns: new[] { "ResultId", "EnrolmentId", "FinishTime", "Position", "RecordedAt" },
                values: new object[] { 1, 1, new TimeSpan(0, 0, 52, 14, 0), 3, new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Category_EventId",
                table: "Category",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolment_CategoryId",
                table: "Enrolment",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolment_ParticipantId",
                table: "Enrolment",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_OrganiserId",
                table: "Event",
                column: "OrganiserId");

            migrationBuilder.CreateIndex(
                name: "IX_Result_EnrolmentId",
                table: "Result",
                column: "EnrolmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Role_RoleName",
                table: "Role",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Email",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Result");

            migrationBuilder.DropTable(
                name: "Enrolment");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
