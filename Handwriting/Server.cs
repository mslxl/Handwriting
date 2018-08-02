using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Handwriting
{
    class Server
    {
        private static Server Instance = null;
        public static Server GetInstance()
        {
            var port = 8288;
            if(Instance == null)
            {
                Instance = new Server();
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(IPAddress.Any,port);
                socket.Bind(endPoint);
                socket.Listen(10);
                Instance.Socket = socket.Accept();
            }
            return Instance;
        }

        private Socket Socket;

        public List<RelativeRoute> RequestCharacterWithTcp(char c)
        {
            var list = new List<RelativeRoute>();
            byte[] b = new byte[4];
            var end = false;
            do
            {
                Socket.Receive(b);
                var sx = BitConverter.ToInt32(b,0);

                Socket.Receive(b);
                var sy = BitConverter.ToInt32(b, 0);

                Socket.Receive(b);
                var ex = BitConverter.ToInt32(b, 0);

                Socket.Receive(b);
                var ey = BitConverter.ToInt32(b, 0);

                if (sx != 0 && sy != 0 && ex != 0 && ey != 0)
                {
                    var route = new RelativeRoute(sx, sy, ex, ey);
                    Console.WriteLine(route);
                    list.Add(route);
                } else
                {
                    end = true;
                }
            } while (!end);
            return list;

        }
        
    }
}
