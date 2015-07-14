
namespace Kikai.Internal.IManagers
{
    public interface IPmp
    {
        bool Authenticate(string user, string sharedSecret);
        bool Authorize(string user, string resource);
    }
}
