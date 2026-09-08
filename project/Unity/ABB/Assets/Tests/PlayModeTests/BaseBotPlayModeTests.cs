using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseBotPlayModeTests
{
    private BotProxy bot;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // BaseBot.Awake() calls InputSystem.actions.FindAction(...); a non-null
        // asset is enough to avoid a NullReferenceException (FindAction returns
        // null for unknown names without throwing).
        if (InputSystem.actions == null)
            InputSystem.actions = ScriptableObject.CreateInstance<InputActionAsset>();
    }

    [SetUp]
    public void SetUp() => bot = new BotProxy();

    [TearDown]
    public void TearDown() => bot.Destroy();

    // ── SP system ────────────────────────────────────────────────────────────

    [Test]
    public void AddSp_IncreasesSp()
    {
        bot.Call("addSp", 30);
        Assert.AreEqual(30, bot.Get<int>("sp"));
    }

    [Test]
    public void AddSp_CapsAtMaxSp()
    {
        int max = bot.Get<int>("maxSp");
        bot.Call("addSp", max + 50);
        Assert.AreEqual(max, bot.Get<int>("sp"));
    }

    [Test]
    public void AddSp_NotBelowZero()
    {
        bot.Call("addSp", -999);
        Assert.AreEqual(0, bot.Get<int>("sp"));
    }

    [Test]
    public void SetSp_ClampsToMaxSp()
    {
        int max = bot.Get<int>("maxSp");
        bot.Call("setSp", max + 100);
        Assert.AreEqual(max, bot.Get<int>("sp"));
    }

    [Test]
    public void SetSp_ClampsToZero()
    {
        bot.Call("setSp", -50);
        Assert.AreEqual(0, bot.Get<int>("sp"));
    }

    // ── HP system ────────────────────────────────────────────────────────────

    [Test]
    public void SetHp_UpdatesHp()
    {
        bot.Call("SetHp", 75);
        Assert.AreEqual(75, bot.Get<int>("hp"));
    }

    [Test]
    public void Hurt_ReducesHp()
    {
        int hp0 = bot.Get<int>("hp");
        bot.Call("Hurt", 10, 0.5f);
        Assert.AreEqual(hp0 - 10, bot.Get<int>("hp"));
    }

    [Test]
    public void Hurt_SetsStateToHurt()
    {
        bot.Call("Hurt", 10, 0.5f);
        Assert.AreEqual("hurt", bot.Get<string>("state"));
    }

    [Test]
    public void Hurt_SetsTimerToHitstun()
    {
        bot.Call("Hurt", 10, 0.5f);
        Assert.AreEqual(0.5f, bot.Get<float>("timer"), 0.001f);
    }

    // ── Combat inputs ────────────────────────────────────────────────────────

    [Test]
    public void ActivateDoAttack_SetsFlag()
    {
        bot.Call("activateDoAttack");
        Assert.IsTrue(bot.Get<bool>("doAttack"));
    }

    [Test]
    public void ActivateDoHeavyAttack_SetsFlag()
    {
        bot.Call("activateDoHeavyAttack");
        Assert.IsTrue(bot.Get<bool>("doHeavyAttack"));
    }

    // ── Ultimate ─────────────────────────────────────────────────────────────

    [Test]
    public void Ultimate_WhenSpAbove50_SetsUltimateState()
    {
        bot.Call("setSp", 60);
        bot.SetField("canUltimate", true);
        bot.Call("UltimateAttack");
        Assert.AreEqual("ultimate", bot.Get<string>("state"));
    }

    [Test]
    public void Ultimate_DeductsFiftySp()
    {
        bot.Call("setSp", 60);
        bot.SetField("canUltimate", true);
        bot.Call("UltimateAttack");
        Assert.AreEqual(10, bot.Get<int>("sp"));
    }

    [Test]
    public void Ultimate_WhenSpTooLow_DoesNotTrigger()
    {
        bot.Call("setSp", 30);
        bot.SetField("canUltimate", true);
        bot.Call("UltimateAttack");
        Assert.AreNotEqual("ultimate", bot.Get<string>("state"));
    }

    // ── Dodge ────────────────────────────────────────────────────────────────

    [Test]
    public void Dodge_SetsDodgeState()
    {
        bot.SetField("canDodge", true);
        bot.Call("doDodge");
        bot.Call("Dodge");
        Assert.AreEqual("dodge", bot.Get<string>("state"));
    }

    [Test]
    public void Dodge_SetsTimer()
    {
        bot.SetField("canDodge", true);
        bot.Call("doDodge");
        bot.Call("Dodge");
        Assert.AreEqual(0.5f, bot.Get<float>("timer"), 0.001f);
    }
}
