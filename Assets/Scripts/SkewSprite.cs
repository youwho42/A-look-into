
using UnityEngine;
using UnityEngine.UI;
public class SkewSprite : Image
{
    [SerializeField]
    private float skewX;
    [SerializeField]
    private float skewY;
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        var r = GetPixelAdjustedRect();
        var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
        Color32 color32 = color;
        vh.Clear();
        vh.AddVert(new Vector3(v.x - skewX, v.y - skewY), color32, new Vector2(0f, 0f));
        vh.AddVert(new Vector3(v.x + skewX, v.w - skewY), color32, new Vector2(0f, 1f));
        vh.AddVert(new Vector3(v.z + skewX, v.w + skewY), color32, new Vector2(1f, 1f));
        vh.AddVert(new Vector3(v.z - skewX, v.y + skewY), color32, new Vector2(1f, 0f));
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}