using UnityEngine;

[AddComponentMenu("Triggers/Destroy on collision Trigger")]
public class DestroyOnCollisionTrigger : MyTrigger
{
    public override void Activate(Entity entity)
    {
        base.Activate(entity);
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
