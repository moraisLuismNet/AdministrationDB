using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministrationDataBaseData.Migrations
{
    /// <inheritdoc />
    public partial class administrationDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MassagesCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Surnames = table.Column<string>(type: "TEXT", nullable: false),
                    DNI = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    From = table.Column<string>(type: "TEXT", nullable: true),
                    Work = table.Column<string>(type: "TEXT", nullable: true),
                    PhysicalActivity = table.Column<string>(type: "TEXT", nullable: true),
                    Pregnancies = table.Column<string>(type: "TEXT", nullable: true),
                    Operations = table.Column<string>(type: "TEXT", nullable: true),
                    OtherObservations = table.Column<string>(type: "TEXT", nullable: true),
                    ReasonForVisit = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MassagesCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pathologies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pathologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", nullable: true),
                    HashDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Massages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MassageDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OtherObservations = table.Column<string>(type: "TEXT", nullable: false),
                    NotificationSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdMassagesCustomer = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Massages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Massages_MassagesCustomers_IdMassagesCustomer",
                        column: x => x.IdMassagesCustomer,
                        principalTable: "MassagesCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MassagesCustomerPathologies",
                columns: table => new
                {
                    MassagesCustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PathologyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MassagesCustomerPathologies", x => new { x.MassagesCustomerId, x.PathologyId });
                    table.ForeignKey(
                        name: "FK_MassagesCustomerPathologies_MassagesCustomers_MassagesCustomerId",
                        column: x => x.MassagesCustomerId,
                        principalTable: "MassagesCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MassagesCustomerPathologies_Pathologies_PathologyId",
                        column: x => x.PathologyId,
                        principalTable: "Pathologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    PathologyId = table.Column<int>(type: "INTEGER", nullable: false),
                    MassagesCustomerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observations_MassagesCustomers_MassagesCustomerId",
                        column: x => x.MassagesCustomerId,
                        principalTable: "MassagesCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Observations_Pathologies_PathologyId",
                        column: x => x.PathologyId,
                        principalTable: "Pathologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Massages_IdMassagesCustomer",
                table: "Massages",
                column: "IdMassagesCustomer");

            migrationBuilder.CreateIndex(
                name: "IX_MassagesCustomerPathologies_PathologyId",
                table: "MassagesCustomerPathologies",
                column: "PathologyId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_MassagesCustomerId",
                table: "Observations",
                column: "MassagesCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_PathologyId",
                table: "Observations",
                column: "PathologyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Massages");

            migrationBuilder.DropTable(
                name: "MassagesCustomerPathologies");

            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "MassagesCustomers");

            migrationBuilder.DropTable(
                name: "Pathologies");
        }
    }
}
