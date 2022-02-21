using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    public string name;
    public float health;
    public string[] protectedBy;

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material mat;
}

public class Enemy4 : Enemy
{
    [Header("Set in Inspector")]
    public Part[] parts;

    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4;

    void Start()
    {
        p0 = p1 = pos;
        InitMovement();

        Transform t;
        foreach (var prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement()
    {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;
        if (u > 1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }


    Part FindPart(string n)
    {
        foreach (var prt in parts)
        {
            if (prt.name == n)
                return prt;
        }
        return null;
    }
    Part FindPart(GameObject go)
    {
        foreach (var prt in parts)
        {
            if (prt.go == go)
                return prt;
        }
        return null;
    }

    bool Destroyed(string n) => Destroyed(FindPart(n));
    bool Destroyed(GameObject go) => Destroyed(FindPart(go));

    bool Destroyed(Part prt)
    {
        if (prt == null) return true;
        return prt.health <= 0;
    }

    void ShowLocolaizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                {
                    var p = other.GetComponent<Projectile>();
                    if (!bndCheck.isOnScreen)
                    {
                        Destroy(other);
                        break;
                    }
                    var goHit = collision.contacts[0].thisCollider.gameObject; // checking
                    Part prtHit = FindPart(goHit);
                    if (prtHit == null)
                    {
                        goHit = collision.contacts[0].otherCollider.gameObject;
                        prtHit = FindPart(goHit);
                    } 

                    if(prtHit.protectedBy != null)
                    {
                        foreach (string s in prtHit.protectedBy)
                        {
                            if (!Destroyed(s))
                            {
                                Destroy(other);
                                return;
                            }
                        }
                    }


                    prtHit.health -= Main.GetWeaponDefinition(p.Type).damageOnHit;
                    ShowLocolaizedDamage(prtHit.mat);
                    if (prtHit.health <= 0)
                    {
                        prtHit.go.SetActive(false);
                    }
                    bool allDeatroyed = true;
                    foreach(var prt in parts)
                    {
                        if (!Destroyed(prt))
                        {
                            allDeatroyed = false;
                            break;
                        }
                    }
                    if (allDeatroyed)
                    {
                        Main.S.ShipDestroyed(this);
                        Destroy(gameObject);
                        particlePrefub = Instantiate(particlePrefub);
                        particlePrefub.transform.position = pos;
                        particlePrefub.Play();
                    }
                    Destroy(other);
                    break;
                }
            default:
                {
                    print("Enemy hit by non-ProjectileHero: " + other.name);
                    break;
                }
        }
    }
}
