using AcmePortal.Common;
using AcmePortal.Data;
using AcmePortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AcmePortal.Pages.Customers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context) => _context = context;

        [BindProperty]
        public Customer Customer { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            Customer = customer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existing = await _context.Customers.FindAsync(Customer.Id);
            if (existing == null) return NotFound();

            existing.Name = Customer.Name;
            existing.Email = Customer.Email;
            existing.Phone = Customer.Phone;
            existing.IsActive = Customer.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Customer",
                EntityId = existing.Id.ToString(),
                Action = AuditType.Updated,
                ChangedBy = User.Identity!.Name!,
                ChangedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
