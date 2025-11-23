// ReSharper disable MemberHidesStaticFromOuterClass
namespace Sienar;

public static class DashboardUrls
{
	private const string Prefix = "/dashboard";
	public const string Index = Prefix;
	public const string About = $"{Prefix}/about";

	public static class Account
	{
		private const string Prefix = $"{DashboardUrls.Prefix}/account";
		public const string Login = $"{Prefix}/login";
		public const string ProcessLogin = $"{Prefix}/process-login";
		public const string Logout = $"{Prefix}/logout";
		public const string Forbidden = $"{Prefix}/forbidden";
		public const string PersonalData = $"{Prefix}/personal-data";
		public const string Locked = $"{Prefix}/locked";
		public const string Delete = $"{Prefix}/delete";
		public const string Deleted = $"{Prefix}/deleted";

		public static class Register
		{
			private const string Prefix = $"{Account.Prefix}/register";
			public const string Index = Prefix;
			public const string Successful = $"{Prefix}/successful";
		}

		public static class Confirm
		{
			private const string Prefix = $"{Account.Prefix}/confirm";
			public const string Index = Prefix;
			public const string Successful = $"{Prefix}/successful";
		}

		public static class ForgotPassword
		{
			private const string Prefix = $"{Account.Prefix}/forgot-password";
			public const string Index = Prefix;
			public const string Successful = $"{Prefix}/successful";
		}

		public static class PasswordChange
		{
			private const string Prefix = $"{Account.Prefix}/password";
			public const string Index = Prefix;
			public const string Successful = $"{Prefix}/successful";
		}

		public static class ResetPassword
		{
			private const string Prefix = $"{Account.Prefix}/reset-password";
			public const string Index = Prefix;
			public const string Successful = $"{Prefix}/successful";
		}
		public static class EmailChange
		{
			private const string Prefix = $"{Account.Prefix}/change-email";
			public const string Index = Prefix;
			public const string Requested = $"{Prefix}/requested";
			public const string Confirm = $"{Prefix}/confirm";
			public const string Successful = $"{Prefix}/successful";
		}
	}

	public static class Users
	{
		private const string Prefix = $"{DashboardUrls.Prefix}/users";
		public const string Index = Prefix;
		public const string Add = $"{Prefix}/add";
	}

	public static class LockoutReasons
	{
		private const string Prefix = $"{DashboardUrls.Prefix}/lockout-reasons";
		public const string Index = Prefix;
		public const string Add = $"{Prefix}/add";
	}
}