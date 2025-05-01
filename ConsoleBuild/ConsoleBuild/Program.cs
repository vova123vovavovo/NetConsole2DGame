namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main()
        {
            Game.SetScreenParams(25, 25);
            Game.InitScreen();
            Game.InitStaticPixels();
            Game.InitPlayerPixel();
            Game.SetPixelsArray(new char[7] { ' ', '.', '#', '0', '@', '&', '$' });
            Game.SetPixel(1);

            Game.CreatePlayerPixel(new PlayerPixel(4, 0, 2, 5, 5));

            while (true)
            {
                Game.UpdatePlayerPixelContolls();

                Game.ClearScreen();

                Game.FillMap();

                Game.Draw();
                Game.DrawPlayerStats();

                Thread.Sleep(100);
            }
        }
    }

    public static class Game
    {
        public static char[] pixels;

        public static char cur_pixel;

        public static int width;
        public static int height;

        public static char[][] screen;
        public static char[] f_screen;

        public static List<StaticPixel> static_pixels;
        public static PlayerPixel player_pixel;

        public static void Draw()
        {
            for (int i = 0; i < static_pixels.Count; i++)
            {
                static_pixels[i].Draw();
            }

            player_pixel.Draw();

            for (int i = 0; i < height; i++)
            {
                for (int a = 0; a < width + 1; a++)
                {
                    if (a == width)
                    {
                        f_screen[(width + 1) * i + a + 1] = '\n';
                    }
                    else
                    {
                        f_screen[(width + 1) * i + a + 1] = screen[i][a];
                    }
                }
            }

            Console.WriteLine(f_screen);
        }


        public static void DrawPlayerStats()
        {
            Console.WriteLine("Player position: X = " + player_pixel.x + " | Y = " + player_pixel.y);

            string dir_str;
            if (player_pixel.dir == 0)
            {
                dir_str = "Up";
            }
            else if (player_pixel.dir == 1)
            {
                dir_str = "Down";
            }
            else if (player_pixel.dir == 2)
            {
                dir_str = "Left";
            }
            else
            {
                dir_str = "Right";
            }

            Console.WriteLine("Direction: " + dir_str);
            Console.WriteLine("Current build pixel: " + pixels[player_pixel.build_pixel_id]);

            Console.WriteLine("-----Controlls-----");
            Console.WriteLine("Movement: W, A, S, D");
            Console.WriteLine("Build/Destroy pixels: B, V");
            Console.WriteLine("Switch pixels: K, L");
        }

        public static void FillMap()
        {
            for (int i = 0; i < height; i++)
            {
                screen[i] = new char[width + 1];
                for (int a = 0; a < width + 1; a++)
                {
                    screen[i][a] = cur_pixel;
                }
            }
        }

        public static void SetPixel(short pixel_id)
        {
            cur_pixel = pixels[pixel_id];
        }

        public static void SetScreenParams(int in_width, int in_height)
        {
            width = in_width;
            height = in_height;
        }

        public static void InitScreen()
        {
            screen = new char[height][];
            f_screen = new char[(width + 1) * height + 1];
        }

        public static void InitStaticPixels()
        {
            static_pixels = new List<StaticPixel>();
        }

        public static void InitPlayerPixel()
        {
            player_pixel = new PlayerPixel();
        }

        public static void SetPixelsArray(char[] in_pixels)
        {
            pixels = in_pixels;
        }

        public static void CreateStaticPixel(StaticPixel in_static_pixel)
        {
            for (int i = 0; i < static_pixels.Count; i++)
            {
                if (static_pixels[i].x == in_static_pixel.x && static_pixels[i].y == in_static_pixel.y)
                {
                    return;
                }
            }

            static_pixels.Add(in_static_pixel);
        }

        public static void CreatePlayerPixel(PlayerPixel in_player_pixel)
        {
            player_pixel = in_player_pixel;
        }

        public static void ClearScreen()
        {
            Console.Clear();
        }

        public static void UpdatePlayerPixelContolls()
        {
            player_pixel.UpdateControlls();
        }

        public static bool CheckStaticPixel(int in_x, int in_y)
        {
            for (int i = 0; i < static_pixels.Count; i++)
            {
                if (static_pixels[i].x == in_x && static_pixels[i].y == in_y)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckStaticPixelWithOut(int in_x, int in_y, out int out_pixel_id)
        {
            for (int i = 0; i < static_pixels.Count; i++)
            {
                if (static_pixels[i].x == in_x && static_pixels[i].y == in_y)
                {
                    out_pixel_id = i;
                    return true;
                }
            }
            out_pixel_id = -1;
            return false;
        }

        public static void DestroyStaticPixelByID(int in_pixel_id)
        {
            static_pixels.RemoveAt(in_pixel_id);
        }
    }

    public class StaticPixel
    {
        public short pixel_id;

        public int x;
        public int y;

        public StaticPixel()
        {

        }

        public StaticPixel(short in_pixel_id, int in_x, int in_y)
        {
            pixel_id = in_pixel_id;
            x = in_x;
            y = in_y;
        }

        public void Draw()
        {
            Game.screen[y][x] = Game.pixels[pixel_id];
        }

        public void Move(int in_x, int in_y)
        {
            x = in_x;
            y = in_y;
        }
    }

    public class PlayerPixel
    {
        public short player_pixel_id;

        public short dir; //0 - up; 1 - down; 2 - left; 3 - right;

        public short build_pixel_id;

        public int p_x;
        public int p_y;

        public int x;
        public int y;

        public PlayerPixel()
        {

        }

        public PlayerPixel(short in_pixel_id, short in_dir, short in_build_pixel_id, int in_x, int in_y)
        {
            player_pixel_id = in_pixel_id;
            dir = in_dir;
            build_pixel_id = in_build_pixel_id;
            x = in_x;
            y = in_y;
        }

        public void Draw()
        {
            y = Math.Clamp(y, 0, Game.height - 1);
            x = Math.Clamp(x, 0, Game.width - 1);
            Game.screen[y][x] = Game.pixels[player_pixel_id];
        }

        public void Move(int in_x, int in_y)
        {
            p_x = x;
            p_y = y;

            x = in_x;
            y = in_y;

            if (CheckCollisionWithStaticPixels())
            {
                x = p_x;
                y = p_y;
                Console.Beep(80, 50);
            }
            else
            {
                Console.Beep(125, 50);
            }
        }

        public void UpdateControlls()
        {
            ConsoleKey console_key = Console.ReadKey().Key;

            if (console_key == ConsoleKey.W)
            {
                Move(x, y - 1);
                dir = 0;
            }
            else if (console_key == ConsoleKey.S)
            {
                Move(x, y + 1);
                dir = 1;
            }
            else if (console_key == ConsoleKey.A)
            {
                Move(x - 1, y);
                dir = 2;
            }
            else if (console_key == ConsoleKey.D)
            {
                Move(x + 1, y);
                dir = 3;
            }
            else if (console_key == ConsoleKey.B)
            {
                BuildBlock();
            }
            else if (console_key == ConsoleKey.V)
            {
                DeleteBlock();
            }
            else if (console_key == ConsoleKey.K)
            {
                SwitchBuildPixel(-1);
            }
            else if (console_key == ConsoleKey.L)
            {
                SwitchBuildPixel(1);
            }
        }

        public void SwitchBuildPixel(short in_id_add)
        {
            build_pixel_id += in_id_add;
            if (build_pixel_id > Game.pixels.Length - 1)
            {
                build_pixel_id = 0;
            }
            else if (build_pixel_id < 0)
            {
                build_pixel_id = (short)(Game.pixels.Length - 1);
            }

            Console.Beep(250, 50);
        }

        public bool CheckCollisionWithStaticPixels()
        {
            for (int i = 0; i < Game.static_pixels.Count; i++)
            {
                if (Game.static_pixels[i].x == x && Game.static_pixels[i].y == y)
                {
                    return true;
                }
            }

            return false;
        }

        public void BuildBlock()
        {
            if (dir == 0)
            {
                Game.CreateStaticPixel(new StaticPixel(build_pixel_id, x, y - 1));
            }
            else if (dir == 1)
            {
                Game.CreateStaticPixel(new StaticPixel(build_pixel_id, x, y + 1));
            }
            else if (dir == 2)
            {
                Game.CreateStaticPixel(new StaticPixel(build_pixel_id, x - 1, y));
            }
            else if (dir == 3)
            {
                Game.CreateStaticPixel(new StaticPixel(build_pixel_id, x + 1, y));
            }

            Console.Beep(500, 50);
        }

        public void DeleteBlock()
        {
            int pixel_id;
            if (dir == 0)
            {
                if (Game.CheckStaticPixelWithOut(x, y - 1, out pixel_id))
                {
                    Game.DestroyStaticPixelByID(pixel_id);
                    Console.Beep(180, 50);
                }
            }
            else if (dir == 1)
            {
                if (Game.CheckStaticPixelWithOut(x, y + 1, out pixel_id))
                {
                    Game.DestroyStaticPixelByID(pixel_id);
                    Console.Beep(180, 50);
                }
            }
            else if (dir == 2)
            {
                if (Game.CheckStaticPixelWithOut(x - 1, y, out pixel_id))
                {
                    Game.DestroyStaticPixelByID(pixel_id);
                    Console.Beep(180, 50);
                }
            }
            else if (dir == 3)
            {
                if (Game.CheckStaticPixelWithOut(x + 1, y, out pixel_id))
                {
                    Game.DestroyStaticPixelByID(pixel_id);
                    Console.Beep(180, 50);
                }
            }
        }
    }
}