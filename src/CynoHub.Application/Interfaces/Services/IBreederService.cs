namespace CynoHub.Application.Interfaces.Services;

public interface IBreederService
{
    Guid? CurrentBreederId { get; }
    bool IsAuthenticated { get; }
}
