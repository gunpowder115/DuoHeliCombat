using UnityEngine;
using static Types;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private SingleProgressUI centralUI;
    [Header("Weapon UI (arrays order: L2 L1 C R1 R2)")]
    [SerializeField] private SingleProgressUI[] singleUI;

    [SerializeField] private Color minigunColor;
    [SerializeField] private Color unguidMisColor;
    [SerializeField] private Color guidMisColor;

    [SerializeField] private Sprite minigunIcon;
    [SerializeField] private Sprite unguidMisIcon;
    [SerializeField] private Sprite guidMisIcon;

    public static UI_Controller Singleton { get; private set; }
    public SingleProgressUI CentralUI => centralUI;

    private const float UI_GROUP_BOTTOM_Y = 88f;
    private const float UI_DELTA_X = 125f;

    private PlayerWeaponController playerWeaponController;

    private void Awake()
    {
        Singleton = this;

        for (int i = 0; i < singleUI.Length; i++)
        {
            singleUI[i].gameObject.SetActive(false);
        }
        centralUI.gameObject.SetActive(true);
    }

    private void Start()
    {
        playerWeaponController = PlayerWeaponController.Instance;
        playerWeaponController.SortSlots();

        for (int i = 0; i < playerWeaponController.WeaponTypesPlayer1.Length; i++)
        {
            if (playerWeaponController.WeaponTypesPlayer1[i] != WeaponType.None)
            {
                singleUI[i].gameObject.SetActive(true);
                singleUI[i].FillColor = SetUiFillColor(playerWeaponController.WeaponTypesPlayer1[i]);
                singleUI[i].IconSprite = SetUiSprite(playerWeaponController.WeaponTypesPlayer1[i]);

                playerWeaponController.LinkUiToWeapon(singleUI[i], i);
            }
        }

        centralUI.FillColor = minigunColor;
        centralUI.IconSprite = minigunIcon;
    }

    public void SetSlotUI(SlotType slot, WeaponType weaponType)
    {
        if (weaponType == WeaponType.None) return;
        SingleProgressUI singleProgressUI = singleUI[(int)slot];
        singleProgressUI.gameObject.SetActive(true);

        switch (weaponType)
        {
            case WeaponType.Minigun:
                singleProgressUI.FillColor = minigunColor;
                singleProgressUI.IconSprite = minigunIcon;
                break;
            case WeaponType.UnguidMissile:
                singleProgressUI.FillColor = unguidMisColor;
                singleProgressUI.IconSprite = unguidMisIcon;
                break;
            case WeaponType.GuidMissile:
                singleProgressUI.FillColor = guidMisColor;
                singleProgressUI.IconSprite = guidMisIcon;
                break;
        }
    }

    private Color SetUiFillColor(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Minigun:
                return minigunColor;
            case WeaponType.UnguidMissile:
                return unguidMisColor;
            case WeaponType.GuidMissile:
                return guidMisColor;
            default:
                return minigunColor;
        }
    }

    private Sprite SetUiSprite(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Minigun:
                return minigunIcon;
            case WeaponType.UnguidMissile:
                return unguidMisIcon;
            case WeaponType.GuidMissile:
                return guidMisIcon;
            default:
                return minigunIcon;
        }
    }
}