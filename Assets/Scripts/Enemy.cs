using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("Set in Inspector: Enemy")]
    public float spped = 10f;
    public float foreRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float showDamageDuration = 0.1f;
    public float poweUpDropChance = 1f;
    public ParticleSystem particlePrefub;

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage;
    public float damageDoneTime;
    public bool notifiedOfDestruction;

    public Vector3 pos
    {
        get => this.transform.position;
        set
        {
            this.transform.position = value;
        }
    }

    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    void Update()
    {
        Move();
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShownDamage();
        }
        if (bndCheck != null && bndCheck.offDown)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= spped * Time.deltaTime;
        pos = tempPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGo = collision.gameObject;
        switch (otherGo.tag)
        {
            case "ProjectileHero":
                {
                    var p = otherGo.GetComponent<Projectile>();
                    if (!bndCheck.isOnScreen)
                    {
                        Destroy(otherGo);
                        break;
                    }
                    ShowDamage();
                    health -= Main.GetWeaponDefinition(p.Type).damageOnHit;
                    if (health <= 0)
                    {
                        if (!notifiedOfDestruction)
                        {
                            Main.S.ShipDestroyed(this);
                        }
                        notifiedOfDestruction = true;
                        Destroy(gameObject);
                        particlePrefub = Instantiate(particlePrefub);
                        particlePrefub.transform.position = pos;
                        particlePrefub.Play();
                    }
                    Destroy(otherGo);
                    break;
                }
            default:
                {
                    print("Enemy hit by non-ProjectileHero: " + otherGo.name);
                    break;
                }
        }
    }

    void ShowDamage()
    {
        foreach (var m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShownDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

}
