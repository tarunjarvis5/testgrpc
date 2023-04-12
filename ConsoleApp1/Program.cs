//using System;
//using System.Net.Http;
//using System.Threading.Tasks;
//using HtmlAgilityPack;

//namespace ConsoleApp1
//{
//    class Program
//    {
//        static async Task Main(string[] args)
//        {
//            var client = new HttpClient();
//            var formContent = new FormUrlEncodedContent(new[]
//            {
//                new KeyValuePair<string, string>("Project", "SKV S")

//            });
//            var response = await client.GetAsync("https://rerait.telangana.gov.in/SearchList/Search", formContent);
//            var responseString = await response.Content.ReadAsStringAsync();

//            var doc = new HtmlDocument();
//            doc.LoadHtml(responseString);

//            var table = doc.DocumentNode.SelectSingleNode("//table[@class='table table-striped grid-table']");
//            if (table == null)
//            {
//                Console.WriteLine("Table not found");
//                return;
//            }

//            var rows = table.SelectNodes(".//tr");
//            if (rows == null)
//            {
//                Console.WriteLine("No rows found");
//                return;
//            }

//            foreach (var row in rows)
//            {
//                var cells = row.SelectNodes(".//td");
//                if (cells == null)
//                {
//                    Console.WriteLine("No cells found in row");
//                    continue;
//                }

//                foreach (var cell in cells)
//                {
//                    Console.Write(cell.InnerText.Trim() + "\t");
//                }
//                Console.WriteLine();
//            }
//        }
//    }
//}




using System.Net.Http;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;



//// Create an HttpClient instance
//using var client = new HttpClient();

//// Set the base URL of the API endpoint
//var baseUrl = "https://rerait.telangana.gov.in/SearchList/Search";

//// Create a dictionary to hold the query parameters
//var queryParams = new Dictionary<string, string>
//{
//    {"CurrentPage" , "2" }
//};

//// Create a query string from the parameters
//var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));

//// Create the final URL with the query string
//var url = $"{baseUrl}?{queryString}";

//// Send the GET request and get the response
//var response = await client.GetAsync(url);

//// Get the response content as a string
//var responseContent = await response.Content.ReadAsStringAsync();



ChromeDriverService service = ChromeDriverService.CreateDefaultService(@"C:\Users\TARUN\Desktop\ConsoleApp1");
var options = new ChromeOptions();
options.AddArgument("--headless");
var driver = new ChromeDriver( service , options);

// Navigate to the first page
driver.Navigate().GoToUrl("https://rerait.telangana.gov.in/SearchList/Search");


var html = driver.PageSource;
var doc = new HtmlDocument();
doc.LoadHtml(html);




var table = doc.DocumentNode.SelectSingleNode("//table[@class='table table-striped grid-table']");





if (table == null)
{
    Console.WriteLine("Table not found");
    return;
}

var rows = table.SelectNodes(".//tr");
if (rows == null)
{
    Console.WriteLine("No rows found");
    return;
}

foreach (var row in rows)
{
    var cells = row.SelectNodes(".//td");
    if (cells == null)
    {
        Console.WriteLine("No cells found in row");
        continue;
    }

    foreach (var cell in cells)
    {
        Console.Write(cell.InnerText.Trim() + "\t");
    }
    Console.WriteLine();
}

var nxtBtn = driver.FindElementByXPath("//button[@id='btnNext']");
nxtBtn.Click();

html = driver.PageSource;
doc = new HtmlDocument();
doc.LoadHtml(html);





table = doc.DocumentNode.SelectSingleNode("//table[@class='table table-striped grid-table']");





if (table == null)
{
    Console.WriteLine("Table not found");
    return;
}

rows = table.SelectNodes(".//tr");
if (rows == null)
{
    Console.WriteLine("No rows found");
    return;
}

foreach (var row in rows)
{
    var cells = row.SelectNodes(".//td");
    if (cells == null)
    {
        Console.WriteLine("No cells found in row");
        continue;
    }

    foreach (var cell in cells)
    {
        Console.Write(cell.InnerText.Trim() + "\t");
    }
    Console.WriteLine();
}




Console.WriteLine();




