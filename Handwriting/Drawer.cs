using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Handwriting
{

    // 该 Class 到处都是 Bug
    // 且作者几天就会忘掉这些是干什么的
   
    class Drawer
    {
        private List<CharacterRouteInfo> list;
        private int MaxCharHeight;
        private int AverageCharWidth;
        private int Width;
        private int Height;

        private int FontSize;
        private int FontRouteSize;

        private int Left;
        private int Right;
        private int Top;
        private int Bottom;

        private int LineSpacing;
        private int CharSpacing;

        private bool AutoWidth = true;

        public void InitAllRoutes(String text)
        {
            list = new List<CharacterRouteInfo>(text.Length);
           
            text.ToList().ForEach((char c)=> {
                if (!CharacterRoute.HasCharacter(c))
                {
                    var routes = RecordCharacter.RequestCharacterRoute(c);
                    CharacterRoute.SaveRouteByChar(c, routes);
                }
                var route = CharacterRoute.GetRouteByChar(c) * FontSize;
                if (route.Height > MaxCharHeight) MaxCharHeight = route.Height;
                AverageCharWidth += route.Width;
                list.Add(route);
            });
            AverageCharWidth /= text.Length;
        }

        public void CreatePaperTemplate(int width,int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void SetPen(int size,int lineSize)
        {
            this.FontSize = size;
            this.FontRouteSize = lineSize;
        }

        public void SetMargin(int left,int right,int top,int bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        public void SetSpacing(int line,int spacing)
        {
            this.LineSpacing = line;
            this.CharSpacing = spacing;
        }

        private Canvas CreatePaper() { 

            var canvas = new Canvas();
            canvas.Width = Width;
            canvas.Height = Height;
            return canvas;
        }

        public List<Canvas> Draw()
        {
            int PosX = Left;
            int PosY = Top;
            var canvases = new List<Canvas>();
            var canvas = CreatePaper();
            foreach (var info in list)
            {
                if(info.Kind == CharacterKind.Space)
                {
                    info.Width = AverageCharWidth * info.Num;
                    info.Height = MaxCharHeight;
                } else if(info.Kind == CharacterKind.NextLine)
                {
                    // 暴力宽度以换行
                    info.Width = Width - Right - PosX - 1;
                }
            

                // 检查是否能写下这次的字
                if(PosX + info.Width > Width - Right)
                {
                    // 写不下
                    // 检查是否能换行
                    if(PosY + MaxCharHeight + LineSpacing > Height - Bottom - MaxCharHeight)
                    {
                        // 不能换行
                        // 换页
                        canvases.Add(canvas);
                        canvas = CreatePaper();
                        PosX = Left;
                        PosY = Top;
                    } else
                    {
                        // 可以换行
                        PosX = Left;
                        PosY += MaxCharHeight + LineSpacing;
                    }
                }
                // 检查完毕后肯定能写下
                if (info.Kind == CharacterKind.General)
                {
                    // 不是 General 也没必要画


                    // 对齐下端所需偏移量
                    //var offsetY = MaxCharHeight - info.Height;
                    var offsetY = 0;
                    foreach (var pair in info.Routes)
                    {
                        var line = new Line();
                        line.Stroke = Brushes.Black;
                        line.StrokeDashCap = PenLineCap.Round;
                        line.StrokeThickness = FontRouteSize;
                        line.X1 = pair.StartX + PosX;
                        line.Y1 = pair.StartY + offsetY + PosY;
                        line.X2 = pair.EndX + PosX;
                        line.Y2 = pair.EndY + offsetY + PosY;

                        Console.WriteLine("Draw Line at ({0},{1},{2},{3})", line.X1, line.Y1, line.X2, line.Y2);
                        canvas.Children.Add(line);
                    }
                }
               
                // 下次位置后移
                PosX += info.Width + CharSpacing;
            }
            canvases.Add(canvas);
            return canvases;
        }
    }
}
