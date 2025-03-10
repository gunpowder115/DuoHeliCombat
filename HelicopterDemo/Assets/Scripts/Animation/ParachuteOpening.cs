using UnityEngine;

public class ParachuteOpening : MonoBehaviour
{
    [SerializeField] private float openingTime = 0.5f;
    [SerializeField] private float openingSpeed = 1f;
    [SerializeField] private float beginScale = 0.2f;

	private float currTime, currScale;

	void Start()
	{
		currScale = beginScale;
	}

	void Update()
	{
		if (currTime > openingTime)
		{
			currScale += openingSpeed * Time.deltaTime;
			if (currScale > 1f) currScale = 1f;
		}
		else
			currTime += Time.deltaTime;

		transform.localScale = new Vector3(currScale, 1f, currScale);
	}
}
