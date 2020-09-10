﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;
using HiSpaceListingService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HiSpaceListingService.Utilities;

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
			var _user = await _context.Users.FirstOrDefaultAsync(d => d.Email == user.Email && d.Password == user.Password && d.Status == true && d.UserType == user.UserType);

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
			try
			{
			return await _context.Users.ToListAsync();
			}
			catch (Exception ex)
			{

				throw ex;
			}

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

		//GET: api/user/ApproveByReMasterId/1/completed
		[HttpGet("ApproveByReMasterId/{REProfessionalMasterId}/{Status}")]
		public ActionResult<bool> ApproveByReMasterId(int REProfessionalMasterId, string Status)
		{
			bool result = true;
			try
			{
				var rEProfessionalMaster = _context.REProfessionalMasters.SingleOrDefault(d => d.REProfessionalMasterId == REProfessionalMasterId);
				if (rEProfessionalMaster != null)
				{
					rEProfessionalMaster.LinkingStatus = Status;
					_context.Entry(rEProfessionalMaster).State = EntityState.Modified;
					_context.SaveChangesAsync();
				}
			}
			catch (DbUpdateConcurrencyException)
			{
				result = false;
			}
			return result;
		}


		[HttpGet]
		[Route("UserEmailExists/{Email}")]
		public bool UserEmailExists(string Email)
		{
			return _context.Users.Any(e => e.Email == Email);
		}

		////Enquiry
		//[HttpPost]
		//[Route("SendEnquiryEmail")]
		//public bool SendEnquiryEmail([FromBody] Enquiry Enquiry)
		//{
		//	var Subject = "New Enquiry";
		//	EmailMessage email = new EmailMessage();
		//	return email.SendEnquiry(Enquiry.To_Email, Subject, Enquiry.Sender_Message, Enquiry.Sender_Phone, Enquiry.Sender_Name, Enquiry.Sender_Email);
		//}

		//Enquiry
		[HttpPost]
		[Route("SendEnquiryEmail")]
		public async Task<bool> SendEnquiryEmail([FromBody] Enquiry Enquiry)
		{
			var Subject = "New Enquiry";
			EmailMessage email = new EmailMessage();

			_context.Enquiries.Add(Enquiry);
			await _context.SaveChangesAsync();

			bool EnquiryEmail = email.SendEnquiry(Enquiry.To_Email, Subject, Enquiry.Sender_Message, Enquiry.Sender_Phone, Enquiry.Sender_Name, Enquiry.Sender_Email);

			if (!EnquiryEmail)
				return false;

			return email.SendEnquirySuccessEmail(Enquiry.Sender_Email, Enquiry.Sender_Name);

		}

		//contact form enquiry
		[HttpGet]
		[Route("SendContactEnquiryEmail/{Name}/{Email}/{Phone}/{Text}")]
		public bool SendContactEnquiryEmail(string Name, string Email, string Phone, string Text)
		{
			var Subject = "New Enquiry";
			EmailMessage email = new EmailMessage();

			return email.SendContactFormEnquiry(Name, Email, Phone, Text, Subject);


		}

		//SendSignupSuccess
		[HttpGet]
		[Route("SendSignupSuccess/{Email}/{UserName}/{Password}")]
		public bool SendSignupSuccess(string Email, string UserName, string Password)
		{
			var Subject = "Welcome To HiSpace";
			EmailMessage email = new EmailMessage();
			return email.SendSignup(Email, Subject, UserName, Password);
		}

		//BackgroundCheckEmail
		[HttpGet]
		[Route("SendBackgroundCheckEmail/{Email}/{UserName}")]
		public bool SendBackgroundCheckEmail(string Email, string UserName)
		{
			var Subject = "HiSapce Verification Confirmation";
			EmailMessage email = new EmailMessage();
			return email.BackgroundCheckEmail(Email, UserName, Subject);
		}


	}
}