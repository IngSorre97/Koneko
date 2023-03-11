using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGrid : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }
    public FitType fitType;
    
    public int rows;
    public int columns;
    public Vector2 cellSize;

    public Vector2 spacing;

    public bool fitX;
    public bool fitY;

    public bool applyAspectRatio;
    public Vector2 aspectRatio;
    

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (transform.childCount == 0) return;
        
        float sqRt = Mathf.Sqrt(transform.childCount);
        switch (fitType)
        {
            case FitType.Uniform:
                rows = Mathf.CeilToInt(sqRt);
                columns = Mathf.CeilToInt(sqRt);
                fitX = true;
                fitY = true;
                break;
            case FitType.Width:
                columns = Mathf.CeilToInt(sqRt);
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                fitX = true;
                fitY = true;
                break;
            case FitType.Height:
                rows = Mathf.CeilToInt(sqRt);
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                fitX = true;
                fitY = true;
                break;
            case FitType.FixedColumns:
                fitX = true;
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                break;
            case FitType.FixedRows:
                fitY = true;
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                break;
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;
        
        float cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * 2) - 
                          (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = (parentHeight / (float)rows) - ((spacing.y/(float)rows) * 2) - 
                           (padding.top / (float)rows) - (padding.bottom / (float)rows);

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount;
        int rowCount;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            if (applyAspectRatio && aspectRatio.x != 0 && aspectRatio.y != 0)
            {
                if (aspectRatio.x < aspectRatio.y)
                {
                    cellSize.x = cellSize.y / aspectRatio.y * aspectRatio.x;
                }
                else
                {
                    cellSize.y = cellSize.x / aspectRatio.x * aspectRatio.y;
                }
            }
            
            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }
}
