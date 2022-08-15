using UnityEngine;

public class InputStorage : MonoBehaviour
{
    public sbyte Horizontal;
    public sbyte Vertical;

    private void Update()
    {
        var h = (int) Input.GetAxisRaw("Horizontal");
        var v = (int) Input.GetAxisRaw("Vertical");

        if (h != 0)
        {
            Horizontal = (sbyte) h;
        }
        
        if (v != 0)
        {
            Vertical = (sbyte) v;
        }
    }

    public void Consume()
    {
        Horizontal = Vertical = 0;
    }
}