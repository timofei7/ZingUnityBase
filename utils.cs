using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// useful static utils
/// </summary>
public class utils {
	
	/// <summary>
	/// finds the midpoint between two points. 
	/// </summary>
	/// <param name="a">
	/// A <see cref="Vector2"/>
	/// </param>
	/// <param name="b">
	/// A <see cref="Vector2"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector2"/>
	/// </returns>
	public static Vector2 MidPoint(Vector2 a, Vector2 b)
	{
		return new Vector2((a.x + b.x) / 2, (a.y + b.y) / 2);
	}
	
	/// <summary>
	/// find a midpoint between two vector3s
	/// </summary>
	/// <param name="a">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="b">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public static Vector3 MidPoint(Vector3 a, Vector3 b)
	{
		return new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2,  (a.z + b.z) / 2);
	}
	
	
	/// <summary>
	/// flattens a 2D list of vector2s
	/// </summary>
	public static List<Vector2> FlattenList(List<List<Vector2>> ll)
	{
		List<Vector2> r = new List<Vector2>();
		foreach (List<Vector2> l in ll)
		{
			r.AddRange(l);
		}
		return r;
	}
	

	
	public static Transform[] GetOnlyChildren(GameObject go)
	{
		Transform [] f = go.GetComponentsInChildren<Transform>();
		Transform[] n = new Transform[f.Length-1];
		int j = 0; 
		for (int i = 0; i < f.Length; i++)
		{
			if (!f[i].name.Equals(go.transform.name))
			{
				n[j] = f[i];
				j++;
			}
		}
		return n;
	}
	
	public static Transform[] Range(Transform[] t, int start, int end)
	{
		Transform[] r = new Transform[end+1 - start];
		for (int i = start; i <= end; i++)
			r[i] = t[i];
		return r;
	}
	
	
	public static Transform[] Reverse(Transform[] t)
	{
		Transform[] n = new Transform[t.Length];
		for (int i = 0; i < t.Length; i++)
		{
			n[t.Length-1 - i] = t[i];
		}
		return n;
	}
	
	public static void KillGOs(List<GameObject> gos)
	{
		if (gos != null)
		{
			foreach (GameObject go in gos)
			{
				try {GameObject.Destroy(go);}
				catch {};
			}
			gos.Clear(); 
		}
	}
	
	
	public static string FormatText(string s, Color c)
	{
		string open = "<color="+Color2Hex(c) +">";
		string close = "</color>";
		string comp =  string.Concat(open, s, close);
		//Debug.LogWarning(comp);
		return comp;
	}
	
	public static string Color2Hex(Color c)
	{
		Color32 c32 = (Color32) c;
		string hex = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", (int) c32.r, (int) c32.g, (int) c32.b, (int) c32.a);
		//Debug.LogWarning(hex);
		return hex;
	}
	
	
	public static T GetRandomEnum<T>()
	{
	    System.Array A = System.Enum.GetValues(typeof(T));
	    T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
	    return V;
	}
	
	
	public static Rect BoundsToScreenRect(Camera cam, Bounds bounds)
	{
	    // Get mesh origin and farthest extent (this works best with simple convex meshes)
	    Vector3 origin = cam.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
	    Vector3 extent = cam.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));
	
	    // Create rect in screen space and return - does not account for camera perspective
	    return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
	}



}


