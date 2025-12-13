

var scene = document.createElement("canvas")
var MAPSIZE = 5000;
scene.width = MAPSIZE
scene.height = MAPSIZE
var stx = scene.getContext("2d")

var canvas = document.createElement("canvas")
canvas.width = window.innerWidth
canvas.height = window.innerHeight

var ctx = canvas.getContext("2d")
var closeButton = document.getElementById("close");

var players = {
    dotNetObj: null,
    circles: [],
    points: []
}
var colors = ["#ff0000", "#00ff00", "#c0c0c0", "#800080", "#008000", "#000080"]

var size_of_map = 200
var size_of_point_on_map = 2

var cur_player = { x: 1, y: 1, player_id: "0", size: 1, speed: 1, name: "name", isDead: false };

var visibility = 1.0

var ViewMap = false

function SetDotNetObject(dotNetObject) {
    players.dotNetObj = dotNetObject;
}

function LoadPlayer(player) {
    cur_player.x = Math.max(Math.min(player.x, MAPSIZE), 0)
    cur_player.y = Math.max(Math.min(player.y, MAPSIZE), 0)
    cur_player.player_id = player.player_id
    cur_player.size = player.size;
    cur_player.speed = player.speed
    cur_player.name = player.name
    cur_player.isDead = player.isDead
}

function UpdateAllPlayers(updatedPlayers) {

    players.points = updatedPlayers;

    for (let point of players.points) {

        if (point.player_id == cur_player.player_id) {
            cur_player.x = Math.max(Math.min(point.x, scene.width), 0)
            cur_player.y = Math.max(Math.min(point.y, scene.height), 0)
            cur_player.size = point.size;
            cur_player.speed = point.speed
            cur_player.name = point.name
            cur_player.isDead = point.isDead
            break
        }

    }
}

function LoadMap(map) {
    players.circles = map
    console.log("ofnjnd", map)
}

function UpdateMap(new_food) {
    players.circles.push(new_food)
}

function DeleteFood(index) {
    players.circles.splice(index, 1)
}

function ResetPlayer(victim_id) {
    if (victim_id == cur_player.player_id) {
        players.dotNetObj.invokeMethodAsync("DeletePlayer", cur_player.player_id)
        players.dotNetObj = null
    }
    else {
        let index = 0;
        for (let item of players.points) {
            if (item.player_id == victim_id) break;
            index++;
        }
        players.points.splice(index, 1)
    }
}

function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    ctx.font = "20px Arial"
    ctx.fillStyle = "black"
    ctx.fillText(`Radius: ${cur_player.size}`, 10, 30, 1000)

    let index = 0;

    for (let circle of players.circles) {

        //console.log(cur_player)
        //console.log(players.points)
        //console.log("--------------------------------------")

        const screenX = (circle.x - cur_player.x) * visibility + canvas.width / 2;
        const screenY = (circle.y - cur_player.y) * visibility + canvas.height / 2;

        if (!circle.is_eated && Math.pow(circle.x - cur_player.x, 2) + Math.pow(circle.y - cur_player.y, 2) <= cur_player.size * cur_player.size) {
            //ws.send(['new_size', cur_player.user_id.toString()].join(' '))
            //ws.send(['eat_food', 'index_of_circle', circles.indexOf(circle).toString()].join(' '))
            players.dotNetObj.invokeMethodAsync("NewSize", cur_player.player_id)
            players.dotNetObj.invokeMethodAsync("EatFood", index)
            circle.is_eated = true
            continue
        }

        //console.log(screenX, cur_player, circle, circle.size, visibility)
        //console.log(screenX + circle.size * visibility, screenY + circle.size * visibility > 0, screenX - circle.size * visibility < canvas.width,
        //screenY - circle.size * visibility < canvas.height)
        if (
            screenX + circle.size * visibility > 0 &&
            screenY + circle.size * visibility > 0 &&
            screenX - circle.size * visibility < canvas.width &&
            screenY - circle.size * visibility < canvas.height
        ) {
            ctx.beginPath();
            ctx.arc(screenX, screenY, circle.size * visibility, 0, Math.PI * 2);
            ctx.fillStyle = circle.color;
            ctx.fill();
        }
        index++
    }

    for (let point of players.points) {

        const screenX = (point.x - cur_player.x) * visibility + canvas.width / 2;
        const screenY = (point.y - cur_player.y) * visibility + canvas.height / 2;

        //if (point.user_id == cur_player.user_id) { console.log('aboba') }

        if (!point.isDead && point.player_id != cur_player.player_id && point.size * visibility < cur_player.size * visibility && Math.sqrt(Math.pow(point.x - cur_player.x, 2) + Math.pow(point.y - cur_player.y, 2)) * 1.2 < cur_player.size * visibility) {
            //ws.send(['kill', 'victim_id:', point.user_id.toString(), 'killer_id:', cur_player.user_id.toString()].join(' '))
            players.dotNetObj.invokeMethodAsync("Kill", point.player_id, cur_player.player_id)
            point.isDead = true
            if (point.player_id == cur_player.player_id) cur_player.isDead = true
        }

        if (!cur_player.isDead && point.player_id != cur_player.player_id && point.size * visibility > cur_player.size * visibility && Math.sqrt(Math.pow(point.x - cur_player.x, 2) + Math.pow(point.y - cur_player.y, 2)) * 1.2 < point.size * visibility) {
            //ws.send(['kill', 'victim_id:', cur_player.user_id.toString(), 'killer_id:', point.user_id.toString()].join(' '))
            //если нет ифа,то вызывается тем, кого убили
            if (players.dotNetObj !== null) players.dotNetObj.invokeMethodAsync("Kill", cur_player.player_id, point.player_id)
            cur_player.isDead = true
        }

        if (
            screenX + point.size * visibility > 0 &&
            screenY + point.size * visibility > 0 &&
            screenX - point.size * visibility < canvas.width &&
            screenY - point.size * visibility < canvas.height
        ) {
            ctx.beginPath();
            ctx.arc(screenX, screenY, point.size * visibility, 0, Math.PI * 2);
            ctx.fillStyle = colors[parseInt(point.player_id[0], 16) % colors.length]
            ctx.fill();
            ctx.font = `${point.size / (point.name.length / 2)}px Arial`
            ctx.fillStyle = "black"
            ctx.fillText(`${point.name}`, screenX - ctx.measureText(point.name).width / 2, screenY - point.size / point.name.length / 2)
        }
    }

    ctx.clearRect(window.innerWidth - size_of_map - size_of_point_on_map, 0, size_of_map, size_of_map)
    ctx.strokeRect(window.innerWidth - size_of_map - size_of_point_on_map, 0, size_of_map, size_of_map)

    for (let point of players.points) {
        ctx.beginPath()
        ctx.arc(Math.min(Math.max((point.x / MAPSIZE) * size_of_map + window.innerWidth - size_of_map, window.innerWidth - size_of_map + size_of_point_on_map), window.innerWidth - size_of_point_on_map), Math.min(Math.max((point.y / MAPSIZE) * size_of_map, size_of_point_on_map), size_of_map - size_of_point_on_map), size_of_point_on_map, 0, Math.PI * 2)
        ctx.fillStyle = colors[parseInt(point.player_id[0], 16) % colors.length]
        ctx.fill();
    }

    if (cur_player.size > 200 && visibility > 0.25) visibility -= 0.01
    else if (cur_player.size > 100 && visibility > 0.5) visibility -= 0.01
    else if (cur_player.size > 50 && visibility > 0.75) visibility -= 0.01

    requestAnimationFrame(() => draw())
}
draw();
document.body.append(canvas)

document.addEventListener('mousemove', e => {
    var data_x, data_y
    data_x = e.clientX - canvas.width / 2;
    data_y = e.clientY - canvas.height / 2;
    //ws.send([`move`, 'index:', cur_player.user_id.toString(), 'X:', data_x.toString(), 'Y:', data_y.toString()].join(" "))
    //console.log(cur_player);
    if (players.dotNetObj !== null) players.dotNetObj.invokeMethodAsync("Move", cur_player.player_id, data_x, data_y)
})
