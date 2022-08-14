using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainingSystem.Domain.Migrations
{
    public partial class AddTrainerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "LoginDTO",
                columns: table => new
                {
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    RememberMe = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginDTO", x => x.Email);
                });
            
            migrationBuilder.CreateTable(
                name: "Trainer",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SectionID = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    ContactNumber = table.Column<int>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainer", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginDTO");

            migrationBuilder.DropTable(
                name: "Trainer");
        }
    }
}
