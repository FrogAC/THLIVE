using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

/*public static class JsonHelper {
	public static T[] FromJson<T> (string jsonArray) {
		jsonArray = WrapArray(jsonArray);
		return FromJsonWrapped<T>(jsonArray);
	}

	public static T[] FromJsonWrapped<T> (string jsonObject) {
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonObject);
		return wrapper.items;
	}

	private static string WrapArray (string jsonArray) {
		return "{ \"items\": " + jsonArray + "}";
	}

	public static string ToJson<T> (T[] array) {
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.items = array;
		return JsonUtility.ToJson(wrapper);
	}

	public static string ToJson<T> (T[] array, bool prettyPrint) {
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.items = array;
		return JsonUtility.ToJson(wrapper, prettyPrint);
	}

	[System.Serializable]
	private class Wrapper<T> {
		public T[] items;
	}
}
*/
[System.Serializable]
public struct hitmapType {
	public int speed;
	public string audiofile;
	public List<hitmapNotes> notes;
}

[System.Serializable]
public class hitmapNotes {
	public double starttime;
	public double endtime;
	public bool longnote;
	public bool parallel;
	public int lane;
	public bool hold;
}


public class NoteCreater : MonoBehaviour {
	private TextAsset hitmapFile;
	public string hitmapName;

	public static int compareByStarttime (hitmapNotes a, hitmapNotes b) {
		return a.starttime.CompareTo(b.starttime);
	}

	void Awake () {
		readHitmap();
	}

	void Update () {
	
	}



	void readHitmap () {		
		hitmapFile = Resources.Load(hitmapName) as TextAsset;
		hitmapType hitmapData = JsonUtility.FromJson<hitmapType>(hitmapFile.text);
		hitmapData.notes.Sort(compareByStarttime);
		foreach (hitmapNotes note in hitmapData.notes) {			
			Debug.Log(note.starttime + " " + note.lane);
		}
	}

	void notesCreate () {
		
	}
}
