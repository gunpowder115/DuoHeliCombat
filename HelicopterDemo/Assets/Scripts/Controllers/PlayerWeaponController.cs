using UnityEngine;
using static Types;

public class PlayerWeaponController
{
    public WeaponType[] WeaponTypesPlayer1 { get; private set; }
    public WeaponType[] WeaponTypesPlayer2 { get; private set; }
    public GameObject[] WeaponsPlayer1 { get; private set; }
    public GameObject[] WeaponsPlayer2 { get; private set; }

    private static PlayerWeaponController instance;

    private const int WEAPON_COUNT = 4;

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
        if (WeaponTypesPlayer1[1] == WeaponType.None && WeaponTypesPlayer1[0] != WeaponType.None)
        {
            WeaponTypesPlayer1[1] = WeaponTypesPlayer1[0];
            WeaponsPlayer1[1] = WeaponsPlayer1[0];
            WeaponTypesPlayer1[0] = WeaponType.None;
            WeaponsPlayer1[0] = null;
        }
        if (WeaponTypesPlayer1[2] == WeaponType.None && WeaponTypesPlayer1[3] != WeaponType.None)
        {
            WeaponTypesPlayer1[2] = WeaponTypesPlayer1[3];
            WeaponsPlayer1[2] = WeaponsPlayer1[3];
            WeaponTypesPlayer1[3] = WeaponType.None;
            WeaponsPlayer1[3] = null;
        }

        if (WeaponTypesPlayer2[1] == WeaponType.None && WeaponTypesPlayer2[0] != WeaponType.None)
        {
            WeaponTypesPlayer2[1] = WeaponTypesPlayer2[0];
            WeaponsPlayer2[1] = WeaponsPlayer2[0];
            WeaponTypesPlayer2[0] = WeaponType.None;
            WeaponsPlayer2[0] = null;
        }
        if (WeaponTypesPlayer2[2] == WeaponType.None && WeaponTypesPlayer2[3] != WeaponType.None)
        {
            WeaponTypesPlayer2[2] = WeaponTypesPlayer2[3];
            WeaponsPlayer2[2] = WeaponsPlayer2[3];
            WeaponTypesPlayer2[3] = WeaponType.None;
            WeaponsPlayer2[3] = null;
        }
    }

    public void LinkUiToWeapon(SingleProgressUI uiSingle, int index)
    {
        switch(WeaponTypesPlayer1[index])
        {
            case WeaponType.Minigun:
                WeaponsPlayer1[index].GetComponent<BarrelLauncher>().uiSingle = uiSingle;
                break;
            case WeaponType.UnguidMissile:
                WeaponsPlayer1[index].GetComponent<UnguidMisSystem>().uiSingle = uiSingle;
                break;
            case WeaponType.GuidMissile:
                WeaponsPlayer1[index].GetComponentInChildren<MissileLauncher>().uiSingle = uiSingle;
                break;
        }
    }
}