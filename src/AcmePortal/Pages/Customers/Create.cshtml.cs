using AcmePortal.Common;
using AcmePortal.Data;
using AcmePortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AcmePortal.Pages.Customers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context) => _context = context;

        [BindProperty]
        public Customer NewCustomer { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            NewCustomer.CreatedAt = DateTime.UtcNow;
            NewCustomer.UpdatedAt = DateTime.UtcNow;

            _context.Customers.Add(NewCustomer);

            _context.AuditLogs.Add(new AuditLog
            {
                EntityName = "Customer",
                EntityId = "0",
                Action = AuditType.Created,
                ChangedBy = User.Identity!.Name!,
                ChangedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
