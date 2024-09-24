using NUnit.Framework;

namespace LeonDrace.SimpleStatemachine.Tests
{
	public class Tests
	{

		[Test]
		public void SetState()
		{
			var stateMachine = new StateMachine();
			var testState = new TestState<int>(1);
			stateMachine.SetState(testState);

			Assert.IsTrue(stateMachine.IsCurrentState(testState));
		}

		[Test]
		public void HasState()
		{
			var stateMachine = new StateMachine();
			var testState = new TestState<int>(1);
			var testState2 = new TestState<float>(2);
			stateMachine.SetState(testState);

			stateMachine.SetState(testState2);

			Assert.IsTrue(stateMachine.HasState(testState));
			Assert.IsTrue(stateMachine.HasState(testState2));
		}

		[Test]
		public void HasCurrentTransition()
		{
			var stateMachine = new StateMachine();
			var testState = new TestState<int>(1);
			var testState2 = new TestState<float>(2);
			stateMachine.SetState(testState);

			var transition = stateMachine.AddTransition(testState, testState2, new FuncPredicate(() => true));

			Assert.IsTrue(stateMachine.HasCurrentTransition(transition));
		}

		[Test]
		public void HasTransitionOfType()
		{
			var stateMachine = new StateMachine();
			var testState = new TestState<int>(1);
			var testState2 = new TestState<float>(2);
			stateMachine.SetState(testState);

			var transition = stateMachine.AddTransition(testState, testState2, new FuncPredicate(() => true));

			Assert.IsTrue(stateMachine.HasTransition<TestState<int>>(transition));
		}

		[Test]
		public void AddTransition()
		{
			var stateMachine = new StateMachine();
			var testState = new TestState<int>(1);
			var testState2 = new TestState2<int>(2);
			stateMachine.SetState(testState);
			var transition = stateMachine.AddTransition(testState, testState2, new FuncPredicate(() => true));

			Assert.IsTrue(stateMachine.HasCurrentTransition(transition));

			stateMachine.Update();

			Assert.IsTrue(stateMachine.CurrentState == testState2);
		}

		[Test]
		public void AddAnyTransition()
		{
			var stateMachine = new StateMachine();
			var testState = new TestState<int>(1);
			var testState2 = new TestState2<int>(2);
			stateMachine.SetState(testState);

			var transition = stateMachine.AddAnyTransition(testState2, new FuncPredicate(() => true));

			Assert.IsTrue(stateMachine.HasAnyTransition(transition));

			stateMachine.Update();

			Assert.IsTrue(stateMachine.CurrentState == testState2);
		}

		[Test]
		public void UpdateStatemachine()
		{
			var stateMachine = new StateMachine();
			var testState = new TestState<int>(1);
			var testState2 = new TestState2<int>(2);
			stateMachine.SetState(testState);

			var transition = stateMachine.AddAnyTransition(testState2, new FuncPredicate(() => stateMachine.IsCurrentState(testState)));
			var transition2 = stateMachine.AddTransition(testState2, testState, new FuncPredicate(() => true));

			Assert.IsTrue(stateMachine.HasAnyTransition(transition));
			Assert.IsTrue(stateMachine.HasTransition<TestState2<int>>(transition2));

			stateMachine.Update();

			Assert.IsTrue(stateMachine.CurrentState == testState2);

			stateMachine.Update();

			Assert.IsTrue(stateMachine.CurrentState == testState);
		}

		private sealed class TestState<T> : BaseState<T>
		{
			public TestState(T blackboard) : base(blackboard)
			{
			}
		}

		private sealed class TestState2<T> : BaseState<T>
		{
			public TestState2(T blackboard) : base(blackboard)
			{
			}
		}
	}
}
