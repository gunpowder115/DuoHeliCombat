using UnityEngine;

public class DestroyedWalker : MonoBehaviour
{
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;

    public void SetDestroyindParams(bool destroyingLeft, bool isLeftWeapon)
    {
        if (destroyingLeft) Destroy(leftLeg);
        else Destroy(rightLeg);
    }
}
