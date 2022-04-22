using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    static public Hero S;

    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    public Text uiCurrWeapon;
    public Text uiShieldLevel;
    public ParticleSystem particlePrefub;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;
    
    [System.NonSerialized]
    public bool isAlive;

    public float shieldLevel
    {
        get => _shieldLevel;
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(gameObject);
                isAlive = false;
                particlePrefub = Instantiate(particlePrefub);
                particlePrefub.transform.position = gameObject.transform.position;
                particlePrefub.Play();
            }
        }
    }


    private GameObject lastTriggerGo;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    void Start()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to asign scond Hero.S");
        }
        // fireDelegate += TempFire;
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
        UpdateGUI();
        isAlive = true;
    }


    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        var pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    void TempFire()
    {
        var projGo = Instantiate<GameObject>(projectilePrefab);
        projGo.transform.position = transform.position;
        Rigidbody rigidB = projGo.GetComponent<Rigidbody>();

        Projectile proj = projGo.GetComponent<Projectile>();
        proj.Type = WeaponType.missle;
        float tSpeed = Main.GetWeaponDefinition(proj.Type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        var root = other.gameObject.transform.root;
        var go = root.gameObject;
        print("triggered: " + go.name);

        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;
        if (go.tag == "Enemy")
        {
            shieldLevel--;
            UpdateGUI();
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        {
            AbsorbPoweUp(go);
        }
        else
        {
            print("triggered by non-enemy: " + go.name);
        }
    }

    public void AbsorbPoweUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            default:
                {
                    if (pu.type == weapons[0].Type)
                    {
                        var w = GetEmptyWeaponSlot();
                        if (w != null)
                        {
                            w.SetType(pu.type);
                        }
                    }
                    else
                    {
                        ClearWeapons();
                        weapons[0].SetType(pu.type);
                    }
                    UpdateGUI();
                    break;
                }
            case WeaponType.shield:
                {
                    shieldLevel++;
                    UpdateGUI();
                    break;
                }
        }
        pu.AbsorbedBy(this.gameObject);
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].Type == WeaponType.none)
                return weapons[i];
        }
        return null;
    }

    void ClearWeapons()
    {
        foreach (var w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }

    void UpdateGUI()
    {
        uiCurrWeapon.text = $"{weapons[0].Type} x{weapons.Where(w => w.Type != WeaponType.none).ToList().Count}";
        uiShieldLevel.text = $"Shield Level: {Mathf.Clamp(shieldLevel, 0, 4)}";
    }
}
