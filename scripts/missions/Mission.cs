using Godot;

[GlobalClass]
public partial class Mission : Resource
{
    [Export] public string MissionId { get; set; } = "";
    [Export] public string Title { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
    [Export] public MissionType Type { get; set; } = MissionType.Delivery;
    [Export] public MissionStatus Status { get; set; } = MissionStatus.NotStarted;
    
    // Mission requirements
    [Export] public Vector3 StartLocation { get; set; } = Vector3.Zero;
    [Export] public Vector3 EndLocation { get; set; } = Vector3.Zero;
    [Export] public float TimeLimit { get; set; } = 0f; // In seconds, 0 = no limit
    [Export] public int RewardMoney { get; set; } = 0;
    [Export] public int RewardXp { get; set; } = 0;
    
    // Mission-specific data
    [Export] public string TargetItem { get; set; } = "";
    [Export] public int TargetQuantity { get; set; } = 0;
    [Export] public float RequiredSpeed { get; set; } = 0f;
    
    // Prerequisites
    [Export] public string[] RequiredMissions { get; set; } = new string[0];
    [Export] public int RequiredLevel { get; set; } = 1;
}

public enum MissionType
{
    Park,
    Delivery,
    TimeTrail,
    Escort,
    Collection,
    Exploration,
    Combat
}

public enum MissionStatus
{
    NotStarted,
    Available,
    InProgress,
    Completed,
    Failed,
    Locked
}