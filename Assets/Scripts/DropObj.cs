using UnityEngine;

[System.Serializable]
public class DropObj
{
    public GameObject obj;
    [Range(0f, 100f)]
    public float chance;
    public int number;
    public static void Drop(Vector3 pos, DropObj[] Drop)
    {
        GameObject k;
        if (Drop.Length != 0)
        {
            foreach (var item in Drop)
            {
                if (item.chance >= UnityEngine.Random.Range(0f, 100f))
                {
                    for (int i = 0; i < item.number; i++)
                    {
                        k = Object.Instantiate(item.obj);
                        k.transform.localPosition = pos + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), 0);
                        k.transform.rotation = Quaternion.identity;
                        k.transform.SetParent(null);
                    }
                }
            }
        }
    }
}
