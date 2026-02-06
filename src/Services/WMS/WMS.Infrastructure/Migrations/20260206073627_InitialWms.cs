using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialWms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "wms");

            migrationBuilder.CreateTable(
                name: "inventories",
                schema: "wms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    warehouse_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    location_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    quantity_on_hand = table.Column<int>(type: "integer", nullable: false),
                    quantity_reserved = table.Column<int>(type: "integer", nullable: false),
                    reorder_point = table.Column<int>(type: "integer", nullable: false),
                    reorder_quantity = table.Column<int>(type: "integer", nullable: false),
                    last_stock_check = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inventory_transactions",
                schema: "wms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inventory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    warehouse_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    quantity_before = table.Column<int>(type: "integer", nullable: false),
                    quantity_after = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    reference_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    source_event_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    transaction_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_transactions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_inventories_sku",
                schema: "wms",
                table: "inventories",
                column: "sku");

            migrationBuilder.CreateIndex(
                name: "ix_inventories_sku_warehouse_id",
                schema: "wms",
                table: "inventories",
                columns: new[] { "sku", "warehouse_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventories_warehouse_id",
                schema: "wms",
                table: "inventories",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transactions_inventory_id",
                schema: "wms",
                table: "inventory_transactions",
                column: "inventory_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transactions_reference_id",
                schema: "wms",
                table: "inventory_transactions",
                column: "reference_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transactions_sku",
                schema: "wms",
                table: "inventory_transactions",
                column: "sku");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_transactions_transaction_date",
                schema: "wms",
                table: "inventory_transactions",
                column: "transaction_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventories",
                schema: "wms");

            migrationBuilder.DropTable(
                name: "inventory_transactions",
                schema: "wms");
        }
    }
}
