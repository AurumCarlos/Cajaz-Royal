using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using Photon.Pun;
using UnityEngine;

public class WeaponHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private Gun _gun;
    [SerializeField] private Material weaponTracerMaterial;
    [SerializeField] private Sprite shootFlashSprite;
    void Start()
    {
        _gun.OnShoot += Gun_OnShoot;
    }

    private void Gun_OnShoot(object sender, Gun.OnShootEventArgs e)
    {
        CreateWeaponTracer(e.gunEndPointPosition, e.shootPosition);
        CreateShootFlash(e.gunEndPointPosition);
        Utils.ShakeCamera(.05f, .2f);

        var dir = (e.shootPosition - e.gunEndPointPosition).normalized;
        var dist = Vector3.Distance(e.gunEndPointPosition, e.shootPosition);
        if (dist > _gun.Range) dist = _gun.Range;
        
        Ray ray = new (e.gunEndPointPosition, dir);
        
        RaycastHit2D hit;
        //Debug.DrawRay(e.gunEndPointPosition, dir, Color.red);
        hit = Physics2D.Raycast(ray.origin, ray.direction, dist);
        
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.GetComponent<Health>())
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("damage", RpcTarget.All, _gun.damage);
            }
        }
    }

    private void CreateShootFlash(Vector3 spawnPos)
    {
        World_Sprite worldSprite = World_Sprite.Create(spawnPos, new Vector3(0.5f, 0.5f, 1), shootFlashSprite, Color.white);
        FunctionTimer.Create(worldSprite.DestroySelf, .1f);
    }

    private void CreateWeaponTracer(Vector3 fromPos, Vector3 targetPos)
    {
        Vector3 dir = (targetPos - fromPos).normalized;
        var eulerZ = Utils.GetAngleFromVector(dir) -90;
        var dist = Vector3.Distance(fromPos, targetPos);

        if (dist > _gun.Range) dist = _gun.Range;
        var spawnPos = fromPos + dir * dist * 0.5f;

        Material tmp = new Material(weaponTracerMaterial);
        tmp.SetTextureScale("_MainTex", new Vector2(1f, dist / 16f));
        World_Mesh worldMesh = World_Mesh.Create(spawnPos, eulerZ, .4f, dist, tmp, null, 10000);

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
                else
                {
                    worldMesh.SetUVCoords(new World_Mesh.UVCoords(16 * frame, 0, 16, 256));
                }
            } 
            return false;
        });
    }
}
