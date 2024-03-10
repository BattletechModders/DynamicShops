using BattleTech;
using System;

namespace DynamicShops;

public class DConditionAttribute : Attribute
{
    public string Name { get; private set; }

    public DConditionAttribute(string name) { Name = name; }
}

public abstract class DCondition
{
    public abstract bool Init(object json);
    public abstract bool IfApply(SimGameState sim, StarSystem CurSystem);
}
