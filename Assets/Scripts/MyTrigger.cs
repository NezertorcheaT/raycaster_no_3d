using UnityEngine;
using NTC.Global.Cache;

[RequireComponent(typeof(Collider2D))]
public abstract class MyTrigger : MonoCache
{
    public int[] LayerToActivate;
    protected Entity neb = null;

    public virtual void Activate(Entity entity) { }
    public virtual void Diactivate(Entity entity) { }
    public void UpdateNebActivate(Entity entity) => neb = entity;
    public void UpdateNebDiactivate() => neb = null;
}
