using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostReactions_Reaction_ReactionId",
                table: "PostReactions");

            migrationBuilder.DropTable(
                name: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_PostReactions_ReactionId",
                table: "PostReactions");

            migrationBuilder.RenameColumn(
                name: "ReactionId",
                table: "PostReactions",
                newName: "reactiontType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reactiontType",
                table: "PostReactions",
                newName: "ReactionId");

            migrationBuilder.CreateTable(
                name: "Reaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reaction", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_ReactionId",
                table: "PostReactions",
                column: "ReactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_Reaction_ReactionId",
                table: "PostReactions",
                column: "ReactionId",
                principalTable: "Reaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
