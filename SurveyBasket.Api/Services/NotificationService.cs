
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Api.Helpers;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace SurveyBasket.Api.Services;

public class NotificationService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor) : INotificationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task SendNewPollsNotification(int? pollId = null)
    {
        IEnumerable<Poll> polls = [];

        if (pollId.HasValue)
        {
            var poll = await _context.Polls
                            .SingleOrDefaultAsync(p => p.Id == pollId && p.IsPublished);
            polls = [poll!];
        }
        else
        {
            polls = await _context.Polls
                            .Where(p => p.IsPublished && p.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                            .AsNoTracking()
                            .ToListAsync();
        }

        var users = await _userManager.Users.ToListAsync();
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        foreach (var poll in polls)
        {
            foreach (var user in users)
            {
                var placeholders = new Dictionary<string, string>
                {
                    {"{{name}}",user.FirstName },
                    {"{{pollTitle}}",poll.Title },
                    {"{{endDate}}",poll.EndsAt.ToString() },
                    {"{{url}}", $"{origin}/polls/start/{poll.Id}" }
                };

                var emailBody = EmailBodyBuilder.GenerateEmailBody("NewPollNotification", placeholders);

                await _emailSender.SendEmailAsync(user.Email!, $"New Poll Available {poll.Title}", emailBody);
            }
        }
    }
}
