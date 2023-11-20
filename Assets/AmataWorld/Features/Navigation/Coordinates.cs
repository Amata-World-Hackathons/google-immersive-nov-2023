namespace AmataWorld.Features.Navigation
{
    /// <summary>
    /// Represents a single route
    /// </summary>
    public record GDirRouteStep
    {
        /// <summary>
        /// The start of the route
        /// </summary>
        public readonly Coordinates start;
        public readonly Coordinates end;
        public readonly GDirPolyline polyline;
        public readonly string htmlInstructions;
        public readonly int distanceInMeters;
        public readonly int durationInSeconds;
    }

    /// <summary>
    /// Represents a sequence of points that form a polyline
    /// </summary>
    public record GDirPolyline
    {
        public readonly Coordinates[] coordinates;
    }

    /// <summary>
    /// Coordinates encapsulate a latitude/longitude pair
    /// </summary>
    public readonly struct Coordinates
    {
        public readonly double latitude;
        public readonly double longitude;
    }
}