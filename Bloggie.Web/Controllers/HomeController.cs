using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bloggie.Web.Models;

namespace Bloggie.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // This action is used to handle 404 errors
    [Route("Home/NotFound")]
    public IActionResult NotFound()
    {
        Response.StatusCode = 404; // Set the status code to 404
        return View(); // Return the NotFound view
    }

    //Call Azure Function to get message
    [HttpGet]
    [Route("Home/GetMessage")]
    public async Task<IActionResult> GetMessage(string name)
    {
        using (var httpClient = new HttpClient())
        {
            // Replace with your Azure Function URL
            string functionUrl = "http://samervinfunctionapptesting-f2bmbxbqewhdh2cd.southcentralus-01.azurewebsites.net/api/SayHi?name=" + name;
            try
            {
                var response = await httpClient.GetStringAsync(functionUrl);
                return Ok(response); // Return the message as a response
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Azure Function");
                return StatusCode(500, "Error calling Azure Function");
            }
        }
    }
}
