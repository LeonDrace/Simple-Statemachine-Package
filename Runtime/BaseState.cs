namespace LeonDrace.SimpleStatemachine
{
	public abstract class BaseState<TBlackboard> : IState
	{
		protected TBlackboard blackboard;

		public BaseState(TBlackboard blackboard)
		{
			this.blackboard = blackboard;
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
