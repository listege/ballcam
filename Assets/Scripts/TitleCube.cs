using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCube : MonoBehaviour
{
	public Renderer cubeRenderer = null;
	public Renderer faceRenderer = null;
	public Renderer toungeRenderer = null;

	void Start ()
	{
		Color backColor = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
		while((backColor.r > 0.6f && backColor.g > 0.6f && backColor.b > 0.6f) ||
			(backColor.r < 0.9f && backColor.g < 0.9f && backColor.b < 0.9f))
			backColor = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
		cubeRenderer.material.SetColor ("_BottomColor", backColor);
		backColor.r *= 0.2f;
		backColor.g *= 0.2f;
		backColor.b *= 0.2f;
		cubeRenderer.material.SetColor ("_TopColor", backColor);
		StartCoroutine ("Corotuine_ToungueDance");
	}

	IEnumerator Corotuine_ToungueDance()
	{
		Transform childTransform = transform.FindChild ("ball_prefab");
		childTransform = childTransform.FindChild("toungue");
		childTransform = childTransform.FindChild ("Armature");
		childTransform = childTransform.FindChild ("Bone.001");
		Rigidbody body1 = childTransform.GetComponent<Rigidbody> ();
		childTransform = childTransform.parent.FindChild ("Bone.005");
		Rigidbody body2 = childTransform.GetComponent<Rigidbody> ();
		float maxSpeed = 0;
		while (true)
		{
			{
				Vector3 forceDirection = Random.onUnitSphere;
				body1.AddForce (forceDirection * 400);
				if (body1.velocity.magnitude > 2f)
					body1.velocity = body1.velocity.normalized * 2f;
			}
			yield return new WaitForSeconds (Random.Range (0.6f, 0.8f));

			{
				Vector3 forceDirection = Random.onUnitSphere;
				body2.AddForce (forceDirection * 500);
				if (body2.velocity.magnitude > 2f)
					body2.velocity = body2.velocity.normalized * 2f;
			}

			yield return new WaitForSeconds (Random.Range (0.4f, 0.55f));
		}
	}
}
