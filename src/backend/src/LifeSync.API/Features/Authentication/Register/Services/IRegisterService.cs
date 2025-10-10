using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Authentication.Register.Services;

public interface IRegisterService
{
    Task<MessageResult> RegisterAsync(RegisterRequest request);
}
