using System.Net;
using System.Net.Sockets;
using System.Drawing;

namespace Server
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
    PlayerQuit,
    PlayerDie
  }

  public class Packet
  {
    public Socket socket;
    public byte[] buffer = new byte[1024];
  }

  public class Bomb
  {
    public int x = -1;
    public int y = -1;
    public double time = -1;
    public Player owner = null;
  }

  public class Player
  {
    public string name = "";
    public int x = -1;
    public int y = -1;
    public int bombs = 0;
    public int points = 0;
    public Color color;
    public Socket socket;
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

}