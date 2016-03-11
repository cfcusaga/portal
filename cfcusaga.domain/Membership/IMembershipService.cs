using System.Threading.Tasks;

namespace cfcusaga.domain.Membership
{
    public interface IMembershipService
    {
        Task<Member> GetMemberInfoFromAspNetUserId(string currentUserId);
    }


}