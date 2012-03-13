using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Klient
{
  public partial class mainForm: Form
  {
    private Socket m_socket;
    private Random m_rand = new Random();
    private AsyncCallback m_dataCallback;
    private List<List<Field>> m_map = new List<List<Field>>();
    private List<Point> m_bombs = new List<Point>();
    private List<Player> m_players = new List<Player>();
    private List<Explosion> m_explo = new List<Explosion>();
    private List<ChatMsg> m_msgs = new List<ChatMsg>();
    private Point m_camera = new Point(0, 0);
    private Player m_player = new Player();
    private Graphics m_canvas;
    private String m_buffer = "";

    public mainForm()
    {
      InitializeComponent();

      gamePictureBox.Image = new Bitmap(512, 512);
      m_canvas = Graphics.FromImage(gamePictureBox.Image);
      m_canvas.SmoothingMode = SmoothingMode.AntiAlias;
      m_canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
    }

    private void drawWelcomeScreen()
    {
      LinearGradientBrush bgBrush = new LinearGradientBrush(new Rectangle(0, 0, 1, 512), Color.FromArgb(203, 214, 255), Color.White, 90);
      m_canvas.FillRectangle(bgBrush, m_canvas.ClipBounds);

      LinearGradientBrush titleBrush = new LinearGradientBrush(new Rectangle(0, 0, 1, 40), Color.FromArgb(245, 190, 0), Color.FromArgb(255, 230, 0), 90);
      m_canvas.DrawString("B   MBERMAN", new Font("Tahoma", 36, FontStyle.Bold), titleBrush, 30, 30);

      drawBomb(75, 50, 3f);
      drawBomberMan(370, 200, Color.Blue, 3f);

      gamePictureBox.Invoke(new Action(() => gamePictureBox.Refresh()));
    }

    private void drawBomb(Point bomb)
    {
      drawBomb((bomb.X - m_camera.X) * 32 + 8, (bomb.Y - m_camera.Y) * 32 + 8, 1.6f);
    }

    private void drawBomb(int x, int y, float scale = 1.0f)
    {
      m_canvas.FillRectangle(new SolidBrush(Color.Gray), x + 2.5f * scale, y - 2 * scale, 5 * scale, 3 * scale);
      m_canvas.DrawRectangle(new Pen(Color.Black), x + 2.5f * scale, y - 2 * scale, 5 * scale, 3 * scale);

      m_canvas.FillEllipse(new SolidBrush(Color.Gray), x, y, 10 * scale, 10 * scale);
      m_canvas.DrawEllipse(new Pen(Color.Black), x, y, 10 * scale, 10 * scale);
    }

    private void drawBomberMan(int x, int y, Color body, float scale = 1.0f)
    {
      SolidBrush whiteBrush = new SolidBrush(Color.White);
      SolidBrush blackBrush = new SolidBrush(Color.Black);
      SolidBrush pinkBrush = new SolidBrush(Color.Fuchsia);
      SolidBrush skinBrush = new SolidBrush(Color.Yellow);
      Pen blackPen = new Pen(Color.Black, 1);

      m_canvas.FillRectangle(whiteBrush, x + 17 * scale, y + 18 * scale, 5 * scale, 15 * scale);
      m_canvas.DrawRectangle(blackPen, x + 17 * scale, y + 18 * scale, 5 * scale, 15 * scale);

      m_canvas.FillEllipse(pinkBrush, x + 10 * scale, y, 20 * scale, 20 * scale);
      m_canvas.DrawEllipse(blackPen, x + 10 * scale, y, 20 * scale, 20 * scale);

      m_canvas.FillRectangle(whiteBrush, x, y + 27 * scale, 40 * scale, 30 * scale);
      m_canvas.DrawRectangle(blackPen, x, y + 27 * scale, 40 * scale, 30 * scale);

      m_canvas.FillRectangle(skinBrush, x + 3 * scale, y + 30 * scale, 34 * scale, 24 * scale);
      m_canvas.DrawRectangle(blackPen, x + 3 * scale, y + 30 * scale, 34 * scale, 24 * scale);

      m_canvas.FillRectangle(blackBrush, x + 13 * scale, y + 34 * scale, 3 * scale, 15 * scale);

      m_canvas.FillRectangle(blackBrush, x + 23 * scale, y + 34 * scale, 3 * scale, 15 * scale);

      m_canvas.FillRectangle(new SolidBrush(body), x + 8 * scale, y + 57 * scale, 22 * scale, 25 * scale);
      m_canvas.DrawRectangle(blackPen, x + 8 * scale, y + 57 * scale, 22 * scale, 25 * scale);

      m_canvas.FillRectangle(blackBrush, x + 8 * scale, y + 72 * scale, 22 * scale, 4 * scale);
      m_canvas.DrawRectangle(blackPen, x + 8 * scale, y + 72 * scale, 22 * scale, 4 * scale);

      m_canvas.FillRectangle(whiteBrush, x + 1 * scale, y + 58 * scale, 7 * scale, 15 * scale);
      m_canvas.DrawRectangle(blackPen, x + 1 * scale, y + 58 * scale, 7 * scale, 15 * scale);

      m_canvas.FillRectangle(whiteBrush, x + 30 * scale, y + 58 * scale, 7 * scale, 15 * scale);
      m_canvas.DrawRectangle(blackPen, x + 30 * scale, y + 58 * scale, 7 * scale, 15 * scale);

      m_canvas.FillRectangle(pinkBrush, x + 1 * scale, y + 73 * scale, 7 * scale, 4 * scale);
      m_canvas.DrawRectangle(blackPen, x + 1 * scale, y + 73 * scale, 7 * scale, 4 * scale);

      m_canvas.FillRectangle(pinkBrush, x + 30 * scale, y + 73 * scale, 7 * scale, 4 * scale);
      m_canvas.DrawRectangle(blackPen, x + 30 * scale, y + 73 * scale, 7 * scale, 4 * scale);

      m_canvas.FillRectangle(whiteBrush, x + 12 * scale, y + 82 * scale, 7 * scale, 10 * scale);
      m_canvas.DrawRectangle(blackPen, x + 12 * scale, y + 82 * scale, 7 * scale, 10 * scale);

      m_canvas.FillRectangle(whiteBrush, x + 19 * scale, y + 82 * scale, 7 * scale, 10 * scale);
      m_canvas.DrawRectangle(blackPen, x + 19 * scale, y + 82 * scale, 7 * scale, 10 * scale);

      m_canvas.FillRectangle(pinkBrush, x + 12 * scale, y + 92 * scale, 7 * scale, 4 * scale);
      m_canvas.DrawRectangle(blackPen, x + 12 * scale, y + 92 * scale, 7 * scale, 4 * scale);

      m_canvas.FillRectangle(pinkBrush, x + 19 * scale, y + 92 * scale, 7 * scale, 4 * scale);
      m_canvas.DrawRectangle(blackPen, x + 19 * scale, y + 92 * scale, 7 * scale, 4 * scale);
    }

    public void drawBomberMan(Player player)
    {
      drawBomberMan((player.x - m_camera.X) * 32 + 10, (player.y - m_camera.Y) * 32 + 1, player.color, 0.3f);
    }

    public void connectToServer()
    {
      try {
        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(serverTextBox.Text), 182);
        m_socket.Connect(ip);
        if(m_socket.Connected) {
          Width -= 210;
          animTimer.Enabled = true;
          connGroupBox.Visible = false;
          connectButton.Visible = false;
          exitButton.Visible = false;
          controlTextBox.Visible = true;
          controlTextBox.Focus();

          sendData(PacketType.Name, m_player.name);
          waitForData();     
        }
      } catch(SocketException e) {
      }
    }

    private void waitForData()
    {
      if(m_socket.Connected) {
        if(m_dataCallback == null) {
          m_dataCallback = new AsyncCallback(dataReceived);
        }

        Packet packet = new Packet();
        packet.socket = m_socket;
        packet.socket.BeginReceive(packet.buffer, 0, packet.buffer.Length, SocketFlags.None, m_dataCallback, packet);
      }
    }

    private void dataReceived(IAsyncResult asyn)
    {
      try {
        Packet packet = (Packet)asyn.AsyncState;
        int size = packet.socket.EndReceive(asyn);
        String packet_string = new String(Encoding.UTF8.GetChars(packet.buffer, 0, size));
        m_buffer += packet_string;

        parseBuffer();

        waitForData();
      } catch(SocketException e) {
      }
    }

    private void parseBuffer()
    {
      String[] lines = m_buffer.Split('|');
      if(lines.Length > 0) {
        foreach(String line in lines) {
          if(line.Length > 0) {
            String[] line_split = line.Split(' ');
            if(line_split.Length > 0) {
              PacketType type = (PacketType)Convert.ToInt32(line_split[0]);

              String data = "";
              for(int i = 1; i < line_split.Length; i++) {
                data += line_split[i] + " ";
              }

              data = data.TrimEnd();

              m_buffer = m_buffer.Substring(line.Length);

              if(m_buffer.Length > 0) {
                if(m_buffer[0] == '|') {
                  m_buffer = m_buffer.Substring(1);
                }
              }

              parsePacket(type, data);
            }
          }
        }
      }
    }

    private void parsePacket(PacketType type, String data)
    {
      switch(type) {
        case PacketType.Chat: {
          ChatMsg msg = new ChatMsg();
          msg.msg = data;

          if(m_msgs.Count > 5) {
            m_msgs.RemoveAt(0);
          }

          m_msgs.Add(msg);

          redrawMap();
        }
        break;

        case PacketType.NameExists: 
        {
          MessageBox.Show("Ten nick jest już używany, wybierz inny.");

          Width += 210;
          animTimer.Enabled = false;
          connGroupBox.Visible = true;
          connectButton.Visible = true;
          exitButton.Visible = true;
          controlTextBox.Visible = false;
        }
        break;

        case PacketType.Map: 
        {
          String[] lines = data.Split(' ');
          int y = 0;
          foreach(String line in lines) {
            line.Trim();

            if(line.Length > 0) {
              List<Field> list = new List<Field>();
              m_map.Add(list);
              for(int x = 0; x < line.Length; x++) {
                Field field = new Field();
                field.type = (FieldType)Convert.ToInt32(line.Substring(x, 1));

                m_map.Last().Add(field);
              }
            }

            y++;
          }

          sendData(PacketType.MapOK, "");

          redrawMap();
        }
        break;

        case PacketType.Spawn: {
          String[] spawn_pos = data.Split(' ');
          if(spawn_pos.Length == 3) {
            m_player.x = Convert.ToInt32(spawn_pos[0]);
            m_player.y = Convert.ToInt32(spawn_pos[1]);
            m_player.color = Color.FromArgb(Convert.ToInt32(spawn_pos[2]));
            
            moveCamera();
            redrawMap();
          }
        }
        break;

        case PacketType.PlayerAdd: {
          String[] player_info = data.Split(' ');
          if(player_info.Length == 4) {
            Player player = new Player();
            player.x = Convert.ToInt32(player_info[0]);
            player.y = Convert.ToInt32(player_info[1]);
            player.name = player_info[2];
            player.color = Color.FromArgb(Convert.ToInt32(player_info[3]));

            m_players.Add(player);
            redrawMap();
          }
        }
        break;

        case PacketType.PlayerMove: {
          String[] info = data.Split(' ');
          if(info.Length == 3) {
            if(info[0] == m_player.name) {
              m_player.x = Convert.ToInt32(info[1]);
              m_player.y = Convert.ToInt32(info[2]);

              moveCamera();
            } else {
              playerByName(info[0]).x = Convert.ToInt32(info[1]);
              playerByName(info[0]).y = Convert.ToInt32(info[2]);
            }

            redrawMap();
          }
        }
        break;

        case PacketType.BombAdd: {
          String[] bomb_pos = data.Split(' ');

          if(bomb_pos.Length == 2) {
            Point bomb = new Point();
            bomb.X = Convert.ToInt32(bomb_pos[0]);
            bomb.Y = Convert.ToInt32(bomb_pos[1]);

            m_bombs.Add(bomb);

            redrawMap();
          }
        }
        break;

        case PacketType.BombBoom: {
          String[] bomb_pos = data.Split(' ');

          if(bomb_pos.Length == 2) {
            int x = Convert.ToInt32(bomb_pos[0]);
            int y = Convert.ToInt32(bomb_pos[1]);

            Point rem = Point.Empty;
            foreach(Point bomb in m_bombs) {
              if((bomb.X == x) && (bomb.Y == y)) {
                rem = bomb;
              }
            }

            if(!rem.IsEmpty) {
              m_bombs.Remove(rem);
            }

            Explosion expl = new Explosion();
            expl.pos.X = x;
            expl.pos.Y = y;
            m_explo.Add(expl);

            if((m_map[y][x].type == FieldType.Breakable)) {
              m_map[y][x].type = FieldType.Nothing;
            } else if((m_map[y - 1][x].type == FieldType.Breakable)) {
              m_map[y - 1][x].type = FieldType.Nothing;
            } else if((m_map[y + 1][x].type == FieldType.Breakable)) {
              m_map[y + 1][x].type = FieldType.Nothing;
            } else if((m_map[y][x - 1].type == FieldType.Breakable)) {
              m_map[y][x - 1].type = FieldType.Nothing;
            } else if((m_map[y][x + 1].type == FieldType.Breakable)) {
              m_map[y][x + 1].type = FieldType.Nothing;
            }

            redrawMap();
          }
        }
        break;

        case PacketType.PlayerQuit: {
          m_players.Remove(playerByName(data));
          redrawMap();
        }
        break;

        default:
          //addLog(String.Format("Nieznany typ pakietu: {0}", type));
        break;
      }
    }

    private Player playerByName(string name)
    {
      foreach(Player player in m_players) {
        if(player.name == name) {
          return player;
        }
      }

      return null;
    }

    private void moveCamera()
    {
      if((m_player.x - m_camera.X) > 14) {
        m_camera.X = m_player.x - 14;
      }

      if((m_player.x - m_camera.X) < 1) {
        m_camera.X = m_player.x - 1;
      }

      if((m_player.y - m_camera.Y) > 14) {
        m_camera.Y = m_player.y - 14;
      }

      if((m_player.y - m_camera.Y) < 1) {
        m_camera.Y = m_player.y - 1;
      }
    }

    private void redrawMap()
    {
      try {
        m_canvas.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, 512, 512));

        int max_y = (m_map.Count > 16) ? 16 : m_map.Count;
        for(int y = 0 + m_camera.Y; y < max_y + m_camera.Y; y++) {
          int max_x = (m_map[y].Count > 16) ? 16 : m_map[y].Count;
          for(int x = 0 + m_camera.X; x < max_x + m_camera.X; x++) {
            switch(m_map[y][x].type) {
              case FieldType.Nothing: {
                m_canvas.FillRectangle(new SolidBrush(Color.ForestGreen), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32, 32, 32));
              }
              break;

              case FieldType.Wall: {
                m_canvas.FillRectangle(new SolidBrush(Color.Silver), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32, 32, 32));
                m_canvas.FillRectangle(new SolidBrush(Color.Gainsboro), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32, 1, 31));
                m_canvas.FillRectangle(new SolidBrush(Color.Gainsboro), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32, 31, 1));
                m_canvas.FillRectangle(new SolidBrush(Color.Gray), new Rectangle((x - m_camera.X) * 32 + 31, (y - m_camera.Y) * 32, 1, 32));
                m_canvas.FillRectangle(new SolidBrush(Color.Gray), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32 + 31, 31, 1));

              }
              break;

              case FieldType.Breakable: {
                m_canvas.FillRectangle(new SolidBrush(Color.FromArgb(162, 128, 89)), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32, 32, 32));
                m_canvas.FillRectangle(new SolidBrush(Color.FromArgb(140, 100, 50)), new Rectangle((x - m_camera.X) * 32 + 7, (y - m_camera.Y) * 32 + 8, 18, 15));

                m_canvas.DrawRectangle(new Pen(Color.Black), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32, 32, 32));
                m_canvas.DrawRectangle(new Pen(Color.Black), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32, 32, 8));
                m_canvas.DrawRectangle(new Pen(Color.Black), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32 + 24, 32, 8));
                m_canvas.DrawRectangle(new Pen(Color.Black), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32 + 8, 8, 16));
                m_canvas.DrawRectangle(new Pen(Color.Black), new Rectangle((x - m_camera.X) * 32 + 24, (y - m_camera.Y) * 32 + 8, 8, 16));
                m_canvas.DrawLine(new Pen(Color.Black), (x - m_camera.X) * 32 + 12, (y - m_camera.Y) * 32 + 8, (x - m_camera.X) * 32 + 12, (y - m_camera.Y) * 32 + 24);
                m_canvas.DrawLine(new Pen(Color.Black), (x - m_camera.X) * 32 + 16, (y - m_camera.Y) * 32 + 8, (x - m_camera.X) * 32 + 16, (y - m_camera.Y) * 32 + 24);
                m_canvas.DrawLine(new Pen(Color.Black), (x - m_camera.X) * 32 + 20, (y - m_camera.Y) * 32 + 8, (x - m_camera.X) * 32 + 20, (y - m_camera.Y) * 32 + 24);

              }
              break;

              default: {
                m_canvas.FillRectangle(new SolidBrush(Color.White), new Rectangle((x - m_camera.X) * 32, (y - m_camera.Y) * 32, 32, 32));
              }
              break;
            }
          }
        }

        foreach(Point bomb in m_bombs) {
          drawBomb(bomb);
        }

        foreach(Explosion expl in m_explo) {
          drawExplosion(expl);
        }

        foreach(Player player in m_players) {
          drawBomberMan(player);
        }

        drawBomberMan(m_player);

        int lines = 0;
        foreach(ChatMsg msg in m_msgs) {
          Font font = new Font("Tahoma", 8, FontStyle.Bold);

          SizeF size = m_canvas.MeasureString(msg.msg, font);
          StringFormat format = new StringFormat();
          m_canvas.FillRectangle(new SolidBrush(Color.FromArgb(200, Color.White)), new RectangleF(510 - size.Width, lines * 13, size.Width + 2, 13));
          m_canvas.DrawString(msg.msg, font, new SolidBrush(Color.Black), 510 - size.Width, lines * 13, format);

          lines++;
        }

        gamePictureBox.Invoke(new Action(() => gamePictureBox.Refresh()));
      } catch(SystemException e) {
      }
    }

    private void drawExplosion(Explosion expl)
    {
      drawBoom(expl.pos.X, expl.pos.Y);

      if(m_map[expl.pos.Y - 1][expl.pos.X].type == FieldType.Nothing) {
        drawBoom(expl.pos.X, expl.pos.Y - 1);
      }
      if(m_map[expl.pos.Y + 1][expl.pos.X].type == FieldType.Nothing) {
        drawBoom(expl.pos.X, expl.pos.Y + 1);
      }
      if(m_map[expl.pos.Y][expl.pos.X + 1].type == FieldType.Nothing) {
        drawBoom(expl.pos.X + 1, expl.pos.Y);
      }
      if(m_map[expl.pos.Y][expl.pos.X - 1].type == FieldType.Nothing) {
        drawBoom(expl.pos.X - 1, expl.pos.Y);
      }
    }

    private void drawBoom(int x, int y)
    {
      m_canvas.FillEllipse(new SolidBrush(Color.DarkRed), (x - m_camera.X) * 32, (y - m_camera.Y) * 32, 32, 32);
      m_canvas.FillEllipse(new SolidBrush(Color.Red), (x - m_camera.X) * 32 + 2.5f, (y - m_camera.Y) * 32  + 2.5f, 27, 27);
      m_canvas.FillEllipse(new SolidBrush(Color.OrangeRed), (x - m_camera.X) * 32 + 5, (y - m_camera.Y) * 32 + 5, 22, 22);
      m_canvas.FillEllipse(new SolidBrush(Color.Orange), (x - m_camera.X) * 32 + 7.5f, (y - m_camera.Y) * 32 + 7.5f, 17, 17);
      m_canvas.FillEllipse(new SolidBrush(Color.Yellow), (x - m_camera.X) * 32 + 10, (y - m_camera.Y) * 32 + 10, 12, 12);
    }

    private void sendData(PacketType type,  String text)
    {
      if(m_socket.Connected) {
        byte [] data = Encoding.UTF8.GetBytes(String.Format("{0} {1}", (int)type, text));
        m_socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(sendCallback), m_socket);
      }
    }

    private static void sendCallback(IAsyncResult ar)
    {
      try {
        Socket client = (Socket)ar.AsyncState;

        int bytesSent = client.EndSend(ar);
        //Console.WriteLine("Sent {0} bytes to server.", bytesSent);

      } catch(Exception e) {
      }
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
      if((nickTextBox.Text != "") && (serverTextBox.Text != "")) {
        m_player.name = nickTextBox.Text;
        connectToServer();
      } else {
        MessageBox.Show("Musisz podać nick i serwer!");
      }      
    }

    private void exitButton_clicked(object sender, EventArgs e)
    {
      Close();
    }

    private void controlTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      switch(e.KeyCode) {
        case Keys.W:
        case Keys.Up:
        sendData(PacketType.MoveRequest, "up");
        break;

        case Keys.S:
        case Keys.Down:
        sendData(PacketType.MoveRequest, "down");
        break;

        case Keys.D:
        case Keys.Right:
        sendData(PacketType.MoveRequest, "right");
        break;

        case Keys.A:
        case Keys.Left:
        sendData(PacketType.MoveRequest, "left");
        break;

        case Keys.Y: {
          chatTextBox.Text = "";
          chatTextBox.Visible = true;
          chatTextBox.Focus();
        }
        break;

        case Keys.F1: {
          sendData(PacketType.MoveRequest, "stats");
        }
        break;

        case Keys.Space:
        sendData(PacketType.MoveRequest, "bomb");
        break;

        default:
        return;
        break;
      }
    }

    private void mainForm_Shown(object sender, EventArgs e)
    {
      drawWelcomeScreen();
    }

    private void animTimer_Tick(object sender, EventArgs e)
    {
      List<Explosion> rem_expl = new List<Explosion>();
      List<ChatMsg> rem_msg = new List<ChatMsg>();

      foreach(Explosion expl in m_explo) {
        expl.time -= 0.1;

        if(expl.time <= 0.0) {
          rem_expl.Add(expl);
        }
      }

      foreach(ChatMsg msg in m_msgs) {
        msg.time -= 0.1;

        if(msg.time <= 0.0) {
          rem_msg.Add(msg);
        }
      }

      foreach(ChatMsg msg in rem_msg) {
        m_msgs.Remove(msg);
      }

      foreach(Explosion expl in rem_expl) {
        m_explo.Remove(expl);
      }

      if((rem_expl.Count > 0) || (rem_msg.Count > 0)) {
        redrawMap();
      }
    }

    private void chatTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if(e.KeyCode == Keys.Enter) {
        if(chatTextBox.Text != "") {
          sendData(PacketType.Chat, chatTextBox.Text);
        }

        chatTextBox.Visible = false;
        controlTextBox.Focus();
      } else if(e.KeyCode == Keys.Escape) {
        chatTextBox.Visible = false;
        controlTextBox.Focus();
      }
    }
  }
}
