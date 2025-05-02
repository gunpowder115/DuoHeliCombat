using UnityEngine;
using static Types;

public class PlayerWeaponController
{
    public WeaponType[] WeaponTypesPlayer1 { get; private set; }
    public WeaponType[] WeaponTypesPlayer2 { get; private set; }
    public GameObject[] WeaponsPlayer1 { get; private set; }
    public GameObject[] WeaponsPlayer2 { get; private set; }

    private static PlayerWeaponController instance;

    private const int WEAPON_COUNT = 5;

    public static PlayerWeaponController Instance
    {
        get
        {
            if (instance == null)
                instance = new PlayerWeaponController();
            return instance;
        }
    }

    private PlayerWeaponController()
    {
        WeaponTypesPlayer1 = new WeaponType[WEAPON_COUNT];
        WeaponTypesPlayer2 = new WeaponType[WEAPON_COUNT];
        WeaponsPlayer1 = new GameObject[WEAPON_COUNT];
        WeaponsPlayer2 = new GameObject[WEAPON_COUNT];
    }

    public void SetWeapon(int playerIndex, WeaponType weaponType, SlotType slot, GameObject weaponObject)
    {
        if (playerIndex == 0)
        {
            WeaponTypesPlayer1[(int)slot] = weaponType;
            WeaponsPlayer1[(int)slot] = weaponObject;
        }
        else
        {
            WeaponTypesPlayer2[(int)slot] = weaponType;
            WeaponsPlayer2[(int)slot] = weaponObject;
        }
    }

    public void SortSlots()
    {
        SortLeftSlots(WeaponTypesPlayer1, WeaponsPlayer1);
        SortRightSlots(WeaponTypesPlayer1, WeaponsPlayer1);

        SortLeftSlots(WeaponTypesPlayer2, WeaponsPlayer2);
        SortRightSlots(WeaponTypesPlayer2, WeaponsPlayer2);
    }

    public void LinkUiToWeapon(WeaponType[] weaponTypes, GameObject[] weapons, SingleProgressUI uiSingle, int index)
    {
        switch(weaponTypes[index])
        {
            case WeaponType.Minigun:
                weapons[index].GetComponentInChildren<BarrelLauncher>().uiSingle = uiSingle;
                break;
            case WeaponType.UnguidMissile:
                weapons[index].GetComponent<UnguidMisSystem>().uiSingle = uiSingle;
                break;
            case WeaponType.GuidMissile:
                weapons[index].GetComponentInChildren<MissileLauncher>().uiSingle = uiSingle;
                break;
        }
    }

    private void SortLeftSlots(WeaponType[] weaponTypes, GameObject[] weaponsPlayer)
    {
        if (weaponTypes[1] == WeaponType.None && weaponTypes[0] != WeaponType.None)
        {
            weaponTypes[1] = weaponTypes[0];
            weaponsPlayer[1] = weaponsPlayer[0];
            weaponTypes[0] = WeaponType.None;
            weaponsPlayer[0] = null;
        }
    }

    private void SortRightSlots(WeaponType[] weaponTypes, GameObject[] weaponsPlayer)
    {
        int lastIndex = weaponTypes.Length - 1;
        if (weaponTypes[lastIndex - 1] == WeaponType.None && weaponTypes[lastIndex] != WeaponType.None)
        {
            weaponTypes[lastIndex - 1] = weaponTypes[lastIndex];
            weaponsPlayer[lastIndex - 1] = weaponsPlayer[lastIndex];
            weaponTypes[lastIndex] = WeaponType.None;
            weaponsPlayer[lastIndex] = null;
        }
    }
}