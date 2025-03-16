using UnityEngine;
using static Types;

public class PlayerUpgrade : MonoBehaviour
{
    [SerializeField] private float cameraSpeed = 10f;
    [SerializeField] private GameObject leftShortWing;
    [SerializeField] private GameObject rightShortWing;
    [SerializeField] private GameObject leftLongWing;
    [SerializeField] private GameObject rightLongWing;
    [SerializeField] private GameObject[] slotPositions;
    [SerializeField] private GameObject[] weaponPrefabs;
    [SerializeField] private GameObject[] slotCameraPos;
    [SerializeField] private GameObject[] wingCameraPos;
    [SerializeField] private GameObject mainCameraPos;
    [SerializeField] private GameObject mainCamera;

    private const int SLOTS_COUNT = 4;

    private int weaponCount;
    private int currSlotIndex, currWingIndex;
    private InputController inputController;
    private InputDeviceBase inputDevice;
    private WeaponSlot[] weaponSlots;
    private UpgradableWing[] wings;
    private Vector3 currCameraPos, tgtCameraPos;
    private Vector3 currCameraRot, tgtCameraRot;
    private UpgradeCameraState cameraState;

    private void Start()
    {
        currCameraPos = tgtCameraPos = mainCameraPos.transform.position;
        currCameraRot = tgtCameraRot = mainCameraPos.transform.rotation.eulerAngles;
        cameraState = UpgradeCameraState.Main;
        weaponCount = weaponPrefabs.Length;
        currSlotIndex = 3;
        currWingIndex = 1;

        weaponSlots = new WeaponSlot[SLOTS_COUNT];
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            GameObject[] weapons = new GameObject[weaponCount];
            for (int j = 0; j < weaponCount; j++)
            {
                weapons[j] = Instantiate(weaponPrefabs[j], slotPositions[i].transform.position, slotPositions[i].transform.rotation);
                weapons[j].SetActive(false);
            }
            weapons[0].SetActive(true);
            weaponSlots[i] = new WeaponSlot(weapons, false);
        }

        wings = new UpgradableWing[2];
        wings[0] = new UpgradableWing(weaponSlots[1], weaponSlots[0], leftLongWing, leftShortWing, WingState.None);
        wings[1] = new UpgradableWing(weaponSlots[2], weaponSlots[3], rightLongWing, rightShortWing, WingState.None);

        inputController = InputController.singleton;
        inputDevice = inputController.GetInputCommon;
        inputDevice.UpgradeUpEvent += Change;
        inputDevice.UpgradeDownEvent += Change;
        inputDevice.UpgradeLeftEvent += Swipe;
        inputDevice.UpgradeRightEvent += Swipe;
        inputDevice.UpgradeSelectEvent += Select;
        inputDevice.UpgradeCancelEvent += Cancel;
    }

    private void Update()
    {
        currCameraPos = Vector3.Lerp(currCameraPos, tgtCameraPos, cameraSpeed * Time.deltaTime);
        currCameraRot = Vector3.Lerp(currCameraRot, tgtCameraRot, cameraSpeed * Time.deltaTime);

        mainCamera.transform.position = currCameraPos;
        mainCamera.transform.rotation = Quaternion.Euler(currCameraRot);

        switch (cameraState)
        {
            case UpgradeCameraState.Main:
                tgtCameraPos = mainCameraPos.transform.position;
                tgtCameraRot = mainCameraPos.transform.rotation.eulerAngles;
                break;
            case UpgradeCameraState.Wing:
                tgtCameraPos = wingCameraPos[currWingIndex].transform.position;
                tgtCameraRot = wingCameraPos[currWingIndex].transform.rotation.eulerAngles;
                break;
            case UpgradeCameraState.Slot:
                tgtCameraPos = slotCameraPos[currSlotIndex].transform.position;
                tgtCameraRot = slotCameraPos[currSlotIndex].transform.rotation.eulerAngles;
                break;
        }
    }

    private void Change(int incr)
    {
        switch (cameraState)
        {
            case UpgradeCameraState.Wing:
                wings[currWingIndex].ChangeWingState(incr);
                break;
            case UpgradeCameraState.Slot:
                weaponSlots[currSlotIndex].ChangeWeapon(incr);
                break;
        }
    }

    private void Swipe(int incr)
    {
        switch (cameraState)
        {
            case UpgradeCameraState.Wing:
                currWingIndex += incr;
                if (currWingIndex >= 2) currWingIndex = 1;
                else if (currWingIndex < 0) currWingIndex = 0;

                tgtCameraPos = wingCameraPos[currWingIndex].transform.position;
                tgtCameraRot = wingCameraPos[currWingIndex].transform.rotation.eulerAngles;

                break;
            case UpgradeCameraState.Slot:
                currSlotIndex += incr;
                if (!weaponSlots[currSlotIndex].Visible) currSlotIndex -= incr;

                if (currSlotIndex >= SLOTS_COUNT) currSlotIndex = SLOTS_COUNT - 1;
                else if (currSlotIndex < 0) currSlotIndex = 0;

                tgtCameraPos = slotCameraPos[currSlotIndex].transform.position;
                tgtCameraRot = slotCameraPos[currSlotIndex].transform.rotation.eulerAngles;
                break;
        }
    }

    private void Select()
    {
        switch (cameraState)
        {
            case UpgradeCameraState.Main:
                cameraState = UpgradeCameraState.Wing;
                break;
            case UpgradeCameraState.Wing:
                cameraState = UpgradeCameraState.Slot;
                if (wings[currWingIndex].CurrWingState != WingState.None)
                {
                    if (currWingIndex == 0)
                        currSlotIndex = 1;
                    else
                        currSlotIndex = 2;
                }
                break;
        }
    }

    private void Cancel()
    {
        switch (cameraState)
        {
            case UpgradeCameraState.Wing:
                cameraState = UpgradeCameraState.Main;
                break;
            case UpgradeCameraState.Slot:
                cameraState = UpgradeCameraState.Wing;
                if (currSlotIndex == 0 || currSlotIndex == 1)
                    currWingIndex = 0;
                else
                    currWingIndex = 1;
                break;
        }
    }
}

public class WeaponSlot
{
    private bool visible;
    private int currWeaponIndex;
    private int weaponCount;
    private GameObject[] weapons;

    public bool Visible => visible;
    public GameObject CurrWeapon => weapons[currWeaponIndex];

    public WeaponSlot(GameObject[] weapons, bool visible)
    {
        this.weapons = weapons;
        this.visible = visible;
        currWeaponIndex = 0;
        weaponCount = weapons.Length;
    }

    public void ChangeWeapon(int incr)
    {
        weapons[currWeaponIndex].SetActive(false);
        currWeaponIndex += incr;
        if (currWeaponIndex >= weaponCount) currWeaponIndex = 0;
        else if (currWeaponIndex < 0) currWeaponIndex = weaponCount - 1;
        weapons[currWeaponIndex].SetActive(true);
    }

    public void SetVisible(bool visible)
    {
        this.visible = visible;
        weapons[currWeaponIndex].SetActive(visible);
    }
}

public class UpgradableWing
{
    private const int ONE_WING_SLOTS_COUNT = 2;

    private WingState currWingState;
    private WeaponSlot[] slots;
    private GameObject longWing, shortWing;

    public WingState CurrWingState => currWingState;

    public UpgradableWing(WeaponSlot slot1, WeaponSlot slot2, GameObject longWing, GameObject shortWing, WingState wingState)
    {
        slots = new WeaponSlot[ONE_WING_SLOTS_COUNT];
        slots[0] = slot1;
        slots[1] = slot2;
        this.longWing = longWing;
        this.shortWing = shortWing;
        currWingState = wingState;
        SetElementsActivity();
    }

    public void ChangeWingState(int incr)
    {
        currWingState += incr;
        if (currWingState > WingState.Long) currWingState = WingState.None;
        else if (currWingState < WingState.None) currWingState = WingState.Long;
        SetElementsActivity();
    }

    private void SetElementsActivity()
    {
        switch (currWingState)
        {
            case WingState.Long:
                slots[0].SetVisible(true);
                slots[1].SetVisible(true);
                longWing.SetActive(true);
                shortWing.SetActive(false);
                break;
            case WingState.Short:
                slots[0].SetVisible(true);
                slots[1].SetVisible(false);
                longWing.SetActive(false);
                shortWing.SetActive(true);
                break;
            case WingState.None:
                slots[0].SetVisible(false);
                slots[1].SetVisible(false);
                longWing.SetActive(false);
                shortWing.SetActive(false);
                break;
        }
    }
}

public enum UpgradeCameraState
{
    Main,
    Wing,
    Slot
}