using UnityEngine;

public class FPS : MonoBehaviour {

	float deltaTime = 0.0f;

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperRight;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;

		string text;
		if (Time.timeScale == 1)
			text = string.Format("{1:0.} FPS", msec, fps);
		else
			text = ("60 FPS");
		
		GUI.Label(rect, text, style);
	}
}
