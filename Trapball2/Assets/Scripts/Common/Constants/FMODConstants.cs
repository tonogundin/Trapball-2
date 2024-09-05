public class FMODConstants
{
    public enum MATERIAL
    {
        WATER,
        GRASS,
        SAND,
        METAL,
        METAL2,
        STONE,
        MUD,
        WOOD,
        NONE
    }

    public enum MUSIC_STATE
    {
        ON_STAGE,
        FINAL
    }

    public const string STATE_MUSIC = "StageFinal";
    public const string PERCENT_STAGE = "percentStage";
    public const string TERRAIN = "Terrain";
    public const string SPEED = "speed";
    public const string ON_TRIGGER = "Ontrigger";


    public const float LIMIT_SOUND_VALUE = 8;

    public enum OBJECTS
    {
        [FMODUtils.StringValue("event:/Objetos/ImpactObjectPlayer")]
        IMPACT_OBJECT_PLAYER,
        [FMODUtils.StringValue("event:/Objetos/ImpactObjectEnemies")]
        IMPACT_OBJECT_ENEMIES,
        [FMODUtils.StringValue("event:/Objetos/GrabMoon")]
        GRAB_MOON,
        [FMODUtils.StringValue("event:/Objetos/FullMoon")]
        FULL_MOON,
        [FMODUtils.StringValue("event:/Objetos/PlatformCrack")]
        PLATFORM_CRACK,
        [FMODUtils.StringValue("event:/Objetos/PlatformBreaking")]
        PLATFORM_BREAKING,
    }
    public enum MOVE
    {
        [FMODUtils.StringValue("event:/Desplazamiento/SFXPlayerRollMud")]
        PLAYER_ROLL,
    }
    public enum AMBIENT
    {
        [FMODUtils.StringValue("event:/Ambientes/AmbienteUnderwater")]
        UNDER_WATER,
    }
    public enum JUMPS
    {
        [FMODUtils.StringValue("event:/Saltos/ImpactoTerrenoPlayer")]
        IMPACT_TERRAIN_PLAYER,
        [FMODUtils.StringValue("event:/Saltos/ImpactoTerrenoEnemies")]
        IMPACT_TERRAIN_ENEMIES,
        [FMODUtils.StringValue("event:/Saltos/ImpactoTerrenoBomba")]
        IMPACT_TERRAIN_BOMB,
        [FMODUtils.StringValue("event:/Saltos/ExitMaterialPlayer")]
        EXIT_TERRAIN_PLAYER,
        [FMODUtils.StringValue("event:/Saltos/ExitMaterialEnemies")]
        EXIT_TERRAIN_ENEMIES,
        [FMODUtils.StringValue("event:/Saltos/ImpactWater")]
        IMPACT_WATER,
        [FMODUtils.StringValue("event:/Saltos/SaltoHigh")]
        HIGH,
        [FMODUtils.StringValue("event:/Saltos/SaltoLow")]
        LOW,
        [FMODUtils.StringValue("event:/Saltos/SaltoBomba")]
        JUMP_BOMB,
    }

    public enum DAMAGE
    {
        [FMODUtils.StringValue("event:/Daño/ImpactoPinchos")]
        IMPACT_SPIKES,
        [FMODUtils.StringValue("event:/Daño/DeathVoice")]
        DEATH_VOICE,
    }

    public enum HUD
    {
        [FMODUtils.StringValue("event:/HUD/Enter-Exit")]
        BUTTON_CLICK,
        [FMODUtils.StringValue("event:/HUD/Select")]
        BUTTON_SELECT,
        [FMODUtils.StringValue("event:/HUD/Stinger_Start")]
        GAME_START,
    }

    public enum BALLMOUSE
    {
        [FMODUtils.StringValue("event:/Enemigos/BallMouseRun")]
        RUN,
        [FMODUtils.StringValue("event:/Enemigos/BallMouseHurt")]
        HURT,
        [FMODUtils.StringValue("event:/Enemigos/BallMouseHit2")]
        HIT,
        [FMODUtils.StringValue("event:/Enemigos/BallMouseInflatingPop")]
        INFLATING_POP,
        [FMODUtils.StringValue("event:/Enemigos/BallMouseJump")]
        JUMP,
        [FMODUtils.StringValue("event:/Enemigos/BallMouseScream")]
        SCREAM,
    }
    public enum BEATLE
    {
        [FMODUtils.StringValue("event:/Enemigos/EscarabajoAttack")]
        ATTACK,
        [FMODUtils.StringValue("event:/Enemigos/EscarabajoCaída")]
        FALL,
        [FMODUtils.StringValue("event:/Enemigos/EscarabajoRun")]
        RUN,
        [FMODUtils.StringValue("event:/Enemigos/EscarabajoSalida")]
        EXIT
    }

    public enum BUSES
    {
        [FMODUtils.StringValue("bus:/MASTER")]
        MASTER,
        [FMODUtils.StringValue("bus:/MUSIC")]
        MUSIC
    }

}
