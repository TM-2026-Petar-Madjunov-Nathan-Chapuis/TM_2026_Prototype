using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CircularBarProgression : VisualElement
{   
    [UxmlAttribute]public float baseLineWidht = 15f;
    [UxmlAttribute]public Color32 color = Color.black;
    [UxmlAttribute]public bool background = false;
    [UxmlAttribute]public Color32 backgroundColor = Color.white;
    [UxmlAttribute]public bool border = false;
    [UxmlAttribute]public Color32 borderColor = Color.black;
    [UxmlAttribute]public bool segments = false;
    [UxmlAttribute]public Color32 segmentsColor = Color.white;
    private float m_Progress;
    [UxmlAttribute]public float progress
    {
        get => m_Progress;
        set
        {
            m_Progress = value;
            MarkDirtyRepaint();
        }
    }

    public CircularBarProgression()
    {
        generateVisualContent += GenerateBar;
        progress = 100f;
    }

    void GenerateBar(MeshGenerationContext context)
    {
        float width = contentRect.width;
        float height = contentRect.height;
        float lineWidth = baseLineWidht*width/100f;
        float borderLineWidht = lineWidth/5f;
        var painter = context.painter2D;
        float startAngle = -90f;
        float endAngle = -90f - (360f*progress/100f);
        float segmentsWidth = 5f;

        painter.lineWidth = lineWidth;
        painter.lineCap = LineCap.Butt;

        if(background)
        {
            painter.lineWidth = lineWidth;
            painter.strokeColor = backgroundColor;

            painter.BeginPath();
            painter.Arc(new Vector2(width*0.5f, height*0.5f), height*0.5f, startAngle - 360f, startAngle);
            painter.Stroke();
        }

        painter.lineWidth = lineWidth;
        painter.strokeColor = color;

        painter.BeginPath();
        painter.Arc(new Vector2(width*0.5f, height*0.5f), height*0.5f, endAngle, startAngle);
        painter.Stroke();

        if(border)
        {
            painter.strokeColor = borderColor;
            painter.lineWidth = borderLineWidht;

            painter.BeginPath();
            painter.Arc(new Vector2(width*0.5f, height*0.5f), height*0.5f + lineWidth/2, startAngle - 360f, startAngle);
            painter.Stroke();

            painter.BeginPath();
            painter.Arc(new Vector2(width*0.5f, height*0.5f), height*0.5f - lineWidth/2, startAngle - 360f, startAngle);
            painter.Stroke();


            painter.lineWidth = lineWidth;

            //border du startAngle
            painter.BeginPath();
            painter.Arc(new Vector2(width*0.5f, height*0.5f), height*0.5f, startAngle - borderLineWidht/2, startAngle + borderLineWidht/2);
            painter.Stroke();

            // border du endAngle (S'actualise)
            painter.BeginPath();
            painter.Arc(new Vector2(width*0.5f, height*0.5f), height*0.5f, endAngle - borderLineWidht/2, endAngle + borderLineWidht/2);
            painter.Stroke();



        }

        if(segments)
        {
            painter.strokeColor = segmentsColor;
            painter.lineWidth = lineWidth/2f;

            painter.BeginPath();
            painter.Arc(new Vector2(width*0.5f, height*0.5f), height*0.5f, startAngle - 90f - segmentsWidth/2, startAngle - 90f + segmentsWidth/2);
            painter.Stroke();

            painter.BeginPath();
            painter.Arc(new Vector2(width*0.5f, height*0.5f), height*0.5f, startAngle - 180f - segmentsWidth/2, startAngle - 180f + segmentsWidth/2);
            painter.Stroke();

            painter.BeginPath();
            painter.Arc(new Vector2(width*0.5f, height*0.5f), height*0.5f, startAngle - 270f - segmentsWidth/2, startAngle - 270f + segmentsWidth/2);
            painter.Stroke();
        }
    }
}
