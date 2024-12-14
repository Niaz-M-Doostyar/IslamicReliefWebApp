using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using OfficeOpenXml.Style;

namespace IR_Admin.Helpers
{
    public static class ExcelStyleHelper
    {
        public static void ApplyHeaderStyle(this ExcelStyle style)
        {
            style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            style.VerticalAlignment = ExcelVerticalAlignment.Center;
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(Color.FromArgb(0xF0, 0xF8, 0xFF)); // AliceBlue
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.WrapText = true;
            ApplyBorder(style.Border); 
        }

        // Extension method to apply merged header style
        public static void ApplyMergedHeaderStyle(this ExcelStyle style)
        {
            style.ApplyHeaderStyle();
            style.Font.Size = 18;
            style.Fill.BackgroundColor.SetColor(Color.FromArgb(0x64, 0x95, 0xED)); // CornflowerBlue
            ApplyBorder(style.Border);
        }

        public static void ApplyBorder(this Border style)
        {
            style.Top.Style = ExcelBorderStyle.Thin;
            style.Left.Style = ExcelBorderStyle.Thin;
            style.Right.Style = ExcelBorderStyle.Thin;
            style.Bottom.Style = ExcelBorderStyle.Thin;
            style.Top.Color.SetColor(Color.Black);
            style.Left.Color.SetColor(Color.Black);
            style.Right.Color.SetColor(Color.Black);
            style.Bottom.Color.SetColor(Color.Black);
        }

    }
}