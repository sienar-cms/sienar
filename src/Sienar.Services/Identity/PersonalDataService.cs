using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Sienar.Errors;
using Sienar.Infrastructure;

namespace Sienar.Identity;

public class PersonalDataService
	: IPersonalDataService
{
	private readonly IUserManager _userManager;
	private readonly IUserAccessor _userAccessor;
	private readonly IEnumerable<IUserPersonalDataRetriever> _personalDataRetrievers;
	private readonly INotificationService _notifier;

	public PersonalDataService(
		IUserManager userManager,
		IUserAccessor userAccessor,
		IEnumerable<IUserPersonalDataRetriever> personalDataRetrievers,
		INotificationService notifier)
	{
		_userManager = userManager;
		_userAccessor = userAccessor;
		_personalDataRetrievers = personalDataRetrievers;
		_notifier = notifier;
	}

	/// <inheritdoc/>
	public async Task<FileDto?> GetPersonalData()
	{
		var userId = _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			_notifier.Error(ErrorMessages.Account.LoginRequired);
			return null;
		}

		var user = await _userManager.GetSienarUser(userId.Value);
		if (user is null)
		{
			_notifier.Error(ErrorMessages.Account.LoginRequired);
			return null;
		}

		var file = new FileDto
		{
			Name = "PersonalData.json"
		};
		file.Mime = MimeMapping.MimeUtility.GetMimeMapping(file.Name);

		// Only include personal data for download
		var personalData = new Dictionary<string, string>();
		var personalDataProps = user
			.GetType()
		    .GetProperties()
		    .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

		foreach (var p in personalDataProps)
		{
			var value = p.GetValue(user)
				?.ToString();
			personalData.Add(p.Name, value ?? "null");
		}

		foreach (var retriever in _personalDataRetrievers)
		{
			var newData = await retriever.GetUserData(user);
			personalData = personalData.Union(newData)
				.ToDictionary(d => d.Key, d => d.Value);
		}

		file.Contents = JsonSerializer.SerializeToUtf8Bytes(personalData);

		return file;
	}
}