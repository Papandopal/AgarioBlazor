using System.Net.WebSockets;
using System.Numerics;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using SPPR13_2.Client.CustomCollections;
using SPPR13_2.Client.Entities;

namespace SPPR13_2.Services
{
    public class Model
    {
        public ConcurrentList<Player> Players { get; set; }
        public ConcurrentList<Food> Foods { get; set; }

        public int PlayerCount { get; set; } = 0;

        //LeaderBoard leaderBoard;
        public Model()
        { 
            Players = new ConcurrentList<Player>();
            Foods = EntitiesService.GetRandFoods(Rules.FoodCount);
        }

        public Player CreatePlayer(string name)
        {
            var new_player = new Player();
            new_player.name = name;
            Players.Add(new_player);
            return new_player;
        }
        public async Task UpdatePlayer(Player player)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].player_id == player.player_id)
                {
                    Players[i] = player;
                    break;
                }
            }
        }
        public Food EatFood(int index)
        {
            Foods.RemoveAt(index);
            return new Food();
        }

        public void Move(string player_id, double new_mouse_x, double new_mouse_y)
        {
            if (Players.Count == 0) return;

            foreach (var player in Players)
            {
                if (player.player_id == player_id)
                {
                    player.Move(new_mouse_x, new_mouse_y);
                    break;
                }
            }

        }
        public void NewSize(string player_id)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].player_id == player_id)
                {
                    Players[i].size += Rules.FoodIncrease;
                    break;
                }
            }
        }
        public void NewSize(string victim_id, string killer_id)
        {
            var victim_player = Players.Where(item => item.player_id == victim_id).FirstOrDefault();
            var killer_player = Players.Where(item => item.player_id == killer_id).FirstOrDefault();

            if (victim_player == null || killer_player == null)
            {
                throw new Exception("victim_player or killer_player is null");
            }

            killer_player.size += victim_player.size;
        }

        public void RemovePlayer(string player_id)
        {
            Players.Remove(Players.First((item) => item.player_id == player_id));
        }
    }
}
