using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Prefab attribute. Use this on child classes to define if they have a prefab associated or not!
/// </summary>
[AttributeUsage(AttributeTargets.Class,Inherited = true)]
public class PrefabAttribute : Attribute
{
    string _name;
	public string Name { get { return this._name; } }
	public PrefabAttribute() { this._name = ""; }
    public PrefabAttribute(string name) { this._name = name; }
}


/// <summary>
/// MONOBEHAVIOR PSEUDO SINGLETON ABSTRACT CLASS
/// usage		: best is to be attached to a gameobject but if not that is ok,
/// 			: this will create one on first access
/// example		: '''public sealed class MyClass : Singleton<MyClass> {'''
/// references	: http://tinyurl.com/d498g8c
/// 			: http://tinyurl.com/cc73a9h
/// 			: http://unifycommunity.com/wiki/index.php?title=Singleton
/// </summary>
public abstract class Singleton<T> : TTMonoBehaviour where T : TTMonoBehaviour
{


	private static T _instance = null;
	public static bool IsAwake { get { return (_instance != null); } }

	/// <summary>
	/// gets the instance of this Singleton
	/// use this for all instance calls:
	/// MyClass.Instance.MyMethod();
	/// or make your public methods static
	/// and have them use Instance
	/// </summary>
	public static T Instance {
		get {
			Type mytype = typeof(T);
			if (_instance == null) {
				_instance = (T)FindObjectOfType (mytype);
				if (_instance == null) {
					//Debug.Log("initializing instance of: " + mytype.Name);
					string goName = mytype.ToString ();
					GameObject go = GameObject.Find (goName);
					if (go == null) // try again searching for a cloned object
					{
						go = GameObject.Find (goName+"(Clone)");
						if (go != null)
						{
							//Debug.Log("found clone of object using it!");
						}
					}

					if (go == null) //if still not found try prefab or create
					{
			            bool hasPrefab = Attribute.IsDefined(mytype, typeof(PrefabAttribute));
						// checks if the [Prefab] attribute is set and pulls that if it can
						if (hasPrefab)
						{
							PrefabAttribute attr = (PrefabAttribute)Attribute.GetCustomAttribute(mytype,typeof(PrefabAttribute));
							string prefabname = attr.Name;
							//Debug.LogWarning(goName + " not found attempting to instantiate prefab... either: " + goName + " or: " + prefabname);
							try
							{
								if (prefabname != "")
								{
									go = (GameObject)Instantiate(Resources.Load(prefabname, typeof(GameObject)));
								}
								else
								{
									go = (GameObject)Instantiate(Resources.Load(goName, typeof(GameObject)));
								}
							} catch (Exception e)
							{
								Debug.LogError("could not instanctiate prefab even though prefab attribute was set: " + e.Message + "\n" + e.StackTrace);
							}
						}
						if (go == null)
						{
							//Debug.LogWarning(goName + " not found creating...");
							go = new GameObject ();
							go.name = goName;
						}
					}
					_instance = go.GetComponent<T>();
					if (_instance == null)
					{
						_instance = go.AddComponent<T>();
					}
				}
				else
				{
					//Debug.Log(mytype.Name + " had to be searched for but was found");
					int count = FindObjectsOfType(mytype).Length;
					if (count > 1)
					{
						Debug.LogError("Singleton: there are " + count.ToString() + " of " + mytype.Name);
						throw new Exception("Too many (" + count.ToString() + ") prefab singletons of type: " + mytype.Name);
					}
				}
			}

			return _instance;
		}
	}

	/// <summary>
	/// for garbage collection
	/// </summary>
	public virtual void OnApplicationQuit ()
	{
		// release reference on exit
		_instance = null;
	}

	// in your child class you can implement Awake()
	// 	and add any initialization code you want such as
	// 	DontDestroyOnLoad(this.gameObject);
	// 	if you want this to persist across loads
	//  or if you want to set a parent object with SetParent()

	/// <summary>
	/// parent this to another gameobject by string
	/// call from Awake if you so desire
	/// </summary>
	protected void SetParent (string parentGOName)
	{
		if (parentGOName != null) {
			GameObject parentGO = GameObject.Find (parentGOName);
			if (parentGO == null) {
				parentGO = new GameObject ();
				parentGO.name = parentGOName;
				parentGO.transform.parent = null;
			}
			this.transform.parent = parentGO.transform;
		}
	}

}
