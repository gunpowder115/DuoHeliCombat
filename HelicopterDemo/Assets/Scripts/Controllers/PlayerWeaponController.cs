using System;
using UnityEngine;
using static Types;

public class PlayerWeaponController
{
    public WeaponType[] WeaponTypesPlayer1 { get; private set; }
    public WeaponType[] WeaponTypesPlayer2 { get; private set; }
    public WeaponType[] WeaponTypesAimingPlayer1 { get; private set; }
    public WeaponType[] WeaponTypesAimingPlayer2 { get; private set; }
    public GameObject[] WeaponsPlayer1 { get; private set; }
    public GameObject[] WeaponsPlayer2 { get; private set; }
    public GameObject[] WeaponsAimingPlayer1 { get; private set; }
    public GameObject[] WeaponsAimingPlayer2 { get; private set; }
    public int SkipLeftPlayer1 => skipLeftPlayer1;
    public int SkipRightPlayer1 => skipRightPlayer1;
    public int SkipLeftPlayer2 => skipLeftPlayer2;
    public int SkipRightPlayer2 => skipRightPlayer2;

    private static PlayerWeaponController instance;
    private int skipLeftPlayer1, skipRightPlayer1, skipLeftPlayer2, skipRightPlayer2;

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

        WeaponTypesAimingPlayer1 = new WeaponType[WEAPON_COUNT];
        WeaponTypesAimingPlayer2 = new WeaponType[WEAPON_COUNT];
        WeaponsAimingPlayer1 = new GameObject[WEAPON_COUNT];
        WeaponsAimingPlayer2 = new GameObject[WEAPON_COUNT];
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

    public void SortAllSlots()
    {
        SortSlots(WeaponTypesPlayer1, WeaponsPlayer1, 0, 1);
        SortSlots(WeaponTypesPlayer1, WeaponsPlayer1, 4, 3);

        SortSlots(WeaponTypesPlayer2, WeaponsPlayer2, 0, 1);
        SortSlots(WeaponTypesPlayer2, WeaponsPlayer2, 4, 3);

        SortSlotsForAiming(WeaponTypesPlayer1, WeaponsPlayer1, WeaponTypesAimingPlayer1, WeaponsAimingPlayer1, out skipLeftPlayer1, out skipRightPlayer1);
        SortSlotsForAiming(WeaponTypesPlayer2, WeaponsPlayer2, WeaponTypesAimingPlayer2, WeaponsAimingPlayer2, out skipLeftPlayer2, out skipRightPlayer2);
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

    private void SortSlots(WeaponType[] weaponTypes, GameObject[] weaponsPlayer, int lastIndex, int preLastIndex)
    {
        if (weaponTypes[preLastIndex] == WeaponType.None && weaponTypes[lastIndex] != WeaponType.None)
        {
            weaponTypes[preLastIndex] = weaponTypes[lastIndex];
            weaponsPlayer[preLastIndex] = weaponsPlayer[lastIndex];
            weaponTypes[lastIndex] = WeaponType.None;
            weaponsPlayer[lastIndex] = null;
        }
    }

    private void SortSlotsForAiming(WeaponType[] weaponTypes, GameObject[] weapons, WeaponType[] weaponTypesAiming, GameObject[] weaponsAiming, out int skipLeftSlots, out int skipRightSlots)
    {
        Array.Copy(weaponTypes, weaponTypesAiming, weaponTypesAiming.Length);
        Array.Copy(weapons, weaponsAiming, weaponsAiming.Length);

        for (int i = 0; i < weaponTypesAiming.Length; i++)
        {
            if (weaponTypesAiming[i] == WeaponType.GuidMissile)
            {
                weaponTypesAiming[i] = WeaponType.None;
                weaponsAiming[i] = null;
            }
        }

        SortSlots(weaponTypesAiming, weaponsAiming, 0, 1);
        SortSlots(weaponTypesAiming, weaponsAiming, 4, 3);

        if (weaponTypesAiming[0] == WeaponType.None && weaponTypesAiming[1] == WeaponType.None)
            skipLeftSlots = 2;
        else if (weaponTypesAiming[0] == WeaponType.None && weaponTypesAiming[1] != WeaponType.None)
            skipLeftSlots = 1;
        else
            skipLeftSlots = 0;

        if (weaponTypesAiming[4] == WeaponType.None && weaponTypesAiming[3] == WeaponType.None)
            skipRightSlots = 2;
        else if (weaponTypesAiming[4] == WeaponType.None && weaponTypesAiming[3] != WeaponType.None)
            skipRightSlots = 1;
        else
            skipRightSlots = 0;
    }
}