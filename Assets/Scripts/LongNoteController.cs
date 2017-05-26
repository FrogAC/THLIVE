using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteController : MonoBehaviour {
	public GameObject Line;

	public void moveNote (HitmapNotes note) {
		if (note.parallel) {
			Line.SetActive(true);
		} else {
			Line.SetActive(false);
		}
	}
}
