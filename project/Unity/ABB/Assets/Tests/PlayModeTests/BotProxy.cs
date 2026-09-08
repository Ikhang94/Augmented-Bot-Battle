using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Wraps a BaseBot instance via reflection so the test assembly does not need a
// compile-time reference to Assembly-CSharp (an asmdef cannot reference it).
// The GameObject is built inactive, given the components BaseBot.Awake expects
// (Rigidbody, Animator) and a non-null customButtons list, then activated so
// Awake runs without throwing.
internal sealed class BotProxy
{
    private const BindingFlags Flags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public readonly GameObject Go;
    private readonly Component bot;
    private readonly Type type;

    public BotProxy()
    {
        type = ResolveType("BaseBot");

        Go = new GameObject("TestBot");
        Go.SetActive(false);

        Go.AddComponent<Rigidbody>();
        var animator = Go.AddComponent<Animator>();

        bot = Go.AddComponent(type);
        SetField("customButtons", new List<GameObject>());
        SetField("animator", animator);

        Go.SetActive(true);
        ((Behaviour)bot).enabled = false; // ran Awake once; keep Update from ticking
    }

    public void Call(string method, params object[] args)
    {
        var argTypes = Array.ConvertAll(args, a => a.GetType());
        var m = type.GetMethod(method, Flags, null, argTypes, null)
                ?? type.GetMethod(method, Flags);
        if (m == null)
            throw new MissingMethodException(type.Name, method);
        m.Invoke(bot, args);
    }

    public T Get<T>(string field) => (T)type.GetField(field, Flags).GetValue(bot);

    public void SetField(string field, object value) =>
        type.GetField(field, Flags).SetValue(bot, value);

    public void Destroy()
    {
        if (Go != null)
            UnityEngine.Object.Destroy(Go);
    }

    private static Type ResolveType(string name)
    {
        var t = Type.GetType(name + ", Assembly-CSharp");
        if (t != null)
            return t;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            t = asm.GetType(name);
            if (t != null)
                return t;
        }
        throw new Exception($"Type '{name}' not found in any loaded assembly.");
    }
}
