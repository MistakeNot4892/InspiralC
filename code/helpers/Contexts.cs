using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace inspiral
{
	internal static class Contexts
	{
		internal static ContextLogin Login = new ContextLogin();
		internal static ContextGeneral General = new ContextGeneral();
	}
}