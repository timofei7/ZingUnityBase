using UnityEngine;
using System.Collections;

public class TestBState : BStateManagerSingleton<TestBState> {

	#region StateSetup
	public static States initialState = TestStates.Test;

	//define the available states here
	public class TestStates : States
	{
		public static States Test = new States("Test");
		public static States Test2 = new States("Test2");
	}
	#endregion StateSetup

	public IEnumerator TestState ()
	{
		// do something
		Debug.LogWarning("bstate: HIII FROM TESTSTATE");
		while (CurrState == TestStates.Test) {
			// fillin in exit conditions/transitions
			// do something
			yield return new WaitForSeconds(5f);
			CurrState = TestStates.Test2;
		}
		Debug.LogWarning("bstate: BYE FROM TESTSTATE");
		// do something
	}

	public IEnumerator Test2State ()
	{
		// do something
		Debug.LogWarning("bstate: HIII FROM TEST2STATE");
		while (CurrState == TestStates.Test2) {
			// fillin in exit conditions/transitions
			// do something
			yield return null;
		}
		Debug.LogWarning("bstate: BYE FROM TEST2STATE");
		CurrState = TestStates.Idle;
		// do something
	}


	// start the first state
	public override IEnumerator Start()
	{
		yield return null;
		ChangeState(initialState);
		NextState();
	}



}
