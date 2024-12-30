using ExcelReaderLib.Models;
using OfficeOpenXml;

namespace ExcelReaderLib;
public static class ExcelReader
{
    static ExcelReader() {
        // Setup the license
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
    }

    public static List<Employee> ExtractDatabaseInformation(string file)
    {
        List<Employee> employees = new();

        using(ExcelPackage package = new ExcelPackage(new FileInfo(file)))
        {
            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                int id = int.Parse(worksheet.Cells[row, 1].Text); // Column 1: Id
                string name = worksheet.Cells[row, 2].Text;       // Column 2: Name
                string occupation = worksheet.Cells[row, 3].Text; // Column 3: Occupation
                DateOnly birthday = DateOnly.Parse(worksheet.Cells[row, 4].Text); // Column 4: Birthday

                employees.Add(new Employee(id, name, occupation, birthday));
            }
        }

        return employees;
    }

    //public static Employee ConvertWorksheetToEmployee(ExcelWorksheet worksheet)
    //    => new Employee(worksheet.Row(0), worksheet.Row(1), worksheet.Row(2), worksheet.Row(3));
}
