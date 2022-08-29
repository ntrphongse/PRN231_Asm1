using BusinessObject;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class MemberRepository : IMemberRepository
    {
        public async Task<Member> AddMemberAsync(Member newMember)
                => await MemberDAO.Instance.AddMemberAsync(newMember);

        public async Task DeleteMemberAsync(int memberId)
                => await MemberDAO.Instance.DeleteMemberAsync(memberId);

        public Member GetDefaultMember()
                => MemberDAO.Instance.GetDefaultMember();

        public async Task<Member> GetMemberAsync(int memberId)
                => await MemberDAO.Instance.GetMemberAsync(memberId);

        public async Task<Member> GetMemberAsync(string memberEmail)
                => await MemberDAO.Instance.GetMemberAsync(memberEmail);

        public async Task<IEnumerable<Member>> GetMembersAsync()
                => await MemberDAO.Instance.GetMembersAsync();

        public async Task<Member> LoginAsync(string email, string password)
                => await MemberDAO.Instance.LoginAsync(email, password);

        public async Task<Member> UpdateMemberAsync(Member updatedMember)
                => await MemberDAO.Instance.UpdateMemberAsync(updatedMember);
    }
}
