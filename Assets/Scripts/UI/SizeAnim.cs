using UnityEngine;
using System.Collections;

public class SizeAnim : MonoBehaviour 
{
	private Coroutine sizeCoroutine = null;
	[SerializeField] private Vector3 minSize = Vector3.zero, maxSize = Vector3.zero;
	[SerializeField] private float sizeChangeTime = 0.0f;

	public void StartSizeCoroutine()
	{
		transform.localScale = minSize;
		sizeCoroutine = StartCoroutine (SizeCoroutine ());
	}

	public void StopSizeCoroutine()
	{
		StopCoroutine (sizeCoroutine);
	}

	private IEnumerator SizeCoroutine()
	{
		bool isGrowing = true;

		while (true)
		{
			Vector3 startingSize = Vector3.zero;
			Vector3 targetSize = Vector3.zero;

			float currentLerpTime = 0.0f;

			if (isGrowing)
			{
				startingSize = minSize;
				targetSize = maxSize;

				while (currentLerpTime < sizeChangeTime)
				{
					currentLerpTime += Time.deltaTime;
					float t = currentLerpTime / sizeChangeTime;
					t = Mathf.Sin (t * Mathf.PI * 0.5f);
					Vector3 target = Vector3.Lerp (startingSize, targetSize, t);
					transform.localScale = target;
					yield return null;
				}

				isGrowing = false;
			}
			else
			{
				startingSize = maxSize;
				targetSize = minSize;

				while (currentLerpTime < sizeChangeTime)
				{
					currentLerpTime += Time.deltaTime;
					float t = currentLerpTime / sizeChangeTime;
					t = 1- Mathf.Cos (t * Mathf.PI * 0.5f);
					Vector3 target = Vector3.Lerp (startingSize, targetSize, t);
					transform.localScale = target;
					yield return null;
				}
				isGrowing = true;
			}

			transform.localScale = targetSize;
			yield return null;
		}
	}
}
