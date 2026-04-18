using AcmePortal.Common;
using AcmePortal.Data;
using AcmePortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AcmePortal.Pages.Customers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context) => _context = context;

        public Customer Customer { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            Customer = customer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Customer",
                EntityId = customer.Id.ToString(),
                Action = AuditType.Deleted,
                ChangedBy = User.Identity!.Name!,
                ChangedAt = DateTime.UtcNow,
                Remarks = $"Deleted: {customer.Name}"
            });

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
