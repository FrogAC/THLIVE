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
	public List<HitmapNotes> lane;
}

[System.Serializable]
public class HitmapNotes {
	public double starttime;
	public double endtime;
	public bool longnote;
	public bool parallel;
	public int lane;
	public bool hold;
	///0:single; 1:long;
	public int type;
}



public class GameController : MonoBehaviour {
	private TextAsset hitmapFile;
	public GameObject notepanel;

	public float radiusScale;
	private Vector2 startPos;
	private Vector2[] endPos = new Vector2[9];
	private static float Width, Aspect, Height;
	///notesCount
	public int p = 0;
	public string hitmapName;
	public hitmapType hitmapData;
	public HitmapNotes[] notes = new HitmapNotes[10000];
	public AudioSource audioSource;
	public double gameTime = 0;

	public List<GameObject> pooledSingle;
	public List<GameObject> pooledLong;
	public Transform notePanel;
	public GameObject[] notePrototypes;
	public int pooledAmount;

	void Awake () {
		initCanvas();
		readHitmap();
		playAudio();
		poolNotes();
	}

	void FixedUpdate () {
		notesCreate();
	}

	#region pool

	void poolNotes () {
		for (int i = 0; i < pooledAmount; i++) {
			GameObject note = Instantiate(notePrototypes[0], notePanel);
			note.SetActive(false);
			pooledSingle.Add(note);
		}		

		for (int i = 0; i < pooledAmount; i++) {
			GameObject note = Instantiate(notePrototypes[1], notePanel);
			note.SetActive(false);
			pooledLong.Add(note);
		}
	}

	public SingleNoteController getSingleController () {
		foreach (GameObject note in pooledSingle) {
			if (!note.activeInHierarchy)
				return note.GetComponent<SingleNoteController>();
		}
		return null/*Instantiate(notePrototypes[type], notePanel)*/;
	}

	public LongNoteController getLongController () {
		foreach (GameObject note in pooledLong) {
			if (!note.activeInHierarchy)
				return note.GetComponent<LongNoteController>();
		}
		return null/*Instantiate(notePrototypes[type], notePanel)*/;
	}

	public void destroyPooledNote (GameObject go) {
		go.SetActive(false);
	}

	#endregion

	public static int compareByStarttime (HitmapNotes a, HitmapNotes b) {
		return a.starttime.CompareTo(b.starttime);
	}

	#region init

	void initCanvas () {
		Aspect = (float)Screen.width / Screen.height;
		Width = 800;
		Height = Width / Aspect;
		startPos = notePanel.GetComponentInChildren<RectTransform>().anchoredPosition;

		float radius = Width / 2 * radiusScale;
		endPos[0].Set(-radius, 0);
		endPos[1].Set(-radius * Mathf.Cos(0.125f * Mathf.PI), -radius * Mathf.Sin(0.125f * Mathf.PI));
		endPos[2].Set(-radius * Mathf.Cos(0.250f * Mathf.PI), -radius * Mathf.Sin(0.250f * Mathf.PI));
		endPos[3].Set(-radius * Mathf.Cos(0.375f * Mathf.PI), -radius * Mathf.Sin(0.375f * Mathf.PI));
		endPos[4].Set(0, -radius);
		endPos[5].Set(radius * Mathf.Cos(0.375f * Mathf.PI), radius * Mathf.Sin(0.375f * Mathf.PI));
		endPos[6].Set(radius * Mathf.Cos(0.250f * Mathf.PI), radius * Mathf.Sin(0.250f * Mathf.PI));
		endPos[7].Set(radius * Mathf.Cos(0.125f * Mathf.PI), radius * Mathf.Sin(0.125f * Mathf.PI));
		endPos[8].Set(radius, 0);
	}

	void readHitmap () {		
		hitmapFile = Resources.Load(hitmapName) as TextAsset;
		hitmapData = JsonUtility.FromJson<hitmapType>(hitmapFile.text);
		hitmapData.lane.Sort(compareByStarttime);
		int i = 0;
		foreach (HitmapNotes note in hitmapData.lane) {	
			if (note.longnote && !note.parallel)
				note.type = 1;
			else if (!note.longnote && !note.parallel)
				note.type = 0;
			else if (!note.longnote && note.parallel)
				note.type = 2;
			else if (note.longnote && note.parallel)
				note.type = 3;
			notes[i] = note;
			i++;
		}
	}

	void playAudio () {
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = Resources.Load(hitmapData.audiofile) as AudioClip;
		audioSource.Play();
	}

	#endregion

	#region CreateNotes

	void notesCreate () {
		gameTime = Time.fixedTime * 1000 + 128000 / hitmapData.speed;
		Debug.Log(gameTime);
		if (gameTime >= notes[p].starttime) {
			switch (notes[p].type) {
			case 0:
				if (notes[p + 1].starttime == notes[p].starttime) {
					notesCreate();
				}
				moveSingle(notes[p]);
				break;
			case 1:
				break;
			}
			p++;
		}
	}

	public void moveSingle (HitmapNotes note) {
		SingleNoteController controller = getSingleController();
		if (note.parallel) {
			controller.Line.SetActive(true);
		} else {
			controller.Line.SetActive(false);
		}

		controller.GetComponent<RectTransform>().anchoredPosition = startPos;
		controller.gameObject.SetActive(true);
		controller.moveTo(endPos[note.lane]);
		destroyPooledNote(controller.gameObject);
	}

	#endregion
}
