﻿using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using CodeFlows.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeFlows.Controllers;

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

    [HttpGet("/get/the/authorization/code")]
    public IActionResult GetTheCode()
    {
        string Authorization_Endpoint = _configuration["OAuth:Authorization_Endpoint"];
        string Response_Type = "code";
        string Client_Id = _configuration["OAuth:Client_Id"];
        string Redirect_Uri = _configuration["OAuth:Redirect_Uri"];
        string Scope = _configuration["OAuth:Scope"];
        const string State = "String viajero";

        string URI = $"{Authorization_Endpoint}?" +
                     $"response_type={Response_Type}&" +
                     $"client_id={Client_Id}&" +
                     $"redirect_uri={Redirect_Uri}&" +
                     $"scope={Scope}&state={State}";
        return Redirect(URI);
    }

    [HttpGet("/authentication/login-callback")]
    public IActionResult LoginCallback([FromQuery] string code, string state)
    {
        return View((code, state));
    }

    [HttpGet("/exchange/the/authorization/code/for/an/access/token")]
    public async Task<IActionResult> ExchangeTheAuthorizationCodeForAnAccessToken(string code, string state)
    {
        const string Grant_Type = "authorization_code";
        string Token_Endpoint = _configuration["OAuth:Token_Endpoint"];
        string Redirect_Uri = _configuration["OAuth:Redirect_Uri"];
        string Client_Id = _configuration["OAuth:Client_Id"];
        string Client_Secret = _configuration["OAuth:Client_Secret"];
        string Scope = _configuration["OAuth:Scope"];

        Dictionary<string, string> BodyData = new Dictionary<string, string>()
        {
            { "grant_type", Grant_Type },
            { "code", code },
            { "redirect_uri", Redirect_Uri },
            { "client_id", Client_Id },
            { "client_secret", Client_Secret },
            { "scope", Scope }
        };

        HttpClient httpClient = new HttpClient();
        var Body = new FormUrlEncodedContent(BodyData);

        var Response = await httpClient.PostAsync(Token_Endpoint, Body);
        var Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
        var JsonContent = await Response.Content.ReadFromJsonAsync<JsonElement>();
        var PrettyPrintJsonContent =
            JsonSerializer.Serialize(JsonContent, new JsonSerializerOptions { WriteIndented = true });

        return View((Status, PrettyPrintJsonContent, Response.IsSuccessStatusCode));
    }

    [HttpPost("/call/the/api")]
    public async Task<IActionResult> CallTheApi(string token)
    {
        var AccessToken = JsonDocument.Parse(token).RootElement
            .GetProperty("access_token").GetString();

        string Api_Endpoint = _configuration["OAuth:Api_Endpoint"];
        var HttpClient = new HttpClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

        var Response = await HttpClient.GetAsync(Api_Endpoint);

        (string Status, string Content) Model;
        Model.Status = $"{(int)Response.StatusCode} {Response.ReasonPhrase}";
        if (Response.IsSuccessStatusCode)
        {
            var JsonElement = JsonSerializer.Deserialize<JsonElement>(
                await Response.Content.ReadAsStringAsync());

            Model.Content = JsonSerializer.Serialize(JsonElement,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web
                        .JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
        }
        else
            Model.Content = await Response.Content.ReadAsStringAsync();

        return View(Model);
    }
}