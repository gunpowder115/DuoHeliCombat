using UnityEngine;
using static Types;

public class UI_Controller : MonoBehaviour
{
    [Header("Weapon UI (arrays order: L2 L1 C R1 R2)")]
    [SerializeField] private SingleProgressUI[] weaponsUI_player1;
    [SerializeField] private SingleProgressUI[] weaponsUI_player2;
    [SerializeField] private GameObject weaponGroupUI_player1;
    [SerializeField] private GameObject weaponGroupUI_player2;

    [SerializeField] private SingleProgressUI hpUI_player1;
    [SerializeField] private SingleProgressUI hpUI_player2;

    [SerializeField] private Color minigunColor;
    [SerializeField] private Color unguidMisColor;
    [SerializeField] private Color guidMisColor;
    [SerializeField] private Color healthColor;

    [SerializeField] private Sprite minigunIcon;
    [SerializeField] private Sprite unguidMisIcon;
    [SerializeField] private Sprite guidMisIcon;

    public static UI_Controller Singleton { get; private set; }
    public SingleProgressUI HP_UI_player1 => hpUI_player1;
    public SingleProgressUI HP_UI_player2 => hpUI_player2;

    private RectTransform rectWeapon_player1, rectWeapon_player2;
    private RectTransform rectHP_player1, rectHP_player2;
    private PlayerWeaponController playerWeaponController;
    private ViewPortController viewPortController;

    private readonly Vector2 uiPosBottom = new Vector2(0f, 88f);
    private readonly Vector2 uiPosBottomLeft = new Vector2(-480f, 88f);
    private readonly Vector2 uiPosBottomRight = new Vector2(480f, 88f);
    private readonly Vector2 uiPosTopLeft = new Vector2(100f, -100f);
    private readonly Vector2 uiPosTopRight = new Vector2(-100f, -100f);
    private readonly Vector2 uiAnchorTopLeft = new Vector2(0f, 1f);
    private readonly Vector2 uiAnchorTopRight = new Vector2(1f, 1f);

    private void Awake()
    {
        Singleton = this;

        for (int i = 0; i < weaponsUI_player1.Length; i++)
        {
            weaponsUI_player1[i].gameObject.SetActive(false);
            weaponsUI_player2[i].gameObject.SetActive(false);
        }
        hpUI_player1.gameObject.SetActive(false);
        hpUI_player2.gameObject.SetActive(false);
        hpUI_player1.FillColor = hpUI_player2.FillColor = healthColor;

        rectWeapon_player1 = weaponGroupUI_player1.GetComponent<RectTransform>();
        rectWeapon_player2 = weaponGroupUI_player2.GetComponent<RectTransform>();
        rectHP_player1 = hpUI_player1.GetComponent<RectTransform>();
        rectHP_player2 = hpUI_player2.GetComponent<RectTransform>();
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
                weaponsUI_player1[i].gameObject.SetActive(true);
                weaponsUI_player1[i].FillColor = SetUiFillColor(playerWeaponController.WeaponTypesPlayer1[i]);
                weaponsUI_player1[i].IconSprite = SetUiSprite(playerWeaponController.WeaponTypesPlayer1[i]);

                playerWeaponController.LinkUiToWeapon(playerWeaponController.WeaponTypesPlayer1, playerWeaponController.WeaponsPlayer1, weaponsUI_player1[i], i);
            }
        }
        for (int i = 0; i < playerWeaponController.WeaponTypesPlayer2.Length; i++)
        {
            if (playerWeaponController.WeaponTypesPlayer2[i] != WeaponType.None)
            {
                weaponsUI_player2[i].gameObject.SetActive(true);
                weaponsUI_player2[i].FillColor = SetUiFillColor(playerWeaponController.WeaponTypesPlayer2[i]);
                weaponsUI_player2[i].IconSprite = SetUiSprite(playerWeaponController.WeaponTypesPlayer2[i]);

                playerWeaponController.LinkUiToWeapon(playerWeaponController.WeaponTypesPlayer2, playerWeaponController.WeaponsPlayer2, weaponsUI_player2[i], i);
            }
        }
    }

    public void MoveUiGroups()
    {
        if (viewPortController.SizeCamera1 == ViewPortController.CameraSize.Full)
        {
            weaponGroupUI_player1.SetActive(true);
            rectWeapon_player1.anchoredPosition = uiPosBottom;
            weaponGroupUI_player2.SetActive(false);

            hpUI_player1.gameObject.SetActive(true);
            hpUI_player2.gameObject.SetActive(false);
            rectHP_player1.anchorMin = rectHP_player1.anchorMax = uiAnchorTopLeft;
            rectHP_player1.anchoredPosition = uiPosTopLeft;
        }
        else if (viewPortController.SizeCamera2 == ViewPortController.CameraSize.Full)
        {
            weaponGroupUI_player2.SetActive(true);
            rectWeapon_player2.anchoredPosition = uiPosBottom;
            weaponGroupUI_player1.SetActive(false);

            hpUI_player1.gameObject.SetActive(false);
            hpUI_player2.gameObject.SetActive(true);
            rectHP_player2.anchorMin = rectHP_player2.anchorMax = uiAnchorTopRight;
            rectHP_player2.anchoredPosition = uiPosTopRight;
        }
        else if (viewPortController.CameraConfig == ViewPortController.CamerasConfig.player1_player2)
        {
            weaponGroupUI_player1.SetActive(true);
            rectWeapon_player1.anchoredPosition = uiPosBottomLeft;
            weaponGroupUI_player2.SetActive(true);
            rectWeapon_player2.anchoredPosition = uiPosBottomRight;

            hpUI_player1.gameObject.SetActive(true);
            hpUI_player2.gameObject.SetActive(true);
            rectHP_player1.anchorMin = rectHP_player1.anchorMax = uiAnchorTopLeft;
            rectHP_player1.anchoredPosition = uiPosTopLeft;
            rectHP_player2.anchorMin = rectHP_player2.anchorMax = uiAnchorTopRight;
            rectHP_player2.anchoredPosition = uiPosTopRight;
        }
        else if (viewPortController.CameraConfig == ViewPortController.CamerasConfig.player2_player1)
        {
            weaponGroupUI_player1.SetActive(true);
            rectWeapon_player1.anchoredPosition = uiPosBottomRight;
            weaponGroupUI_player2.SetActive(true);
            rectWeapon_player2.anchoredPosition = uiPosBottomLeft;

            hpUI_player1.gameObject.SetActive(true);
            hpUI_player2.gameObject.SetActive(true);
            rectHP_player1.anchorMin = rectHP_player1.anchorMax = uiAnchorTopRight;
            rectHP_player1.anchoredPosition = uiPosTopRight;
            rectHP_player2.anchorMin = rectHP_player2.anchorMax = uiAnchorTopLeft;
            rectHP_player2.anchoredPosition = uiPosTopLeft;
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