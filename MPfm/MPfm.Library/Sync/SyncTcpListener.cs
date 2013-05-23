// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MPfm.Library.Sync
{
    public class SyncTcpListener
    {
        private TcpListener server;

        public int Port { get; private set; }

        public SyncTcpListener(int port)
        {
            Port = port;
            Initialize();
        }

        private void Initialize()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, Port);
        }

        public void Start()
        {
            try
            {
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop. 
                while(true) 
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests. 
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();            
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    // Loop to receive all the data sent by the client. 
                    int i;
                    while((i = stream.Read(bytes, 0, bytes.Length))!=0) 
                    {   
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);            
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch(SocketException ex)
            {
                Console.WriteLine("SyncTcpListener - SocketException: {0}", ex);
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncTcpListener - Exception: {0}", ex);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }
    }
}
