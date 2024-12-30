using Microsoft.EntityFrameworkCore;
using ExcelReaderLib.Models;

namespace ExcelReaderApp.Models;

internal class EmployeeModelContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }

    public EmployeeModelContext(DbContextOptions<EmployeeModelContext> options): base(options) {}
    
}
