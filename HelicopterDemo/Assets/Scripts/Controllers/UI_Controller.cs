using UnityEngine;
using static Types;

public class UI_Controller : MonoBehaviour
{
    [Header("Weapon UI (arrays order: L2 L1 C R1 R2)")]
    [SerializeField] private SingleProgressUI[] singleUI_player1;
    [SerializeField] private SingleProgressUI[] singleUI_player2;
    [SerializeField] private GameObject weaponGroupUI_player1;
    [SerializeField] private GameObject weaponGroupUI_player2;

    [SerializeField] private Color minigunColor;
    [SerializeField] private Color unguidMisColor;
    [SerializeField] private Color guidMisColor;

    [SerializeField] private Sprite minigunIcon;
    [SerializeField] private Sprite unguidMisIcon;
    [SerializeField] private Sprite guidMisIcon;

    public static UI_Controller Singleton { get; private set; }

    private PlayerWeaponController playerWeaponController;
    private ViewPortController viewPortController;

    private readonly Vector2 uiPosBottom = new Vector2(0f, 88f);
    private readonly Vector2 uiPosBottomLeft = new Vector2(-480f, 88f);
    private readonly Vector2 uiPosBottomRight = new Vector2(480f, 88f);

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
        viewPortController = ViewPortController.singleton;
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

    private void Update()
    {
        SetWeaponUiGroups();
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

    private void SetWeaponUiGroups()
    {
        if (viewPortController.SizeCamera1 == ViewPortController.CameraSize.Full)
        {
            weaponGroupUI_player1.SetActive(true);
            weaponGroupUI_player1.GetComponent<RectTransform>().anchoredPosition = uiPosBottom;

            weaponGroupUI_player2.SetActive(false);
        }
        else if (viewPortController.SizeCamera2 == ViewPortController.CameraSize.Full)
        {
            weaponGroupUI_player2.SetActive(true);
            weaponGroupUI_player2.GetComponent<RectTransform>().anchoredPosition = uiPosBottom;

            weaponGroupUI_player1.SetActive(false);
        }
        else if (viewPortController.CameraConfig == ViewPortController.CamerasConfig.player1_player2)
        {
            weaponGroupUI_player1.SetActive(true);
            weaponGroupUI_player1.GetComponent<RectTransform>().anchoredPosition = uiPosBottomLeft;

            weaponGroupUI_player2.SetActive(true);
            weaponGroupUI_player2.GetComponent<RectTransform>().anchoredPosition = uiPosBottomRight;
        }
        else if (viewPortController.CameraConfig == ViewPortController.CamerasConfig.player2_player1)
        {
            weaponGroupUI_player1.SetActive(true);
            weaponGroupUI_player1.GetComponent<RectTransform>().anchoredPosition = uiPosBottomRight;

            weaponGroupUI_player2.SetActive(true);
            weaponGroupUI_player2.GetComponent<RectTransform>().anchoredPosition = uiPosBottomLeft;
        }
    }
}