using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour {

	public GameObject collisionExplosion;
	public Vector3 m_target;
	public float speed;

	void Update()
	{

		float step = speed * Time.deltaTime;
		if (m_target != null)
		{
			if (transform.position == m_target)
			{
				Explode();
				return;
			}
			transform.position = Vector3.MoveTowards(transform.position, m_target, step);
		}
	}

	public void setTarget(Vector3 target)
	{
		m_target = target;
	}

	public void Explode()
	{
		if (collisionExplosion != null)
		{
			GameObject explosion = (GameObject)Instantiate(collisionExplosion, transform.position, transform.rotation);
			Destroy(gameObject);
			Destroy(explosion, 1f);
		}
	}
}
