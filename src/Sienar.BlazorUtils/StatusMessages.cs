using System;
using Sienar.Infrastructure.Entities;

namespace Sienar;

public static partial class StatusMessages
{
	public const string Unknown = "An unknown error has occurred. If you continue to have problems, please notify the webmaster.";

	public static class Crud<TEntity>
	{
		public static string CreateFailed() => $"Unable to create new {EntityExtensions.GetEntityName<TEntity>()}";
		public static string CreateSuccessful() => $"{EntityExtensions.GetEntityName<TEntity>()} created successfully";
		public static string ReadSingleFailed() => $"Unable to read {EntityExtensions.GetEntityName<TEntity>()}";
		public static string ReadMultipleFailed() => $"Unable to read {EntityExtensions.GetEntityPluralName<TEntity>()}";
		public static string UpdateFailed() => $"Unable to update {EntityExtensions.GetEntityName<TEntity>()}";
		public static string UpdateSuccessful() => $"{EntityExtensions.GetEntityName<TEntity>()} updated successfully";
		public static string DeleteFailed() => $"Unable to delete {EntityExtensions.GetEntityName<TEntity>()}";
		public static string DeleteSuccessful() => $"{EntityExtensions.GetEntityName<TEntity>()} deleted successfully";
		public static string NotFound(Guid id) => $"{EntityExtensions.GetEntityName<TEntity>()} with ID {id} not found";
	}

	public static class Database
	{
		public const string QueryFailed = "Failed to query database";
	}
}