using System;
using Microsoft.AspNetCore.Builder;

namespace Sienar.Infrastructure;

/// <summary>
/// Contains prioritized middlewares for <see cref="SienarAppBuilder"/>
/// </summary>
public class MiddlewareProvider : PrioritizedDictionaryOfLists<Action<WebApplication>>;