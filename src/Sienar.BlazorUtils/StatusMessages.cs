using System;
using Sienar.Infrastructure.Entities;

namespace Sienar;

public static class StatusMessages
{
	public const string Unknown = "An unknown error has occurred. If you continue to have problems, please notify the webmaster.";

	public static class Crud<TEntity> where TEntity : EntityBase
	{
		public static string CreateFailed() => $"Unable to create new {typeof(TEntity).GetEntityName()}";
		public static string CreateSuccessful() => $"{typeof(TEntity).GetEntityName()} created successfully";
		public static string ReadSingleFailed() => $"Unable to read {typeof(TEntity).GetEntityName()}";
		public static string ReadMultipleFailed() => $"Unable to read {typeof(TEntity).GetEntityPluralName()}";
		public static string UpdateFailed() => $"Unable to update {typeof(TEntity).GetEntityName()}";
		public static string UpdateSuccessful() => $"{typeof(TEntity).GetEntityName()} updated successfully";
		public static string DeleteFailed() => $"Unable to delete {typeof(TEntity).GetEntityName()}";
		public static string DeleteSuccessful() => $"{typeof(TEntity).GetEntityName()} deleted successfully";
		public static string NotFound(Guid id) => $"{typeof(TEntity).GetEntityName()} with ID {id} not found";
		public static string NoPermission() => $"You do not have permission to access this {typeof(TEntity).GetEntityName()}";
	}

	public static class Database
	{
		public const string QueryFailed = "Failed to query database";
	}
}