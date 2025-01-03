﻿
using FuncionalApiDesenvolvedorIO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FuncionalApiDesenvolvedorIO.Controllers;

[ApiController]
[Route("api/v1/conta")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtSettings _jwtSettings;
    public AuthController(SignInManager<IdentityUser> signInManager,
                            UserManager<IdentityUser> userManager,
                            IOptions<JwtSettings> jwtSettings
                         )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        Console.WriteLine(_jwtSettings.Secret);
        Console.WriteLine(_jwtSettings);
    }

    [HttpPost]
    [Route("registrar")]
    public async Task<ActionResult> Register(RegisterUserViewModel registerUser)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage));
        }
        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return Ok(GenerateJwt());
        }
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(errors);
        }
        return Problem("Erro ao registrar o usuário");
    }

    [HttpPost]
    [Route("login")]

    public async Task<ActionResult> Login(LoginUserViewModel login)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage));
        }
        var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, true);
        if (result.Succeeded)
        {
            return Ok(GenerateJwt());

        }
        return Problem("Usuário ou senha inválidos");
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public string GenerateJwt()
    {

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Emissor,
            Audience = _jwtSettings.Audiencia,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.Expiration),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });
        return tokenHandler.WriteToken(token);

    }
}