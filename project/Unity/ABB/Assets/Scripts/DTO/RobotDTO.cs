[System.Serializable]
public class RobotDTO
{
    public int robotID;
    public string robotName;
}

[System.Serializable]
public class RobotList
{
    public RobotDTO[] robots;
}