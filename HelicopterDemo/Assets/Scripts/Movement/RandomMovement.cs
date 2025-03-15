using UnityEngine;

public class RandomMovement : MonoBehaviour
{
	[SerializeField] private float maxRandInput = 1f;
	[SerializeField] private float minMoveTime = 0.5f;
	[SerializeField] private float maxMoveTime = 1f;

	private float currTime, tgtTime;
	private Vector3 randomInput;

	private void Start()
	{
		randomInput = new Vector3(0f, 0f, 0f);
	}

	public Vector3 GetRandomInput()
	{
		if (currTime > tgtTime)
		{
			randomInput.x = Random.Range(-maxRandInput, maxRandInput);
			randomInput.z = Random.Range(-maxRandInput, maxRandInput);
			currTime = 0f;
			tgtTime = Random.Range(minMoveTime, maxMoveTime);
		}
		else
			currTime += Time.deltaTime;
		return randomInput;
	}

}
