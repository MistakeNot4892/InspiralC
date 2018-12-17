// With thanks to Brian Rogers on StackOverflow, whose code I copypasted.
using System;
using Newtonsoft.Json.Linq;

public static class JsonExtensions
{
	public static bool IsNullOrEmpty(this JToken token)
	{
		return (token == null) ||
			   (token.Type == JTokenType.Array && !token.HasValues) ||
			   (token.Type == JTokenType.Object && !token.HasValues) ||
			   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
			   (token.Type == JTokenType.Null);
	}
}