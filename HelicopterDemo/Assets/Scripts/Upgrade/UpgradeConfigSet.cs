using System;
using UnityEngine;
using static Types;

public class UpgradeConfigSet : MonoBehaviour
{
    [SerializeField] private bool randomConfig = false;

    [SerializeField] private WingState wingStateLeft = WingState.None;
    [SerializeField] private WeaponType L1 = WeaponType.None;
    [SerializeField] private WeaponType L2 = WeaponType.None;
    [SerializeField] private WingState wingStateRight = WingState.None;
    [SerializeField] private WeaponType R1 = WeaponType.None;
    [SerializeField] private WeaponType R2 = WeaponType.None;

    [SerializeField] private GameObject leftShortWing;
    [SerializeField] private GameObject rightShortWing;
    [SerializeField] private GameObject leftLongWing;
    [SerializeField] private GameObject rightLongWing;
    [SerializeField] private GameObject[] weaponPrefabs; //according to enum WeaponType

    private const float X_R1 = 0.74f;
    private const float X_R2 = 1.243f;
    private const float X_L1 = -X_R1;
    private const float X_L2 = -X_R2;
    private const float Y = -0.307f;
    private const float Z = 0f;

    private void Awake()
    {
        if (randomConfig)
        {
            var wingStateArr = Enum.GetValues(typeof(WingState));
            wingStateLeft = (WingState)wingStateArr.GetValue(UnityEngine.Random.Range(0, wingStateArr.Length));
            wingStateRight = (WingState)wingStateArr.GetValue(UnityEngine.Random.Range(0, wingStateArr.Length));
            var weaponTypeArr = Enum.GetValues(typeof(WeaponType));
            L1 = (WeaponType)weaponTypeArr.GetValue(UnityEngine.Random.Range(0, weaponTypeArr.Length));
            L2 = (WeaponType)weaponTypeArr.GetValue(UnityEngine.Random.Range(0, weaponTypeArr.Length));
            R1 = (WeaponType)weaponTypeArr.GetValue(UnityEngine.Random.Range(0, weaponTypeArr.Length));
            R1 = (WeaponType)weaponTypeArr.GetValue(UnityEngine.Random.Range(0, weaponTypeArr.Length));
        }

        Transform parent = leftShortWing.transform.parent;
        Quaternion rotation = leftShortWing.transform.rotation;
        Vector3 L1_pos = new Vector3(X_L1, Y, Z);
        Vector3 L2_pos = new Vector3(X_L2, Y, Z);
        Vector3 R1_pos = new Vector3(X_R1, Y, Z);
        Vector3 R2_pos = new Vector3(X_R2, Y, Z);
        Vector4 parentPos = new Vector4(parent.position.x, parent.position.y, parent.position.z, 0f);

        SetWingActivity(wingStateLeft, leftShortWing, leftLongWing);
        SetWingActivity(wingStateRight, rightShortWing, rightLongWing);

        if (wingStateLeft > WingState.None)
            Instantiate(weaponPrefabs[(int)L1], parentPos + parent.localToWorldMatrix * L1_pos, rotation, parent);

        if (wingStateLeft > WingState.Short)
            Instantiate(weaponPrefabs[(int)L2], parentPos + parent.localToWorldMatrix * L2_pos, rotation, parent);

        if (wingStateRight > WingState.None)
            Instantiate(weaponPrefabs[(int)R1], parentPos + parent.localToWorldMatrix * R1_pos, rotation, parent);

        if (wingStateRight > WingState.Short)
            Instantiate(weaponPrefabs[(int)R2], parentPos + parent.localToWorldMatrix * R2_pos, rotation, parent);
    }

    private void SetWingActivity(WingState state, GameObject shortWing, GameObject longWing)
    {
        shortWing.SetActive(false);
        longWing.SetActive(false);
        switch (state)
        {
            case WingState.Short:
                shortWing.SetActive(true);
                break;
            case WingState.Long:
                longWing.SetActive(true);
                break;
            default: break;
        }
    }
}
