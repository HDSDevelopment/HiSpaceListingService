using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiSpaceListingService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly HiSpaceListingContext _context;

		public UserController(HiSpaceListingContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Authenticate the user.
		/// </summary>
		/// <returns>The authenticated user.</returns>
		// POST: api/user/AuthenticateUser
		[HttpPost]
		[Route("AuthenticateUser")]
		public async Task<ActionResult<User>> AuthenticateUser([FromBody] User user)
		{
			var _user = await _context.Users.FirstOrDefaultAsync(d => d.Email == user.Email && d.Password == user.Password && d.Status == true);

			if (_user == null)
			{
				return BadRequest(new { message = "Email or password is incorrect" });
			}
			else
			{
				_user.LoginCount = _user.LoginCount + 1;
				_user.LastLoginDateTime = DateTime.Now;
				_context.Entry(_user).State = EntityState.Modified;
				await _context.SaveChangesAsync();
			}
			return Ok(new
			{
				UserId = _user.UserId,
				Email = _user.Email,
				Password = _user.Password,
				UserType = _user.UserType,
				CompanyName = _user.CompanyName,
				UserStatus = _user.UserStatus
			});
		}

		/// <summary>
		/// Gets the list of all Users.
		/// </summary>
		/// <returns>The list of Users.</returns>
		// GET: api/user/Users
		[HttpGet]
		[Route("GetUsers")]
		public async Task<ActionResult<IEnumerable<User>>> GetUsers()
		{
			return await _context.Users.ToListAsync();
		}

		/// <summary>
		/// Gets the User by UserId.
		/// </summary>
		/// <returns>The User by UserId.</returns>
		// GET: api/user/GetUser/1
		//[HttpGet("GetUser/{UserId}")]
		[HttpGet]
		[Route("GetUser/{UserId}")]
		public async Task<ActionResult<User>> GetUser(int UserId)
		{
			var user = await _context.Users.FindAsync(UserId);

			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		/// <summary>
		/// Add the User.
		/// </summary>
		/// <returns>The User by UserId.</returns>
		// POST: api/user/AddUser
		[HttpPost]
		[Route("AddUser")]
		public async Task<ActionResult<User>> AddUser([FromBody] User user)
		{
			user.CreatedDateTime = DateTime.Now;

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetUser", new { UserId = user.UserId }, user);
		}

		/// <summary>
		/// Update the User by UserId.
		/// </summary>
		/// <returns>The User by UserId.</returns>
		// PUT: api/user/UpdateUser
		[HttpPut]
		[Route("UpdateUser/{UserId}")]
		public async Task<IActionResult> UpdateUser(int UserId, [FromBody]  User user)
		{
			if (UserId != user.UserId || user == null)
			{
				return BadRequest();
			}

			using (var trans = _context.Database.BeginTransaction())
			{
				try
				{
					user.ModidyDateTime = DateTime.Now;
					user.ModifyBy = user.UserId;
					_context.Entry(user).State = EntityState.Modified;

					try
				{
						await _context.SaveChangesAsync();
					}
					catch (DbUpdateConcurrencyException)
					{
						if (!UserExists(UserId))
						{
							return NotFound();
						}
						else
						{
							throw;
						}
					}

					trans.Commit();
				}
				catch (Exception)
				{
					trans.Rollback();
				}
			}

			return NoContent();
		}

		private bool UserExists(int UserId)
		{
			return _context.Users.Any(e => e.UserId == UserId);
		}

		/// <summary>
		/// Delete the User by UserId.
		/// </summary>
		/// <returns>The Users list.</returns>
		// POST: api/user/DeleteUser
		[HttpPost]
		[Route("DeleteUser/{UserId}")]
		public async Task<ActionResult<IEnumerable<User>>> DeleteUser(int UserId)
		{
			var user = await _context.Users.FindAsync(UserId);
			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			return CreatedAtAction("GetUsers", user);
		}

		//GET: api/user/ApproveByUserId/1/completed
		[HttpGet("ApproveByUserId/{UserId}/{Status}")]
		public ActionResult<bool> ApproveByUserId(int UserId, string Status)
		{
			bool result = true;
			if (UserId == 0)
			{
				result = false;
			}
			try
			{
				var user = _context.Users.SingleOrDefault(d => d.UserId == UserId);
				if (user != null)
				{
					user.UserStatus = Status;
					_context.Entry(user).State = EntityState.Modified;
					_context.SaveChangesAsync();
				}
			}
			catch (DbUpdateConcurrencyException)
			{
				result = false;
			}
			return result;
		}

	}
}