using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    [SerializeField] private bool isNpc = true;
    [SerializeField] private int membersCount = 3;
    [SerializeField] private float squadRadius = 12f;
    [SerializeField] private GameObject memberPrefab;

    private UnitController unitController;

    public float SquadRadius => squadRadius;
    public List<GameObject> Members { get; private set; }
    public List<NpcGround> Npcs { get; private set; }

    private void Awake()
    {
        unitController = UnitController.Singleton;
        Members = new List<GameObject>();
        Npcs = new List<NpcGround>();

        InitMembers();
    }

    private void InitMembers()
    {
        Vector3 dir = new Vector3(0f, 0f, -1f);
        Quaternion rot = Quaternion.Euler(0f, 360f / membersCount, 0f);

        for (int i = 0; i < membersCount; i++)
        {
            GameObject member = Instantiate(memberPrefab, transform.position + new Vector3(0f, 0f, 0f), transform.rotation, transform);
            member.transform.Translate(dir * squadRadius / 2f);
            Members.Add(member);
            dir = rot * dir;
        }

        if (isNpc)
        {
            for (int i = 0; i < membersCount; i++)
            {
                Npcs.Add(Members[i].GetComponent<NpcGround>());
                Npcs[i].NpcSquad = this;
                unitController.AddNpc(Npcs[i]);
            }
        }
    }

    public bool RemoveMember(NpcGround member)
    {
        if (Npcs.Contains(member))
            Npcs.Remove(member);
        return Npcs.Count > 0;
    }

    public Vector3 GetSquadPos()
    {
        Vector3 pos = Vector3.zero;
        int count = 0;
        foreach (var npc in Npcs)
        {
            if (npc)
            {
                pos += npc.gameObject.transform.position;
                count++;
            }
        }
        pos /= count;
        return pos;
    }

    public Vector3 GetSquadPos(int npc1, int npc2)
    {
        Vector3 pos = Vector3.zero;
        pos += Npcs[npc1].gameObject.transform.position;
        pos += Npcs[npc2].gameObject.transform.position;
        pos /= 2f;
        return pos;
    }

    public Vector3 GetSquadPos(int npc) => Npcs[npc].gameObject.transform.position;
}
