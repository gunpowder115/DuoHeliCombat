using System;
using UnityEngine;
using static Types;

public class UpgradeConfigSet : MonoBehaviour
{
    [SerializeField] private bool randomConfig = false;
    [SerializeField] private int playerIndex = 0;
    [SerializeField] private int playerHelicopterType = 0;

    [SerializeField] private WingState wingStateLeft = WingState.None;
    [SerializeField] private WeaponType L2 = WeaponType.None;
    [SerializeField] private WeaponType L1 = WeaponType.None;
    [SerializeField] private WeaponType C = WeaponType.Minigun;
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
    private const float Y_T1 = -0.307f;
    private const float Y_T2 = -0.2f;
    private const float Z_T1 = 0f;
    private const float Z_T2 = -0.228f;

    private const float X_C = 0f;
    private const float Y_C_T1 = -0.5f;
    private const float Y_C_T2 = -0.411f;
    private const float Z_C_T1 = 1.75f;
    private const float Z_C_T2 = 1.5f;

    private PlayerWeaponController playerWeaponController;

    private void Awake()
    {
        playerWeaponController = PlayerWeaponController.Instance;

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
        float y_pos = playerHelicopterType == 0 ? Y_T1 : Y_T2;
        float z_pos = playerHelicopterType == 0 ? Z_T1 : Z_T2;
        Vector3 L1_pos = new Vector3(X_L1, y_pos, z_pos);
        Vector3 L2_pos = new Vector3(X_L2, y_pos, z_pos);
        Vector3 R1_pos = new Vector3(X_R1, y_pos, z_pos);
        Vector3 R2_pos = new Vector3(X_R2, y_pos, z_pos);
        Vector3 C_pos = playerHelicopterType == 0 ? new Vector3(X_C, Y_C_T1, Z_C_T1) : new Vector3(X_C, Y_C_T2, Z_C_T2);
        Vector4 parentPos = new Vector4(parent.position.x, parent.position.y, parent.position.z, 0f);

        SetWingActivity(wingStateLeft, leftShortWing, leftLongWing);
        SetWingActivity(wingStateRight, rightShortWing, rightLongWing);

        if (wingStateLeft > WingState.None)
        {
            var obj = Instantiate(weaponPrefabs[(int)L1], parentPos + parent.localToWorldMatrix * L1_pos, rotation, parent);
            playerWeaponController.SetWeapon(playerIndex, L1, SlotType.L1, obj);
        }

        if (wingStateLeft > WingState.Short)
        {
            var obj = Instantiate(weaponPrefabs[(int)L2], parentPos + parent.localToWorldMatrix * L2_pos, rotation, parent);
            playerWeaponController.SetWeapon(playerIndex, L2, SlotType.L2, obj);
        }

        if (wingStateRight > WingState.None)
        {
            var obj = Instantiate(weaponPrefabs[(int)R1], parentPos + parent.localToWorldMatrix * R1_pos, rotation, parent);
            playerWeaponController.SetWeapon(playerIndex, R1, SlotType.R1, obj);
        }

        if (wingStateRight > WingState.Short)
        {
            var obj = Instantiate(weaponPrefabs[(int)R2], parentPos + parent.localToWorldMatrix * R2_pos, rotation, parent);
            playerWeaponController.SetWeapon(playerIndex, R2, SlotType.R2, obj);
        }

        //central minigun - there is always
        {
            var obj = Instantiate(weaponPrefabs[(int)C], parentPos + parent.localToWorldMatrix * C_pos, rotation, parent);
            playerWeaponController.SetWeapon(playerIndex, C, SlotType.C, obj);
        }
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
