using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class MemberWithRole : Member
    {
        public MemberWithRole(Member member)
        {
            Email = member.Email;
            MemberId = member.MemberId;
        }

        public MemberWithRole()
        {
        }

        public string MemberRoleString { get; set; }
    }
}
