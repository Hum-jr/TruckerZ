using Godot;

[System.Serializable]
public partial class Mission : Resource
{
	[Export] public string MissionId { get; set; }
	[Export] public string Title { get; set; }
	[Export] public string Description { get; set; }
	[Export] public Texture2D Icon { get; set; }
	[Export] public MissionType Type { get; set; }
	[Export] public MissionStatus Status { get; set; } = MissionStatus.NotStarted;
	
	// Mission requirements
	[Export] public Vector3 StartLocation { get; set; }
	[Export] public Vector3 EndLocation { get; set; }
	[Export] public float TimeLimit { get; set; } // In seconds, 0 = no limit
	[Export] public int RewardMoney { get; set; }
	[Export] public int RewardXP { get; set; }
	
	// Mission-specific data
	[Export] public string TargetItem { get; set; } // For delivery missions
	[Export] public int TargetQuantity { get; set; } // Amount to deliver
	[Export] public float RequiredSpeed { get; set; } // For time trials
	
	// Prerequisites
	[Export] public string[] RequiredMissions { get; set; } // Must complete these first
	[Export] public int RequiredLevel { get; set; }
}

public enum MissionType
{
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
