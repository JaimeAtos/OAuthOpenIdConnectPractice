using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CodeFlowWithOpenIdConnect.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace CodeFlowWithOpenIdConnect.Controllers;

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
        var AccessToken =
            await HttpContext.GetTokenAsync("access_token");
        string Api_Endpoint =
            _configuration["OAuth:Api_Endpoint"];
        var HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        var Response = await HttpClient.GetAsync(Api_Endpoint);
        (string Status, string Content) Model;
        Model.Status =
            $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
        if (Response.IsSuccessStatusCode)
        {
            var JsonElement =
                JsonSerializer.Deserialize<JsonElement>(
                    await Response.Content.ReadAsStringAsync());
            Model.Content =
                JsonSerializer.Serialize(JsonElement,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web
                            .JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });
        }
        else
        {
            Model.Content =
                await Response.Content.ReadAsStringAsync();
        }
        return View(Model);
    }


    
}