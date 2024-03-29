﻿using System;
using System.Net.Mail;
using NLog;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Service.Helpers {

	public class UserMessageMailer {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public void Send(string messagesUrl, UserMessage message) {

			ParamIs.NotNull(() => message);

			if (string.IsNullOrEmpty(message.Receiver.Email))
				return;

			MailAddress to;

			try {
				to = new MailAddress(message.Receiver.Email);
			} catch (FormatException x) {
				log.WarnException("Unable to validate receiver email", x);
				return;
			}

			var mailMessage = new MailMessage();
			mailMessage.To.Add(to);
			mailMessage.Subject = "New private message from " + message.Sender.Name;
			mailMessage.Body =
				"Hi " + message.Receiver.Name + ",\n\n" +
				"You have received a message from " + message.Sender.Name + ".\n" +
				"You can view your messages at " + messagesUrl + " and decline from receiving any future messages by changing your settings.\n\n" +
				"- VocaDB mailer";

			var client = new SmtpClient();

			try {
				client.Send(mailMessage);
			} catch (SmtpException x) {
				log.ErrorException("Unable to send mail", x);
			}

		}

	}

}
