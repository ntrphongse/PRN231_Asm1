using eStoreLibrary;
using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class MemberDAO
    {
        private static MemberDAO instance = null;
        private static readonly object instanceLock = new object();
        private MemberDAO()
        {

        }

        public static MemberDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new MemberDAO();
                    }
                    return instance;
                }
            }
        }

        public Member GetDefaultMember()
        {
            return JsonConvert.DeserializeObject<Member>(eStoreApiConfiguration.DefaultAccount);
        }

        public async Task<Member> LoginAsync(string email, string password)
        {
            var database = new FStoreContext();
            IEnumerable<Member> members = await database.Members.ToListAsync();
            members = members.Prepend(GetDefaultMember());
            return members.SingleOrDefault(member => member.Email.ToLower().Equals(email.ToLower())
                                    && member.Password.Equals(password));
        }

        public async Task<IEnumerable<Member>> GetMembersAsync()
        {
            var database = new FStoreContext();
            return await database.Members.ToListAsync();
        }

        private async Task<int> GetNextMemberIdAsync()
        {
            var database = new FStoreContext();
            return await database.Members.MaxAsync(mem => mem.MemberId) + 1;
        }

        public async Task<Member> GetMemberAsync(int memberId)
        {
            var database = new FStoreContext();
            return await database.Members.SingleOrDefaultAsync(member => member.MemberId == memberId);
        }

        public async Task<Member> GetMemberAsync(string memberEmail)
        {
            var database = new FStoreContext();
            return await database.Members.SingleOrDefaultAsync(member => member.Email.ToLower().Equals(memberEmail.ToLower()));
        }

        public async Task<Member> AddMemberAsync(Member newMember)
        {
            CheckMember(newMember);
            if (await GetMemberAsync(newMember.Email) != null)
            {
                throw new ApplicationException($"Member with email {newMember.Email} is existed!!");
            }
            var database = new FStoreContext();
            newMember.MemberId = await GetNextMemberIdAsync();
            await database.Members.AddAsync(newMember);
            await database.SaveChangesAsync();
            return newMember;
        }

        public async Task<Member> UpdateMemberAsync(Member updatedMember)
        {
            Member member = await GetMemberAsync(updatedMember.MemberId);
            if (member == null)
            {
                throw new ApplicationException($"Member with the ID {updatedMember.MemberId} does not exist! Please check with the developer for more information");
            }
            if (!updatedMember.Email.Equals(member.Email))
            {
                throw new ApplicationException($"Email is not applicable to be updated!! Please try again...");
            }
            CheckMember(updatedMember);
            var database = new FStoreContext();
            database.Members.Update(updatedMember);
            await database.SaveChangesAsync();
            return updatedMember;
        }

        public async Task DeleteMemberAsync(int memberId)
        {
            Member deletedMember = await GetMemberAsync(memberId);
            if (deletedMember == null)
            {
                throw new Exception($"Member with the ID {memberId} does not exist! Please check again...");
            }
            var database = new FStoreContext();
            database.Members.Remove(deletedMember);
            await database.SaveChangesAsync();
        }

        private void CheckMember(Member member)
        {
            member.Email.IsEmail("Email is not a valid email address!!");
            member.CompanyName.StringValidate(allowEmpty: false,
                emptyErrorMessage: "Company Name cannot be empty!!",
                minLength: 2, minLengthErrorMessage: "Company Name needs to be at least 2 characters!!",
                maxLength: 40, maxLengthErrorMessage: "Company Name is limited to 40 characters!!");
            member.City.StringValidate(allowEmpty: false,
                emptyErrorMessage: "City cannot be empty!!",
                minLength: 2, minLengthErrorMessage: "City needs to be at least 1 characters!!",
                maxLength: 15, maxLengthErrorMessage: "City is limited to 15 characters!!");
            member.Country.StringValidate(allowEmpty: false,
                emptyErrorMessage: "City cannot be empty!!",
                minLength: 2, minLengthErrorMessage: "City needs to be at least 1 characters!!",
                maxLength: 15, maxLengthErrorMessage: "City is limited to 15 characters!!");
            member.Password.StringValidate(allowEmpty: false,
                emptyErrorMessage: "Password cannot be empty!!",
                minLength: 6, minLengthErrorMessage: "Password needs to be at least 6 characters!!",
                maxLength: 30, maxLengthErrorMessage: "Password is limited to 30 characters!!");
        }
    }
}
