using System.Linq;
using UnityEngine;

public class VoodooDoll : Weapon
{
    public override string weaponName { get; set; } = "voodoo doll";

    private bool thrown = false;
    private float slowTime = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (thrown && rb.velocity.magnitude <= 4)
        {
            slowTime += Time.deltaTime;
        }
        base.Update();
        if (slowTime >= 3) Destroy(gameObject);
    }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        // TODO: merge this var with Exploder.ExplosionEnabled
        thrown = true;
        slowTime = 0;
        var players = from i in GameObject.FindGameObjectsWithTag("Player")
                      where i.GetComponent<PirateController>().teamNum != thrower.teamNum
                      select i;
        // TODO: make this select a single pirate, not every single one
        GameObject p = null;
        foreach (GameObject player in players)
        {
            p = player;
            player.GetComponent<LaunchController>().AddLaunchForce(velocity);
        }
        Singleton.Instance.vcam.Follow = p.transform;
    }
}
