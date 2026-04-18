using AcmePortal.Common;
using AcmePortal.Model;

namespace AcmePortal.Tests.Workflow;

public class ProductRequestWorkflowTests
{
    [Fact]
    public void ValidateApproval_AddRequest_AlwaysValid()
    {
        var product = new Product { Quantity = 5 };
        var request = new ProductRequest
        {
            RequestType = RequestType.Add,
            RequestedQuantity = 100,
            Product = product
        };

        var (isValid, error) = ProductRequestWorkflow.ValidateApproval(request);

        Assert.True(isValid);
        Assert.Null(error);
    }

    [Fact]
    public void ValidateApproval_RemoveRequest_SufficientQuantity_IsValid()
    {
        var product = new Product { Quantity = 10 };
        var request = new ProductRequest
        {
            RequestType = RequestType.Remove,
            RequestedQuantity = 5,
            Product = product
        };

        var (isValid, error) = ProductRequestWorkflow.ValidateApproval(request);

        Assert.True(isValid);
        Assert.Null(error);
    }

    [Fact]
    public void ValidateApproval_RemoveRequest_ExactQuantity_IsValid()
    {
        var product = new Product { Quantity = 5 };
        var request = new ProductRequest
        {
            RequestType = RequestType.Remove,
            RequestedQuantity = 5,
            Product = product
        };

        var (isValid, error) = ProductRequestWorkflow.ValidateApproval(request);

        Assert.True(isValid);
        Assert.Null(error);
    }

    [Fact]
    public void ValidateApproval_AddRequest_NullProduct_IsValid()
    {
        // Add requests never inspect the Product reference — null should pass.
        var request = new ProductRequest
        {
            RequestType = RequestType.Add,
            RequestedQuantity = 10,
            Product = null
        };

        var (isValid, error) = ProductRequestWorkflow.ValidateApproval(request);

        Assert.True(isValid);
        Assert.Null(error);
    }

    [Fact]
    public void ValidateApproval_RemoveRequest_NullProduct_IsValid()
    {
        // The guard checks `request.Product != null` first, so a null product
        // on a Remove silently passes. This test documents (and pins) that
        // current behaviour — it may indicate a logic gap worth reviewing.
        var request = new ProductRequest
        {
            RequestType = RequestType.Remove,
            RequestedQuantity = 5,
            Product = null
        };

        var (isValid, error) = ProductRequestWorkflow.ValidateApproval(request);

        Assert.True(isValid);
        Assert.Null(error);
    }

    [Fact]
    public void ValidateApproval_RemoveRequest_RequestedQuantityZero_IsValid()
    {
        // Removing 0 units should never fail the quantity check.
        var product = new Product { Quantity = 0 };
        var request = new ProductRequest
        {
            RequestType = RequestType.Remove,
            RequestedQuantity = 0,
            Product = product
        };

        var (isValid, error) = ProductRequestWorkflow.ValidateApproval(request);

        Assert.True(isValid);
        Assert.Null(error);
    }

    [Fact]
    public void ValidateApproval_RemoveRequest_InsufficientQuantity_IsInvalid()
    {
        var product = new Product { Quantity = 5 };
        var request = new ProductRequest
        {
            RequestType = RequestType.Remove,
            RequestedQuantity = 10,
            Product = product
        };

        var (isValid, error) = ProductRequestWorkflow.ValidateApproval(request);

        Assert.False(isValid);
        Assert.Equal("Insufficient quantity", error);
    }

    [Fact]
    public void ValidateApproval_RemoveRequest_ZeroQuantity_IsInvalid()
    {
        var product = new Product { Quantity = 0 };
        var request = new ProductRequest
        {
            RequestType = RequestType.Remove,
            RequestedQuantity = 1,
            Product = product
        };

        var (isValid, error) = ProductRequestWorkflow.ValidateApproval(request);

        Assert.False(isValid);
        Assert.Equal("Insufficient quantity", error);
    }

    [Fact]
    public void ApplyQuantityChange_AddRequest_IncreasesQuantity()
    {
        var product = new Product { Quantity = 10, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        var request = new ProductRequest { RequestType = RequestType.Add, RequestedQuantity = 5 };

        ProductRequestWorkflow.ApplyQuantityChange(request, product);

        Assert.Equal(15, product.Quantity);
    }

    [Fact]
    public void ApplyQuantityChange_RemoveRequest_DecreasesQuantity()
    {
        var product = new Product { Quantity = 10, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        var request = new ProductRequest { RequestType = RequestType.Remove, RequestedQuantity = 3 };

        ProductRequestWorkflow.ApplyQuantityChange(request, product);

        Assert.Equal(7, product.Quantity);
    }

    [Fact]
    public void ApplyQuantityChange_RemoveRequest_ExactQuantity_QuantityBecomesZero()
    {
        // Removing exactly what's in stock should land at 0, not negative.
        var product = new Product { Quantity = 5, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        var request = new ProductRequest { RequestType = RequestType.Remove, RequestedQuantity = 5 };

        ProductRequestWorkflow.ApplyQuantityChange(request, product);

        Assert.Equal(0, product.Quantity);
    }

    [Fact]
    public void ApplyQuantityChange_UpdatesProductUpdatedAt()
    {
        var before = DateTime.UtcNow.AddDays(-1);
        var product = new Product { Quantity = 10, UpdatedAt = before };
        var request = new ProductRequest { RequestType = RequestType.Add, RequestedQuantity = 1 };

        ProductRequestWorkflow.ApplyQuantityChange(request, product);

        Assert.True(product.UpdatedAt > before);
    }

    [Fact]
    public void ApplyQuantityChange_UpdatedAt_IsUtc()
    {
        // The implementation uses DateTime.UtcNow — verify the Kind is preserved.
        var product = new Product { Quantity = 10, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        var request = new ProductRequest { RequestType = RequestType.Add, RequestedQuantity = 1 };

        ProductRequestWorkflow.ApplyQuantityChange(request, product);

        Assert.Equal(DateTimeKind.Utc, product.UpdatedAt.Kind);
    }

    [Fact]
    public void ApplyQuantityChange_AddRequest_ZeroQuantity_QuantityUnchanged()
    {
        // Adding 0 should be a no-op on the stock value.
        var product = new Product { Quantity = 10, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        var request = new ProductRequest { RequestType = RequestType.Add, RequestedQuantity = 0 };

        ProductRequestWorkflow.ApplyQuantityChange(request, product);

        Assert.Equal(10, product.Quantity);
    }

    [Fact]
    public void ApplyQuantityChange_RemoveRequest_ZeroQuantity_QuantityUnchanged()
    {
        // Removing 0 should be a no-op on the stock value.
        var product = new Product { Quantity = 10, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        var request = new ProductRequest { RequestType = RequestType.Remove, RequestedQuantity = 0 };

        ProductRequestWorkflow.ApplyQuantityChange(request, product);

        Assert.Equal(10, product.Quantity);
    }

    [Fact]
    public void ApplyQuantityChange_WithoutValidation_CanProduceNegativeQuantity()
    {
        // ApplyQuantityChange has no guard of its own — calling it without a
        // prior ValidateApproval can drive Quantity below zero.
        // This test documents the contract boundary: callers are responsible
        // for validation before applying changes.
        var product = new Product { Quantity = 3, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        var request = new ProductRequest { RequestType = RequestType.Remove, RequestedQuantity = 10 };

        ProductRequestWorkflow.ApplyQuantityChange(request, product);

        Assert.Equal(-7, product.Quantity);
    }

    [Fact]
    public void ApplyQuantityChange_AddRequest_LargeQuantity_NoOverflow()
    {
        // Confirm that adding a very large amount does not silently overflow.
        // If Quantity is int, int.MaxValue - 1 + 1 = int.MaxValue (safe).
        var product = new Product { Quantity = int.MaxValue - 1, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        var request = new ProductRequest { RequestType = RequestType.Add, RequestedQuantity = 1 };

        ProductRequestWorkflow.ApplyQuantityChange(request, product);

        Assert.Equal(int.MaxValue, product.Quantity);
    }
}
