namespace SPPR13_2.Client.Entities
{
    public class Food
    {
        public double x { get; set; }
        public double y { get; set; }
        public double size { get; set; }
        public string color { get; set; }
        public bool is_eated { get; set; }
        public string type { get; set; }

        public Food()
        {
            x = EntitiesService.GetRandCoord();
            y = EntitiesService.GetRandCoord();
            size = Rules.FoodSize;
            color = EntitiesService.GetRandColor();
            is_eated = false;
            type = "Food";
        }

    }

}
