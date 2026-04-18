using System.ComponentModel.DataAnnotations;

namespace AcmePortal.Model;

public enum RequestType { Add, Remove }

public enum RequestStatus { Draft, Submitted, Approved, Rejected }

public class ProductRequest
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public RequestType RequestType { get; set; }

    [Range(1, int.MaxValue)]
    public int RequestedQuantity { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Draft;

    [MaxLength(1000)]
    public string? ReviewRemark { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Task 5 - Solutioning (Based on problem statement)
    /// </summary>
    public int CustomerId { get; set; }

    public DateTime DateOfPurchased { get; set; }

    [Required, MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal TotalPrice { get; set; }

    [Required, MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string TransactionRefNo { get; set; } = string.Empty;
}
