using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SingletonWithPrereqsExtension {
	
	
	//if Loader ever adds any new components have to add them here
	//TODO:  sync with loader automatically?   the problem is just doing a scene load doesn't work and ends up with multiple GameControl objects (one singleton generated other loaded)
	public static void Require<T>(this Singleton<T> singleton , string[] prereqs) where T : TTMonoBehaviour
	{
		if (Application.isEditor && Application.loadedLevelName != "Loader")
		{
			//Debug.LogWarning("we are alone, load up loader elements");
			
			foreach (string p in prereqs)
			{
				if (GameObject.Find(p) == null && GameObject.Find(p+"(Clone)") == null)
				{
					//Debug.Log("loading prereq " + p + " for: " + singleton.name); 
					GameObject.Instantiate(Resources.Load(p, typeof(GameObject)));
				}
			}
		}
	}

	
}
