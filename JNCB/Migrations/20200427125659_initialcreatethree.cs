using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace JNCB.Migrations
{
    public partial class initialcreatethree : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    accountNumber = table.Column<long>(nullable: false),
                    balance = table.Column<float>(nullable: false),
                    cardNum = table.Column<string>(nullable: true),
                    availableAmount = table.Column<float>(nullable: false),
                    type = table.Column<string>(nullable: true),
                    userID = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.accountNumber);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_UserIdone",
                        column: x => x.userID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accountTransaction",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    transactionDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<float>(nullable: false),
                    receivingAccount = table.Column<long>(nullable: false),
                    remarks = table.Column<string>(maxLength: 60, nullable: true),
                    type = table.Column<string>(nullable: false),
                    senderAccount = table.Column<long>(nullable: false),
                    userID = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accountTransaction", x => x.id);

                    table.ForeignKey(
                        name: "FK_Users_UserIdthree",
                        column: x => x.userID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                       name: "FK_Account_accountNumber",
                       column: x => x.senderAccount,
                       principalTable: "Account",
                       principalColumn: "accountNumber",

                       onUpdate: ReferentialAction.Cascade);

                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                          name: "Account");

            migrationBuilder.DropTable(
                name: "accountTransaction");



        }
    }
}
