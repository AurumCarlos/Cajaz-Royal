using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

public class WeaponHandlerSolo : MonoBehaviourPunCallbacks
{
    [SerializeField] private Gun _gun;
    [SerializeField] private Material weaponTracerMaterial;
    [SerializeField] private Sprite shootFlashSprite;
    
    [SerializeField] private InventoryHandlerSolo playerInventory;
    void Start()
    {
        _gun.OnShoot += Gun_OnShoot;
        _gun.OnReload += Gun_OnReload;
    }

    private int? SearchAmmoIndex(Item item)
    {
        for (int i = 0; i < playerInventory.Inventory.Length; i++)
        {
            var storedItem = playerInventory.Inventory[i];
            if (storedItem == _gun.Munition) return i;
        }
        return null;
    }
    
    private void Gun_OnReload(object sender, Gun.OnShootEventArgs _)
    {
        Item equiped = playerInventory.Equiped;
        int? storedAmmoIndex = SearchAmmoIndex(equiped);
        if (storedAmmoIndex != null)
        {
            Item storedAmmo = playerInventory.Inventory[storedAmmoIndex.Value];
            int toDraw = _gun.CapaciteeChargeur - _gun.Munition.Amount;
            
            if (storedAmmo.Amount <= toDraw)
            {
                _gun.Munition.Amount += storedAmmo.Amount;
                playerInventory.Inventory[storedAmmoIndex.Value] = null;
            }
            else
            {
                storedAmmo.Amount -= toDraw;
                _gun.Munition.Amount += toDraw;
            }
        }
        
        playerInventory.UpdateInventory();

        playerInventory.uiInventory.Inventory = playerInventory.Inventory;
        playerInventory.uiInventory.RefreshInventory();
    }

    private void Gun_OnShoot(object sender, Gun.OnShootEventArgs e)
    {
        Utils.ShakeCamera(.05f, .2f);

        var dir = (e.shootPosition - e.gunEndPointPosition).normalized;
        Ray ray = new (e.gunEndPointPosition, dir);
        
        //Debug.DrawRay(e.gunEndPointPosition, dir, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _gun.Range);
        if (hit.collider != null)
        {
            CreateWeaponTracer(e.gunEndPointPosition, e.shootPosition, Vector3.Distance(e.shootPosition, hit.transform.position));
            CreateShootFlash(e.gunEndPointPosition);
            RatHealth r = hit.transform.gameObject.GetComponent<RatHealth>();
            if (r)
            {
                r.damage(_gun.damage);
                //hit.transform.gameObject.GetComponent<PhotonView>()?.RPC("Damage", RpcTarget.All, _gun.damage);
            }
        }
        else
        {
            CreateWeaponTracer(e.gunEndPointPosition, e.shootPosition, _gun.Range);
            CreateShootFlash(e.gunEndPointPosition);
        }
    }
    
    private void CreateShootFlash(Vector3 spawnPos)
    {
        World_Sprite worldSprite = World_Sprite.Create(spawnPos, new Vector3(0.5f, 0.5f, 1), shootFlashSprite, Color.white);
        FunctionTimer.Create(worldSprite.DestroySelf, .1f);
    }
    
    private void CreateWeaponTracer(Vector3 fromPos, Vector3 targetPos, float dist)
    {
        Vector3 dir = (targetPos - fromPos).normalized;
        var eulerZ = Utils.GetAngleFromVector(dir) -90;
        var spawnPos = fromPos + dir * dist * 0.5f;

        //Material tmp = new Material(weaponTracerMaterial);
        //tmp.SetTextureScale("_MainTex", new Vector2(1f, dist / 16f));
        World_Mesh worldMesh = World_Mesh.Create(spawnPos, eulerZ, .4f, dist, weaponTracerMaterial, null, 10000);

        int frame = 0;
        float framerate = .016f;
        float timer = .1f;
        worldMesh.SetUVCoords(new World_Mesh.UVCoords(0, 0, 16, 256));
        FunctionUpdater.Create(() =>
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                frame++;
                timer += framerate;
                if (frame >= 4)
                {
                    worldMesh.DestroySelf();
                    return true;
                }
                worldMesh.SetUVCoords(new World_Mesh.UVCoords(16 * frame, 0, 16, 256));
                
            } 
            return false;
        });
    }
}
