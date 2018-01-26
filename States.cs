using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for States
/// inherit this sucker and define your own like so:
/// public class MyStates : States {
/// public static States NewState = new State("NewState");
/// then access all your new states like so:
/// MyStates.NewState
/// </summary>
public class States
{
	public string Name { get; set; } //auto properties

	protected States() {} //required but blank
	public States (string name)
	{
		Name = name;
	}

	// common default states
	public static States Idle = new States ("Idle");
	public static States Running = new States ("Running");
	public static States None = new States ("None");

	public override string ToString()
	{
		return Name + "State";
	}

}
