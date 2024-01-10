using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

[ApiController]
[Route("[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly CalcContext _context;

    public CalculatorController(CalcContext context)
    {
        _context = context;
    }

    [HttpGet("json")]
    public ActionResult<IEnumerable<double>> GetFromJson()
    {
        string jsonFilePath = "JSONData.json";
        string jsonText = System.IO.File.ReadAllText(jsonFilePath);
        var jsonArray = JsonConvert.DeserializeObject<double[]>(jsonText);
        return Ok(jsonArray);
    }

    [HttpGet("xml")]
    public ActionResult<IEnumerable<double>> GetFromXml()
    {
        string xmlFilePath = "XMLData.xml";
        XDocument docs = XDocument.Load(xmlFilePath);
        double[] xmlArray = docs.Root?
                .Element("items")?
                .Elements("item")
                .Select(e => double.Parse(e.Value))
                .ToArray();

        return Ok(xmlArray);
    }

    [HttpGet("sqlite")]
    public ActionResult<IEnumerable<double>> GetFromSQLite()
    {
        string connectionString = "Data Source=путь_к_вашей_базе_данных.sqlite";
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM Nums";
            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    var nums = new List<double>();

                    while (reader.Read())
                    {
                        double num = reader.GetDouble(0);
                        nums.Add(num);
                    }
                    return Ok(nums);
                }
            }
        }
    }

    [HttpPost("calculate")]
    public ActionResult<double> PerformCalculation([FromBody] CalculationInputModel inputModel)
    {
        double result = 0;

        switch (inputModel.Operation)
        {
            case "+":
                result = GetResult(inputModel.Operand, (a, b) => a + b);
                break;
            case "-":
                result = GetResult(inputModel.Operand, (a, b) => a - b);
                break;
            case "*":
                result = GetResult(inputModel.Operand, (a, b) => a * b);
                break;
            case "/":
                if (inputModel.Operand != 0)
                {
                    result = GetResult(inputModel.Operand, (a, b) => a / b);
                }
                else
                {
                    return BadRequest("Ошибка: деление на ноль!");
                }
                break;
            default:
                return BadRequest("Ошибка: введена неверная операция!");
        }

        return Ok(result);
    }

    private double GetResult(double operand, Func<double, double, double> operation)
    {
        using var db = new CalcContext();

        var nums = db.Calculators.OrderByDescending(c => c.Id).FirstOrDefault()?.nums ?? new double[10];

        var resultsArray = new List<double>(nums);
        resultsArray.Insert(0, operand);

        double result = operation(resultsArray[0], resultsArray[1]);

        resultsArray.Insert(1, result);

        db.Add(new Calculators { nums = resultsArray.Take(10).ToArray() });
        db.SaveChanges();

        return result;
    }

}

