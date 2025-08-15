using System;
using UnityEngine;
using static Types;

public class RadialMenuSelector : MonoBehaviour
{
    [SerializeField] private int selectedIndex = 0;
    [SerializeField] private float selectedScale = 1.3f;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float triggerValue = 0.5f;

    private GameObject[] Icons => GetComponent<RadialMenuLayout>().Icons;

    public InputDeviceBase InputDevice { get; set; }
    public event Action<int> SelectBuildingEvent;

    private bool upLast, downLast, rightLast, leftLast;

    private void Start()
    {
        ChangeSelection(selectedIndex);
    }

    void Update()
    {
        Vector2 input = InputDevice.GetInput();
        bool up = input.y > triggerValue;
        bool down = input.y < -triggerValue;
        bool right = input.x > triggerValue;
        bool left = input.x < -triggerValue;

        if (up && !upLast)
        {
            if (selectedIndex == 2)
                selectedIndex = 1;
            else if (selectedIndex == 6)
                selectedIndex = 7;
            else
                selectedIndex = 0;

            ChangeSelection(selectedIndex);
        }
        if (down && !downLast)
        {
            if (selectedIndex == 2)
                selectedIndex = 3;
            else if (selectedIndex == 6)
                selectedIndex = 5;
            else
                selectedIndex = 4;

            ChangeSelection(selectedIndex);
        }
        if (right && !rightLast)
        {
            if (selectedIndex == 0)
                selectedIndex = 1;
            else if (selectedIndex == 4)
                selectedIndex = 3;
            else
                selectedIndex = 2;

            ChangeSelection(selectedIndex);
        }
        if (left && !leftLast)
        {
            if (selectedIndex == 0)
                selectedIndex = 7;
            else if (selectedIndex == 4)
                selectedIndex = 5;
            else
                selectedIndex = 6;

            ChangeSelection(selectedIndex);
        }

        upLast = up;
        downLast = down;
        rightLast = right;
        leftLast = left;

        Debug.Log(InputDevice.FastMoving);
        if (InputDevice.FastMoving)
        {
            SelectBuildingEvent?.Invoke(selectedIndex);
        }
    }

    public void ResetSelectedIndex() => ChangeSelection(selectedIndex = 0);

    private void ChangeSelection(int selectedIndex)
    {
        for (int i = 0; i < Icons.Length; i++)
        {
            float targetScale = (i == selectedIndex) ? selectedScale : normalScale;
            if (Icons[i])
                Icons[i].GetComponent<RectTransform>().localScale = Vector3.one * targetScale;
        }
    }
}
