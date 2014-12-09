using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResultView : MonoBehaviour 
{
	public Text ResultText;
	public void SetTime(float time)
	{
		ResultText.text = Mathf.FloorToInt(time * 100f).ToString();
	}
}
