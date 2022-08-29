using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using Repository;
using System.Security.Claims;
using eStoreLibrary;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using eStoreAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberRepository memberRepository;

        public MembersController(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(MemberWithRole), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [AllowAnonymous]
        public async Task<IActionResult> Login(MemberLoginModel loginInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(loginInfo.Email) ||
                    string.IsNullOrEmpty(loginInfo.Password))
                {
                    throw new ApplicationException("Login Information is invalid!! Please check again...");
                }

                Member loginMember = await memberRepository.LoginAsync(loginInfo.Email, loginInfo.Password);
                if (loginMember == null)
                {
                    throw new ApplicationException("Failed to login! Please check the information again...");
                }
                MemberRole memberRole = MemberRole.USER;
                Member defaultMember = memberRepository.GetDefaultMember();
                if (loginMember.Email.Equals(defaultMember.Email))
                {
                    memberRole = MemberRole.ADMIN;
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, loginMember.Email),
                    new Claim(ClaimTypes.NameIdentifier, loginMember.MemberId.ToString()),
                    new Claim(ClaimTypes.Role, memberRole.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = false,
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                //loginMember.MemberId = 0;
                loginMember.Password = "";
                loginMember.Orders = null;

                MemberWithRole returnMember = new MemberWithRole(loginMember);
                returnMember.MemberRoleString = memberRole.ToString();

                return StatusCode(200, returnMember);
            } catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Members/logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return StatusCode(204);
        }

        // GET: api/Members
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Member>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetMembers()
        {
            try
            {
                return StatusCode(200, await memberRepository.GetMembersAsync());
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Member), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> GetMember(int id)
        {
            try
            {
                MemberRole role = HttpContext.User.GetMemberRole();
                if (role == MemberRole.USER)
                {
                    if (id != HttpContext.User.GetMemberId())
                    {
                        return Unauthorized();
                    }
                }
                Member member = await memberRepository.GetMemberAsync(id);
                if (member == null)
                {
                    return StatusCode(404, "Member ID is not existed!!");
                }
                return StatusCode(200, member);
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.MemberId)
            {
                return StatusCode(400, "ID is not the same!!");
            }
            
            try
            {
                MemberRole role = HttpContext.User.GetMemberRole();
                if (role == MemberRole.USER)
                {
                    if (id != HttpContext.User.GetMemberId())
                    {
                        return Unauthorized();
                    }
                }
                await memberRepository.UpdateMemberAsync(member);
                return StatusCode(204, "Update successfully!");
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(typeof(Member), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> PostMember(Member member)
        {
            try
            {
                Member createdMember = await memberRepository.AddMemberAsync(member);
                return StatusCode(201, createdMember);
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            try
            {
                await memberRepository.DeleteMemberAsync(id);
                return StatusCode(204, "Delete successfully!");
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
