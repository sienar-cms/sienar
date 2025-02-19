﻿using System;
using MudBlazor;
using Sienar.Data;

namespace Sienar.Infrastructure;

public class NotificationService : INotificationService
{
	private readonly ISnackbar _snackbar;

	public NotificationService(ISnackbar snackbar)
	{
		_snackbar = snackbar;
	}

	/// <inheritdoc />
	public void Success(string message)
	{
		_snackbar.Add(message, Severity.Success, ConfigureSnackbar);
	}

	/// <inheritdoc />
	public void Warning(string message)
	{
		_snackbar.Add(message, Severity.Warning, ConfigureDangerSnackbar);
	}

	/// <inheritdoc />
	public void Info(string message)
	{
		_snackbar.Add(message, Severity.Info, ConfigureSnackbar);
	}

	/// <inheritdoc />
	public void Error(string message)
	{
		_snackbar.Add(message, Severity.Error, ConfigureDangerSnackbar);
	}

	/// <inheritdoc />
	public void Notify(Notification notification)
	{
		var isDanger = notification.Type is
			NotificationType.Error or NotificationType.Warning;

		_snackbar.Add(
			notification.Message,
			MapNotificationType(notification.Type),
			isDanger ? ConfigureDangerSnackbar : ConfigureSnackbar);
	}

	private void ConfigureSnackbar(SnackbarOptions o)
	{
		o.ShowTransitionDuration = 500;
		o.HideTransitionDuration = 500;
		o.VisibleStateDuration = 5000;
	}

	private void ConfigureDangerSnackbar(SnackbarOptions o)
	{
		o.ShowTransitionDuration = 500;
		o.HideTransitionDuration = 500;
		o.RequireInteraction = true;
	}

	private static Severity MapNotificationType(NotificationType type)
		=> type switch
		{
			NotificationType.Success => Severity.Success,
			NotificationType.Warning => Severity.Warning,
			NotificationType.Info => Severity.Info,
			NotificationType.Error => Severity.Error,
			_ => throw new InvalidOperationException($"Invalid notification type {type}")
		};
}