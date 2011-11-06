﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Web.Helpers;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Models;
using VocaDb.Web.Models.User;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers
{
    public class UserController : ControllerBase
    {

    	private int LoggedUserId {
    		get {
    			
				if (!LoginManager.IsLoggedIn)
					throw new InvalidOperationException("Not logged in");

    			return LoginManager.LoggedUser.Id;

    		}
    	}

		private UserService Service {
			get { return MvcApplication.Services.Users; }
		}

		private UserDetailsContract GetUserDetails() {

			return Service.GetUserDetails(LoginManager.LoggedUser.Id);

		}

		public JsonResult FindByName(string term) {

			var users = Service.FindUsersByName(term).Select(u => u.Name).ToArray();

			return Json(users, "text/json", JsonRequestBehavior.AllowGet);

		}

		public ActionResult ForgotPassword() {

			return View();

		}

		[HttpPost]
		public ActionResult ForgotPassword(ForgotPassword model) {

			if (!ReCaptcha.Validate(ConfigurationManager.AppSettings["ReCAPTCHAKey"]))
				ModelState.AddModelError("CAPTCHA", "CAPTCHA is invalid.");

			if (!ModelState.IsValid) {
				SaveErrorsToTempData();
				return RedirectToAction("Login");
			}

			try {
				Service.RequestPasswordReset(model.Username, model.Email, ConfigurationManager.AppSettings["HostAddress"] + Url.Action("ResetPassword", "User"));
				TempData.SetStatusMessage("Password reset message has been sent");
				return RedirectToAction("Login");
			} catch (UserNotFoundException) {
				ModelState.AddModelError("Username", "Username or email doesn't match");
				SaveErrorsToTempData();
				return RedirectToAction("Login");
			}

		}

			//
        // GET: /User/

        public ActionResult Index() {
        	ViewBag.Users = Service.GetUsers();
            return View();
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id)
        {
			var model = Service.GetUserDetails(id);
			return View(model);
		}

		public ActionResult Profile(string id) {

			var model = Service.GetUserByNameNonSensitive(id);
			return View("Details", model);

		}

       public ActionResult Login()
        {

			RestoreErrorsFromTempData();

            return View();
        } 

        //
        // POST: /Session/Create

        [HttpPost]
		public ActionResult Login(LoginModel model, string returnUrl)
        {

			if (ModelState.IsValid) {
				// Attempt to register the user

				var user = Service.CheckAuthentication(model.UserName, model.Password, CfHelper.GetRealIp(Request));

				if (user == null) {
					ModelState.AddModelError("", "Username or password doesn't match");
				} else {

					FormsAuthentication.SetAuthCookie(model.UserName, false);

					return RedirectToAction("Index", "Home");

				}

			}

        	return View(model);

		}

		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public ActionResult ComposeMessage(ComposeMessage model) {

			if (!ModelState.IsValid) {
				SaveErrorsToTempData();
				return RedirectToAction("Messages");
			}

			var contract = model.ToContract(LoggedUserId);

			try {
				Service.SendMessage(contract, ConfigurationManager.AppSettings["HostAddress"] + Url.Action("Messages", "User"));
			} catch (UserNotFoundException x) {
				ModelState.AddModelError("ReceiverName", x.Message);
				SaveErrorsToTempData();
				return RedirectToAction("Messages");
			}

			TempData.SetStatusMessage("Message has been sent");

			return RedirectToAction("Messages");

		}

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View(new RegisterModel());
        } 

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(RegisterModel model)
        {

			if (!ReCaptcha.Validate(ConfigurationManager.AppSettings["ReCAPTCHAKey"]))
				ModelState.AddModelError("CAPTCHA", "CAPTCHA is invalid.");

			if (ModelState.IsValid) {
				// Attempt to register the user

				var user = Service.Create(model.UserName, model.Password, Request.UserHostAddress);

				if (user == null) {
					ModelState.AddModelError("UserName", "Username is already taken.");
				} else {

					FormsAuthentication.SetAuthCookie(model.UserName, false);

					return RedirectToAction("Index", "Home");

				}

			}

			return View(model);
            
        }
        
        //
        // GET: /User/Edit/5
 
        public ActionResult Edit(int id)
        {

			LoginManager.VerifyPermission(PermissionFlags.ManageUsers);

        	var user = Service.GetUser(id);
            return View(new UserEdit(user));

        }

        //
        // POST: /User/Edit/5

        [HttpPost]
		public ActionResult Edit(UserEdit model, IEnumerable<PermissionFlagEntry> permissions) {

			model.Permissions = permissions.ToArray();
			Service.UpdateUser(model.ToContract());

        	return RedirectToAction("Details", new {id = model.Id});

        }

		public PartialViewResult Message(int messageId) {

			return PartialView("Message", Service.GetMessageDetails(messageId));

		}

		public ActionResult Messages() {

			var user = Service.GetUserWithMessages(LoggedUserId);
			RestoreErrorsFromTempData();

			return View(user);

		}

		public ActionResult MySettings() {

			LoginManager.VerifyPermission(PermissionFlags.EditProfile);			

			var user = GetUserDetails();

			return View(new MySettingsModel(user));

		}

		[HttpPost]
		public ActionResult MySettings(MySettingsModel model) {

			var user = LoginManager.LoggedUser;

			if (user.Id != model.Id)
				return new HttpStatusCodeResult(403);

			if (!ModelState.IsValid)
				return View(new MySettingsModel(GetUserDetails()));

			if (!string.IsNullOrEmpty(model.Email)) {
				try {
					new MailAddress(model.Email);
				} catch (FormatException) {
					ModelState.AddModelError("Email", "Invalid email address");
					return View(new MySettingsModel(GetUserDetails()));
				}
			}

			try {
				var newUser = Service.UpdateUserSettings(model.ToContract());				
				LoginManager.SetLoggedUser(newUser);
			} catch (InvalidPasswordException x) {
				ModelState.AddModelError("OldPass", x.Message);				
			}

			return View(new MySettingsModel(GetUserDetails()));

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void RemoveAlbumFromUser(int albumId) {
			
			Service.RemoveAlbumFromUser(LoggedUserId, albumId);

		}

		public ActionResult ResetPassword(Guid id) {

			var model = new ResetPassword();

			if (!Service.CheckPasswordResetRequest(id)) {
				ModelState.AddModelError("", "Request ID is invalid. It might have been used already.");
			} else {
				model.RequestId = id;
			}

			return View(model);

		}

		[HttpPost]
		public ActionResult ResetPassword(ResetPassword model) {

			if (!ModelState.IsValid) {
				return ResetPassword(model.RequestId);
			}

			var user = Service.ResetPassword(model.RequestId, model.NewPass);
			FormsAuthentication.SetAuthCookie(user.Name, false);

			return RedirectToAction("Index", "Home");

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void UpdateAlbumForUserMediaType(int albumForUserId, MediaType mediaType) {
			
			Service.UpdateAlbumForUserMediaType(albumForUserId, mediaType);

		}

    	[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddNewAlbum(string newAlbumName) {

			var link = Service.AddAlbum(LoggedUserId, newAlbumName);
			return PartialView("AlbumForUserSettingsRow", new AlbumForUserEditModel(link));

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public PartialViewResult AddExistingAlbum(int albumId) {

			var link = Service.AddAlbum(LoggedUserId, albumId);
			return PartialView("AlbumForUserSettingsRow", new AlbumForUserEditModel(link));

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void DeleteAlbumForUser(int albumForUserId) {

			Service.DeleteAlbumForUser(albumForUserId);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void AddSongToFavorites(int songId) {

			Service.AddSongToFavorites(LoggedUserId, songId);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void RemoveSongFromFavorites(int songId) {

			Service.RemoveSongFromFavorites(LoggedUserId, songId);

		}

        //
        // GET: /User/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /User/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

		public ActionResult Disable(int id) {

			Service.DisableUser(id);

			return RedirectToAction("Details", new { id });

		}

    }
}
