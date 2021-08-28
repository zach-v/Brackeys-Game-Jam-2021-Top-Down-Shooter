using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
	public enum FitType
	{
		Uniform, Width, Height, FixedRows, FixedColumns
	}
	public FitType fitType;
	public int rows;
	public int columns;
	public Vector2 spacing;
	public bool fitX;
	public bool fitY;
	[ReadOnly] public Vector2 cellSize;
	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
		{
			// Row and column calculation
			float sqrt = Mathf.Sqrt(transform.childCount);
			rows = Mathf.CeilToInt(sqrt);
			columns = Mathf.CeilToInt(sqrt);
		}
		// Fit type consideration
		if (fitType == FitType.Width || fitType == FitType.FixedColumns)
		{
			rows = Mathf.CeilToInt(transform.childCount / (float)columns);
		}
		if (fitType == FitType.Height || fitType == FitType.FixedRows)
		{
			columns = Mathf.CeilToInt(transform.childCount / (float)rows);
		}
		// Get the parent dimensions
		float parentWidth = rectTransform.rect.width;
		float parentHeight = rectTransform.rect.height;
		// Cell calculations
		float cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * 2) - (padding.left / (float)columns) - (padding.right / (float)columns);
		float cellHeight = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * 2) - (padding.top / (float)rows) - (padding.bottom / (float)rows);
		// Set it to be visible in inspector
		cellSize.x = fitX ? cellWidth : cellSize.x;
		cellSize.y = fitY ? cellHeight : cellSize.y;
		// Iterate through the children with respect to the cell counts
		int columnCount = 0;
		int rowCount = 0;
		for (int i = 0; i < rectChildren.Count; i++)
		{
			rowCount = i / columns;
			columnCount = i % columns;

			var item = rectChildren[i];
			// Set position based off padding and spacing
			var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
			var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

			SetChildAlongAxis(item, 0, xPos, cellSize.x);
			SetChildAlongAxis(item, 1, yPos, cellSize.y);
		}
	}
	public void AddGunButton(Gun newGun)
	{

	}
	public override void CalculateLayoutInputVertical() {}
	public override void SetLayoutHorizontal() {}
	public override void SetLayoutVertical() {}
}
