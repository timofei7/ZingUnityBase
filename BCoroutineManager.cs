using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// JobManager is just a proxy object so we have a launcher for the coroutines
/// adapted from: https://github.com/prime31/P31UnityAddOns/blob/master/Scripts/Misc/JobManager.cs
/// </summary>
public class BCoroutineManager : Singleton<BCoroutineManager>
{
	void Awake() { 	DontDestroyOnLoad(this.gameObject); }
}

public class BCoroutine
{
	public event System.Action<bool> BSComplete;

	private bool _running;
	public bool running { get { return _running; } }

	private bool _paused;
	public bool paused { get { return _paused; } }

	private IEnumerator _coroutine;
	private bool _bCoroutineWasKilled;
	private Stack<BCoroutine> _childStack;


	#region constructors

	public BCoroutine( IEnumerator coroutine ) : this( coroutine, true )
	{}


	public BCoroutine( IEnumerator coroutine, bool shouldStart )
	{
		_coroutine = coroutine;

		if( shouldStart )
			start();
	}

	#endregion constructors


	#region static State makers

	public static BCoroutine make( IEnumerator coroutine )
	{
		return new BCoroutine( coroutine );
	}


	public static BCoroutine make( IEnumerator coroutine, bool shouldStart )
	{
		return new BCoroutine( coroutine, shouldStart );
	}

	#endregion static State makers


	private IEnumerator doWork()
	{
		// null out the first run through in case we start paused
		yield return null;

		while( _running )
		{
			if( _paused )
			{
				yield return null;
			}
			else
			{
				// run the next iteration and stop if we are done
				if( _coroutine.MoveNext() )
				{
					yield return _coroutine.Current;
				}
				else
				{
					// run our child jobs if we have any
					if( _childStack != null )
						yield return BCoroutineManager.Instance.StartCoroutine( runChildren() );

					_running = false;
				}
			}
		}

		// fire off a complete event
		if( BSComplete != null )
			BSComplete( _bCoroutineWasKilled );
	}



	public void start()
	{
		_running = true;
		BCoroutineManager.Instance.StartCoroutine( doWork() );
	}


	public IEnumerator startAsCoroutine()
	{
		_running = true;
		yield return BCoroutineManager.Instance.StartCoroutine( doWork() );
	}


	public void pause()
	{
		_paused = true;
	}


	public void resume()
	{
		_paused = false;
	}


	public void kill()
	{
		_bCoroutineWasKilled = true;
		_running = false;
		_paused = false;
	}


	public void kill( float delayInSeconds )
	{
		var delay = (int)( delayInSeconds * 1000 );
		new System.Threading.Timer( obj =>
		{
			lock( this )
			{
				kill();
			}
		}, null, delay, System.Threading.Timeout.Infinite );
	}


	private IEnumerator runChildren()
	{
		if( _childStack != null && _childStack.Count > 0 )
		{
			do
			{
				BCoroutine child = _childStack.Pop();
				yield return BCoroutineManager.Instance.StartCoroutine( child.startAsCoroutine() );
			}
			while( _childStack.Count > 0 );
		}
	}



	public BCoroutine createAndAddChild( IEnumerator coroutine )
	{
		var j = new BCoroutine( coroutine, false );
		addChild( j );
		return j;
	}


	public void addChild( BCoroutine child )
	{
		if( _childStack == null )
			_childStack = new Stack<BCoroutine>();
		_childStack.Push( child );
	}


	public void removeChild( BCoroutine child )
	{
		if( _childStack.Contains( child ) )
		{
			var childStack = new Stack<BCoroutine>( _childStack.Count - 1 );
			var allCurrentChildren = _childStack.ToArray();
			System.Array.Reverse( allCurrentChildren );

			for( var i = 0; i < allCurrentChildren.Length; i++ )
			{
				var j = allCurrentChildren[i];
				if( j != child )
					childStack.Push( j );
			}

			// assign the new stack
			_childStack = childStack;
		}
	}


}
