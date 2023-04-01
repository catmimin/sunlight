﻿using Microsoft.AspNetCore.Mvc;
using SunLight.Authorization;
using SunLight.Dtos.Request.Login;
using SunLight.Dtos.Response;
using SunLight.Dtos.Response.Login;
using SunLight.Services;

namespace SunLight.Controllers;

[ApiController]
[XMessageCodeCheck]
[Route("main.php/login")]
public class LoginController : LlsifController
{
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("authKey")]
    [XMessageCodeCheck(performCheck: false)]
    [Produces(typeof(ServerResponse<AuthKeyResponse>))]
    public async Task<IActionResult> AuthKeyAsync([FromBody] AuthKeyRequest requestData)
    {
        var userSession = await _loginService.StartSessionAsync(requestData.DummyToken);

        var response = new AuthKeyResponse
        {
            AuthorizeToken = userSession.AuthorizeToken.ToString(),
            DummyToken = userSession.ServerKey
        };

        return SendResponse(response);
    }

    [HttpPost("login")]
    [Produces(typeof(ServerResponse<LoginResponse>))]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest requestData, [FromHeader] string authorize)
    {
        var parsedAuthorizeHeader = AuthorizeHeader.FromString(authorize);

        try
        {
            var authenticatedUser =
                await _loginService.LoginAsync(requestData.LoginKey, requestData.LoginPasswd,
                    parsedAuthorizeHeader.Token);

            var response = new LoginResponse
            {
                AuthorizeToken = authenticatedUser.AuthorizeToken.ToString(),
                UserId = authenticatedUser.UserId,
                IdfaEnabled = false,
                SkipLoginNews = true
            };

            return SendResponse(response);
        }
        catch (Exception)
        {
            return SendResponse(new ErrorResponse(errorCode: 407), jsonStatusCode: 600);
        }
    }

    [HttpPost("startUp")]
    [Produces(typeof(ServerResponse<LoginResponse>))]
    public async Task<IActionResult> StartUpAsync([FromBody] LoginRequest requestData, [FromHeader] string authorize)
    {
        var parsedAuthorizeHeader = AuthorizeHeader.FromString(authorize);

        try
        {
            var authenticatedUser =
                await _loginService.RegisterAsync(requestData.LoginKey, requestData.LoginPasswd,
                    parsedAuthorizeHeader.Token);

            var response = new LoginResponse
            {
                AuthorizeToken = authenticatedUser.AuthorizeToken.ToString(),
                UserId = authenticatedUser.UserId,
                IdfaEnabled = false,
                SkipLoginNews = true
            };

            return SendResponse(response);
        }
        catch (Exception)
        {
            return SendResponse(new ErrorResponse(errorCode: 407), jsonStatusCode: 600);
        }
    }

    [HttpPost("topInfo")]
    [ApiCall("login", "topInfo")]
    [Produces(typeof(ServerResponse<LoginResponse>))]
    public IActionResult TopInfo()
    {
        var response = new LoginResponse
        {
            IdfaEnabled = false,
            SkipLoginNews = true
        };

        return Ok(response);
    }

    [HttpPost("topInfoOnce")]
    [ApiCall("login", "topInfoOnce")]
    [Produces(typeof(ServerResponse<LoginResponse>))]
    public async Task<IActionResult> TopInfoOnceAsync()
    {
        var response = new LoginResponse
        {
            IdfaEnabled = false,
            SkipLoginNews = true
        };

        await Task.Delay(100);

        return Ok(response);
    }
}