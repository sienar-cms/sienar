using System;
using System.Threading.Tasks;

namespace Sienar.Identity;

public interface IVerificationCodeManager
{
	Task<VerificationCode> CreateCode(
		SienarUser user,
		string type);

	Task DeleteCode(
		SienarUser user,
		string type);

	Task<VerificationCode?> GetCode(
		SienarUser user,
		string type);

	VerificationCodeStatus GetCodeStatus(
		VerificationCode? code,
		Guid suppliedCode);

	Task<VerificationCodeStatus> GetCodeStatus(
		SienarUser user,
		string type,
		Guid suppliedCode);

	Task<VerificationCodeStatus> VerifyCode(
		SienarUser user,
		string type,
		Guid suppliedCode,
		bool deleteIfValid);
}