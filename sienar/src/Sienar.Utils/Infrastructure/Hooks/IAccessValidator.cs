﻿using System.Threading.Tasks;

namespace Sienar.Infrastructure.Hooks;

// ReSharper disable once TypeParameterCanBeVariant
public interface IAccessValidator<T>
{
	Task Validate(
		AccessValidationContext context,
		ActionType action,
		T? input);
}