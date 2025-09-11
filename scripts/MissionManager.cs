using Godot;

// In a MissionManager node or similar
public partial class MissionManager : Node
{
    private void LoadMission(string missionPath)
    {
        Mission mission = GD.Load<Mission>(missionPath);
        GD.Print($"Loaded mission: {mission.Title}");
        // Use the mission data...
    }
}