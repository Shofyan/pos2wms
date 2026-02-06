using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pos");

            migrationBuilder.CreateTable(
                name: "returns",
                schema: "pos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    return_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    original_sale_id = table.Column<Guid>(type: "uuid", nullable: false),
                    original_transaction_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    store_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    terminal_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cashier_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    return_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    refund_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    refund_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_returns", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sales",
                schema: "pos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    store_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    terminal_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cashier_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    customer_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sub_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    paid_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    change_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sales", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "return_items",
                schema: "pos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    return_id = table.Column<Guid>(type: "uuid", nullable: false),
                    original_sale_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    product_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    refund_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    condition = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    warehouse_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    location_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    restock_required = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_return_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_return_items_returns_return_id",
                        column: x => x.return_id,
                        principalSchema: "pos",
                        principalTable: "returns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "pos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sale_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_payments_sales_sale_id",
                        column: x => x.sale_id,
                        principalSchema: "pos",
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sale_items",
                schema: "pos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sale_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    product_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    tax_rate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    warehouse_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    location_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sale_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_sale_items_sales_sale_id",
                        column: x => x.sale_id,
                        principalSchema: "pos",
                        principalTable: "sales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_payments_sale_id",
                schema: "pos",
                table: "payments",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "ix_return_items_return_id",
                schema: "pos",
                table: "return_items",
                column: "return_id");

            migrationBuilder.CreateIndex(
                name: "ix_returns_original_sale_id",
                schema: "pos",
                table: "returns",
                column: "original_sale_id");

            migrationBuilder.CreateIndex(
                name: "ix_returns_return_number",
                schema: "pos",
                table: "returns",
                column: "return_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sale_items_sale_id",
                schema: "pos",
                table: "sale_items",
                column: "sale_id");

            migrationBuilder.CreateIndex(
                name: "ix_sale_items_sku",
                schema: "pos",
                table: "sale_items",
                column: "sku");

            migrationBuilder.CreateIndex(
                name: "ix_sales_completed_at",
                schema: "pos",
                table: "sales",
                column: "completed_at");

            migrationBuilder.CreateIndex(
                name: "ix_sales_created_at",
                schema: "pos",
                table: "sales",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_sales_status",
                schema: "pos",
                table: "sales",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_sales_store_id",
                schema: "pos",
                table: "sales",
                column: "store_id");

            migrationBuilder.CreateIndex(
                name: "ix_sales_transaction_number",
                schema: "pos",
                table: "sales",
                column: "transaction_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payments",
                schema: "pos");

            migrationBuilder.DropTable(
                name: "return_items",
                schema: "pos");

            migrationBuilder.DropTable(
                name: "sale_items",
                schema: "pos");

            migrationBuilder.DropTable(
                name: "returns",
                schema: "pos");

            migrationBuilder.DropTable(
                name: "sales",
                schema: "pos");
        }
    }
}
