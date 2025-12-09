using Backend.Models.Entities;
using Backend.Repository;
using OfficeOpenXml;
using System.Text;

namespace Backend.Services;

public class TelemetryService : ITelemetryService
{
    private readonly ITelemetryRepository repository;
    private readonly string outputDir =  @"D:\Files";

    public TelemetryService(ITelemetryRepository repository)
    {
        this.repository = repository;
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);
            
    }

    public async Task GenerateAndStoreAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var entity = new TelemetryEntity
        {
            Timestamp = now,
            Voltage = Random.Shared.NextDouble() * (12.6 - 3.2) + 3.2,
            Temp = Random.Shared.NextDouble() * (80 - (-50)) - 50,
            SourceFile = $"telemetry_{now:yyyyMMdd_HHmmss}.csv",
            IsOk = Random.Shared.Next(0, 2) == 1
        };

        await repository.AddAsync(entity);

        await SaveCsvAsync(entity);
        await SaveExcelAsync(new[] { entity });
    }

    private async Task SaveCsvAsync(TelemetryEntity e)
    {
        var path = Path.Combine(outputDir, e.SourceFile);
        var sb = new StringBuilder();
        sb.AppendLine("Timestamp,Voltage,Temp,SourceFile,IsOk");
        sb.AppendLine($"{e.Timestamp:yyyy-MM-dd HH:mm:ss},{e.Voltage:F2},{e.Temp:F2},{e.SourceFile},{(e.IsOk ? "ИСТИНА" : "ЛОЖЬ")}");
        await File.WriteAllTextAsync(path, sb.ToString(), Encoding.UTF8);
    }

    private async Task SaveExcelAsync(IEnumerable<TelemetryEntity> data)
    {
        var fileName = $"telemetry_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        var path = Path.Combine(outputDir, fileName);

        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Telemetry");


        ws.Cells[1, 1].Value = "Timestamp";
        ws.Cells[1, 2].Value = "Voltage";
        ws.Cells[1, 3].Value = "Temp";
        ws.Cells[1, 4].Value = "SourceFile";
        ws.Cells[1, 5].Value = "IsOk";

        int row = 2;
        foreach (var e in data)
        {
            ws.Cells[row, 1].Value = e.Timestamp;
            ws.Cells[row, 1].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss"; // Формат даты

            ws.Cells[row, 2].Value = e.Voltage;
            ws.Cells[row, 3].Value = e.Temp;
            ws.Cells[row, 4].Value = e.SourceFile;
            ws.Cells[row, 5].Value = e.IsOk ? "ИСТИНА" : "ЛОЖЬ";

            row++;
        }

        ws.Cells[ws.Dimension.Address].AutoFitColumns();

        await File.WriteAllBytesAsync(path, package.GetAsByteArray());
    }

}
