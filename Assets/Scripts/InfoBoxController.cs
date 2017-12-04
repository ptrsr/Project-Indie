using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InfoBoxController : MonoBehaviour {

    public Text Header, InfoText;
	public void UpdateInfo()
    {
        Header.text = transform.GetChild(0).GetComponent<Text>().text;
        InfoText.text = transform.GetChild(2).GetComponent<Text>().text;
    }
}
