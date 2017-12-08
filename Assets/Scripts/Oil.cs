using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour
{
    [SerializeField]
    private Shader _shader;

	[SerializeField] private Sprite [] oilSprites;

	void Start ()
	{
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		renderer.sprite = oilSprites [Random.Range (0, 3)];

		Material mat = new Material (_shader);

		mat.SetVector ("_TexelSize", new Vector2 (1.0f / Screen.width, 1.0f / Screen.height));

		//Ruud.Fix ();
       	//renderer.material = new Material (_shader);

		Destroy (gameObject, 5.5f);
	}
}
