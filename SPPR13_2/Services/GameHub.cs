using System.Collections;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;
using SPPR13_2.Client.Entities;

namespace SPPR13_2.Services
{
    public class GameHub : Hub
    {
        private static Dictionary<string, Model> models = new() {
            { "One", new Model() },
            { "Two", new Model() },
            { "Three", new Model() }
        };
        private Model cur_model;
        private string cur_server;
        private int PlayersCount = 0;
        public GameHub()
        {
        }
        public void InitializeModel(string server)
        {
            cur_server = server;
            cur_model = models[server];
        }
        public async Task CreatePlayerAndLoadMap(string name, string server)
        {
            cur_server = server;
            cur_model = models[server];
            var new_player = cur_model.CreatePlayer(name);
            Context.Items["model"] = cur_model;
            Context.Items["server"] = cur_server;
            await Groups.AddToGroupAsync(Context.ConnectionId, server);
            await Clients.Caller.SendAsync("LoadPlayer", new_player);
            await Clients.Caller.SendAsync("LoadMap", cur_model.Foods.ToList());
        }
        public async Task CreatePlayer(string name, string server)
        {
            cur_model = Context.Items["model"] as Model;

            var new_player = cur_model.CreatePlayer(name);
            await Groups.AddToGroupAsync(Context.ConnectionId, server);
            await Clients.Caller.SendAsync("LoadPlayer", new_player);
        }
        public async Task LoadMap()
        {
            cur_model = Context.Items["model"] as Model;

            await Clients.Caller.SendAsync("LoadMap", cur_model.Foods.ToList());
        }

        public async Task UpdateAllPlayers()
        {
            cur_server = Context.Items["server"] as string;
            cur_model = Context.Items["model"] as Model;

            await Clients.Group(cur_server).SendAsync("UpdateAllPlayers", cur_model.Players.ToList());

            Context.Items["server"] = cur_server;
        }

        public async Task UpdatePlayer(Player player)
        {
            cur_model = Context.Items["model"] as Model;

            await cur_model.UpdatePlayer(player);

            Context.Items["model"] = cur_model;
        }

        public async Task EatFood(int index)
        {
            cur_server = Context.Items["server"] as string;
            cur_model = Context.Items["model"] as Model;

            var new_food = cur_model.EatFood(index);
            await Clients.Group(cur_server).SendAsync("AddFood", new_food);
            await Clients.Group(cur_server).SendAsync("DeleteFood", index);

            Context.Items["model"] = cur_model;
        }

        public async Task Kill(string victim_id, string killer_id)
        {
            cur_server = Context.Items["server"] as string;
            cur_model = Context.Items["model"] as Model;

            cur_model.NewSize(victim_id, killer_id);
            cur_model.RemovePlayer(victim_id);

            await Clients.Group(cur_server).SendAsync("ResetPlayer", victim_id);
            Context.Items["model"] = cur_model;
        }

        public async Task ResetPlayer(string player_id)
        {
            await Clients.Caller.SendAsync("ResetPlayer", player_id);
        }

        public async Task Move(string player_id, double new_mouse_x, double new_mouse_y)
        {
            cur_server = Context.Items["server"] as string;
            cur_model = Context.Items["model"] as Model;

            cur_model.Move(player_id, new_mouse_x, new_mouse_y);
            Context.Items["model"] = cur_model;
        }
        public async Task NewSize(string player_id)
        {
            cur_model = Context.Items["model"] as Model;

            cur_model.NewSize(player_id);

            Context.Items["model"] = cur_model;
        }

        public async Task EndPlay(string player_id)
        {
            cur_server = Context.Items["server"] as string;
            cur_model = Context.Items["model"] as Model;
            cur_model.Players.Remove(cur_model.Players.First(item=>item.player_id==player_id));

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, cur_server);

            Context.Items["model"] = cur_model;
        }
    }
}
