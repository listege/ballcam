using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinCylinder : MonoBehaviour
{
	public float spinSpeed = 1;
	protected List<GameObject> touchingObjects = new List<GameObject> ();

	void FixedUpdate()
	{
		transform.Rotate(0, spinSpeed * Time.fixedDeltaTime, 0);
		float topY = transform.localPosition.y + transform.localScale.y;
		Vector2 center = new Vector2 (transform.localPosition.x, transform.localPosition.z);
		float radius = transform.localScale.z;
		foreach(GameObject ball in touchingObjects)
		{
			float ballBottomY = ball.transform.localPosition.y - ball.transform.localScale.y / 2;
			Vector2 ballCenter = new Vector2 (ball.transform.localPosition.x, ball.transform.localPosition.z);
			if (Vector2.Distance (center, ballCenter) <= radius && Mathf.Abs (ballBottomY - topY) < 0.25f)
			{
				// 대충 계산해서 위에서있단 판단이서면 돌려
				Vector2 directionVector = ballCenter - center;
				directionVector = RotateRadians (directionVector, -spinSpeed * Time.fixedDeltaTime * Mathf.Deg2Rad);
				Vector3 newPosition = ball.transform.localPosition;
				newPosition.x = center.x + directionVector.x;
				newPosition.z = center.y + directionVector.y;
				ball.transform.localPosition = newPosition;
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision != null && collision.gameObject.layer == 10)
			touchingObjects.Add (collision.gameObject);
	}

	void OnCollisionExit(Collision collision)
	{
		if(collision != null && collision.gameObject.layer == 10)
			touchingObjects.Remove (collision.gameObject);
	}

	public Vector2 RotateRadians(Vector2 v, float radians)
	{
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);

		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}
}
