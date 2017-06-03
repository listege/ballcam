using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instruction_ISeeYou : MonoBehaviour
{
	static public Instruction_ISeeYou Instance = null;

	[SerializeField]
	protected Image _image = null;

	void Awake()
	{
		Instance = this;
		_image.color = Color.white;
		gameObject.SetActive (false);
	}

	public void Show()
	{
		gameObject.SetActive (true);
	}

	public void Hide()
	{
		gameObject.SetActive (false);
	}
}
