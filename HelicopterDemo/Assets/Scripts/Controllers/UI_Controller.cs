using UnityEngine;
using static Types;
using static ViewPortController;

public class UI_Controller : MonoBehaviour
{
    [Header("Weapon UI (arrays order: L2 L1 C R1 R2)")]
    [SerializeField] private SingleProgressUI[] weaponsUI_player1;
    [SerializeField] private SingleProgressUI[] weaponsUI_player2;
    [SerializeField] private GameObject weaponGroupUI_player1;
    [SerializeField] private GameObject weaponGroupUI_player2;

    [SerializeField] private SingleProgressUI hpUI_player1;
    [SerializeField] private SingleProgressUI hpUI_player2;

    [SerializeField] private SingleProgressUI cargoUI_player1;
    [SerializeField] private SingleProgressUI cargoUI_player2;

    [SerializeField] private Color minigunColor;
    [SerializeField] private Color unguidMisColor;
    [SerializeField] private Color guidMisColor;
    [SerializeField] private Color healthColor;
    [SerializeField] private Color cargoColor;

    [SerializeField] private Sprite minigunIcon;
    [SerializeField] private Sprite unguidMisIcon;
    [SerializeField] private Sprite guidMisIcon;
    [SerializeField] private Sprite blueKeyIcon;
    [SerializeField] private Sprite purpleKeyIcon;
    [SerializeField] private Sprite redKeyIcon;
    [SerializeField] private Sprite yellowKeyIcon;
    [SerializeField] private Sprite blueFlagIcon;
    [SerializeField] private Sprite redFlagIcon;
    [SerializeField] private Sprite bombIcon;

    [SerializeField] private float speed = 10f;

    public static UI_Controller Singleton { get; private set; }
    public SingleProgressUI HP_UI_player1 => hpUI_player1;
    public SingleProgressUI HP_UI_player2 => hpUI_player2;
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }

    private RectTransform rectWeapon_player1, rectWeapon_player2;
    private RectTransform rectHP_player1, rectHP_player2;
    private RectTransform rectCargo_player1, rectCargo_player2;
    private PlayerWeaponController playerWeaponController;
    private ViewPortController viewPortController;
    private Vector2 uiTgtPosPlayer1, uiTgtPosPlayer2;

    private readonly Vector2 uiPosBottom = new Vector2(0f, 88f);
    private readonly Vector2 uiPosBottomLeftNear = new Vector2(-350f, 88f);
    private readonly Vector2 uiPosBottomRightNear = new Vector2(350f, 88f);
    private readonly Vector2 uiPosBottomLeftMiddle = new Vector2(-480f, 88f);
    private readonly Vector2 uiPosBottomRightMiddle = new Vector2(480f, 88f);
    private readonly Vector2 uiPosBottomLeftFar = new Vector2(-610f, 88f);
    private readonly Vector2 uiPosBottomRightFar = new Vector2(610f, 88f);
    private readonly Vector2 uiPosTopLeft = new Vector2(100f, -100f);
    private readonly Vector2 uiPosTopRight = new Vector2(-100f, -100f);

    private readonly Vector2 uiAnchorTop = new Vector2(0.5f, 1f);
    private readonly Vector2 uiAnchorTopLeft = new Vector2(0f, 1f);
    private readonly Vector2 uiAnchorTopRight = new Vector2(1f, 1f);

    private readonly float uiElementDelta = 125f;

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

        cargoUI_player1.gameObject.SetActive(false);
        cargoUI_player2.gameObject.SetActive(false);
        cargoUI_player1.FillColor = cargoUI_player2.FillColor = cargoColor;

        rectCargo_player1 = cargoUI_player1.GetComponent<RectTransform>();
        rectCargo_player2 = cargoUI_player2.GetComponent<RectTransform>();
    }

    private void Start()
    {
        viewPortController = ViewPortController.singleton;
        playerWeaponController = PlayerWeaponController.Instance;
        playerWeaponController.SortAllSlots();

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

    private void Update()
    {
        rectWeapon_player1.anchoredPosition = Vector2.MoveTowards(rectWeapon_player1.anchoredPosition, uiTgtPosPlayer1, speed * Time.deltaTime);
        rectWeapon_player2.anchoredPosition = Vector2.MoveTowards(rectWeapon_player2.anchoredPosition, uiTgtPosPlayer2, speed * Time.deltaTime);
    }

    public void MoveUiGroups()
    {
        if (viewPortController.SizeCamera1 == CameraSize.Full)
        {
            weaponGroupUI_player1.SetActive(true);
            weaponGroupUI_player2.SetActive(false);
            if (Player1 && Player1.Aiming)
                uiTgtPosPlayer1 = uiPosBottomRightFar + new Vector2(playerWeaponController.SkipRightPlayer1, 0f) * uiElementDelta;
            else
                uiTgtPosPlayer1 = uiPosBottom;

            hpUI_player1.gameObject.SetActive(true);
            hpUI_player2.gameObject.SetActive(false);
            rectHP_player1.anchorMin = rectHP_player1.anchorMax = uiAnchorTopLeft;
            rectHP_player1.anchoredPosition = uiPosTopLeft;

            rectCargo_player1.anchorMin = rectCargo_player1.anchorMax = uiAnchorTopRight;
            rectCargo_player1.anchoredPosition = uiPosTopRight;
        }
        else if (viewPortController.SizeCamera2 == CameraSize.Full)
        {
            weaponGroupUI_player2.SetActive(true);
            weaponGroupUI_player1.SetActive(false);
            if (Player2 && Player2.Aiming)
                uiTgtPosPlayer2 = uiPosBottomLeftFar - new Vector2(playerWeaponController.SkipRightPlayer1, 0f) * uiElementDelta;
            else
                uiTgtPosPlayer2 = uiPosBottom;

            hpUI_player1.gameObject.SetActive(false);
            hpUI_player2.gameObject.SetActive(true);
            rectHP_player2.anchorMin = rectHP_player2.anchorMax = uiAnchorTopRight;
            rectHP_player2.anchoredPosition = uiPosTopRight;

            rectCargo_player2.anchorMin = rectCargo_player2.anchorMax = uiAnchorTopLeft;
            rectCargo_player2.anchoredPosition = uiPosTopLeft;
        }
        else if (viewPortController.CameraConfig == CamerasConfig.player1_player2)
        {
            weaponGroupUI_player1.SetActive(true);
            weaponGroupUI_player2.SetActive(true);
            if (Player1 && Player1.Aiming)
                uiTgtPosPlayer1 = uiPosBottomLeftNear + new Vector2(playerWeaponController.SkipRightPlayer1, 0f) * uiElementDelta;
            else
                uiTgtPosPlayer1 = uiPosBottomLeftMiddle;
            if (Player2 && Player2.Aiming)
                uiTgtPosPlayer2 = uiPosBottomRightNear - new Vector2(playerWeaponController.SkipRightPlayer1, 0f) * uiElementDelta;
            else
                uiTgtPosPlayer2 = uiPosBottomRightMiddle;

            hpUI_player1.gameObject.SetActive(true);
            hpUI_player2.gameObject.SetActive(true);
            rectHP_player1.anchorMin = rectHP_player1.anchorMax = uiAnchorTopLeft;
            rectHP_player1.anchoredPosition = uiPosTopLeft;
            rectHP_player2.anchorMin = rectHP_player2.anchorMax = uiAnchorTopRight;
            rectHP_player2.anchoredPosition = uiPosTopRight;

            rectCargo_player1.anchorMin = rectCargo_player1.anchorMax = rectCargo_player2.anchorMin = rectCargo_player2.anchorMax = uiAnchorTop;
            rectCargo_player1.anchoredPosition = uiPosTopRight;
            rectCargo_player2.anchoredPosition = uiPosTopLeft;
        }
        else if (viewPortController.CameraConfig == CamerasConfig.player2_player1)
        {
            weaponGroupUI_player1.SetActive(true);
            weaponGroupUI_player2.SetActive(true);

            hpUI_player1.gameObject.SetActive(true);
            hpUI_player2.gameObject.SetActive(true);
            rectHP_player1.anchorMin = rectHP_player1.anchorMax = uiAnchorTopRight;
            rectHP_player1.anchoredPosition = uiPosTopRight;
            rectHP_player2.anchorMin = rectHP_player2.anchorMax = uiAnchorTopLeft;
            rectHP_player2.anchoredPosition = uiPosTopLeft;

            rectCargo_player1.anchorMin = rectCargo_player1.anchorMax = rectCargo_player2.anchorMin = rectCargo_player2.anchorMax = uiAnchorTop;
            rectCargo_player1.anchoredPosition = uiPosTopLeft;
            rectCargo_player2.anchoredPosition = uiPosTopRight;
        }
    }

    public void SetCargoUI(Players playerIndex, PickableUpType cargoType)
    {
        SingleProgressUI cargoUI = playerIndex == Players.Player1 ? cargoUI_player1 : cargoUI_player2;
        cargoUI.gameObject.SetActive(true);
        switch (cargoType)
        {
            case PickableUpType.BlueKey: cargoUI.IconSprite = blueKeyIcon; break;
            case PickableUpType.PurpleKey: cargoUI.IconSprite = purpleKeyIcon; break;
            case PickableUpType.RedKey: cargoUI.IconSprite = redKeyIcon; break;
            case PickableUpType.YellowKey: cargoUI.IconSprite = yellowKeyIcon; break;

            case PickableUpType.BlueFlag: cargoUI.IconSprite = blueFlagIcon; break;
            case PickableUpType.RedFlag: cargoUI.IconSprite = redFlagIcon; break;

            case PickableUpType.Bomb: cargoUI.IconSprite = bombIcon; break;
        }
    }

    public void UnsetCargoUI(Players playerIndex)
    {
        SingleProgressUI cargoUI = playerIndex == Players.Player1 ? cargoUI_player1 : cargoUI_player2;
        cargoUI.gameObject.SetActive(false);
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