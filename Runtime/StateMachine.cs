using System;
using System.Collections.Generic;

namespace LeonDrace.SimpleStatemachine
{
	public class StateMachine
	{
		private StateNode current;

		private Dictionary<Type, StateNode> nodes = new();
		private List<ITransition> anyTransitions = new();

		public IState CurrentState => current == null ? null : current.State;

		public void Update()
		{
			ITransition transition = GetTransition();
			if (transition != null)
			{
				ChangeState(transition.To);
			}

			current.State?.OnUpdate();
		}

		/// <summary>
		/// Add state and set the new current state.
		/// </summary>
		/// <param name="state"></param>
		public void SetState(IState state)
		{
			if (current == null)
			{
				current = GetOrAddNode(state);
				current.State?.OnEnter();
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
			return GetOrAddNode(from).AddTransition(to, condition);
		}

		/// <summary>
		/// Add a global transition.
		/// </summary>
		/// <param name="to"></param>
		/// <param name="condition"></param>
		/// <returns></returns>
		public ITransition AddAnyTransition(IState to, IPredicate condition)
		{
			anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
			return anyTransitions[anyTransitions.Count - 1];
		}

		/// <summary>
		/// Check if a transition exists.
		/// </summary>
		/// <param name="transition"></param>
		/// <returns></returns>
		public bool HasAnyTransition(ITransition transition)
		{
			return anyTransitions.Contains(transition);
		}

		/// <summary>
		/// Check if the current state has this transition.
		/// </summary>
		/// <param name="transition"></param>
		/// <returns></returns>
		public bool HasCurrentTransition(ITransition transition)
		{
			if (current == null) return false;
			return current.Transitions.Contains(transition);
		}

		/// <summary>
		/// Check if any state has this transition.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="transition"></param>
		/// <returns></returns>
		public bool HasTransition<T>(ITransition transition)
		{
			if (nodes.TryGetValue(typeof(T), out var node))
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
			if (current == null) return false;
			return current.State == state;
		}

		private StateNode GetOrAddNode(IState state)
		{
			StateNode node = nodes.GetValueOrDefault(state.GetType());

			if (node == null)
			{
				node = new StateNode(state);
				nodes.Add(state.GetType(), node);
			}

			return node;
		}

		private void ChangeState(IState state)
		{
			if (state == current.State) return;

			IState previousState = current.State;
			IState nextState = nodes[state.GetType()].State;

			previousState?.OnExit();
			nextState?.OnEnter();
			current = nodes[state.GetType()];
		}

		private ITransition GetTransition()
		{
			//Evaluate any transitions
			int count = anyTransitions.Count;
			for (int i = 0; i < count; i++)
			{
				if (anyTransitions[i].Condition.Evaluate())
				{
					return anyTransitions[i];
				}
			}

			//Evaluate current state transitions.
			return current.EvaluateTransitions();
		}

		private sealed class StateNode
		{
			public IState State { get; }
			public List<ITransition> Transitions => transitions;

			private List<ITransition> transitions;

			public StateNode(IState state)
			{
				State = state;
				transitions = new List<ITransition>();
			}

			public ITransition AddTransition(IState to, IPredicate condition)
			{
				transitions.Add(new Transition(to, condition));
				return transitions[transitions.Count - 1];
			}

			public ITransition EvaluateTransitions()
			{
				int count = transitions.Count;
				for (int i = 0; i < count; i++)
				{
					if (transitions[i].Condition.Evaluate())
					{
						return transitions[i];
					}
				}
				return null;
			}
		}
	}
}
