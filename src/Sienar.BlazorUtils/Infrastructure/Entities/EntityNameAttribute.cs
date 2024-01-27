using System;

namespace Sienar.Infrastructure.Entities;

[AttributeUsage(AttributeTargets.Class)]
public class EntityNameAttribute : Attribute
{
	public required string Singular { get; set; }
	public required string Plural { get; set; }
}