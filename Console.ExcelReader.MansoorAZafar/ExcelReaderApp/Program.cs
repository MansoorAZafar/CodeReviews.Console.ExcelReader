using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ExcelReaderLib.Models;
using ExcelReaderLib;
using Microsoft.Extensions.DependencyInjection;
using ExcelReaderApp.Models;
using ExcelReaderApp.Controller;
/*
__________________________
|                        |
|     pseudo program     |
|________________________|

BEGIN:
    
    BUILD_CONFIGURATION_FILE
    
    IF DATABASE:
        PRINT("DELETING OLD DATABASE......")
        DELETE(DATABASE)

    PRINT("MAKING DATABASE")
    CREATE(DATABASE)

    TEXT = READEXCEL(SHEET)
    SEED_DATABASE(TEXT)

END
*/

// 1. Setup Configuration and get information
IConfiguration config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", false, false)
                            .Build();

string file = config["File"] ?? "N/A";
string connectionString = config.GetConnectionString("DB_CONN_STR") ?? "N/A";


// 2.  Setup Database info
ServiceCollection services = new ServiceCollection();
services.AddDbContext<EmployeeModelContext>(options =>
    options.UseSqlite(connectionString));
var serviceProvider = services.BuildServiceProvider();


// 3. Delete & Create Database
using (var context = serviceProvider.GetRequiredService<EmployeeModelContext>())
{
    // Delete the Db
    Task task = context.Database.EnsureDeletedAsync();
    Utilities.HandleTask(task, "Deleting Database...");

    // Create the Db
    task = context.Database.EnsureCreatedAsync();
    Utilities.HandleTask(task, "Creating Database...");

    task = context.Database.MigrateAsync();
    Utilities.HandleTask(task, "Migrating Database...");


    // 4. Read Excel Sheet (Need the result so manual dispose + another task)
    Task<List<Employee>> task2 = Task.Run(() => ExcelReader.ExtractDatabaseInformation(file));
    Utilities.HandleTask(task2, "Reading Excel Sheet...", false);
    
    List<Employee> people = task2.Result;
    task2.Dispose();


    // 5. Seed Database
    task = context.Employees.AddRangeAsync(people);
    Utilities.HandleTask(task, "Adding Data to Database...");
    context.SaveChanges();


    //6. Display Tables
    System.Console.WriteLine("Displaying Data:\n");
    task = Task.Run(() => Utilities.DisplayTable(context.Employees.ToList(), ["Id", "Name", "Occupation", "Birthday"]));
    Utilities.HandleTask(task, "");
}
