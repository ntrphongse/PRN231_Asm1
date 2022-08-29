using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IMemberRepository
    {
        Member GetDefaultMember();
        Task<Member> LoginAsync(string email, string password);
        Task<IEnumerable<Member>> GetMembersAsync();
        Task<Member> GetMemberAsync(int memberId);
        Task<Member> GetMemberAsync(string memberEmail);
        Task<Member> AddMemberAsync(Member newMember);
        Task<Member> UpdateMemberAsync(Member updatedMember);
        Task DeleteMemberAsync(int memberId);
    }
}
