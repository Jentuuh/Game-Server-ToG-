﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Dynamic;
using System.ComponentModel.DataAnnotations;

namespace TowerOfGodServer
{
    class Server
    {
        public static int m_max_players { get; private set; }
        public static int m_port { get; private set; }
        public static Dictionary<int, Client> client_dictionary = new Dictionary<int, Client>();

        private static TcpListener m_tcp_listener;

        public static void Start(int max_players, int port_nr)
        {
            m_max_players = max_players;
            m_port = port_nr;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            m_tcp_listener = new TcpListener(IPAddress.Any, m_port);
            m_tcp_listener.Start();
            m_tcp_listener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server started on {port_nr}. ");
        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = m_tcp_listener.EndAcceptTcpClient(result);
            m_tcp_listener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}");

            for(int i = 1; i <= m_max_players; i++)
            {
                // If this client slot wasn't taken yet
                if(client_dictionary[i].m_tcp.m_socket == null)
                {
                    // Connect the client
                    client_dictionary[i].m_tcp.Connect(client);
                    return;
                }
            }
            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void InitializeServerData()
        {
            for(int i = 1; i <= m_max_players; i++)
            {
                client_dictionary.Add(i, new Client(i));
            }
        }
    }
}
