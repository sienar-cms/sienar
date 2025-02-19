﻿using System.Threading.Tasks;
using Sienar.Hooks;

namespace Sienar.Services;

/// <summary>
/// Runs after-action hooks for a hookable request
/// </summary>
/// <typeparam name="T">the type of the request or entity</typeparam>
// ReSharper disable once TypeParameterCanBeVariant
public interface IAfterActionService<T>
{
	/// <summary>
	///  Runs all after-action hooks for a hookable request
	/// </summary>
	/// <param name="input">the request or entity</param>
	/// <param name="action">the action type</param>
	Task Run(
		T input,
		ActionType action);
}
