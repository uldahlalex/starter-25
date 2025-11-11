using System.ComponentModel.DataAnnotations;
using api.DTOs.Requests;
using dataccess;
using JWT.Algorithms;
using JWT.Builder;
using ValidationException = Bogus.ValidationException;

namespace api.Services;

public class AuthService(MyDbContext ctx, TimeProvider timeProvider) : IAuthService
{

    public async Task<JwtClaims> VerifyAndDecodeToken(string token)
    {
        throw new NotImplementedException();
    }

    public async Task<JwtResponse> Login(LoginRequestDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<JwtResponse> Register(RegisterRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var isEmailTaken = ctx.Libraryusers.Any(u => u.Email == dto.Email);
        if (isEmailTaken)
            throw new ValidationException("Email is already taken");

        var user = new Libraryuser()
        {
            Email = dto.Email,
            Createdat = timeProvider.GetUtcNow().Date
        };
        
        var token = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA512Algorithm())
            .AddClaim(nameof(Libraryuser.Id), user.Id)
            .Encode();
        return new JwtResponse()
        {
            Token = token,
        };
    }
    

    public async Task<JwtClaims> WhoAmI(string authorization)
    {
        if (string.IsNullOrWhiteSpace(authorization))
            throw new ValidationException("No token attached!");

        var json = JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA512Algorithm())
            .MustVerifySignature()
            .Decode<JwtClaims>(authorization)
                   ?? throw new ValidationException("Could not decode to JwtClaims");
        return json;
    }
}