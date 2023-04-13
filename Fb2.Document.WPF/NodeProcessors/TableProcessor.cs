﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Fb2.Document.Constants;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.WPF.Entities;
using Fb2.Document.WPF.NodeProcessors.Base;
using Fb2Table = Fb2.Document.Models.Table;
using Fb2TableRow = Fb2.Document.Models.TableRow;
using Paragraph = System.Windows.Documents.Paragraph;
using Fb2TableCell = Fb2.Document.Models.TableCell;

namespace Fb2.Document.WPF.NodeProcessors;

public class TableProcessor : NodeProcessorBase
{
    public override List<TextElement> Process(RenderingContext context)
    {
        var table = context.CurrentNode as Fb2Table;

        var content = table.Content;

        var tableView = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var rowsCount = content.Count;

        var tableRowSpans = new List<RowSpanModel>();

        for (var rowIndex = 0; rowIndex < rowsCount; rowIndex++)
        {
            var collSpanDelta = 0;

            tableView.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });

            var row = content[rowIndex] as Fb2TableRow;

            if (row == null)
                continue;

            for (int columnIndex = 0; columnIndex < row.Content.Count; columnIndex++)
            {
                var resultingColumn = collSpanDelta != 0 ? columnIndex + collSpanDelta : columnIndex;

                if (tableView.ColumnDefinitions.Count == 0 || tableView.ColumnDefinitions.Count <= resultingColumn)
                    tableView.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Auto)
                    });

                var cellNode = row.Content[columnIndex];

                resultingColumn = ApplyColumnSpanDelta(tableRowSpans, rowIndex, resultingColumn);

                // TODO: later add config to pass settings
                var cellContainer = CreateCellContainer(cellNode, rowIndex, resultingColumn);

                var cellContentPresenter = CreateCellContentPresenter(cellNode, context);

                cellContainer.Child = cellContentPresenter;

                Grid.SetRow(cellContainer, rowIndex);
                Grid.SetColumn(cellContainer, resultingColumn);

                var collSpanNum = TryGetSpan(cellNode, AttributeNames.ColumnSpan);
                Grid.SetColumnSpan(cellContainer, collSpanNum);
                collSpanDelta += collSpanNum - 1;

                var rowSpanNum = TryGetSpan(cellNode, AttributeNames.RowSpan);
                // add columnspan to model if one cell has both
                Grid.SetRowSpan(cellContainer, rowSpanNum);
                var affectedRowEndIndex = rowIndex + (rowSpanNum - 1);

                tableRowSpans.Add(new RowSpanModel(columnIndex, rowIndex, affectedRowEndIndex, resultingColumn, collSpanNum));

                tableView.Children.Add(cellContainer);
            }
        }

        var scroll = CreateScrollView();
        scroll.Content = tableView;

        var outerBorder = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Child = scroll
        };

        var result = new InlineUIContainer { Child = outerBorder };

        return new List<TextElement>(1) { result };
    }

    private int ApplyColumnSpanDelta(List<RowSpanModel> tableRowSpans, int rowIndex, int resultingColumn)
    {
        if (!tableRowSpans.Any())
            return resultingColumn;

        var effectiveRowSpans = tableRowSpans.Where(trs => trs.StartRowIndex < rowIndex && trs.EndRowIndex >= rowIndex);

        if (effectiveRowSpans == null || !effectiveRowSpans.Any())
            return resultingColumn;

        var orderedActuals = effectiveRowSpans.OrderBy(trs => trs.ActualColumnIndex).ThenBy(trs => trs.StartRowIndex);

        foreach (var cellIndexDelta in orderedActuals)
        {
            if (cellIndexDelta.ColumnIndex > resultingColumn ||
                cellIndexDelta.ActualColumnIndex > resultingColumn)
                break;

            resultingColumn += cellIndexDelta.ColSpan;
        }

        return resultingColumn;
    }

    private ScrollViewer CreateScrollView() => new ScrollViewer
    {
        HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
        PanningMode = PanningMode.Both,
        //HorizontalScrollMode = ScrollMode.Enabled,
        VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
        //VerticalScrollMode = ScrollMode.Enabled,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
        MinWidth = 500,
        Padding = new Thickness(0, 5, 0, 12),
        //ZoomMode = ZoomMode.Disabled
    };

    private Border CreateCellContainer(Fb2Node cellModel, int rowIndex, int columnIndex)
    {
        var cellContainer = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var cellBorderBrush = new SolidColorBrush(Colors.DarkGray);

        //TODO: configurable
        var cellBorderThickness = new Thickness(0, 0, 1, 1);

        if (cellModel is TableHeader)
        {
            cellContainer.Background = new SolidColorBrush(Colors.Gray);
            cellContainer.BorderBrush = cellBorderBrush;
            cellContainer.BorderThickness = cellBorderThickness;
        }
        else if (cellModel is Fb2TableCell)
        {
            if (rowIndex == 0)
                cellBorderThickness.Top += 1;

            if (columnIndex == 0)
                cellBorderThickness.Left += 1;

            cellContainer.BorderBrush = cellBorderBrush;
            cellContainer.BorderThickness = cellBorderThickness;
        }

        return cellContainer;
    }

    private RichTextBox CreateCellContentPresenter(Fb2Node cellModel, RenderingContext context)
    {
        //var textPresenter = new RichTextBlock
        //{
        //    FontSize = context.RenderingConfig.BaseFontSize,
        //    HorizontalAlignment = HorizontalAlignment.Stretch,
        //    // TODO : configurable ?
        //    Padding = new Thickness(7)
        //};

        var textPresenter = new RichTextBox
        {
            IsReadOnly = true,
            IsReadOnlyCaretVisible = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch,
            MinWidth = 100
        };
        var flowDocument = new FlowDocument
        {
            FontSize = context.RenderingConfig.BaseFontSize,
            ColumnWidth = 99999999,
            IsColumnWidthFlexible = false
            //PageWidth = 100
            //MinPageWidth = 100
        };

        if (cellModel.TryGetAttribute(AttributeNames.Align, true, out var align) &&
            Enum.TryParse<TextAlignment>(align.Value, true, out var horAlign))
            flowDocument.TextAlignment = horAlign;

        if (cellModel.TryGetAttribute(AttributeNames.VerticalAlign, true, out var verAlign) &&
            Enum.TryParse<VerticalAlignment>(verAlign.Value, true, out var verticalAlignment))
            textPresenter.VerticalAlignment = verticalAlignment;

        var cellContent = ElementSelector(cellModel, context);

        var cellBlocks = context.Utils.Paragraphize(cellContent);

        foreach (var paragraph in cellBlocks)
            flowDocument.Blocks.Add(paragraph as Paragraph);

        textPresenter.Document = flowDocument;

        return textPresenter;
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

    private class RowSpanModel
    {
        public int ColumnIndex { get; private set; }
        public int StartRowIndex { get; private set; }
        public int EndRowIndex { get; private set; }
        public int ColSpan { get; private set; }
        public int ActualColumnIndex { get; private set; }

        public RowSpanModel(
            int columnIndex,
            int rowIndex,
            int affectedRowEndIndex,
            int actualColumnIndex,
            int colSpan = 0)
        {
            ColumnIndex = columnIndex;
            StartRowIndex = rowIndex;
            EndRowIndex = affectedRowEndIndex;
            ActualColumnIndex = actualColumnIndex;
            ColSpan = colSpan;
        }
    }
}
