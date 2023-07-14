using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CodeFlowsWithOpenIdConnect.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace CodeFlowsWithOpenIdConnect.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
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

    [Authorize]
    [HttpGet("/view/authentication/data")]
    public IActionResult ViewAuthenticationData()
    {
        return View();
    }

    [Authorize]
    [HttpGet("/call/the/api")]
    public async Task<IActionResult> CallTheApi()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        string apiEndpoint = _configuration["OAuth:Api_Endpoint"];

        var HttpClient = new HttpClient();

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var Response = await HttpClient.GetAsync(apiEndpoint);
        (string Status, string Content) Model;
        Model.Status = $"{(int)Response.StatusCode} {Response.Headers}";

        if (Response.IsSuccessStatusCode)
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(await Response.Content.ReadAsStringAsync());

            Model.Content = JsonSerializer.Serialize(jsonElement,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
        }
        else
            Model.Content = await Response.Content.ReadAsStringAsync();

        return View(Model);
    }
}