#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Sienar.Errors;
using Sienar.Extensions;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Infrastructure.Hooks;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class PersonalDataProcessor : IProcessor<PersonalDataResult>
{
	private readonly IUserManager _userManager;
	private readonly IUserAccessor _userAccessor;
	private readonly IEnumerable<IUserPersonalDataRetriever> _personalDataRetrievers;

	public PersonalDataProcessor(
		IUserManager userManager,
		IUserAccessor userAccessor,
		IEnumerable<IUserPersonalDataRetriever> personalDataRetrievers)
	{
		_userManager = userManager;
		_userAccessor = userAccessor;
		_personalDataRetrievers = personalDataRetrievers;
	}

	public async Task<HookResult<PersonalDataResult>> Process()
	{
		var userId = await _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			return this.Unauthorized(message: CmsErrors.Account.LoginRequired);
		}

		var user = await _userManager.GetSienarUser(userId.Value);
		if (user is null)
		{
			return this.Unauthorized(message: CmsErrors.Account.LoginRequired);
		}

		var file = new DownloadFile
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

		return this.Success(new(file), "Personal data downloaded successfully");
	}
}