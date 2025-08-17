using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleRingRenderer : MonoBehaviour
{
    [Range(3, 256)] public int segments = 64;
    [Min(0)] public float radius = 0.5f;
    [Min(0)] public float thickness = 0.06f;

    LineRenderer lr;
    Color _color = Color.white;

    void Awake()
    {
        EnsureInit();
        ApplyGeometry();
        ApplyColor();
    }

    // 설정 및 초기화
    void EnsureInit()
    {
        if (lr != null) return;

        lr = GetComponent<LineRenderer>();
        if (lr == null)
        {
            return;
        }
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = Mathf.Max(3, segments);
        lr.widthMultiplier = thickness;
    }

    public float Radius
    {
        get => radius;
        set
        {
            radius = Mathf.Max(0, value);
            ApplyGeometry();
        }
    }

    public float Thickness
    {
        get => thickness;
        set
        {
            thickness = Mathf.Max(0, value);
            EnsureInit();
            if (lr != null) lr.widthMultiplier = thickness;
        }
    }

    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            ApplyColor();
        }
    }

    public float Alpha
    {
        get => _color.a;
        set
        {
            _color.a = Mathf.Clamp01(value);
            ApplyColor();
        }
    }
    // 변경사항 적용
    void ApplyGeometry()
    {
        EnsureInit();
        if (lr == null) return;

        segments = Mathf.Max(3, segments);
        if (lr.positionCount != segments) lr.positionCount = segments;

        float step = 2f * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            float a = i * step;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0f));
        }
    }

    //색상 변경사항 적용
    void ApplyColor()
    {
        EnsureInit();
        if (lr == null) return;

        lr.startColor = _color;
        lr.endColor   = _color;
    }
}
