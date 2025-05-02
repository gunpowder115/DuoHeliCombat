using UnityEngine;
using static Types;

public class UI_Controller : MonoBehaviour
{
    [Header("Weapon UI (arrays order: L2 L1 C R1 R2)")]
    [SerializeField] private SingleProgressUI[] singleUI_player1;
    [SerializeField] private SingleProgressUI[] singleUI_player2;

    [SerializeField] private Color minigunColor;
    [SerializeField] private Color unguidMisColor;
    [SerializeField] private Color guidMisColor;

    [SerializeField] private Sprite minigunIcon;
    [SerializeField] private Sprite unguidMisIcon;
    [SerializeField] private Sprite guidMisIcon;

    public static UI_Controller Singleton { get; private set; }

    private PlayerWeaponController playerWeaponController;

    private void Awake()
    {
        Singleton = this;

        for (int i = 0; i < singleUI_player1.Length; i++)
        {
            singleUI_player1[i].gameObject.SetActive(false);
            singleUI_player2[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        playerWeaponController = PlayerWeaponController.Instance;
        playerWeaponController.SortSlots();

        for (int i = 0; i < playerWeaponController.WeaponTypesPlayer1.Length; i++)
        {
            if (playerWeaponController.WeaponTypesPlayer1[i] != WeaponType.None)
            {
                singleUI_player1[i].gameObject.SetActive(true);
                singleUI_player1[i].FillColor = SetUiFillColor(playerWeaponController.WeaponTypesPlayer1[i]);
                singleUI_player1[i].IconSprite = SetUiSprite(playerWeaponController.WeaponTypesPlayer1[i]);

                playerWeaponController.LinkUiToWeapon(playerWeaponController.WeaponTypesPlayer1, playerWeaponController.WeaponsPlayer1, singleUI_player1[i], i);
            }
        }
        for (int i = 0; i < playerWeaponController.WeaponTypesPlayer2.Length; i++)
        {
            if (playerWeaponController.WeaponTypesPlayer2[i] != WeaponType.None)
            {
                singleUI_player2[i].gameObject.SetActive(true);
                singleUI_player2[i].FillColor = SetUiFillColor(playerWeaponController.WeaponTypesPlayer2[i]);
                singleUI_player2[i].IconSprite = SetUiSprite(playerWeaponController.WeaponTypesPlayer2[i]);

                playerWeaponController.LinkUiToWeapon(playerWeaponController.WeaponTypesPlayer2, playerWeaponController.WeaponsPlayer2, singleUI_player2[i], i);
            }
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