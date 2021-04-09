using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using color;
using geoLib;

namespace Variance
{
    using Polygon = List<GeoLibPointF>;
    using Polygons = List<List<GeoLibPointF>>;
    //a very simple class that builds an SVG file with any number of 
    //polygons of the specified formats ...
    public class SVGBuilder
    {
        public class StyleInfo
        {
            public Int32 pft { get; set; }
            public MyColor brushClr { get; set; }
            public MyColor penClr { get; set; }
            public double penWidth { get; set; }
            public Int32[] dashArray { get; set; }
            public Boolean showCoords { get; set; }
            public StyleInfo Clone()
            {
                StyleInfo si = new StyleInfo();
                si.pft = pft;
                si.brushClr = brushClr;
                si.dashArray = dashArray;
                si.penClr = penClr;
                si.penWidth = penWidth;
                si.showCoords = showCoords;
                return si;
            }
            public StyleInfo()
            {
                pft = 0; // PolyFillType.pftNonZero;
                brushClr = MyColor.AntiqueWhite;
                dashArray = null;
                penClr = MyColor.Black;
                penWidth = 0.8;
                showCoords = false;
            }
        }

        public class PolyInfo
        {
            public Polygons polygons { get; set; }
            public StyleInfo si { get; set; }
            //public Color pi_color;
        }

        public class BoundingRect
        {
            public double bottom { get; set; }
            public double top { get; set; }
            public double left { get; set; }
            public double right { get; set; }
        }

        public StyleInfo style;
        private List<PolyInfo> PolyInfoList;
        const string svg_header = "<?xml version=\"1.0\" standalone=\"no\"?>\n" +
        "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.0//EN\"\n" +
        "\"http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd\">\n\n" +
        "<svg width=\"{0}px\" height=\"{1}px\" viewBox=\"0 0 {2} {3}\" " +
        "version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">\n\n";
        const string svg_path_format = "\"\n style=\"fill:{0};" +
          " fill-opacity:0; fill-rule:{2}; stroke:{3};" +
          " stroke-opacity:{4:f2}; stroke-width:{5:f2};\"/>\n\n";

        public SVGBuilder()
        {
            PolyInfoList = new List<PolyInfo>();
            style = new StyleInfo();
        }

        public void AddPolygons(List<GeoLibPointF[]> pointArrayList)
        {
            Polygons tempPolygonsList = new Polygons();
            for (int listMember = 0; listMember < pointArrayList.Count(); listMember++)
            {
                Polygon tempPolygon = new Polygon();
                for (int pt = 0; pt < pointArrayList[listMember].Count(); pt++)
                {
                    tempPolygon.Add(pointArrayList[listMember][pt]);
                }
                tempPolygonsList.Add(tempPolygon.ToList());
            }
            AddPolygons(tempPolygonsList.ToList());
        }

        public void AddPolygons(GeoLibPointF[] pointArray)
        {
            Polygons tempPolygonsList = new Polygons();
            Polygon tempPolygon = new Polygon();
            for (int pt = 0; pt < pointArray.Count(); pt++)
            {
                tempPolygon.Add(pointArray[pt]);
            }
            tempPolygonsList.Add(tempPolygon.ToList());
            AddPolygons(tempPolygonsList.ToList());
        }

        public void AddPolygons(Polygons poly)
        {
            if (poly.Count == 0) return;
            PolyInfo pi = new PolyInfo();
            pi.polygons = poly;
            pi.si = style.Clone();
            PolyInfoList.Add(pi);
        }

        public Boolean SaveToFile(string filename, double scale = 10.0, Int32 margin = 10)
        {
            // if (scale == 0) scale = 1.0;
            // if (margin < 0) margin = 0;

            //calculate the bounding rect ...
            Int32 i = 0, j = 0;
            while (i < PolyInfoList.Count)
            {
                j = 0;
                while (j < PolyInfoList[i].polygons.Count &&
                    PolyInfoList[i].polygons[j].Count == 0) j++;
                if (j < PolyInfoList[i].polygons.Count) break;
                i++;
            }
            if (i == PolyInfoList.Count) return false;
            BoundingRect rec = new BoundingRect();
            rec.left = PolyInfoList[i].polygons[j][0].X;
            rec.right = rec.left;
            rec.top = PolyInfoList[0].polygons[j][0].Y;
            rec.bottom = rec.top;

            for (; i < PolyInfoList.Count; i++)
            {
                foreach (Polygon pg in PolyInfoList[i].polygons)
                    foreach (GeoLibPointF pt in pg)
                    {
                        if (pt.X < rec.left) rec.left = pt.X;
                        else if (pt.X > rec.right) rec.right = pt.X;
                        if (pt.Y < rec.top) rec.top = pt.Y;
                        else if (pt.Y > rec.bottom) rec.bottom = pt.Y;
                    }
            }

            rec.left = rec.left * scale;
            rec.top = rec.top * scale;
            rec.right = rec.right * scale;
            rec.bottom = rec.bottom * scale;
            double offsetX = -rec.left + margin;
            double offsetY = -rec.top + margin;

            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.Write(svg_header,
                    (rec.right - rec.left) + margin * 2,
                    (rec.bottom - rec.top) + margin * 2,
                    (rec.right - rec.left) + margin * 2,
                    (rec.bottom - rec.top) + margin * 2);

                foreach (PolyInfo pi in PolyInfoList)
                {
                    writer.Write(" <path d=\"");
                    foreach (Polygon p in pi.polygons)
                    {
                        if (p.Count < 3) continue;
                        writer.Write(String.Format(NumberFormatInfo.InvariantInfo, " M {0:f2} {1:f2}",
                            p[0].X * scale + offsetX,
                            p[0].Y * scale + offsetY));
                        for (Int32 k = 1; k < p.Count; k++)
                        {
                            writer.Write(String.Format(NumberFormatInfo.InvariantInfo, " L {0:f2} {1:f2}",
                            p[k].X * scale + offsetX,
                            p[k].Y * scale + offsetY));
                        }
                        writer.Write(" z");
                    }

                    writer.Write(String.Format(NumberFormatInfo.InvariantInfo, svg_path_format,
                    pi.si.brushClr.ToHtml(),
                    (float)pi.si.brushClr.A / 255,
                    (pi.si.pft == 0),
                    pi.si.penClr.ToHtml(),
                    (float)pi.si.penClr.A / 255,
                    pi.si.penWidth));

                    if (pi.si.showCoords)
                    {
                        writer.Write("<g font-family=\"Verdana\" font-size=\"11\" fill=\"black\">\n\n");
                        foreach (Polygon p in pi.polygons)
                        {
                            foreach (GeoLibPointF pt in p)
                            {
                                double x = pt.X;
                                double y = pt.Y;
                                writer.Write(String.Format(
                                    "<text x=\"{0}\" y=\"{1}\">{2},{3}</text>\n",
                                    (x * scale + offsetX), (y * scale + offsetY), x, y));

                            }
                            writer.Write("\n");
                        }
                        writer.Write("</g>\n");
                    }
                }
                writer.Write("</svg>\n");
            }
            return true;
        }
    }
}
