using System.Collections.Generic;

namespace AmataWorld.Features.Navigation.GoogleAPIs
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [System.Serializable]
    public class Bounds
    {
        public Coordinates northeast;
        public Coordinates southwest;
    }

    [System.Serializable]
    public class FormattedValue
    {
        public string text;
        public int value;
    }

    [System.Serializable]
    public class GeocodedWaypoint
    {
        public string geocoder_status;
        public string place_id;
        public List<string> types;
    }

    [System.Serializable]
    public class Leg
    {
        public FormattedValue distance;
        public FormattedValue duration;
        public string end_address;
        public Coordinates end_location;
        public string start_address;
        public Coordinates start_location;
        public List<Step> steps;
        public List<object> traffic_speed_entry;
        public List<object> via_waypoint;
    }

    [System.Serializable]
    public class OverviewPolyline
    {
        [System.NonSerialized]
        private List<Coordinates> _pp;
        public List<Coordinates> parsedPoints
        {
            get
            {
                if (_pp == null)
                    _pp = GoogleAPIsUtils.DecodePolylinePoints(points);
                return _pp;
            }
        }
        public string points;
    }

    [System.Serializable]
    public class Polyline
    {
        [System.NonSerialized]
        private List<Coordinates> _pp;
        public List<Coordinates> parsedPoints
        {
            get
            {
                if (_pp == null)
                    _pp = GoogleAPIsUtils.DecodePolylinePoints(points);
                return _pp;
            }
        }
        public string points;
    }

    [System.Serializable]
    public class Root
    {
        public List<GeocodedWaypoint> geocoded_waypoints;
        public List<Route> routes;
        public string status;
    }

    [System.Serializable]
    public class Route
    {
        public Bounds bounds;
        public string copyrights;
        public List<Leg> legs;
        public OverviewPolyline overview_polyline;
        public string summary;
        public List<string> warnings;
        public List<object> waypoint_order;
    }

    [System.Serializable]
    public class Coordinates
    {
        public double lat;
        public double lng;
    }

    [System.Serializable]
    public class Step
    {
        public FormattedValue distance;
        public FormattedValue duration;
        public Coordinates end_location;
        public string html_instructions;
        public Polyline polyline;
        public Coordinates start_location;
        public string travel_mode;
        public string maneuver;
    }


}