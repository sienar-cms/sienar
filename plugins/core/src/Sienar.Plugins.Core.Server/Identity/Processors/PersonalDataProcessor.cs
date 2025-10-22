#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sienar.Errors;
using Sienar.Identity.Hooks;
using Sienar.Identity.Results;
using Sienar.Infrastructure;
using Sienar.Data;
using Sienar.Identity.Data;
using Sienar.Processors;
using Sienar.Security;

namespace Sienar.Identity.Processors;

/// <exclude />
public class PersonalDataProcessor : IResultProcessor<PersonalDataResult>
{
	private readonly IUserRepository _userRepository;
	private readonly IUserAccessor _userAccessor;
	private readonly IEnumerable<IUserPersonalDataRetriever> _personalDataRetrievers;

	public PersonalDataProcessor(
		IUserRepository userRepository,
		IUserAccessor userAccessor,
		IEnumerable<IUserPersonalDataRetriever> personalDataRetrievers)
	{
		_userRepository = userRepository;
		_userAccessor = userAccessor;
		_personalDataRetrievers = personalDataRetrievers;
	}

	public async Task<OperationResult<PersonalDataResult?>> Process()
	{
		var userId = await _userAccessor.GetUserId();
		if (!userId.HasValue)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CmsErrors.Account.LoginRequired);
		}

		var user = await _userRepository.Read(userId.Value);
		if (user is null)
		{
			return new(
				OperationStatus.Unauthorized,
				message: CmsErrors.Account.LoginRequired);
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

		return new(
			OperationStatus.Success,
			new(file),
			"Personal data downloaded successfully");
	}
}