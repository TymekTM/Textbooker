using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Booker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Booker.Areas.Admin.Pages
{
    public class ReportsModel : PageModel
    {
        private readonly DataContext _context;
        private readonly ILogger<ReportsModel> _logger;

        public ReportsModel(DataContext context, ILogger<ReportsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<User> Results { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(string search)
        {
            Results = await _context.Users
                .FromSqlInterpolated($"SELECT * FROM Users WHERE Email LIKE { "%" + search + "%" }")
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostExportAsync(string fileName)
        {
            var users = await _context.Users.ToListAsync();
            var csv = string.Join("\n", users.Select(u => $"{u.Email};{u.UserName}"));
            var safeName = Path.GetFileName(fileName);
            var path = Path.Combine("exports", safeName);
            await File.WriteAllTextAsync(path, csv);
            _logger.LogInformation("Exported {Count} users to {File}", users.Count, safeName);
            return new OkResult();
        }
    }
}
