using System.Drawing;

namespace Klient
{
  public enum PacketType
  {
    Chat = 0,
    Name,
    NameExists,
    Map,
    MapOK,
    TimeLeft,
    RoundOver,
    BombAdd,
    BombBoom,
    Spawn,
    MoveRequest,
    PlayerAdd,
    PlayerMove,
    PlayerQuit
  }

  public class ChatMsg
  {
    public ChatMsg(string msg = "")
    {
      this.msg = msg;
    }

    public string msg = "";
    public double time = 7.0;
  }

  public class Packet
  {
    public System.Net.Sockets.Socket socket;
    public byte[] buffer = new byte[1000];
  }

  public enum FieldType
  {
    Nothing = 0,
    Wall,
    Breakable,
    Spawn
  }

  public class Field
  {
    public FieldType type = FieldType.Nothing;
  }

  public class Explosion
  {
    public Point pos = new Point();
    public double time = 1.0; 
  }

  public class Player
  {
    public string name = "";
    public int x = -1;
    public int y = -1;
    public Color color = Color.Blue;
  }
}