using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addImage : MonoBehaviour {

    public Sprite[] sprites;
    public GameObject imagePrototype;

	// Use this for initialization
	void Start () {
        Canvas canny = GetComponent<Canvas>();
        foreach (Sprite sprite in sprites)
        {
            GameObject imageGO = Instantiate(imagePrototype);
            imageGO.name = sprite.name;
            imageGO.transform.parent = gameObject.transform;
            UnityEngine.UI.Image image = imageGO.GetComponent<UnityEngine.UI.Image>();
            image.sprite = sprite;
            RectTransform trafo = imageGO.GetComponent<RectTransform>();
            trafo.localScale = Vector3.one;
            image.SetNativeSize();
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
