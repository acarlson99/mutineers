public class PiecesOfEight : Exploder
{

    public override string weaponName { get; set; } = "pieces of eight";
    public override EWeaponType weaponType { get; set; } = EWeaponType.Po8;

    public PiecesOfEight()
    {
        weaponCount = 8;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
