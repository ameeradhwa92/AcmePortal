using AcmePortal.Common;
using AcmePortal.Data;
using AcmePortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace AcmePortal.Pages.ProductRequests;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context) => _context = context;

    [BindProperty]
    public ProductRequest NewRequest { get; set; } = new();

    public SelectList CustomerSelectList { get; set; } = null!;

    public SelectList ProductSelectList { get; set; } = null!;

    private async Task PopulateSelectListsAsync()
    {
        var products = await _context.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
        ProductSelectList = new SelectList(products, "Id", "Name");

        var customers = await _context.Customers.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
        CustomerSelectList = new SelectList(customers, "Id", "Name");
    }

    public async Task OnGetAsync()
    {
        await PopulateSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateSelectListsAsync();
            return Page();
        }

        NewRequest.Status = RequestStatus.Draft;
        NewRequest.CreatedBy = User.Identity!.Name!;
        NewRequest.CreatedAt = DateTime.UtcNow;
        NewRequest.UpdatedAt = DateTime.UtcNow;

        NewRequest.DateOfPurchased = DateTime.UtcNow;
        NewRequest.CustomerName = (await _context.Customers.FindAsync(NewRequest.CustomerId))?.Name ?? "Unknown Customer";

        var product = await _context.Products.FindAsync(NewRequest.ProductId);
        NewRequest.ProductName = product?.Name ?? "Unknown Product";
        NewRequest.TotalPrice = (product?.Price ?? 0) * NewRequest.RequestedQuantity;

        NewRequest.TransactionRefNo = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "/" + NewRequest.Id;

        if (product != null)
        {
            if (NewRequest.RequestType == RequestType.Remove && NewRequest.RequestedQuantity > product.Quantity)
            {
                ModelState.AddModelError(string.Empty, "Requested quantity exceeds available product stock.");
                await PopulateSelectListsAsync();
                return Page();
            }
        }

        _context.ProductRequests.Add(NewRequest);
        await _context.SaveChangesAsync(); // Save first so NewRequest.Id is populated

        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = "ProductRequest",
            EntityId = NewRequest.Id.ToString(),
            Action = AuditType.Created,
            ChangedBy = User.Identity!.Name!,
            ChangedAt = DateTime.UtcNow,
            Remarks = $"Type: {NewRequest.RequestType}, Qty: {NewRequest.RequestedQuantity}"
        });
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
