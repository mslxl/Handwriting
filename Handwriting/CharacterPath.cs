using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Handwriting
{
    public class RelativeRoute
    {
        public RelativeRoute(int startX,int startY,int endX,int endY)
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
    public class CharacterPath
    {
        private static DirectoryInfo CharacterDictionary = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database"));

        private static void CheckDictionary()
        {
            if (!CharacterDictionary.Exists) CharacterDictionary.Create();
        }
    
        public static bool HasCharacter(char c)
        {
            return new FileInfo(Path.Combine(CharacterDictionary.FullName, "c" + ((int)c))).Exists;
        }

        public static List<RelativeRoute> GetPathByChar(char c)
        {
            CheckDictionary();
            var file = new FileInfo(Path.Combine(CharacterDictionary.FullName,"c" + ((int)c)));
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
            return paths;
        }

        public static void SavePathByChar(char c,List<RelativeRoute> routes)
        {
            CheckDictionary();
            var file = new FileInfo(Path.Combine(CharacterDictionary.FullName, "c" + ((int)c)));
            if (!file.Exists) file.Create();
            var writer = file.OpenWrite();
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
