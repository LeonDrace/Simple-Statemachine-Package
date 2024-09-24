namespace LeonDrace.SimpleStatemachine
{
	public interface IState
	{
		void OnEnter();
		void OnUpdate();
		void OnExit();
	}
}
