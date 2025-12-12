using System.Numerics;
using System.Threading;

namespace SPPR13_2.Client.Entities
{
    public class Player : IDisposable
    {
        public Player() : this(Guid.NewGuid().ToString())
        {
            x = EntitiesService.GetRandCoord();
            y = EntitiesService.GetRandCoord();
            size = Rules.PlayerBasicSize;
            speed = Rules.PlayerBasicSpeed;
            mouse_x = x;
            mouse_y = y;
            StartTimer();
        }
        public Player(string player_id)
        {
            this.player_id = player_id;
            
        }
        public double x { get; set; }
        public double y { get; set; }
        public string player_id { get; set; }
        public double size { get; set; }
        public double speed { get; set; }
        public string name { get; set; } = "name";
        public bool isDead { get; set; } = false;
        public double mouse_x { get; set; }
        public double mouse_y { get; set; }

        private TimerCallback time_call_back;
        private Timer timer;

        public void StartTimer()
        {
            time_call_back = new TimerCallback(GoToMouse);
            timer = new Timer(time_call_back, null, 0, Rules.TimerPeriod);
        }

        public void Move(double new_mouse_x, double new_mouse_y)
        {
            mouse_x = new_mouse_x;
            mouse_y = new_mouse_y;
        }

        void GoToMouse(object obj)
        {
            var new_mouse_x = mouse_x;
            var new_mouse_y = mouse_y;
            double len_vector = Math.Sqrt(Math.Pow(new_mouse_x, 2) + Math.Pow(new_mouse_y, 2));
            var x_norm = Math.Abs(new_mouse_x) / len_vector;
            var y_norm = Math.Abs(new_mouse_y) / len_vector;

            if (new_mouse_x > 0)
            {
                x += x_norm * speed;
            }
            else
            {
                x -= x_norm * speed;
            }

            x = Math.Min(Math.Max(x, 0), Rules.MapSize);
            
            if (new_mouse_y > 0)
            {
                y += y_norm * speed;
            }
            else
            {
                y -= y_norm * speed;
            }

            y = Math.Min(Math.Max(y, 0), Rules.MapSize);

            //Console.WriteLine($"{new_mouse_x} {new_mouse_y} {x} {y} {x_norm} {y_norm}");
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    };

}
