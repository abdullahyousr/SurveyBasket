
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Helpers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SurveyBasket.Api.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager, 
    SignInManager<ApplicationUser> signInManager,   
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext context) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ApplicationDbContext _context = context;
    private readonly int _refreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponce>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //Check user
        if(await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponce>(UserErrors.InvalidCredentials);

        if(user.IsDisabled)
            return Result.Failure<AuthResponce>(UserErrors.DisabledUser);
        //Check Password
        //var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
        //if(!isValidPassword)
        //    return Result.Failure<AuthResponce>(UserErrors.InvalidCredentials);
        var result = await _signInManager.PasswordSignInAsync(user, password,false, true);
        if(result.Succeeded)
        {

            var (userRoles , userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            //Generate JWT Token
            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

            //Generate RefreshToken Token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var responce = new AuthResponce(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(responce);
        }

        var error = result.IsNotAllowed 
            ? UserErrors.EmailNotConfirmed 
            : result.IsLockedOut 
            ? UserErrors.LockedUser 
            : UserErrors.InvalidCredentials;
        return Result.Failure<AuthResponce>(error);
    }

    public async Task<Result<AuthResponce>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if(userId is null)
            return Result.Failure<AuthResponce>(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);
        
        if(user is null)
            return Result.Failure<AuthResponce>(UserErrors.InvalidRefrestToken);
        
        if (user.IsDisabled)
            return Result.Failure<AuthResponce>(UserErrors.DisabledUser);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponce>(UserErrors.LockedUser);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
        
        if(userRefreshToken is null)
            return Result.Failure<AuthResponce>(UserErrors.InvalidRefrestToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        //Generate JWT Token
        var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);

        var (newtoken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

        //Generate RefreshToken Token
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);
        var responce = new AuthResponce(user.Id, user.Email, user.FirstName, user.LastName, newtoken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(responce);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidRefrestToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefrestToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
        if(emailExists)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation Code:{code}", code);

            await SendConfirmationEmail(user, code);

            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if(await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if(user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = request.Code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch(FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, DefaultRoles.Member.Name);
            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation Code:{code}", code);

        await SendConfirmationEmail(user, code);

        return Result.Success();
    }
    
    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if(await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed with { StatusCode = StatusCodes.Status400BadRequest});

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset Code:{code}", code);

        await SendResetPasswordEmail(user, code);

        return Result.Success();

    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if(result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));

    }
    
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private async Task SendConfirmationEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            new Dictionary<string, string>
            {
                {"{{name}}", user.FirstName},
                {"{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            });
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket: Email Confirmation", emailBody));
        await Task.CompletedTask;
    }

    private async Task SendResetPasswordEmail(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            new Dictionary<string, string>
            {
                {"{{name}}", user.FirstName},
                {"{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
            });
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket: Change Password", emailBody));
        await Task.CompletedTask;
    }
    private async Task<(IEnumerable<string> roles, IEnumerable<string> permssions)> GetUserRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        //cc await _context.Roles
        //    .Join(_context.RoleClaims,
        //    role => role.Id,
        //    claim => claim.RoleId,
        //    (role, claim) => new { role, claim }
        //    )
        //    .Where(x => userRoles.Contains(x.role.Name!))
        //    .Select(x => x.claim.ClaimValue!)
        //    .Distinct()
        //    .ToListAsync(cancellationToken);
        var userPermissions = await (from role in _context.Roles
                                    join claim in _context.RoleClaims 
                                    on role.Id equals claim.RoleId
                                    where userRoles.Contains(role.Name!)
                                    select claim.ClaimValue!)
                                    .Distinct()
                                    .ToListAsync(cancellationToken);


        return (userRoles, userPermissions);
    }

}
