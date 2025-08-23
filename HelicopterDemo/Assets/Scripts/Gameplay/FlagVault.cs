using System.Collections;
using UnityEngine;
using static Types;

public class FlagVault : MonoBehaviour
{
    [SerializeField] private FadingOut vaultCap;
    [Header("Red, blue, green, yellow")]
    [SerializeField] private GameObject[] keyPrefabs;

    private const float keyDeltaHor = 1.95f;
    private const float keyDeltaVer = 0.495f;
    private const float flagX = 0.87f;
    private const float flagY = 0.4f;

    private bool[] keys;
    private Vector3[] keyPositions;

    public bool VaultIsUnlocked { get; private set; }

    private void Start()
    {
        keyPositions = new Vector3[4];
        keyPositions[(int)KeyType.Red] = new Vector3(0f, keyDeltaVer, -keyDeltaHor);
        keyPositions[(int)KeyType.Blue] = new Vector3(-keyDeltaHor, keyDeltaVer, 0f);
        keyPositions[(int)KeyType.Green] = new Vector3(keyDeltaHor, keyDeltaVer, 0f);
        keyPositions[(int)KeyType.Yellow] = new Vector3(0f, keyDeltaVer, keyDeltaHor);

        Vector4 thisPos = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0f);

        keys = new bool[4] { true, true, true, false };
        for (int i = 0; i < keys.Length; i++)
            if (keys[i]) Instantiate(keyPrefabs[i], thisPos + transform.localToWorldMatrix * keyPositions[i], new Quaternion(), transform);
    }

    public void SetKey(KeyType type)
    {
        keys[(int)type] = true;
        Vector4 thisPos = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0f);
        Instantiate(keyPrefabs[(int)type], thisPos + transform.localToWorldMatrix * keyPositions[(int)type], new Quaternion(), transform);
        foreach (var key in keys)
            if (!key)
                return;
        UnlockVault();
    }

    private void UnlockVault()
    {
        VaultIsUnlocked = true;
        vaultCap.Active = true;
    }
}
