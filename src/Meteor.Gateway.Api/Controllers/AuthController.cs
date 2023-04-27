using System.Net.Mime;
using MapsterMapper;
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

        var response = _mapper.Map<Models.Auth.AuthResult>(authResult);
        return Ok(response);
    }
}