using System.Net.Mime;
using MapsterMapper;
using Meteor.Gateway.Api.Constants;
using Meteor.Gateway.Api.Models;
using Meteor.Gateway.Api.Models.Auth;
using Meteor.Gateway.Core.Dtos;
using Meteor.Gateway.Core.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Meteor.Gateway.Api.Controllers;

[ApiController, Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    private readonly IMapper _mapper;

    public AuthController(IAuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    [HttpPost("")]
    [SwaggerOperation("Authorize user by login (email) and password.")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Authorization result (access and refresh tokens)",
        typeof(Models.Auth.AuthResult),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Authorization error.",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Models.Auth.AuthResult>> AuthorizeAsync(
        [SwaggerRequestBody("User credentials")] UserCredentials credentials
    )
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ipAddress))
        {
            return BadRequest(new ErrorResponse("Unable to determine the IP address."));
        }

        var signInDto = _mapper.Map<SignInDto>(credentials);
        signInDto.IpAddress = ipAddress;
        var authResult = await _authService.AuthorizeAsync(signInDto);

        SetRefreshTokenToCookie(authResult.RefreshToken);

        var response = _mapper.Map<Models.Auth.AuthResult>(authResult);
        return Ok(response);
    }

    [HttpPost("refresh")]
    [SwaggerOperation("Refreshes access and refresh tokens.")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "Refreshed tokens.",
        typeof(Models.Auth.AuthResult),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "Refresh token info error.",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Refresh token not found.",
        typeof(ErrorResponse),
        MediaTypeNames.Application.Json
    )]
    public async Task<ActionResult<Models.Auth.AuthResult>> RefreshAsync()
    {
        if (!Request.Cookies.TryGetValue(CookieNames.RefreshToken, out var token))
        {
            return BadRequest(new ErrorResponse("Refresh token was not found in cookies."));
        }

        var authResult = await _authService.RefreshAsync(token);

        SetRefreshTokenToCookie(authResult.RefreshToken);

        var response = _mapper.Map<Models.Auth.AuthResult>(authResult);
        return Ok(response);
    }

    private void SetRefreshTokenToCookie(TokenInfo tokenInfo)
    {
        var options = new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
        };
        Response.Cookies.Append(CookieNames.RefreshToken, tokenInfo.Token, options);
    }
}