﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdministrationDataBaseData.Migrations
{
    /// <inheritdoc />
    public partial class pilates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Objectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objectives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PilatesCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Surnames = table.Column<string>(type: "TEXT", nullable: false),
                    DNI = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    AccountNumber = table.Column<string>(type: "TEXT", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    OtherObservations = table.Column<string>(type: "TEXT", nullable: true),
                    IllnessInjuryPathology = table.Column<bool>(type: "INTEGER", nullable: true),
                    DiseaseInjuryPathologyObservations = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilatesCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PilatesCustomerObjectives",
                columns: table => new
                {
                    PilatesCustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectiveId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilatesCustomerObjectives", x => new { x.PilatesCustomerId, x.ObjectiveId });
                    table.ForeignKey(
                        name: "FK_PilatesCustomerObjectives_Objectives_ObjectiveId",
                        column: x => x.ObjectiveId,
                        principalTable: "Objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PilatesCustomerObjectives_PilatesCustomers_PilatesCustomerId",
                        column: x => x.PilatesCustomerId,
                        principalTable: "PilatesCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SessionObservations = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    PilatesCustomerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_PilatesCustomers_PilatesCustomerId",
                        column: x => x.PilatesCustomerId,
                        principalTable: "PilatesCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PilatesCustomerObjectives_ObjectiveId",
                table: "PilatesCustomerObjectives",
                column: "ObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_PilatesCustomerId",
                table: "Sessions",
                column: "PilatesCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PilatesCustomerObjectives");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Objectives");

            migrationBuilder.DropTable(
                name: "PilatesCustomers");
        }
    }
}
