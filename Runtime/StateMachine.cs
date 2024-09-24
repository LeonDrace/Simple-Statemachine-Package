using System;
using System.Collections.Generic;

namespace LeonDrace.SimpleStatemachine
{
	/// <summary>
	/// A simple type based statemachine.
	/// </summary>
	public class StateMachine
	{
		private StateNode m_Current;

		private Dictionary<Type, StateNode> m_Nodes = new();
		private List<ITransition> m_AnyTransitions = new();

		public IState CurrentState => m_Current == null ? null : m_Current.State;

		public void Update()
		{
			ITransition transition = GetTransition();
			if (transition != null)
			{
				ChangeState(transition.To);
			}

			m_Current.State?.OnUpdate();
		}

		/// <summary>
		/// Add state and set the new current state.
		/// </summary>
		/// <param name="state"></param>
		public void SetState(IState state)
		{
			if (m_Current == null)
			{
				m_Current = GetOrAddNode(state);
				m_Current.State?.OnEnter();
				return;
			}

			ChangeState(GetOrAddNode(state).State);
		}

		/// <summary>
		/// Add a state dependent transition.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="condition"></param>
		/// <returns></returns>
		public ITransition AddTransition(IState from, IState to, IPredicate condition)
		{
			return GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
		}

		/// <summary>
		/// Add a global transition.
		/// </summary>
		/// <param name="to"></param>
		/// <param name="condition"></param>
		/// <returns></returns>
		public ITransition AddAnyTransition(IState to, IPredicate condition)
		{
			var transition = new Transition(GetOrAddNode(to).State, condition);
			m_AnyTransitions.Add(transition);
			return transition;
		}

		/// <summary>
		/// Check if a transition exists.
		/// </summary>
		/// <param name="transition"></param>
		/// <returns></returns>
		public bool HasAnyTransition(ITransition transition)
		{
			return m_AnyTransitions.Contains(transition);
		}

		/// <summary>
		/// Check if the current state has this transition.
		/// </summary>
		/// <param name="transition"></param>
		/// <returns></returns>
		public bool HasCurrentTransition(ITransition transition)
		{
			if (m_Current == null) return false;
			return m_Current.Transitions.Contains(transition);
		}

		/// <summary>
		/// Check if any state has this transition.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="transition"></param>
		/// <returns></returns>
		public bool HasTransition<T>(ITransition transition)
		{
			if (m_Nodes.TryGetValue(typeof(T), out var node))
			{
				return node.Transitions.Contains(transition);
			}
			return false;
		}

		/// <summary>
		/// Check if it is the current state.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		public bool IsCurrentState(IState state)
		{
			if (m_Current == null) return false;
			return m_Current.State == state;
		}

		private StateNode GetOrAddNode(IState state)
		{
			StateNode node = m_Nodes.GetValueOrDefault(state.GetType());

			if (node == null)
			{
				node = new StateNode(state);
				m_Nodes.Add(state.GetType(), node);
			}

			return node;
		}

		private void ChangeState(IState state)
		{
			if (state == m_Current.State) return;

			IState previousState = m_Current.State;
			IState nextState = m_Nodes[state.GetType()].State;

			previousState?.OnExit();
			nextState?.OnEnter();
			m_Current = m_Nodes[state.GetType()];
		}

		private ITransition GetTransition()
		{
			//Evaluate any transitions.
			int count = m_AnyTransitions.Count;
			for (int i = 0; i < count; i++)
			{
				if (m_AnyTransitions[i].Condition.Evaluate())
				{
					return m_AnyTransitions[i];
				}
			}

			//Evaluate current state transitions.
			return m_Current.EvaluateTransitions();
		}

		private sealed class StateNode
		{
			public IState State { get; }
			public List<ITransition> Transitions => m_Transitions;

			private List<ITransition> m_Transitions;

			public StateNode(IState state)
			{
				State = state;
				m_Transitions = new List<ITransition>();
			}

			public ITransition AddTransition(IState to, IPredicate condition)
			{
				m_Transitions.Add(new Transition(to, condition));
				return m_Transitions[m_Transitions.Count - 1];
			}

			public ITransition EvaluateTransitions()
			{
				int count = m_Transitions.Count;
				for (int i = 0; i < count; i++)
				{
					if (m_Transitions[i].Condition.Evaluate())
					{
						return m_Transitions[i];
					}
				}
				return null;
			}
		}
	}
}
