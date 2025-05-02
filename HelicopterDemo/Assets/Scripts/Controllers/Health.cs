using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int lostHpPartForManeuver = 3;
    [SerializeField] private int playerIndex = -1;
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private GameObject healthBarPrefab;

    private float health;
    private UnitController unitController;
    private Npc npc;
    private Building building;
    private GameObject smoke;
    private HealthBar healthBar;
    private GameObject damageSourcePlayer;
    private UI_Controller UI_Controller;
    private SingleProgressUI uiSingle;

    public bool IsAlive { get; private set; }
    public bool IsHurt { get; set; }
    public bool IsUnderAttack { get; set; }
    public bool LostHpPart { get; set; }
    public float CurrHp => health;
    public Npc AttackSource { get; private set; }
    private float NormHealth => health / baseHealth;

    public void Hurt(float damage, bool damageFromPlayer = false, Npc attackSource = null)
    {
        CheckLostHpPart(damage);

        health -= damage;
        IsHurt = true;
        IsUnderAttack = true;
        AttackSource = attackSource;

        if (damageFromPlayer)
        {
            if (healthBarPrefab && !healthBar)
            {
                healthBar = Instantiate(healthBarPrefab, transform.position + Vector3.up * 3f, transform.rotation, transform).GetComponent<HealthBar>();
                healthBar.SetFullHealth(baseHealth);

                damageSourcePlayer = unitController.FindClosestPlayer(npc ? npc : building ? building : null, out float dist).gameObject;
            }

            if (healthBar)
            {
                healthBar.SetHealth(health);
                healthBar.SetDamageSource(damageSourcePlayer);
            }
        }

        if (health <= 50f)
        {
            if (!smoke && smokePrefab)
            {
                smoke = Instantiate(smokePrefab, transform);
                smoke.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (smoke)
            {
                float scale = (baseHealth - health) / baseHealth;
                smoke.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        if (health <= 0f)
        {
            IsAlive = false;
            if (npc) npc.RequestDestroy();
            if (building) building.RequestDestroy();
            uiSingle?.SetDisable();
        }

        uiSingle?.SetCircleAmount(NormHealth);
    }

    public void SetAlive(bool isAlive)
    {
        IsAlive = isAlive;
        health = isAlive ? baseHealth : 0f;
        if (isAlive && smoke)
            Destroy(smoke);
        uiSingle?.SetCircleAmount(NormHealth);
        uiSingle?.SetEnable();
    }

    private void Start()
    {
        IsAlive = true;
        health = baseHealth;
        unitController = UnitController.Singleton;
        npc = GetComponent<Npc>();
        building = GetComponent<Building>();

        if (playerIndex >= 0)
        {
            UI_Controller = UI_Controller.Singleton;
            uiSingle = playerIndex == 0 ? UI_Controller.HP_UI_player1 : UI_Controller.HP_UI_player2;
        }
    }

    private void CheckLostHpPart(float damage)
    {
        for (int i = lostHpPartForManeuver - 1; i >= 1; i--)
        {
            float partHp = baseHealth * (float)i / (float)lostHpPartForManeuver;
            if (health > partHp && (health - damage) < partHp)
                LostHpPart = true;
        }
    }
}
