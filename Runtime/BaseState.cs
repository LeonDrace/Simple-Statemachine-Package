namespace LeonDrace.SimpleStatemachine
{
	public abstract class BaseState<TBlackboard> : IState
	{
		protected TBlackboard m_Blackboard;

		public BaseState(TBlackboard blackboard)
		{
			this.m_Blackboard = blackboard;
		}

		public virtual void OnEnter()
		{

		}

		public virtual void OnExit()
		{

		}

		public virtual void OnUpdate()
		{

		}
	}
}
