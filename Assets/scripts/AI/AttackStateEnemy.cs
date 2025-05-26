using CodeMonkey.Utils;
using Photon.Pun;
using UnityEngine;

namespace AI
{
    public class AttackStateEnemy :AttackState
    {
        // Start is called before the first frame update
        public AttackStateEnemy(AIAgent ai) : base(ai)
        {
        }
        


        private static readonly int IsWalking = Animator.StringToHash("is_walking");
        private bool is_seeing_right;
        private float timeBetweenShots = 1.5f;
        private float shotCooldown;

        private float raycastRange = 9f;
    

        public override void Enter()
        {
            Enemy.Path.destination = Enemy.Target.position;
            shotCooldown = 0f;
            Enemy.animator.SetBool(IsWalking, false);
           
            //Enemy.Path.canMove = false;
        }

        public override void Update()
        {
            //Enemy.animator.SetBool(IsWalking, false);
            // Vector2 dir = Enemy.Target.position - Enemy.gun.transform.transform.position;
            // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            // Enemy.gun.transform.rotation = Quaternion.Euler(0, 0, angle);
            Enemy.AimAtTarget();
            if (shotCooldown <= 0f)
            {
                PerformRaycastAttack();
                shotCooldown = timeBetweenShots;
            }
            else
            {
                shotCooldown -= Time.deltaTime;
            }

            if (Enemy.Target == null)
            {
                //Enemy.Path.canMove = true;
                Enemy.animator.SetBool(IsWalking, true);
                Enemy.SwitchState(Enemy.patrolState);
                return;
            }

            float distance = Vector2.Distance(Enemy.transform.position, Enemy.Target.position + new Vector3(0f, 0.5f, 0f));
            if (distance > raycastRange)
            {
               // Enemy.Path.canMove = true;
                Enemy.animator.SetBool(IsWalking, true);
                Enemy.SwitchState(Enemy.chaseState);
                return;
            }

           
        }

        private void PerformRaycastAttack()
        {
            Vector2 direction = (Enemy.Target.position - Enemy.transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(Enemy.muzzle.position, direction,raycastRange);
            Enemy.AimAtTarget();
            CreateShootFlash(Enemy.muzzle.transform.position);
            CreateWeaponTracer(Enemy.muzzle.transform.position,Enemy.Target.position);
            if (hit.collider != null)
            {
                PhotonView targetPhotonView = hit.collider.GetComponent<PhotonView>();
                if (hit.transform.gameObject.GetComponent<HealthSolo>())
                {
                    hit.transform.gameObject.GetComponent<HealthSolo>().Damage(20);
                    //hit.transform.gameObject.GetComponent<PhotonView>().RPC("Damage", RpcTarget.All,10 );
                }
            }

            //Debug.DrawRay(Enemy.transform.position, direction * raycastRange, Color.red, 0.5f);
        }
        private void CreateShootFlash(Vector3 spawnPos)
        {
            
            World_Sprite worldSprite = World_Sprite.Create(spawnPos, new Vector3(0.5f, 0.5f, 1), Enemy.shootFlashSprite, Color.white);
            FunctionTimer.Create(worldSprite.DestroySelf, .1f);
        }
        private void CreateWeaponTracer(Vector3 fromPos, Vector3 targetPos)
        {
            Vector3 dir = (targetPos - fromPos).normalized;
            var eulerZ = Utils.GetAngleFromVector(dir) -90;
            var dist = Vector3.Distance(fromPos, targetPos);

            if (dist >raycastRange) dist = raycastRange;
            var spawnPos = fromPos + dir * dist * 0.5f;

            Material tmp = new Material(Enemy.weaponTracerMaterial);
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
    
}
