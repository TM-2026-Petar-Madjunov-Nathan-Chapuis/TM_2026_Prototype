using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemWheelVectorImager : MonoBehaviour
{
    [SerializeField] public int slotNumber = 8;
    [SerializeField] private float angleOffset = 0f;
    [SerializeField, Range(0, 1)] private float innerVsOuterRadiusRatio = 0.7f;
    [SerializeField] private float innerLineWidth = 10f;
    [SerializeField] private float circleLineWidth = 10f;
    [SerializeField] private float outerLineWidth = 10f;
    [SerializeField] private float highlightedAngleIncrease = 80f;
    [SerializeField] private float animationSpeed = 1f;
    [SerializeField] private Color innerLineColor;
    [SerializeField] private Color circleLineColor;
    [SerializeField] private Color outerLineColor;
    [SerializeField] private Color innerCircleColor;
    [SerializeField] private Color outerCircleColor;
    [SerializeField] private Color highlightedColor;
    public float highlightAnimation; // 0 --> 1, where 0 is default state and 1 finished animation
    public int hoveredIndex { get; private set; }
    private VisualElement itemWheelHolder;
    public void SetHoveredIndex(int i, VisualElement itemWheelHolder)
    {
        if (i != hoveredIndex)
        {
            hoveredIndex = i;
            highlightAnimation = 0;
        }
        this.itemWheelHolder = itemWheelHolder; //could be null, optional
    }
    void Update()
    {
        if (highlightAnimation < 1)
        {
            highlightAnimation += Time.unscaledDeltaTime * animationSpeed;
            this.itemWheelHolder?.MarkDirtyRepaint();
        }
    }
    public void Draw(MeshGenerationContext ctx)
    {
        float sideLength = ctx.visualElement.resolvedStyle.width;
        Vector2 center = new Vector2(sideLength / 2, sideLength / 2);
        Painter2D painter2D = ctx.painter2D;
        Angle a360 = Angle.Radians((float)(2 * Math.PI));
        float radius = sideLength / 2;
        float innerRadius = radius * innerVsOuterRadiusRatio;

        //DRAW OUTER CIRCLE
        painter2D.strokeColor = circleLineColor;
        painter2D.fillColor = outerCircleColor;
        painter2D.BeginPath();
        painter2D.lineWidth = circleLineWidth;
        painter2D.Arc(center, radius, 0, a360);
        painter2D.Stroke();
        painter2D.ClosePath();

        painter2D.Arc(center, innerRadius, 0, a360);
        painter2D.ClosePath();

        painter2D.Fill(FillRule.OddEven);

        //DRAW INNER CIRCLE
        painter2D.fillColor = innerCircleColor;
        painter2D.BeginPath();
        painter2D.Arc(center, innerRadius, 0, a360);
        painter2D.Stroke();
        painter2D.Fill();

        float[] angles = GetAllLineAngles();
        (List<(Vector2, Vector2)>, List<Vector2>) positions = GetAllLinePositions(angles, radius, innerRadius, center);
        //DRAW EACH OUTER LINE
        painter2D.strokeColor = outerLineColor;
        painter2D.lineWidth = outerLineWidth;
        foreach ((Vector2, Vector2) pair in positions.Item1)
        {
            painter2D.BeginPath();
            painter2D.MoveTo(pair.Item1);
            painter2D.LineTo(pair.Item2);
            painter2D.Stroke();
        }

        //DRAW EACH INNER LINE
        painter2D.lineWidth = innerLineWidth;
        painter2D.strokeColor = innerLineColor;
        foreach (Vector2 v in positions.Item2)
        {
            painter2D.BeginPath();
            painter2D.MoveTo(center);
            painter2D.LineTo(v);
            painter2D.Stroke();
        }
        if (hoveredIndex >= 0)
        {
            float start = angles[hoveredIndex == 0 ? slotNumber - 1 : hoveredIndex - 1];
            float end = angles[hoveredIndex];

            painter2D.BeginPath();
            // outer arc
            painter2D.Arc(center, radius, start, end);
            // connect to inner arc start
            painter2D.LineTo(
                center + new Vector2(Mathf.Cos(end * Mathf.Deg2Rad) * innerRadius, Mathf.Sin(end * Mathf.Deg2Rad) * innerRadius)
            );
            painter2D.ClosePath();
            // inner arc
            painter2D.Arc(center, innerRadius, end, start);
            painter2D.ClosePath();

            painter2D.fillColor = highlightedColor;
            painter2D.Fill(FillRule.OddEven);
        }
    }
    private float[] GetAllLineAngles()
    {
        float[] lineAngles = new float[slotNumber];
        float angleStepSize = 360f / slotNumber;

        for (int i = 0; i < slotNumber; i++)
        {
            lineAngles[i] = angleOffset + i * angleStepSize;
        }

        if (hoveredIndex >= 0)
        {
            float halfIncrease = highlightedAngleIncrease * 0.5f * highlightAnimation;

            lineAngles[hoveredIndex == 0 ? slotNumber - 1 : hoveredIndex - 1] -= halfIncrease;
            lineAngles[hoveredIndex] += halfIncrease; //creates the hole
        }

        return lineAngles;
    }
    private (List<(Vector2, Vector2)>, List<Vector2>) GetAllLinePositions(float[] lineAngles, float radius, float innerRadius, Vector2 center)
    {
        List<(Vector2, Vector2)> couples = new();
        List<Vector2> innerLinePos = new();
        foreach (float angle in lineAngles)
        {
            Vector2 first = new Vector2();
            first.x = Mathf.Cos(angle * math.TORADIANS) * innerRadius + center.x; // (cleared than new( x = ...) to me)
            first.y = Mathf.Sin(angle * math.TORADIANS) * innerRadius + center.y;
            Vector2 second = new Vector2();
            second.x = Mathf.Cos(angle * math.TORADIANS) * radius + center.x;
            second.y = Mathf.Sin(angle * math.TORADIANS) * radius + center.y;

            innerLinePos.Add(first);
            couples.Add((first, second));
        }
        return (couples, innerLinePos);
    }
    public Vector2[] getAllSlotCenters(float sideLength)
    {
        Vector2[] l = new Vector2[slotNumber];
        Vector2 center = new Vector2(sideLength / 2, sideLength / 2);
        float radius = sideLength / 2;
        float innerRadius = radius * innerVsOuterRadiusRatio;
        float angleStepSizeDegrees = 360f / slotNumber;

        Angle angle = Angle.Degrees(0);
        for (int i = 0; i < slotNumber; i++)
        {
            Vector2 v = new();
            float r = (radius - innerRadius) / 2 + innerRadius;
            v.x = Mathf.Cos(angle.ToRadians()) * r + center.x;
            v.y = Mathf.Sin(angle.ToRadians()) * r + center.y;

            l[i] = v;
            angle = Angle.Degrees(angle.ToDegrees() + angleStepSizeDegrees);
        }
        return l;
    }
    public int NearestItemWheelPosition(Vector2 pos, float sideLength)
    {
        Vector2 center = new(sideLength / 2f, sideLength / 2f);
        if (Vector2.Distance(pos, center) > sideLength / 2) return -1;
        pos = pos - center; //convert to local
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360f; //needed to convert [-180;180] to [0; 360]
        angle += angleOffset;
        int i = Mathf.FloorToInt(angle / (360f / slotNumber));
        if (i == this.slotNumber) i = 0;
        return i;
    }
}