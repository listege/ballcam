using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instruction_Switching : MonoBehaviour
{
	static public List<Instruction_Switching> Instances = new List<Instruction_Switching>();

	[SerializeField]
	protected Image _image = null;

	void Awake()
	{
		Instances.Add (this);
		_image.color = Color.white;
		gameObject.SetActive (false);
	}

	public void Show()
	{
		gameObject.SetActive (true);
		StartCoroutine ("Coroutine_Show");
	}

	IEnumerator Coroutine_Show()
	{
		yield return new WaitForSeconds (1.0f);

		gameObject.SetActive (false);
	}
}
