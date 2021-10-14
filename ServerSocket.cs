using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;


namespace MineHopWebsite
{
    public class ServerSocket
    {
        private int port;
        private Socket serverSocket;

        public ServerSocket(int port)
        {
            this.port = port;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("205.144.171.246"), port);

            /* Associates a Socket with a local endpoint. */
            serverSocket.Bind(serverEndPoint);

            /*Places a Socket in a listening state.
             * The maximum length of the pending connections queue is 100 
             */
            serverSocket.Listen(100);
        }

        public void start()
        {
            Console.WriteLine("Starting the Server");

            /* Accept Connection Requests */

            Socket accepted;
            while (true)
            {
                accepted = serverSocket.Accept();
                /* Get the size of the send buffer of the Socket. */
                int bufferSize = accepted.SendBufferSize;
                byte[] buffer = new byte[bufferSize];

                /* Receives data from a bound Socket into a receive buffer. It return the number of bytes received. */
                int bytesRead = accepted.Receive(buffer);

                byte[] formatted = new byte[bytesRead];

                for (int i = 0; i < bytesRead; i++)
                {
                    formatted[i] = buffer[i];
                }

                String receivedData = Encoding.UTF8.GetString(formatted);

                switch (receivedData.Split("$$")[0])
                {
                    case "leaderboard":
                        Program.leaderboard = new List<LeaderElement>();
                        //leaderboard$$MAP_NAME##PLAYER_NAME##TIME~~MAP_NAME##PLAYER_NAME##TIME
                        String[] elements = receivedData.Split("$$")[1].Split("~~");
                        for (int i = 0; i < elements.Length; i++)
                        {
                            String[] stuff = elements[i].Split("##");
                            Program.leaderboard.Add(new LeaderElement(stuff[0], stuff[1], stuff[2]));
                        }
                        //Program.UpdateLeaderBoard();
                        break;
                }
            }
        }

        /*public ActionResult UpdateLeaderboard(ArrayList list)
        {
            foreach (LeaderElement element in list)
            {
                Console.WriteLine(element.playerName);
            }
        }*/
    }
}