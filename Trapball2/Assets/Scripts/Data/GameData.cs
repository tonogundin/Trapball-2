using System.Collections.Generic;


[System.Serializable]
public class GameData
{
    public List<ZoneSave> saves = new List<ZoneSave>(3);
}

public class ZoneSave
{
    long totalCourage = 0;
    public List<Stage> stages = new List<Stage>(2048);
}

public class Stage
{
    int courage = 0;
    int percentCourage = 0;
    bool isOpened = false;
}
