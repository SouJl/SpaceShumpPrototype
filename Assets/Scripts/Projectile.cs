using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck boundsCheck;
    private Renderer rend;

    [Header("Set Dynamicaly")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;
    public WeaponType Type
    {
        get => _type;
        set
        {
            SetType(value);
        }
    }

    void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(boundsCheck.offUp)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Устанавливает цвет снаряда
    /// </summary>
    /// <param name="eType"></param>
    public void SetType(WeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}
