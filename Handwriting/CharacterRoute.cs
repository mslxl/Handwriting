using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Handwriting
{
    public class RelativeRoute
    {
        public RelativeRoute(int startX, int startY, int endX, int endY)
        {
            this.StartX = startX;
            this.StartY = startY;
            this.EndX = endX;
            this.EndY = endY;
        }

        public int StartX;
        public int StartY;
        public int EndX;
        public int EndY;

        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})",StartX,StartY,EndX,EndY);
        }
    }
    public enum CharacterKind
    {
        General,Space,NextLine
    }

    public class CharacterRouteInfo
    {
        public List<RelativeRoute> Routes;
        public int Height;
        public int Width;
        public CharacterKind Kind = CharacterKind.General;
        public int Num = 1;
        public CharacterRouteInfo(List<RelativeRoute> routes)
        {
            this.Routes = routes;
            foreach(var r in routes)
            {
                var w = r.StartX > r.EndX ? r.StartX : r.EndX;
                var h = r.StartY > r.EndY ? r.StartY : r.EndY;
                if (w > Width) Width = w;
                if (h > Height) Height = h;
            }
        }

        public static CharacterRouteInfo operator *(CharacterRouteInfo a, int b)
        {
            if (a.Kind == CharacterKind.General)
            {
                var routes = a.Routes.Select((RelativeRoute route) =>
                {
                    return new RelativeRoute(route.StartX * b, route.StartY * b, route.EndX * b, route.EndY * b);
                });
                var newInfo = new CharacterRouteInfo(routes.ToList());
                return newInfo;
            }
            
            return a;
        }
    }
    public class CharacterRoute
    {
        private static DirectoryInfo CharacterDictionary = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database"));

        private static void CheckDictionary()
        {
            if (!CharacterDictionary.Exists) CharacterDictionary.Create();
        }
    
        private static FileInfo NewCharFile(char c)
        {
            CheckDictionary();
            return new FileInfo(Path.Combine(CharacterDictionary.FullName, "c" + ((int)c)));
        }

        public static bool HasCharacter(char c)
        {
            if (c == ' ' || c == '\n' || c == '\t') return true;
            return NewCharFile(c).Exists;
        }

        public static CharacterRouteInfo GetRouteByChar(char c)
        {
            if (c == ' ' || c == '\n' || c == '\t') {
                var info = new CharacterRouteInfo(new List<RelativeRoute>());
                if(c == ' ')
                {
                    info.Kind = CharacterKind.Space;
                } else if(c == '\t')
                {
                    info.Kind = CharacterKind.Space;
                    info.Num = 4;
                } else if(c == '\n')
                {
                    info.Kind = CharacterKind.NextLine;
                }
                return info;
            }
            var file = NewCharFile(c);
            var paths = new List<RelativeRoute>();
            if (!file.Exists || file.Length % 16 != 0)
            {
                throw new Exception("TODO");
            }
            var reader = file.OpenRead();
            byte[] b = new byte[4];
            for(var i = 0; i < reader.Length / 16; i++)
            {
                int sx, sy, ex, ey;
                reader.Read(b, 0, 4);
                sx = BitConverter.ToInt32(b, 0);
                reader.Read(b, 0, 4);
                sy = BitConverter.ToInt32(b, 0);
                reader.Read(b, 0, 4);
                ex = BitConverter.ToInt32(b, 0);
                reader.Read(b, 0, 4);
                ey = BitConverter.ToInt32(b, 0);
                paths.Add(new RelativeRoute(sx, sy, ex, ey));
            }    
            return new CharacterRouteInfo(paths);
        }

        public static void SaveRouteByChar(char c,List<RelativeRoute> routes)
        {

            var file = NewCharFile(c);
            FileStream writer;
            if (!file.Exists)
                writer = file.Create();
            else
                writer = file.OpenWrite();

            foreach(var route in routes)
            {
                writer.Write(BitConverter.GetBytes(route.StartX), 0, 4);
                writer.Write(BitConverter.GetBytes(route.StartY), 0, 4);
                writer.Write(BitConverter.GetBytes(route.EndX), 0, 4);
                writer.Write(BitConverter.GetBytes(route.EndY), 0, 4);
            }
            writer.Flush();
            writer.Close();
        }
    }
}
