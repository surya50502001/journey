using BuilderJourney.Domain.Entities;

namespace BuilderJourney.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user);
}
