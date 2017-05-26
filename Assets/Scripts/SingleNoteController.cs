using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingleNoteController : MonoBehaviour {
	public GameObject Line;

	public void moveTo (Vector2 end) {
		gameObject.GetComponent<RectTransform>().Translate((Vector3)end * Time.fixedDeltaTime);
	}

}
