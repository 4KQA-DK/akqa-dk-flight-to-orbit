namespace UmbracoProject.DTO
{
    public class GetRocketResponse
    {
        public Guid RocketKey { get; set; }

        public string RocketName { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string Status { get; set; } = "Idle";

    }
}
