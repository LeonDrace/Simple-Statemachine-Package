using System;

namespace LeonDrace.SimpleStatemachine
{
	public class FuncPredicate : IPredicate
	{
		private readonly Func<bool> m_Func;

		public FuncPredicate(Func<bool> func)
		{
			this.m_Func = func;
		}

		public bool Evaluate() => m_Func.Invoke();
	}
}
