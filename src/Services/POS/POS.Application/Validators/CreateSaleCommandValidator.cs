using FluentValidation;
using POS.Application.Commands.Sales;

namespace POS.Application.Validators;

/// <summary>
/// Validator for CreateSaleCommand
/// </summary>
public sealed class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required")
            .MaximumLength(20).WithMessage("Store ID must be at most 20 characters");

        RuleFor(x => x.TerminalId)
            .NotEmpty().WithMessage("Terminal ID is required")
            .MaximumLength(20).WithMessage("Terminal ID must be at most 20 characters");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be a 3-letter ISO code");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required");

        RuleForEach(x => x.Items).SetValidator(new SaleItemRequestValidator());
    }
}

public sealed class SaleItemRequestValidator : AbstractValidator<SaleItemRequest>
{
    public SaleItemRequestValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .MinimumLength(3).WithMessage("SKU must be at least 3 characters")
            .MaximumLength(50).WithMessage("SKU must be at most 50 characters");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must be at most 200 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative");

        RuleFor(x => x.TaxRate)
            .InclusiveBetween(0, 1).WithMessage("Tax rate must be between 0 and 1 (0% to 100%)");

        RuleFor(x => x.DiscountAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative");
    }
}
