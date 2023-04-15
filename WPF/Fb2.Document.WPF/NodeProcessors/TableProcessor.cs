using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Fb2.Document.Constants;
using Fb2.Document.Models.Base;
using Fb2.Document.WPF.Entities;
using Fb2.Document.WPF.NodeProcessors.Base;
using Fb2Table = Fb2.Document.Models.Table;
using Fb2TableRow = Fb2.Document.Models.TableRow;
using Table = System.Windows.Documents.Table;
using TableCell = System.Windows.Documents.TableCell;
using TableRow = System.Windows.Documents.TableRow;

namespace Fb2.Document.WPF.NodeProcessors;

public class TableProcessor : NodeProcessorBase
{

    public override List<TextElement> Process(RenderingContext context)
    {
        var fb2Table = context.CurrentNode as Fb2Table;
        var content = fb2Table.Content;


        var table = new Table();
        table.BorderThickness = new Thickness(1);
        table.BorderBrush = Brushes.DarkGray;
        table.CellSpacing = 0;

        var rowGroup = new TableRowGroup();


        var rowsCount = content.Count;

        for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
        {
            var row = content[rowIndex] as Fb2TableRow;

            if (row == null)
                continue;

            var tableRow = new TableRow();

            var rowContent = row.Content;
            for (int columnIndex = 0; columnIndex < rowContent.Count; columnIndex++)
            {
                var tableCell = new TableCell();
                tableCell.BorderBrush = Brushes.DarkGray;
                tableCell.BorderThickness = new Thickness(1);
                tableCell.Padding = new Thickness(8);

                var cellNode = row.Content[columnIndex];

                var cellContent = ElementSelector(cellNode, context);
                var cellBlocks = context.Utils.Paragraphize(cellContent);

                if (cellNode.TryGetAttribute(AttributeNames.Align, true, out var align) &&
                    Enum.TryParse<TextAlignment>(align.Value, true, out var horAlign))
                    tableCell.TextAlignment = horAlign;

                if (cellNode.TryGetAttribute(AttributeNames.VerticalAlign, true, out var verAlign) &&
                    Enum.TryParse<VerticalAlignment>(verAlign.Value, true, out var verticalAlignment))
                {
                    // dirty trick #1
                    var margin = verticalAlignment switch
                    {
                        VerticalAlignment.Top => new Thickness(0, 0, 0, 30),
                        VerticalAlignment.Center => new Thickness(0, 30, 0, 30),
                        VerticalAlignment.Bottom => new Thickness(0, 30, 0, 0),
                        VerticalAlignment.Stretch => new Thickness(0),
                    };

                    foreach (var block in cellBlocks)
                    {
                        (block as Block).Margin = margin;
                    }
                }

                tableCell.Blocks.AddRange(cellBlocks);

                var collSpanNum = TryGetSpan(cellNode, AttributeNames.ColumnSpan);
                if (collSpanNum > 1)
                    tableCell.ColumnSpan = collSpanNum;

                var rowSpanNum = TryGetSpan(cellNode, AttributeNames.RowSpan);
                if (rowSpanNum > 1)
                    tableCell.RowSpan = rowSpanNum;

                tableRow.Cells.Add(tableCell);

            }

            rowGroup.Rows.Add(tableRow);
        }

        table.RowGroups.Add(rowGroup);

        return new List<TextElement>(1) { table };
    }

    private int TryGetSpan(Fb2Node cell, string spanAttrName)
    {
        if (cell == null || string.IsNullOrWhiteSpace(spanAttrName))
            throw new ArgumentNullException();

        if (cell.TryGetAttribute(spanAttrName, true, out var span) &&
            int.TryParse(span.Value, out int spanNumber) && spanNumber > 1)
            return spanNumber;

        return 1;
    }
}
