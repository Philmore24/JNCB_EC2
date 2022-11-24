using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace JNCB.Migrations
{
    public partial class newtranstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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


               });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "accountTransaction");
        }
    }
}
