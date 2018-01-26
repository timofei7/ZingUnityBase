using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

/// <summary>
/// state class for all states
/// </summary>
///
public abstract class BStateManager : TTMonoBehaviour
{
	public event StateChangedEventHandler StateChangedEvent;

	public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);

	public class StateChangedEventArgs : System.EventArgs {
		public States NewState { get; set;}
	}

	public bool stateLocked;
	public string whatStateAmIIn;
	public bool skipNext;

	/// <summary>
	/// the current/next state
	/// </summary>
	private States currState;

	public States CurrState {
		get {
			return this.currState;
		}
		set {
			whatStateAmIIn = value.ToString();
			previousState = this.currState;
			currState = value;
		}
	}


	public States previousState;
	public States pausedState;

	public BCoroutine currStateCoroutine;
	public BCoroutine pausedStateCoroutine; // if we need to save a state while paused and run another
	public bool pausedStateExists;


	/// <summary>
	/// Start this StateManager instance
	/// </summary>
	public virtual IEnumerator Start()
	{
		//Debug.LogWarning("BStateManager: default Start Idle state starting");
		currState = previousState = States.Idle;
		pausedState = States.None;
		yield return null;
		NextState ();
	}


	/// <summary>
	/// Changes the state.
	/// </summary>
	/// <param name='state'>
	/// State.
	/// </param>
	public void ChangeState (States newstate)
	{
		if (!stateLocked)
		{
			this.CurrState = newstate;
		}
	}

	public void PauseAndChangeState (States newstate)
	{
		if (!stateLocked)
		{
			//Debug.Log("BStateManager: pausing state " + currState + " and changing to: " + newstate);
			pausedState = currState;
			pausedStateExists = true;
			currStateCoroutine.pause();
			OnPause(pausedState);
			pausedStateCoroutine = currStateCoroutine;
			ChangeState(newstate);
			NextState(); //needs to be called to jumpstart the next foreground state
		}
	}

	public void Pause()
	{
		//Debug.Log("BStateManager: pausing state " + currState);
		pausedState = currState;
		pausedStateExists = true;
		currStateCoroutine.pause();
		OnPause(pausedState);
	}


	public void ResumePausedState()
	{
		if (pausedStateCoroutine != null)
		{
			currStateCoroutine.BSComplete += (obj) =>
			{
				Debug.LogWarning("on resumption of state " + pausedState + " state " + previousState + " has completed as well as intermediate state of: " + currState);
				skipNext = true; // don't actually rerun the state though
				CurrState = pausedState;
				currStateCoroutine = pausedStateCoroutine;
				pausedStateCoroutine = null;
				currStateCoroutine.resume();
				pausedStateExists = false;
				pausedState = States.None;
			};
			OnResume(pausedState);
			CurrState = States.Idle; // this is just a temporary state to drop into to cause the current state to complete
		}

	}

	public void Resume()
	{
		OnResume(pausedState);
		currState = pausedState;
		pausedState = States.None;
		currStateCoroutine.resume();
		pausedStateExists = false;
	}


	/// <summary>
	/// turns out coroutines compile down to state machines and make for a very clever and easy to read state pattern
	/// this changes the state
	/// </summary>
	public void NextState ()
	{
		if ((pausedStateCoroutine == null || pausedState != States.None) && !skipNext) //skip next here to not do nextstate when resuming
		{
			string methodName = CurrState.Name + "State";
			//Debug.Log("BStateManager: NextState: " + this.name + " STATE to: " + methodName + " from previous state of: " + previousState);


			try
			{
				Type thisType = this.GetType();
				MethodInfo theMethod = thisType.GetMethod(methodName);
				currStateCoroutine = BCoroutine.make( ((IEnumerator)theMethod.Invoke(this, null)) );
				currStateCoroutine.BSComplete += HandleCurrStateCoroutineBSComplete;
				//fire off state changed event
				NotifyStateChange(CurrState);
			}
			catch (Exception e)
			{
				Debug.LogWarning("failed to start next state -- could be ok if there simply is no next state. error was: " + e.Message);
			}
		}
		skipNext = false;
	}


	private void HandleCurrStateCoroutineBSComplete (bool wasKilled)
	{
		if (CurrState != States.None && !wasKilled)
		{
			NextState();
		}
		else
		{
			Debug.LogWarning("BStateManger: state " + CurrState + " dropping off into none -- will need a manual NextState call to restart");
		}
	}



	/// <summary>
	/// defacto paused state
	/// </summary>
	public virtual IEnumerator IdleState ()
	{
		// do something
		while (currState == States.Idle)
		{
			// fillin in exit conditions/transitions
			// do something
			yield return null;
		}
		// do something
	}

	protected void NotifyStateChange(States s)
	{
		if (StateChangedEvent != null)
		{
			StateChangedEventArgs e = new StateChangedEventArgs();
			e.NewState = s;
			StateChangedEvent(this, e);
		}
	}

	protected IEnumerator NoneState()
	{
		yield return null;
		currStateCoroutine.kill();
	}

	public virtual void OnPause(States state)
	{

	}

	public virtual void OnResume(States state)
	{

	}

	/// <summary>
	/// searches the current class for a state by name and invokes it if found
	/// </summary>
	public bool SyncState(States state)
	{
		bool r = false;
		var targetAssembly = Assembly.GetAssembly(this.GetType());
		var subtypes = targetAssembly.GetTypes().Where(t => t.IsSubclassOf(typeof(States)));
		foreach (System.Type st in subtypes)
		{
            if (st.DeclaringType == this.GetType())
			{
				//Debug.Log("found subtype: " +this.name + " : " + st.Name);
				var fields = st.GetFields(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
				foreach (FieldInfo fi in fields)
				{
					if (fi.Name == state.Name)
					{
						//Debug.Log("found state: " + fi.Name);
						CurrState = (States) fi.GetValue(null);
						r = true;
					}
				}
			}
		}

		return r;
	}


}
