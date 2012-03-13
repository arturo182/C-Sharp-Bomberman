using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace Server
{
  public partial class mainForm: Form
  {
    private Socket m_socket;
    private List<Player> m_players = new List<Player>();
    private List<Point> m_spawns = new List<Point>();
    private List<Bomb> m_bombs = new List<Bomb>();
    private List<List<Field>> m_map = new List<List<Field>>();
    private Random m_rand = new Random();
    private string m_currentMap = "";
    private double m_roundTime = 5.0 * 60.0;
    private int m_bombLimit = 3;
    private double m_currentTime = 0;

    public mainForm()
    {
      InitializeComponent();
    }

    private void addLog(string log)
    {
      logTextBox.Invoke(new Action(() => logTextBox.AppendText(log + Environment.NewLine)));
    }

    private void startButton_Click(object sender, EventArgs e)
    {
      preloadRound();

      IPEndPoint ip = new IPEndPoint(IPAddress.Any, 182);

      m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      m_socket.Bind(ip);
      m_socket.Listen(4);
      m_socket.BeginAccept(new AsyncCallback(clientConnected), null);
      //m_mainSocket.BeginDisconnect(true, new AsyncCallback(clientDisconnected), null);

      startRound();

      addLog(String.Format("Serwer uruchomiony, nasłuchuje na {0}:{1}.", localIP(), ip.Port));
    }

    private void startRound()
    {
      addLog("Początek nowej rundy!");
      roundTmer.Start();
    }

    private void preloadRound()
    {
      loadRandomMap();

      m_currentTime = m_roundTime;
    }

    private void loadRandomMap()
    {
      string [] maps = Directory.GetFiles("maps/", "*.txt");
      addLog(String.Join(" ", maps));

      if(maps.Length > 0) {
        string map = maps[m_rand.Next(maps.Length)];

        while(map == m_currentMap) {
          map = maps[m_rand.Next(maps.Length)];
        }

        loadMap(map);
      } else {
        addLog("Brak map!");
      }
    }

    private void loadMap(string fileName)
    {
      if(File.Exists(fileName)) {
        using(StreamReader sr = File.OpenText(fileName)) {
          string s = "";
          int y = 0;
          int type;
          while((s = sr.ReadLine()) != null) {
            List<Field> list = new List<Field>();
            m_map.Add(list);

            for(int x = 0; x < s.Length; x++) {
              type = Convert.ToInt32(s.Substring(x, 1));

              Field field = new Field();

              if((FieldType)type == FieldType.Spawn) {
                m_spawns.Add(new Point(x, y));

                type = (int)FieldType.Nothing;
              }

              field.type = (FieldType)type;

              m_map.Last().Add(field);
            }

            y++;
          }
        }

        m_currentMap = fileName;
        addLog(String.Format("Zaladowano mape \"{0}\"!", Path.GetFileNameWithoutExtension(fileName)));
      } else {
        addLog("Plik mapy nie istnieje!");
      }
    }

    private void clientConnected(IAsyncResult asyn)
    {
      try {
        Socket sock = m_socket.EndAccept(asyn);

        addLog(String.Format("Nowy gracz z IP {0}.", sock.RemoteEndPoint.ToString()));

        waitForData(sock);

        Player player = new Player();
        player.socket = sock;
        m_players.Add(player);

        m_socket.BeginAccept(new AsyncCallback(clientConnected), null);
      } catch(SocketException e) {
        MessageBox.Show(e.ToString());
      }
    }

    private void clientDisconnected(IAsyncResult asyn)
    {
      addLog("Rozlaczony!");
      m_socket.BeginDisconnect(true, new AsyncCallback(clientDisconnected), null);
    }

    private void dataReceived(IAsyncResult asyn)
    {
      Packet packet = (asyn.AsyncState as Packet);

      try {
        int size = packet.socket.EndReceive(asyn);

        String packet_string = new String(Encoding.UTF8.GetChars(packet.buffer, 0, size));
        String[] packet_data = packet_string.Split(' ');
        //MessageBox.Show(packet_string);

        PacketType type = (PacketType)Convert.ToInt32(packet_data[0]);

        String data = "";
        for(int i = 1; i < packet_data.Length; i++) {
          data += packet_data[i] + " ";
        }

        data = data.TrimEnd();

        Player player = playerBySocket(packet.socket);

        switch(type) {
          case PacketType.Chat:
            sendData(String.Format("{0} {1}: {2}", (int)PacketType.Chat, player.name, data));
            addLog(String.Format("{0}: {1}", player.name, data));
          break;

          case PacketType.Name:
            if(playerByName(data) != null) {
              sendDataTo(String.Format("{0}", (int)PacketType.NameExists), player);
              m_players.Remove(player);
              packet.socket.Close();
              return;
            } else {
              player.name = data;
              addLog(String.Format("Gracz {0} znany jest teraz jako \"{1}\".", m_players.IndexOf(player), player.name));
              sendDataTo(String.Format("{0} {1}", (int)PacketType.Map, serializeMap()), player);
            }
          break;

          case PacketType.MapOK:
            int rand = m_rand.Next(m_spawns.Count-1);
            Point spawn = m_spawns.ElementAt(rand);

            player.x = spawn.X;
            player.y = spawn.Y;
            player.color = randomColor();

            sendData(String.Format("{0} {1} {2} {3} {4}", (int)PacketType.PlayerAdd, spawn.X, spawn.Y, player.name, player.color.ToArgb()), player);
            sendData(String.Format("{0} Dołącza do nas {1}", (int)PacketType.Chat, player.name), player);
            sendDataTo(String.Format("{0} {1} {2} {3}", (int)PacketType.Spawn, spawn.X, spawn.Y, player.color.ToArgb()), player);
            foreach(Player playa in m_players) {
              if(playa != player) {
                sendDataTo(String.Format("{0} {1} {2} {3} {4}", (int)PacketType.PlayerAdd, playa.x, playa.y, playa.name, playa.color.ToArgb()), player);
              }
            }
          break;

          case PacketType.MoveRequest:
            if(data == "bomb") {
              if(player.bombs < m_bombLimit) {
                Bomb bomb = new Bomb();
                bomb.x = player.x;
                bomb.y = player.y;
                bomb.time = 3.0;
                bomb.owner = player;
                player.bombs++;

                m_bombs.Add(bomb);

                sendData(String.Format("{0} {1} {2}", (int)PacketType.BombAdd, bomb.x, bomb.y));
              }
            } else if(data == "stats") {
              String stats = "";

              foreach(Player playa in m_players) {
                stats += String.Format("{0}: {1}, ", playa.name, playa.points);
              }

              stats = stats.Substring(0, stats.Length - 2);

              sendDataTo(String.Format("{0} Statystyki: {1}", (int)PacketType.Chat, stats), player);
            } else {
              if(canPlayerMove(data, player)) {
                sendData(String.Format("{0} {1} {2} {3}", (int)PacketType.PlayerMove, player.name, player.x, player.y));
              }
            }
          break;

          default:
            addLog(String.Format("Nieznany typ pakietu: {0}", type));
          break;
        }

        waitForData(packet.socket);
      } catch(SocketException e) {
        if(e.ErrorCode == 10054) {
          Player player = playerBySocket(packet.socket);
          sendData(String.Format("{0} {1}", (int)PacketType.PlayerQuit, player.name), player);
          m_players.Remove(player);
          packet.socket.Close();
        } else {
          MessageBox.Show(e.ToString());
        }
      }
    }

    private Color randomColor()
    {
      Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Gray, Color.Gold, Color.Fuchsia, Color.LightBlue, Color.LightGreen };
      return colors[m_rand.Next(colors.Length)];
    }

    private bool canPlayerMove(string data, Player player)
    {
      int x = player.x;
      int y = player.y;
      bool ret = true;

      switch(data) {
        case "up":
          y -= 1; 
        break;

        case "down":
          y += 1;
        break;

        case "left":
          x -= 1;
        break;

        case "right":
          x += 1;
        break;
      }

      if(m_map[y][x].type != FieldType.Nothing) {
        ret = false;
      }

      foreach(Bomb bomb in m_bombs) {
        if((bomb.x == x) && (bomb.y == y)) {
          ret = false;
        }
      }

      if(ret) {
        player.x = x;
        player.y = y;
      }
      
      return ret;
    }

    private string serializeMap()
    {
      string map = "";

      for(int x = 0; x < m_map.Count; x++) {
        for(int y = 0; y < m_map[x].Count; y++) {
          map += Convert.ToString((int)m_map[x][y].type);
        }

        map += " ";
      }

      return map;
    }

    private Player playerBySocket(Socket socket)
    {
      foreach(Player player in m_players) {
        if(player.socket == socket) {
          return player;
        }
      }

      return null;
    }

    private Player playerByName(String name)
    {
      foreach(Player player in m_players) {
        if(player.name == name) {
          return player;
        }
      }

      return null;
    }

    private void sendData(string data, Player except = null)
    {
      foreach(Player player in m_players) {
        if(player.socket.Connected && (player != except)) {
          sendDataAsync(data, player.socket);
        }
      }
    }

    private void sendDataTo(string data, Player player)
    {
      sendDataAsync(data, player.socket);
    }

    private void sendDataAsync(string data, Socket socket)
    {
      data += "|";

      try {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(sendCallback), socket);
      } catch(Exception e) {
        MessageBox.Show(e.ToString());
      }
    }

    private static void sendCallback(IAsyncResult ar)
    {
      try {
        Socket socket = (Socket)ar.AsyncState;

        if(socket != null) {
          int bytesSent = socket.EndSend(ar);
        }
      } catch(Exception e) {
        //MessageBox.Show(e.ToString());
      }
    }

    private void waitForData(Socket socket)
    {
      Packet packet = new Packet();
      packet.socket = socket;
      packet.socket.BeginReceive(packet.buffer, 0, packet.buffer.Length, SocketFlags.None, new AsyncCallback(dataReceived), packet);
    }

    private string localIP()
    {
      string strHostName = Dns.GetHostName();

      IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

      foreach(IPAddress ipaddress in iphostentry.AddressList) {
        if(ipaddress.AddressFamily != AddressFamily.InterNetworkV6) {
          return ipaddress.ToString();
        }
      }

      return strHostName;
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void roundTmer_Tick(object sender, EventArgs e)
    {
      m_currentTime -= 0.1;

      List<Bomb> rem = new List<Bomb>();
      foreach(Bomb bomb in m_bombs) {
        bomb.time -= 0.1;

        if(bomb.time <= 0.0) {
          sendData(String.Format("{0} {1} {2}", (int)PacketType.BombBoom, bomb.x, bomb.y));

          if((m_map[bomb.y][bomb.x].type == FieldType.Breakable)) {
            m_map[bomb.y][bomb.x].type = FieldType.Nothing;
          } else if((m_map[bomb.y-1][bomb.x].type == FieldType.Breakable)) {
            m_map[bomb.y-1][bomb.x].type = FieldType.Nothing;
          } else if((m_map[bomb.y + 1][bomb.x].type == FieldType.Breakable)) {
            m_map[bomb.y + 1][bomb.x].type = FieldType.Nothing;
          } else if((m_map[bomb.y][bomb.x - 1].type == FieldType.Breakable)) {
            m_map[bomb.y][bomb.x - 1].type = FieldType.Nothing;
          } else if((m_map[bomb.y][bomb.x + 1].type == FieldType.Breakable)) {
            m_map[bomb.y][bomb.x + 1].type = FieldType.Nothing;
          }

          foreach(Player player in m_players) {
            if(((bomb.x == player.x) && (bomb.y == player.y)) || ((bomb.x + 1 == player.x) && (bomb.y == player.y)) || ((bomb.x == player.x) && (bomb.y + 1 == player.y)) || ((bomb.x - 1 == player.x) && (bomb.y == player.y)) || ((bomb.x == player.x) && (bomb.y - 1 == player.y))) {
              sendData(String.Format("{0} {1}", (int)PacketType.PlayerQuit, player.name));

              if(bomb.owner == player) {
                sendData(String.Format("{0} {1} postanowił skrócić swoje męki.", (int)PacketType.Chat, player.name));
                bomb.owner.points--;
              } else {
                sendData(String.Format("{0} {1} nadział się na bombę {2}.", (int)PacketType.Chat, player.name, bomb.owner.name));
                bomb.owner.points++;
              }

              int rand = m_rand.Next(m_spawns.Count - 1);
              Point spawn = m_spawns.ElementAt(rand);

              player.x = spawn.X;
              player.y = spawn.Y;
              player.color = randomColor();

              sendData(String.Format("{0} {1} {2} {3} {4}", (int)PacketType.PlayerAdd, spawn.X, spawn.Y, player.name, player.color.ToArgb()), player);
              sendDataTo(String.Format("{0} {1} {2} {3}", (int)PacketType.Spawn, spawn.X, spawn.Y, player.color.ToArgb()), player);
            }
          }

          rem.Add(bomb);
        }
      }

      foreach(Bomb bomb in rem) {
        bomb.owner.bombs--;
        m_bombs.Remove(bomb);
      }
    }
  }
}
