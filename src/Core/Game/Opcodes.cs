namespace GTA3SaveEditor.Core.Game
{
    public enum Opcode : short
    {
        NOP = 0,
        WAIT,
        GOTO,
        SHAKE_CAM,
        SET_VAR_INT,
        SET_VAR_FLOAT,
        SET_LVAR_INT,
        SET_LVAR_FLOAT,
        ADD_VAL_TO_INT_VAR,
        ADD_VAL_TO_FLOAT_VAR,
        ADD_VAL_TO_INT_LVAR,
        ADD_VAL_TO_FLOAT_LVAR,
        SUB_VAL_FROM_INT_VAR,
        SUB_VAL_FROM_FLOAT_VAR,
        SUB_VAL_FROM_INT_LVAR,
        SUB_VAL_FROM_FLOAT_LVAR,
        MULT_INT_VAR_BY_VAL,
        MULT_FLOAT_VAR_BY_VAL,
        MULT_INT_LVAR_BY_VAL,
        MULT_FLOAT_LVAR_BY_VAL,
        DIV_INT_VAR_BY_VAL,
        DIV_FLOAT_VAR_BY_VAL,
        DIV_INT_LVAR_BY_VAL,
        DIV_FLOAT_LVAR_BY_VAL,
        IS_INT_VAR_GREATER_THAN_NUMBER,
        IS_INT_LVAR_GREATER_THAN_NUMBER,
        IS_NUMBER_GREATER_THAN_INT_VAR,
        IS_NUMBER_GREATER_THAN_INT_LVAR,
        IS_INT_VAR_GREATER_THAN_INT_VAR,
        IS_INT_LVAR_GREATER_THAN_INT_LVAR,
        IS_INT_VAR_GREATER_THAN_INT_LVAR,
        IS_INT_LVAR_GREATER_THAN_INT_VAR,
        IS_FLOAT_VAR_GREATER_THAN_NUMBER,
        IS_FLOAT_LVAR_GREATER_THAN_NUMBER,
        IS_NUMBER_GREATER_THAN_FLOAT_VAR,
        IS_NUMBER_GREATER_THAN_FLOAT_LVAR,
        IS_FLOAT_VAR_GREATER_THAN_FLOAT_VAR,
        IS_FLOAT_LVAR_GREATER_THAN_FLOAT_LVAR,
        IS_FLOAT_VAR_GREATER_THAN_FLOAT_LVAR,
        IS_FLOAT_LVAR_GREATER_THAN_FLOAT_VAR,
        IS_INT_VAR_GREATER_OR_EQUAL_TO_NUMBER,
        IS_INT_LVAR_GREATER_OR_EQUAL_TO_NUMBER,
        IS_NUMBER_GREATER_OR_EQUAL_TO_INT_VAR,
        IS_NUMBER_GREATER_OR_EQUAL_TO_INT_LVAR,
        IS_INT_VAR_GREATER_OR_EQUAL_TO_INT_VAR,
        IS_INT_LVAR_GREATER_OR_EQUAL_TO_INT_LVAR,
        IS_INT_VAR_GREATER_OR_EQUAL_TO_INT_LVAR,
        IS_INT_LVAR_GREATER_OR_EQUAL_TO_INT_VAR,
        IS_FLOAT_VAR_GREATER_OR_EQUAL_TO_NUMBER,
        IS_FLOAT_LVAR_GREATER_OR_EQUAL_TO_NUMBER,
        IS_NUMBER_GREATER_OR_EQUAL_TO_FLOAT_VAR,
        IS_NUMBER_GREATER_OR_EQUAL_TO_FLOAT_LVAR,
        IS_FLOAT_VAR_GREATER_OR_EQUAL_TO_FLOAT_VAR,
        IS_FLOAT_LVAR_GREATER_OR_EQUAL_TO_FLOAT_LVAR,
        IS_FLOAT_VAR_GREATER_OR_EQUAL_TO_FLOAT_LVAR,
        IS_FLOAT_LVAR_GREATER_OR_EQUAL_TO_FLOAT_VAR,
        IS_INT_VAR_EQUAL_TO_NUMBER,
        IS_INT_LVAR_EQUAL_TO_NUMBER,
        IS_INT_VAR_EQUAL_TO_INT_VAR,
        IS_INT_LVAR_EQUAL_TO_INT_LVAR,
        IS_INT_VAR_EQUAL_TO_INT_LVAR,
        IS_INT_VAR_NOT_EQUAL_TO_NUMBER,
        IS_INT_LVAR_NOT_EQUAL_TO_NUMBER,
        IS_INT_VAR_NOT_EQUAL_TO_INT_VAR,
        IS_INT_LVAR_NOT_EQUAL_TO_INT_LVAR,
        IS_INT_VAR_NOT_EQUAL_TO_INT_LVAR,
        IS_FLOAT_VAR_EQUAL_TO_NUMBER,
        IS_FLOAT_LVAR_EQUAL_TO_NUMBER,
        IS_FLOAT_VAR_EQUAL_TO_FLOAT_VAR,
        IS_FLOAT_LVAR_EQUAL_TO_FLOAT_LVAR,
        IS_FLOAT_VAR_EQUAL_TO_FLOAT_LVAR,
        IS_FLOAT_VAR_NOT_EQUAL_TO_NUMBER,
        IS_FLOAT_LVAR_NOT_EQUAL_TO_NUMBER,
        IS_FLOAT_VAR_NOT_EQUAL_TO_FLOAT_VAR,
        IS_FLOAT_LVAR_NOT_EQUAL_TO_FLOAT_LVAR,
        IS_FLOAT_VAR_NOT_EQUAL_TO_FLOAT_LVAR,
        GOTO_IF_TRUE,
        GOTO_IF_FALSE,
        TERMINATE_THIS_SCRIPT,
        START_NEW_SCRIPT,
        GOSUB,
        RETURN,
        LINE,
        CREATE_PLAYER,
        GET_PLAYER_COORDINATES,
        SET_PLAYER_COORDINATES,
        IS_PLAYER_IN_AREA_2D,
        IS_PLAYER_IN_AREA_3D,
        ADD_INT_VAR_TO_INT_VAR,
        ADD_FLOAT_VAR_TO_FLOAT_VAR,
        ADD_INT_LVAR_TO_INT_LVAR,
        ADD_FLOAT_LVAR_TO_FLOAT_LVAR,
        ADD_INT_VAR_TO_INT_LVAR,
        ADD_FLOAT_VAR_TO_FLOAT_LVAR,
        ADD_INT_LVAR_TO_INT_VAR,
        ADD_FLOAT_LVAR_TO_FLOAT_VAR,
        SUB_INT_VAR_FROM_INT_VAR,
        SUB_FLOAT_VAR_FROM_FLOAT_VAR,
        SUB_INT_LVAR_FROM_INT_LVAR,
        SUB_FLOAT_LVAR_FROM_FLOAT_LVAR,
        SUB_INT_VAR_FROM_INT_LVAR,
        SUB_FLOAT_VAR_FROM_FLOAT_LVAR,
        SUB_INT_LVAR_FROM_INT_VAR,
        SUB_FLOAT_LVAR_FROM_FLOAT_VAR,
        MULT_INT_VAR_BY_INT_VAR,
        MULT_FLOAT_VAR_BY_FLOAT_VAR,
        MULT_INT_LVAR_BY_INT_LVAR,
        MULT_FLOAT_LVAR_BY_FLOAT_LVAR,
        MULT_INT_VAR_BY_INT_LVAR,
        MULT_FLOAT_VAR_BY_FLOAT_LVAR,
        MULT_INT_LVAR_BY_INT_VAR,
        MULT_FLOAT_LVAR_BY_FLOAT_VAR,
        DIV_INT_VAR_BY_INT_VAR,
        DIV_FLOAT_VAR_BY_FLOAT_VAR,
        DIV_INT_LVAR_BY_INT_LVAR,
        DIV_FLOAT_LVAR_BY_FLOAT_LVAR,
        DIV_INT_VAR_BY_INT_LVAR,
        DIV_FLOAT_VAR_BY_FLOAT_LVAR,
        DIV_INT_LVAR_BY_INT_VAR,
        DIV_FLOAT_LVAR_BY_FLOAT_VAR,
        ADD_TIMED_VAL_TO_FLOAT_VAR,
        ADD_TIMED_VAL_TO_FLOAT_LVAR,
        ADD_TIMED_FLOAT_VAR_TO_FLOAT_VAR,
        ADD_TIMED_FLOAT_LVAR_TO_FLOAT_LVAR,
        ADD_TIMED_FLOAT_LVAR_TO_FLOAT_VAR,
        ADD_TIMED_FLOAT_VAR_TO_FLOAT_LVAR,
        SUB_TIMED_VAL_FROM_FLOAT_VAR,
        SUB_TIMED_VAL_FROM_FLOAT_LVAR,
        SUB_TIMED_FLOAT_VAR_FROM_FLOAT_VAR,
        SUB_TIMED_FLOAT_LVAR_FROM_FLOAT_LVAR,
        SUB_TIMED_FLOAT_LVAR_FROM_FLOAT_VAR,
        SUB_TIMED_FLOAT_VAR_FROM_FLOAT_LVAR,
        SET_VAR_INT_TO_VAR_INT,
        SET_LVAR_INT_TO_LVAR_INT,
        SET_VAR_FLOAT_TO_VAR_FLOAT,
        SET_LVAR_FLOAT_TO_LVAR_FLOAT,
        SET_VAR_FLOAT_TO_LVAR_FLOAT,
        SET_LVAR_FLOAT_TO_VAR_FLOAT,
        SET_VAR_INT_TO_LVAR_INT,
        SET_LVAR_INT_TO_VAR_INT,
        CSET_VAR_INT_TO_VAR_FLOAT,
        CSET_VAR_FLOAT_TO_VAR_INT,
        CSET_LVAR_INT_TO_VAR_FLOAT,
        CSET_LVAR_FLOAT_TO_VAR_INT,
        CSET_VAR_INT_TO_LVAR_FLOAT,
        CSET_VAR_FLOAT_TO_LVAR_INT,
        CSET_LVAR_INT_TO_LVAR_FLOAT,
        CSET_LVAR_FLOAT_TO_LVAR_INT,
        ABS_VAR_INT,
        ABS_LVAR_INT,
        ABS_VAR_FLOAT,
        ABS_LVAR_FLOAT,
        GENERATE_RANDOM_FLOAT,
        GENERATE_RANDOM_INT,
        CREATE_CHAR,
        DELETE_CHAR,
        CHAR_WANDER_DIR,
        CHAR_WANDER_RANGE,
        CHAR_FOLLOW_PATH,
        CHAR_SET_IDLE,
        GET_CHAR_COORDINATES,
        SET_CHAR_COORDINATES,
        IS_CHAR_STILL_ALIVE,
        IS_CHAR_IN_AREA_2D,
        IS_CHAR_IN_AREA_3D,
        CREATE_CAR,
        DELETE_CAR,
        CAR_GOTO_COORDINATES,
        CAR_WANDER_RANDOMLY,
        CAR_SET_IDLE,
        GET_CAR_COORDINATES,
        SET_CAR_COORDINATES,
        IS_CAR_STILL_ALIVE,
        SET_CAR_CRUISE_SPEED,
        SET_CAR_DRIVING_STYLE,
        SET_CAR_MISSION,
        IS_CAR_IN_AREA_2D,
        IS_CAR_IN_AREA_3D,
        SPECIAL_0,
        SPECIAL_1,
        SPECIAL_2,
        SPECIAL_3,
        SPECIAL_4,
        SPECIAL_5,
        SPECIAL_6,
        SPECIAL_7,
        PRINT_BIG,
        PRINT,
        PRINT_NOW,
        PRINT_SOON,
        CLEAR_PRINTS,
        GET_TIME_OF_DAY,
        SET_TIME_OF_DAY,
        GET_MINUTES_TO_TIME_OF_DAY,
        IS_POINT_ON_SCREEN,
        DEBUG_ON,
        DEBUG_OFF,
        RETURN_TRUE,
        RETURN_FALSE,
        VAR_INT,
        VAR_FLOAT,
        LVAR_INT,
        LVAR_FLOAT,
        LBRACKET,
        RBRACKET,
        REPEAT,
        ENDREPEAT,
        IF,
        IFNOT,
        ELSE,
        ENDIF,
        WHILE,
        WHILENOT,
        ENDWHILE,
        ANDOR,
        LAUNCH_MISSION,
        MISSION_HAS_FINISHED,
        STORE_CAR_CHAR_IS_IN,
        STORE_CAR_PLAYER_IS_IN,
        IS_CHAR_IN_CAR,
        IS_PLAYER_IN_CAR,
        IS_CHAR_IN_MODEL,
        IS_PLAYER_IN_MODEL,
        IS_CHAR_IN_ANY_CAR,
        IS_PLAYER_IN_ANY_CAR,
        IS_BUTTON_PRESSED,
        GET_PAD_STATE,
        LOCATE_PLAYER_ANY_MEANS_2D,
        LOCATE_PLAYER_ON_FOOT_2D,
        LOCATE_PLAYER_IN_CAR_2D,
        LOCATE_STOPPED_PLAYER_ANY_MEANS_2D,
        LOCATE_STOPPED_PLAYER_ON_FOOT_2D,
        LOCATE_STOPPED_PLAYER_IN_CAR_2D,
        LOCATE_PLAYER_ANY_MEANS_CHAR_2D,
        LOCATE_PLAYER_ON_FOOT_CHAR_2D,
        LOCATE_PLAYER_IN_CAR_CHAR_2D,
        LOCATE_CHAR_ANY_MEANS_2D,
        LOCATE_CHAR_ON_FOOT_2D,
        LOCATE_CHAR_IN_CAR_2D,
        LOCATE_STOPPED_CHAR_ANY_MEANS_2D,
        LOCATE_STOPPED_CHAR_ON_FOOT_2D,
        LOCATE_STOPPED_CHAR_IN_CAR_2D,
        LOCATE_CHAR_ANY_MEANS_CHAR_2D,
        LOCATE_CHAR_ON_FOOT_CHAR_2D,
        LOCATE_CHAR_IN_CAR_CHAR_2D,
        LOCATE_PLAYER_ANY_MEANS_3D,
        LOCATE_PLAYER_ON_FOOT_3D,
        LOCATE_PLAYER_IN_CAR_3D,
        LOCATE_STOPPED_PLAYER_ANY_MEANS_3D,
        LOCATE_STOPPED_PLAYER_ON_FOOT_3D,
        LOCATE_STOPPED_PLAYER_IN_CAR_3D,
        LOCATE_PLAYER_ANY_MEANS_CHAR_3D,
        LOCATE_PLAYER_ON_FOOT_CHAR_3D,
        LOCATE_PLAYER_IN_CAR_CHAR_3D,
        LOCATE_CHAR_ANY_MEANS_3D,
        LOCATE_CHAR_ON_FOOT_3D,
        LOCATE_CHAR_IN_CAR_3D,
        LOCATE_STOPPED_CHAR_ANY_MEANS_3D,
        LOCATE_STOPPED_CHAR_ON_FOOT_3D,
        LOCATE_STOPPED_CHAR_IN_CAR_3D,
        LOCATE_CHAR_ANY_MEANS_CHAR_3D,
        LOCATE_CHAR_ON_FOOT_CHAR_3D,
        LOCATE_CHAR_IN_CAR_CHAR_3D,
        CREATE_OBJECT,
        DELETE_OBJECT,
        ADD_SCORE,
        IS_SCORE_GREATER,
        STORE_SCORE,
        GIVE_REMOTE_CONTROLLED_CAR_TO_PLAYER,
        ALTER_WANTED_LEVEL,
        ALTER_WANTED_LEVEL_NO_DROP,
        IS_WANTED_LEVEL_GREATER,
        CLEAR_WANTED_LEVEL,
        SET_DEATHARREST_STATE,
        HAS_DEATHARREST_BEEN_EXECUTED,
        ADD_AMMO_TO_PLAYER,
        ADD_AMMO_TO_CHAR,
        ADD_AMMO_TO_CAR,
        IS_PLAYER_STILL_ALIVE,
        IS_PLAYER_DEAD,
        IS_CHAR_DEAD,
        IS_CAR_DEAD,
        SET_CHAR_THREAT_SEARCH,
        SET_CHAR_THREAT_REACTION,
        SET_CHAR_OBJ_NO_OBJ,
        ORDER_DRIVER_OUT_OF_CAR,
        ORDER_CHAR_TO_DRIVE_CAR,
        ADD_PATROL_POINT,
        IS_PLAYER_IN_GANGZONE,
        IS_PLAYER_IN_ZONE,
        IS_PLAYER_PRESSING_HORN,
        HAS_CHAR_SPOTTED_PLAYER,
        ORDER_CHAR_TO_BACKDOOR,
        ADD_CHAR_TO_GANG,
        IS_CHAR_OBJECTIVE_PASSED,
        SET_CHAR_DRIVE_AGGRESSION,
        SET_CHAR_MAX_DRIVESPEED,
        CREATE_CHAR_INSIDE_CAR,
        WARP_PLAYER_FROM_CAR_TO_COORD,
        MAKE_CHAR_DO_NOTHING,
        SET_CHAR_INVINCIBLE,
        SET_PLAYER_INVINCIBLE,
        SET_CHAR_GRAPHIC_TYPE,
        SET_PLAYER_GRAPHIC_TYPE,
        HAS_PLAYER_BEEN_ARRESTED,
        STOP_CHAR_DRIVING,
        KILL_CHAR,
        SET_FAVOURITE_CAR_MODEL_FOR_CHAR,
        SET_CHAR_OCCUPATION,
        CHANGE_CAR_LOCK,
        SHAKE_CAM_WITH_POINT,
        IS_CAR_MODEL,
        IS_CAR_REMAP,
        HAS_CAR_JUST_SUNK,
        SET_CAR_NO_COLLIDE,
        IS_CAR_DEAD_IN_AREA_2D,
        IS_CAR_DEAD_IN_AREA_3D,
        IS_TRAILER_ATTACHED,
        IS_CAR_ON_TRAILER,
        HAS_CAR_GOT_WEAPON,
        PARK,
        HAS_PARK_FINISHED,
        KILL_ALL_PASSENGERS,
        SET_CAR_BULLETPROOF,
        SET_CAR_FLAMEPROOF,
        SET_CAR_ROCKETPROOF,
        IS_CARBOMB_ACTIVE,
        GIVE_CAR_ALARM,
        PUT_CAR_ON_TRAILER,
        IS_CAR_CRUSHED,
        CREATE_GANG_CAR,
        CREATE_CAR_GENERATOR,
        SWITCH_CAR_GENERATOR,
        ADD_PAGER_MESSAGE,
        DISPLAY_ONSCREEN_TIMER,
        CLEAR_ONSCREEN_TIMER,
        DISPLAY_ONSCREEN_COUNTER,
        CLEAR_ONSCREEN_COUNTER,
        SET_ZONE_CAR_INFO,
        IS_CHAR_IN_GANG_ZONE,
        IS_CHAR_IN_ZONE,
        SET_CAR_DENSITY,
        SET_PED_DENSITY,
        POINT_CAMERA_AT_PLAYER,
        POINT_CAMERA_AT_CAR,
        POINT_CAMERA_AT_CHAR,
        RESTORE_CAMERA,
        SHAKE_PAD,
        SET_ZONE_PED_INFO,
        SET_TIME_SCALE,
        IS_CAR_IN_AIR,
        SET_FIXED_CAMERA_POSITION,
        POINT_CAMERA_AT_POINT,
        ADD_BLIP_FOR_CAR_OLD,
        ADD_BLIP_FOR_CHAR_OLD,
        ADD_BLIP_FOR_OBJECT_OLD,
        REMOVE_BLIP,
        CHANGE_BLIP_COLOUR,
        DIM_BLIP,
        ADD_BLIP_FOR_COORD_OLD,
        CHANGE_BLIP_SCALE,
        SET_FADING_COLOUR,
        DO_FADE,
        GET_FADING_STATUS,
        ADD_HOSPITAL_RESTART,
        ADD_POLICE_RESTART,
        OVERRIDE_NEXT_RESTART,
        DRAW_SHADOW,
        GET_PLAYER_HEADING,
        SET_PLAYER_HEADING,
        GET_CHAR_HEADING,
        SET_CHAR_HEADING,
        GET_CAR_HEADING,
        SET_CAR_HEADING,
        GET_OBJECT_HEADING,
        SET_OBJECT_HEADING,
        IS_PLAYER_TOUCHING_OBJECT,
        IS_CHAR_TOUCHING_OBJECT,
        SET_PLAYER_AMMO,
        SET_CHAR_AMMO,
        SET_CAR_AMMO,
        LOAD_CAMERA_SPLINE,
        MOVE_CAMERA_ALONG_SPLINE,
        GET_CAMERA_POSITION_ALONG_SPLINE,
        DECLARE_MISSION_FLAG,
        DECLARE_MISSION_FLAG_FOR_CONTACT,
        DECLARE_BASE_BRIEF_ID_FOR_CONTACT,
        IS_PLAYER_HEALTH_GREATER,
        IS_CHAR_HEALTH_GREATER,
        IS_CAR_HEALTH_GREATER,
        ADD_BLIP_FOR_CAR,
        ADD_BLIP_FOR_CHAR,
        ADD_BLIP_FOR_OBJECT,
        ADD_BLIP_FOR_CONTACT_POINT,
        ADD_BLIP_FOR_COORD,
        CHANGE_BLIP_DISPLAY,
        ADD_ONE_OFF_SOUND,
        ADD_CONTINUOUS_SOUND,
        REMOVE_SOUND,
        IS_CAR_STUCK_ON_ROOF,
        ADD_UPSIDEDOWN_CAR_CHECK,
        REMOVE_UPSIDEDOWN_CAR_CHECK,
        SET_CHAR_OBJ_WAIT_ON_FOOT,
        SET_CHAR_OBJ_FLEE_ON_FOOT_TILL_SAFE,
        SET_CHAR_OBJ_GUARD_SPOT,
        SET_CHAR_OBJ_GUARD_AREA,
        SET_CHAR_OBJ_WAIT_IN_CAR,
        IS_PLAYER_IN_AREA_ON_FOOT_2D,
        IS_PLAYER_IN_AREA_IN_CAR_2D,
        IS_PLAYER_STOPPED_IN_AREA_2D,
        IS_PLAYER_STOPPED_IN_AREA_ON_FOOT_2D,
        IS_PLAYER_STOPPED_IN_AREA_IN_CAR_2D,
        IS_PLAYER_IN_AREA_ON_FOOT_3D,
        IS_PLAYER_IN_AREA_IN_CAR_3D,
        IS_PLAYER_STOPPED_IN_AREA_3D,
        IS_PLAYER_STOPPED_IN_AREA_ON_FOOT_3D,
        IS_PLAYER_STOPPED_IN_AREA_IN_CAR_3D,
        IS_CHAR_IN_AREA_ON_FOOT_2D,
        IS_CHAR_IN_AREA_IN_CAR_2D,
        IS_CHAR_STOPPED_IN_AREA_2D,
        IS_CHAR_STOPPED_IN_AREA_ON_FOOT_2D,
        IS_CHAR_STOPPED_IN_AREA_IN_CAR_2D,
        IS_CHAR_IN_AREA_ON_FOOT_3D,
        IS_CHAR_IN_AREA_IN_CAR_3D,
        IS_CHAR_STOPPED_IN_AREA_3D,
        IS_CHAR_STOPPED_IN_AREA_ON_FOOT_3D,
        IS_CHAR_STOPPED_IN_AREA_IN_CAR_3D,
        IS_CAR_STOPPED_IN_AREA_2D,
        IS_CAR_STOPPED_IN_AREA_3D,
        LOCATE_CAR_2D,
        LOCATE_STOPPED_CAR_2D,
        LOCATE_CAR_3D,
        LOCATE_STOPPED_CAR_3D,
        GIVE_WEAPON_TO_PLAYER,
        GIVE_WEAPON_TO_CHAR,
        GIVE_WEAPON_TO_CAR,
        SET_PLAYER_CONTROL,
        FORCE_WEATHER,
        FORCE_WEATHER_NOW,
        RELEASE_WEATHER,
        SET_CURRENT_PLAYER_WEAPON,
        SET_CURRENT_CHAR_WEAPON,
        SET_CURRENT_CAR_WEAPON,
        GET_OBJECT_COORDINATES,
        SET_OBJECT_COORDINATES,
        GET_GAME_TIMER,
        TURN_CHAR_TO_FACE_COORD,
        TURN_PLAYER_TO_FACE_COORD,
        STORE_WANTED_LEVEL,
        IS_CAR_STOPPED,
        MARK_CHAR_AS_NO_LONGER_NEEDED,
        MARK_CAR_AS_NO_LONGER_NEEDED,
        MARK_OBJECT_AS_NO_LONGER_NEEDED,
        DONT_REMOVE_CHAR,
        DONT_REMOVE_CAR,
        DONT_REMOVE_OBJECT,
        CREATE_CHAR_AS_PASSENGER,
        SET_CHAR_OBJ_KILL_CHAR_ON_FOOT,
        SET_CHAR_OBJ_KILL_PLAYER_ON_FOOT,
        SET_CHAR_OBJ_KILL_CHAR_ANY_MEANS,
        SET_CHAR_OBJ_KILL_PLAYER_ANY_MEANS,
        SET_CHAR_OBJ_FLEE_CHAR_ON_FOOT_TILL_SAFE,
        SET_CHAR_OBJ_FLEE_PLAYER_ON_FOOT_TILL_SAFE,
        SET_CHAR_OBJ_FLEE_CHAR_ON_FOOT_ALWAYS,
        SET_CHAR_OBJ_FLEE_PLAYER_ON_FOOT_ALWAYS,
        SET_CHAR_OBJ_GOTO_CHAR_ON_FOOT,
        SET_CHAR_OBJ_GOTO_PLAYER_ON_FOOT,
        SET_CHAR_OBJ_LEAVE_CAR,
        SET_CHAR_OBJ_ENTER_CAR_AS_PASSENGER,
        SET_CHAR_OBJ_ENTER_CAR_AS_DRIVER,
        SET_CHAR_OBJ_FOLLOW_CAR_IN_CAR,
        SET_CHAR_OBJ_FIRE_AT_OBJECT_FROM_VEHICLE,
        SET_CHAR_OBJ_DESTROY_OBJECT,
        SET_CHAR_OBJ_DESTROY_CAR,
        SET_CHAR_OBJ_GOTO_AREA_ON_FOOT,
        SET_CHAR_OBJ_GOTO_AREA_IN_CAR,
        SET_CHAR_OBJ_FOLLOW_CAR_ON_FOOT_WITH_OFFSET,
        SET_CHAR_OBJ_GUARD_ATTACK,
        SET_CHAR_AS_LEADER,
        SET_PLAYER_AS_LEADER,
        LEAVE_GROUP,
        SET_CHAR_OBJ_FOLLOW_ROUTE,
        ADD_ROUTE_POINT,
        PRINT_WITH_NUMBER_BIG,
        PRINT_WITH_NUMBER,
        PRINT_WITH_NUMBER_NOW,
        PRINT_WITH_NUMBER_SOON,
        SWITCH_ROADS_ON,
        SWITCH_ROADS_OFF,
        GET_NUMBER_OF_PASSENGERS,
        GET_MAXIMUM_NUMBER_OF_PASSENGERS,
        SET_CAR_DENSITY_MULTIPLIER,
        SET_CAR_HEAVY,
        CLEAR_CHAR_THREAT_SEARCH,
        ACTIVATE_CRANE,
        DEACTIVATE_CRANE,
        SET_MAX_WANTED_LEVEL,
        SAVE_VAR_INT,
        SAVE_VAR_FLOAT,
        IS_CAR_IN_AIR_PROPER,
        IS_CAR_UPSIDEDOWN,
        GET_PLAYER_CHAR,
        CANCEL_OVERRIDE_RESTART,
        SET_POLICE_IGNORE_PLAYER,
        ADD_PAGER_MESSAGE_WITH_NUMBER,
        START_KILL_FRENZY,
        READ_KILL_FRENZY_STATUS,
        SQRT,
        LOCATE_PLAYER_ANY_MEANS_CAR_2D,
        LOCATE_PLAYER_ON_FOOT_CAR_2D,
        LOCATE_PLAYER_IN_CAR_CAR_2D,
        LOCATE_PLAYER_ANY_MEANS_CAR_3D,
        LOCATE_PLAYER_ON_FOOT_CAR_3D,
        LOCATE_PLAYER_IN_CAR_CAR_3D,
        LOCATE_CHAR_ANY_MEANS_CAR_2D,
        LOCATE_CHAR_ON_FOOT_CAR_2D,
        LOCATE_CHAR_IN_CAR_CAR_2D,
        LOCATE_CHAR_ANY_MEANS_CAR_3D,
        LOCATE_CHAR_ON_FOOT_CAR_3D,
        LOCATE_CHAR_IN_CAR_CAR_3D,
        GENERATE_RANDOM_FLOAT_IN_RANGE,
        GENERATE_RANDOM_INT_IN_RANGE,
        LOCK_CAR_DOORS,
        EXPLODE_CAR,
        ADD_EXPLOSION,
        IS_CAR_UPRIGHT,
        TURN_CHAR_TO_FACE_CHAR,
        TURN_CHAR_TO_FACE_PLAYER,
        TURN_PLAYER_TO_FACE_CHAR,
        SET_CHAR_OBJ_GOTO_COORD_ON_FOOT,
        SET_CHAR_OBJ_GOTO_COORD_IN_CAR,
        CREATE_PICKUP,
        HAS_PICKUP_BEEN_COLLECTED,
        REMOVE_PICKUP,
        SET_TAXI_LIGHTS,
        PRINT_BIG_Q,
        PRINT_WITH_NUMBER_BIG_Q,
        SET_GARAGE,
        SET_GARAGE_WITH_CAR_MODEL,
        SET_TARGET_CAR_FOR_MISSION_GARAGE,
        IS_CAR_IN_MISSION_GARAGE,
        SET_FREE_BOMBS,
        SET_POWERPOINT,
        SET_ALL_TAXI_LIGHTS,
        IS_CAR_ARMED_WITH_ANY_BOMB,
        APPLY_BRAKES_TO_PLAYERS_CAR,
        SET_PLAYER_HEALTH,
        SET_CHAR_HEALTH,
        SET_CAR_HEALTH,
        GET_PLAYER_HEALTH,
        GET_CHAR_HEALTH,
        GET_CAR_HEALTH,
        IS_CAR_ARMED_WITH_BOMB,
        CHANGE_CAR_COLOUR,
        SWITCH_PED_ROADS_ON,
        SWITCH_PED_ROADS_OFF,
        CHAR_LOOK_AT_CHAR_ALWAYS,
        CHAR_LOOK_AT_PLAYER_ALWAYS,
        PLAYER_LOOK_AT_CHAR_ALWAYS,
        STOP_CHAR_LOOKING,
        STOP_PLAYER_LOOKING,
        SWITCH_HELICOPTER,
        SET_GANG_ATTITUDE,
        SET_GANG_GANG_ATTITUDE,
        SET_GANG_PLAYER_ATTITUDE,
        SET_GANG_PED_MODELS,
        SET_GANG_CAR_MODEL,
        SET_GANG_WEAPONS,
        SET_CHAR_OBJ_RUN_TO_AREA,
        SET_CHAR_OBJ_RUN_TO_COORD,
        IS_PLAYER_TOUCHING_OBJECT_ON_FOOT,
        IS_CHAR_TOUCHING_OBJECT_ON_FOOT,
        LOAD_SPECIAL_CHARACTER,
        HAS_SPECIAL_CHARACTER_LOADED,
        FLASH_CAR,
        FLASH_CHAR,
        FLASH_OBJECT,
        IS_PLAYER_IN_REMOTE_MODE,
        ARM_CAR_WITH_BOMB,
        SET_CHAR_PERSONALITY,
        SET_CUTSCENE_OFFSET,
        SET_ANIM_GROUP_FOR_CHAR,
        SET_ANIM_GROUP_FOR_PLAYER,
        REQUEST_MODEL,
        HAS_MODEL_LOADED,
        MARK_MODEL_AS_NO_LONGER_NEEDED,
        GRAB_PHONE,
        SET_REPEATED_PHONE_MESSAGE,
        SET_PHONE_MESSAGE,
        HAS_PHONE_DISPLAYED_MESSAGE,
        TURN_PHONE_OFF,
        DRAW_CORONA,
        DRAW_LIGHT,
        STORE_WEATHER,
        RESTORE_WEATHER,
        STORE_CLOCK,
        RESTORE_CLOCK,
        RESTART_CRITICAL_MISSION,
        IS_PLAYER_PLAYING,
        SET_COLL_OBJ_NO_OBJ,
        SET_COLL_OBJ_WAIT_ON_FOOT,
        SET_COLL_OBJ_FLEE_ON_FOOT_TILL_SAFE,
        SET_COLL_OBJ_GUARD_SPOT,
        SET_COLL_OBJ_GUARD_AREA,
        SET_COLL_OBJ_WAIT_IN_CAR,
        SET_COLL_OBJ_KILL_CHAR_ON_FOOT,
        SET_COLL_OBJ_KILL_PLAYER_ON_FOOT,
        SET_COLL_OBJ_KILL_CHAR_ANY_MEANS,
        SET_COLL_OBJ_KILL_PLAYER_ANY_MEANS,
        SET_COLL_OBJ_FLEE_CHAR_ON_FOOT_TILL_SAFE,
        SET_COLL_OBJ_FLEE_PLAYER_ON_FOOT_TILL_SAFE,
        SET_COLL_OBJ_FLEE_CHAR_ON_FOOT_ALWAYS,
        SET_COLL_OBJ_FLEE_PLAYER_ON_FOOT_ALWAYS,
        SET_COLL_OBJ_GOTO_CHAR_ON_FOOT,
        SET_COLL_OBJ_GOTO_PLAYER_ON_FOOT,
        SET_COLL_OBJ_LEAVE_CAR,
        SET_COLL_OBJ_ENTER_CAR_AS_PASSENGER,
        SET_COLL_OBJ_ENTER_CAR_AS_DRIVER,
        SET_COLL_OBJ_FOLLOW_CAR_IN_CAR,
        SET_COLL_OBJ_FIRE_AT_OBJECT_FROM_VEHICLE,
        SET_COLL_OBJ_DESTROY_OBJECT,
        SET_COLL_OBJ_DESTROY_CAR,
        SET_COLL_OBJ_GOTO_AREA_ON_FOOT,
        SET_COLL_OBJ_GOTO_AREA_IN_CAR,
        SET_COLL_OBJ_FOLLOW_CAR_ON_FOOT_WITH_OFFSET,
        SET_COLL_OBJ_GUARD_ATTACK,
        SET_COLL_OBJ_FOLLOW_ROUTE,
        SET_COLL_OBJ_GOTO_COORD_ON_FOOT,
        SET_COLL_OBJ_GOTO_COORD_IN_CAR,
        SET_COLL_OBJ_RUN_TO_AREA,
        SET_COLL_OBJ_RUN_TO_COORD,
        ADD_PEDS_IN_AREA_TO_COLL,
        ADD_PEDS_IN_VEHICLE_TO_COLL,
        CLEAR_COLL,
        IS_COLL_IN_CARS,
        LOCATE_COLL_ANY_MEANS_2D,
        LOCATE_COLL_ON_FOOT_2D,
        LOCATE_COLL_IN_CAR_2D,
        LOCATE_STOPPED_COLL_ANY_MEANS_2D,
        LOCATE_STOPPED_COLL_ON_FOOT_2D,
        LOCATE_STOPPED_COLL_IN_CAR_2D,
        LOCATE_COLL_ANY_MEANS_CHAR_2D,
        LOCATE_COLL_ON_FOOT_CHAR_2D,
        LOCATE_COLL_IN_CAR_CHAR_2D,
        LOCATE_COLL_ANY_MEANS_CAR_2D,
        LOCATE_COLL_ON_FOOT_CAR_2D,
        LOCATE_COLL_IN_CAR_CAR_2D,
        LOCATE_COLL_ANY_MEANS_PLAYER_2D,
        LOCATE_COLL_ON_FOOT_PLAYER_2D,
        LOCATE_COLL_IN_CAR_PLAYER_2D,
        IS_COLL_IN_AREA_2D,
        IS_COLL_IN_AREA_ON_FOOT_2D,
        IS_COLL_IN_AREA_IN_CAR_2D,
        IS_COLL_STOPPED_IN_AREA_2D,
        IS_COLL_STOPPED_IN_AREA_ON_FOOT_2D,
        IS_COLL_STOPPED_IN_AREA_IN_CAR_2D,
        GET_NUMBER_OF_PEDS_IN_COLL,
        SET_CHAR_HEED_THREATS,
        SET_PLAYER_HEED_THREATS,
        GET_CONTROLLER_MODE,
        SET_CAN_RESPRAY_CAR,
        IS_TAXI,
        UNLOAD_SPECIAL_CHARACTER,
        RESET_NUM_OF_MODELS_KILLED_BY_PLAYER,
        GET_NUM_OF_MODELS_KILLED_BY_PLAYER,
        ACTIVATE_GARAGE,
        SWITCH_TAXI_TIMER,
        CREATE_OBJECT_NO_OFFSET,
        IS_BOAT,
        SET_CHAR_OBJ_GOTO_AREA_ANY_MEANS,
        SET_COLL_OBJ_GOTO_AREA_ANY_MEANS,
        IS_PLAYER_STOPPED,
        IS_CHAR_STOPPED,
        MESSAGE_WAIT,
        ADD_PARTICLE_EFFECT,
        SWITCH_WIDESCREEN,
        ADD_SPRITE_BLIP_FOR_CAR,
        ADD_SPRITE_BLIP_FOR_CHAR,
        ADD_SPRITE_BLIP_FOR_OBJECT,
        ADD_SPRITE_BLIP_FOR_CONTACT_POINT,
        ADD_SPRITE_BLIP_FOR_COORD,
        SET_CHAR_ONLY_DAMAGED_BY_PLAYER,
        SET_CAR_ONLY_DAMAGED_BY_PLAYER,
        SET_CHAR_PROOFS,
        SET_CAR_PROOFS,
        IS_PLAYER_IN_ANGLED_AREA_2D,
        IS_PLAYER_IN_ANGLED_AREA_ON_FOOT_2D,
        IS_PLAYER_IN_ANGLED_AREA_IN_CAR_2D,
        IS_PLAYER_STOPPED_IN_ANGLED_AREA_2D,
        IS_PLAYER_STOPPED_IN_ANGLED_AREA_ON_FOOT_2D,
        IS_PLAYER_STOPPED_IN_ANGLED_AREA_IN_CAR_2D,
        IS_PLAYER_IN_ANGLED_AREA_3D,
        IS_PLAYER_IN_ANGLED_AREA_ON_FOOT_3D,
        IS_PLAYER_IN_ANGLED_AREA_IN_CAR_3D,
        IS_PLAYER_STOPPED_IN_ANGLED_AREA_3D,
        IS_PLAYER_STOPPED_IN_ANGLED_AREA_ON_FOOT_3D,
        IS_PLAYER_STOPPED_IN_ANGLED_AREA_IN_CAR_3D,
        DEACTIVATE_GARAGE,
        GET_NUMBER_OF_CARS_COLLECTED_BY_GARAGE,
        HAS_CAR_BEEN_TAKEN_TO_GARAGE,
        SET_SWAT_REQUIRED,
        SET_FBI_REQUIRED,
        SET_ARMY_REQUIRED,
        IS_CAR_IN_WATER,
        GET_CLOSEST_CHAR_NODE,
        GET_CLOSEST_CAR_NODE,
        CAR_GOTO_COORDINATES_ACCURATE,
        START_PACMAN_RACE,
        START_PACMAN_RECORD,
        GET_NUMBER_OF_POWER_PILLS_EATEN,
        CLEAR_PACMAN,
        START_PACMAN_SCRAMBLE,
        GET_NUMBER_OF_POWER_PILLS_CARRIED,
        CLEAR_NUMBER_OF_POWER_PILLS_CARRIED,
        IS_CAR_ON_SCREEN,
        IS_CHAR_ON_SCREEN,
        IS_OBJECT_ON_SCREEN,
        GOSUB_FILE,
        GET_GROUND_Z_FOR_3D_COORD,
        START_SCRIPT_FIRE,
        IS_SCRIPT_FIRE_EXTINGUISHED,
        REMOVE_SCRIPT_FIRE,
        SET_COMEDY_CONTROLS,
        BOAT_GOTO_COORDS,
        BOAT_STOP,
        IS_PLAYER_SHOOTING_IN_AREA,
        IS_CHAR_SHOOTING_IN_AREA,
        IS_CURRENT_PLAYER_WEAPON,
        IS_CURRENT_CHAR_WEAPON,
        CLEAR_NUMBER_OF_POWER_PILLS_EATEN,
        ADD_POWER_PILL,
        SET_BOAT_CRUISE_SPEED,
        GET_RANDOM_CHAR_IN_AREA,
        GET_RANDOM_CHAR_IN_ZONE,
        IS_PLAYER_IN_TAXI,
        IS_PLAYER_SHOOTING,
        IS_CHAR_SHOOTING,
        CREATE_MONEY_PICKUP,
        SET_CHAR_ACCURACY,
        GET_CAR_SPEED,
        LOAD_CUTSCENE,
        CREATE_CUTSCENE_OBJECT,
        SET_CUTSCENE_ANIM,
        START_CUTSCENE,
        GET_CUTSCENE_TIME,
        HAS_CUTSCENE_FINISHED,
        CLEAR_CUTSCENE,
        RESTORE_CAMERA_JUMPCUT,
        CREATE_COLLECTABLE1,
        SET_COLLECTABLE1_TOTAL,
        IS_PROJECTILE_IN_AREA,
        DESTROY_PROJECTILES_IN_AREA,
        DROP_MINE,
        DROP_NAUTICAL_MINE,
        IS_CHAR_MODEL,
        LOAD_SPECIAL_MODEL,
        CREATE_CUTSCENE_HEAD,
        SET_CUTSCENE_HEAD_ANIM,
        SIN,
        COS,
        GET_CAR_FORWARD_X,
        GET_CAR_FORWARD_Y,
        CHANGE_GARAGE_TYPE,
        ACTIVATE_CRUSHER_CRANE,
        PRINT_WITH_2_NUMBERS,
        PRINT_WITH_2_NUMBERS_NOW,
        PRINT_WITH_2_NUMBERS_SOON,
        PRINT_WITH_3_NUMBERS,
        PRINT_WITH_3_NUMBERS_NOW,
        PRINT_WITH_3_NUMBERS_SOON,
        PRINT_WITH_4_NUMBERS,
        PRINT_WITH_4_NUMBERS_NOW,
        PRINT_WITH_4_NUMBERS_SOON,
        PRINT_WITH_5_NUMBERS,
        PRINT_WITH_5_NUMBERS_NOW,
        PRINT_WITH_5_NUMBERS_SOON,
        PRINT_WITH_6_NUMBERS,
        PRINT_WITH_6_NUMBERS_NOW,
        PRINT_WITH_6_NUMBERS_SOON,
        SET_CHAR_OBJ_FOLLOW_CHAR_IN_FORMATION,
        PLAYER_MADE_PROGRESS,
        SET_PROGRESS_TOTAL,
        REGISTER_JUMP_DISTANCE,
        REGISTER_JUMP_HEIGHT,
        REGISTER_JUMP_FLIPS,
        REGISTER_JUMP_SPINS,
        REGISTER_JUMP_STUNT,
        REGISTER_UNIQUE_JUMP_FOUND,
        SET_UNIQUE_JUMPS_TOTAL,
        REGISTER_PASSENGER_DROPPED_OFF_TAXI,
        REGISTER_MONEY_MADE_TAXI,
        REGISTER_MISSION_GIVEN,
        REGISTER_MISSION_PASSED,
        SET_CHAR_RUNNING,
        REMOVE_ALL_SCRIPT_FIRES,
        IS_FIRST_CAR_COLOUR,
        IS_SECOND_CAR_COLOUR,
        HAS_CHAR_BEEN_DAMAGED_BY_WEAPON,
        HAS_CAR_BEEN_DAMAGED_BY_WEAPON,
        IS_CHAR_IN_CHARS_GROUP,
        IS_CHAR_IN_PLAYERS_GROUP,
        EXPLODE_CHAR_HEAD,
        EXPLODE_PLAYER_HEAD,
        ANCHOR_BOAT,
        SET_ZONE_GROUP,
        START_CAR_FIRE,
        START_CHAR_FIRE,
        GET_RANDOM_CAR_OF_TYPE_IN_AREA,
        GET_RANDOM_CAR_OF_TYPE_IN_ZONE,
        HAS_RESPRAY_HAPPENED,
        SET_CAMERA_ZOOM,
        CREATE_PICKUP_WITH_AMMO,
        SET_CAR_RAM_CAR,
        SET_CAR_BLOCK_CAR,
        SET_CHAR_OBJ_CATCH_TRAIN,
        SET_COLL_OBJ_CATCH_TRAIN,
        SET_PLAYER_NEVER_GETS_TIRED,
        SET_PLAYER_FAST_RELOAD,
        SET_CHAR_BLEEDING,
        SET_CAR_FUNNY_SUSPENSION,
        SET_CAR_BIG_WHEELS,
        SET_FREE_RESPRAYS,
        SET_PLAYER_VISIBLE,
        SET_CHAR_VISIBLE,
        SET_CAR_VISIBLE,
        IS_AREA_OCCUPIED,
        START_DRUG_RUN,
        HAS_DRUG_RUN_BEEN_COMPLETED,
        HAS_DRUG_PLANE_BEEN_SHOT_DOWN,
        SAVE_PLAYER_FROM_FIRES,
        DISPLAY_TEXT,
        SET_TEXT_SCALE,
        SET_TEXT_COLOUR,
        SET_TEXT_JUSTIFY,
        SET_TEXT_CENTRE,
        SET_TEXT_WRAPX,
        SET_TEXT_CENTRE_SIZE,
        SET_TEXT_BACKGROUND,
        SET_TEXT_BACKGROUND_COLOUR,
        SET_TEXT_BACKGROUND_ONLY_TEXT,
        SET_TEXT_PROPORTIONAL,
        SET_TEXT_FONT,
        INDUSTRIAL_PASSED,
        COMMERCIAL_PASSED,
        SUBURBAN_PASSED,
        ROTATE_OBJECT,
        SLIDE_OBJECT,
        REMOVE_CHAR_ELEGANTLY,
        SET_CHAR_STAY_IN_SAME_PLACE,
        IS_NASTY_GAME,
        UNDRESS_CHAR,
        DRESS_CHAR,
        START_CHASE_SCENE,
        STOP_CHASE_SCENE,
        IS_EXPLOSION_IN_AREA,
        IS_EXPLOSION_IN_ZONE,
        START_DRUG_DROP_OFF,
        HAS_DROP_OFF_PLANE_BEEN_SHOT_DOWN,
        FIND_DROP_OFF_PLANE_COORDINATES,
        CREATE_FLOATING_PACKAGE,
        PLACE_OBJECT_RELATIVE_TO_CAR,
        MAKE_OBJECT_TARGETTABLE,
        ADD_ARMOUR_TO_PLAYER,
        ADD_ARMOUR_TO_CHAR,
        OPEN_GARAGE,
        CLOSE_GARAGE,
        WARP_CHAR_FROM_CAR_TO_COORD,
        SET_VISIBILITY_OF_CLOSEST_OBJECT_OF_TYPE,
        HAS_CHAR_SPOTTED_CHAR,
        SET_CHAR_OBJ_HAIL_TAXI,
        HAS_OBJECT_BEEN_DAMAGED,
        START_KILL_FRENZY_HEADSHOT,
        ACTIVATE_MILITARY_CRANE,
        WARP_PLAYER_INTO_CAR,
        WARP_CHAR_INTO_CAR,
        SWITCH_CAR_RADIO,
        SET_AUDIO_STREAM,
        PRINT_WITH_2_NUMBERS_BIG,
        PRINT_WITH_3_NUMBERS_BIG,
        PRINT_WITH_4_NUMBERS_BIG,
        PRINT_WITH_5_NUMBERS_BIG,
        PRINT_WITH_6_NUMBERS_BIG,
        SET_CHAR_WAIT_STATE,
        SET_CAMERA_BEHIND_PLAYER,
        SET_MOTION_BLUR,
        PRINT_STRING_IN_STRING,
        CREATE_RANDOM_CHAR,
        SET_CHAR_OBJ_STEAL_ANY_CAR,
        SET_2_REPEATED_PHONE_MESSAGES,
        SET_2_PHONE_MESSAGES,
        SET_3_REPEATED_PHONE_MESSAGES,
        SET_3_PHONE_MESSAGES,
        SET_4_REPEATED_PHONE_MESSAGES,
        SET_4_PHONE_MESSAGES,
        IS_SNIPER_BULLET_IN_AREA,
        GIVE_PLAYER_DETONATOR,
        SET_COLL_OBJ_STEAL_ANY_CAR,
        SET_OBJECT_VELOCITY,
        SET_OBJECT_COLLISION,
        IS_ICECREAM_JINGLE_ON,
        PRINT_STRING_IN_STRING_NOW,
        PRINT_STRING_IN_STRING_SOON,
        SET_5_REPEATED_PHONE_MESSAGES,
        SET_5_PHONE_MESSAGES,
        SET_6_REPEATED_PHONE_MESSAGES,
        SET_6_PHONE_MESSAGES,
        IS_POINT_OBSCURED_BY_A_MISSION_ENTITY,
        LOAD_ALL_MODELS_NOW,
        ADD_TO_OBJECT_VELOCITY,
        DRAW_SPRITE,
        DRAW_RECT,
        LOAD_SPRITE,
        LOAD_TEXTURE_DICTIONARY,
        REMOVE_TEXTURE_DICTIONARY,
        SET_OBJECT_DYNAMIC,
        SET_CHAR_ANIM_SPEED,
        PLAY_MISSION_PASSED_TUNE,
        CLEAR_AREA,
        FREEZE_ONSCREEN_TIMER,
        SWITCH_CAR_SIREN,
        SWITCH_PED_ROADS_ON_ANGLED,
        SWITCH_PED_ROADS_OFF_ANGLED,
        SWITCH_ROADS_ON_ANGLED,
        SWITCH_ROADS_OFF_ANGLED,
        SET_CAR_WATERTIGHT,
        ADD_MOVING_PARTICLE_EFFECT,
        SET_CHAR_CANT_BE_DRAGGED_OUT,
        TURN_CAR_TO_FACE_COORD,
        IS_CRANE_LIFTING_CAR,
        DRAW_SPHERE,
        SET_CAR_STATUS,
        IS_CHAR_MALE,
        SCRIPT_NAME,
        CHANGE_GARAGE_TYPE_WITH_CAR_MODEL,
        FIND_DRUG_PLANE_COORDINATES,
        SAVE_INT_TO_DEBUG_FILE,
        SAVE_FLOAT_TO_DEBUG_FILE,
        SAVE_NEWLINE_TO_DEBUG_FILE,
        POLICE_RADIO_MESSAGE,
        SET_CAR_STRONG,
        REMOVE_ROUTE,
        SWITCH_RUBBISH,
        REMOVE_PARTICLE_EFFECTS_IN_AREA,
        SWITCH_STREAMING,
        IS_GARAGE_OPEN,
        IS_GARAGE_CLOSED,
        START_CATALINA_HELI,
        CATALINA_HELI_TAKE_OFF,
        REMOVE_CATALINA_HELI,
        HAS_CATALINA_HELI_BEEN_SHOT_DOWN,
        SWAP_NEAREST_BUILDING_MODEL,
        SWITCH_WORLD_PROCESSING,
        REMOVE_ALL_PLAYER_WEAPONS,
        GRAB_CATALINA_HELI,
        CLEAR_AREA_OF_CARS,
        SET_ROTATING_GARAGE_DOOR,
        ADD_SPHERE,
        REMOVE_SPHERE,
        CATALINA_HELI_FLY_AWAY,
        SET_EVERYONE_IGNORE_PLAYER,
        STORE_CAR_CHAR_IS_IN_NO_SAVE,
        STORE_CAR_PLAYER_IS_IN_NO_SAVE,
        IS_PHONE_DISPLAYING_MESSAGE,
        DISPLAY_ONSCREEN_TIMER_WITH_STRING,
        DISPLAY_ONSCREEN_COUNTER_WITH_STRING,
        CREATE_RANDOM_CAR_FOR_CAR_PARK,
        IS_COLLISION_IN_MEMORY,
        SET_WANTED_MULTIPLIER,
        SET_CAMERA_IN_FRONT_OF_PLAYER,
        IS_CAR_VISIBLY_DAMAGED,
        DOES_OBJECT_EXIST,
        LOAD_SCENE,
        ADD_STUCK_CAR_CHECK,
        REMOVE_STUCK_CAR_CHECK,
        IS_CAR_STUCK,
        LOAD_MISSION_AUDIO,
        HAS_MISSION_AUDIO_LOADED,
        PLAY_MISSION_AUDIO,
        HAS_MISSION_AUDIO_FINISHED,
        GET_CLOSEST_CAR_NODE_WITH_HEADING,
        HAS_IMPORT_GARAGE_SLOT_BEEN_FILLED,
        CLEAR_THIS_PRINT,
        CLEAR_THIS_BIG_PRINT,
        SET_MISSION_AUDIO_POSITION,
        ACTIVATE_SAVE_MENU,
        HAS_SAVE_GAME_FINISHED,
        NO_SPECIAL_CAMERA_FOR_THIS_GARAGE,
        ADD_BLIP_FOR_PICKUP_OLD,
        ADD_BLIP_FOR_PICKUP,
        ADD_SPRITE_BLIP_FOR_PICKUP,
        SET_PED_DENSITY_MULTIPLIER,
        FORCE_RANDOM_PED_TYPE,
        SET_TEXT_DRAW_BEFORE_FADE,
        GET_COLLECTABLE1S_COLLECTED,
        REGISTER_EL_BURRO_TIME,
        SET_SPRITES_DRAW_BEFORE_FADE,
        SET_TEXT_RIGHT_JUSTIFY,
        PRINT_HELP,
        CLEAR_HELP,
        FLASH_HUD_OBJECT,
        FLASH_RADAR_BLIP,
        IS_CHAR_IN_CONTROL,
        SET_GENERATE_CARS_AROUND_CAMERA,
        CLEAR_SMALL_PRINTS,
        HAS_MILITARY_CRANE_COLLECTED_ALL_CARS,
        SET_UPSIDEDOWN_CAR_NOT_DAMAGED,
        CAN_PLAYER_START_MISSION,
        MAKE_PLAYER_SAFE_FOR_CUTSCENE,
        USE_TEXT_COMMANDS,
        SET_THREAT_FOR_PED_TYPE,
        CLEAR_THREAT_FOR_PED_TYPE,
        GET_CAR_COLOURS,
        SET_ALL_CARS_CAN_BE_DAMAGED,
        SET_CAR_CAN_BE_DAMAGED,
        MAKE_PLAYER_UNSAFE,
        LOAD_COLLISION,
        GET_BODY_CAST_HEALTH,
        SET_CHARS_CHATTING,
        MAKE_PLAYER_SAFE,
        SET_CAR_STAYS_IN_CURRENT_LEVEL,
        SET_CHAR_STAYS_IN_CURRENT_LEVEL,
        REGISTER_4X4_ONE_TIME,
        REGISTER_4X4_TWO_TIME,
        REGISTER_4X4_THREE_TIME,
        REGISTER_4X4_MAYHEM_TIME,
        REGISTER_LIFE_SAVED,
        REGISTER_CRIMINAL_CAUGHT,
        REGISTER_AMBULANCE_LEVEL,
        REGISTER_FIRE_EXTINGUISHED,
        TURN_PHONE_ON,
        REGISTER_LONGEST_DODO_FLIGHT,
        REGISTER_DEFUSE_BOMB_TIME,
        SET_TOTAL_NUMBER_OF_KILL_FRENZIES,
        BLOW_UP_RC_BUGGY,
        REMOVE_CAR_FROM_CHASE,
        IS_FRENCH_GAME,
        IS_GERMAN_GAME,
        CLEAR_MISSION_AUDIO,
        SET_FADE_IN_AFTER_NEXT_ARREST,
        SET_FADE_IN_AFTER_NEXT_DEATH,
        SET_GANG_PED_MODEL_PREFERENCE,
        SET_CHAR_USE_PEDNODE_SEEK,
        SWITCH_VEHICLE_WEAPONS,
        SET_GET_OUT_OF_JAIL_FREE,
        SET_FREE_HEALTH_CARE,
        IS_CAR_DOOR_CLOSED,
        LOAD_AND_LAUNCH_MISSION,
        LOAD_AND_LAUNCH_MISSION_INTERNAL,
        SET_OBJECT_DRAW_LAST,
        GET_AMMO_IN_PLAYER_WEAPON,
        GET_AMMO_IN_CHAR_WEAPON,
        REGISTER_KILL_FRENZY_PASSED,
        SET_CHAR_SAY,
        SET_NEAR_CLIP,
        SET_RADIO_CHANNEL,
        OVERRIDE_HOSPITAL_LEVEL,
        OVERRIDE_POLICE_STATION_LEVEL,
        FORCE_RAIN,
        DOES_GARAGE_CONTAIN_CAR,
        SET_CAR_TRACTION,
        ARE_MEASUREMENTS_IN_METRES,
        CONVERT_METRES_TO_FEET,
        MARK_ROADS_BETWEEN_LEVELS,
        MARK_PED_ROADS_BETWEEN_LEVELS,
        SET_CAR_AVOID_LEVEL_TRANSITIONS,
        SET_CHAR_AVOID_LEVEL_TRANSITIONS,
        IS_THREAT_FOR_PED_TYPE,
        CLEAR_AREA_OF_CHARS,
        SET_TOTAL_NUMBER_OF_MISSIONS,
        CONVERT_METRES_TO_FEET_INT,
        REGISTER_FASTEST_TIME,
        REGISTER_HIGHEST_SCORE,
        WARP_CHAR_INTO_CAR_AS_PASSENGER,
        IS_CAR_PASSENGER_SEAT_FREE,
        GET_CHAR_IN_CAR_PASSENGER_SEAT,
        SET_CHAR_IS_CHRIS_CRIMINAL,
        START_CREDITS,
        STOP_CREDITS,
        ARE_CREDITS_FINISHED,
        CREATE_SINGLE_PARTICLE,
        SET_CHAR_IGNORE_LEVEL_TRANSITIONS,
        GET_CHASE_CAR,
        START_BOAT_FOAM_ANIMATION,
        UPDATE_BOAT_FOAM_ANIMATION,
        SET_MUSIC_DOES_FADE,
        SET_INTRO_IS_PLAYING,
        SET_PLAYER_HOOKER,
        PLAY_END_OF_GAME_TUNE,
        STOP_END_OF_GAME_TUNE,
        GET_CAR_MODEL,
        IS_PLAYER_SITTING_IN_CAR,
        IS_PLAYER_SITTING_IN_ANY_CAR,
        SET_SCRIPT_FIRE_AUDIO,
        ARE_ANY_CAR_CHEATS_ACTIVATED,
        SET_CHAR_SUFFERS_CRITICAL_HITS,
        IS_PLAYER_LIFTING_A_PHONE,
        IS_CHAR_SITTING_IN_CAR,
        IS_CHAR_SITTING_IN_ANY_CAR,
        IS_PLAYER_ON_FOOT,
        IS_CHAR_ON_FOOT,
        LOAD_COLLISION_WITH_SCREEN,
        LOAD_SPLASH_SCREEN,
        SET_CAR_IGNORE_LEVEL_TRANSITIONS,
        MAKE_CRAIGS_CAR_A_BIT_STRONGER,
        SET_JAMES_CAR_ON_PATH_TO_PLAYER,
        LOAD_END_OF_GAME_TUNE,
        ENABLE_PLAYER_CONTROL_CAMERA,
        SET_OBJECT_ROTATION,
        GET_DEBUG_CAMERA_COORDINATES,
        GET_DEBUG_CAMERA_FRONT_VECTOR,
        IS_PLAYER_TARGETTING_ANY_CHAR,
        IS_PLAYER_TARGETTING_CHAR,
        IS_PLAYER_TARGETTING_OBJECT,
        TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME,
        DISPLAY_TEXT_WITH_NUMBER,
        DISPLAY_TEXT_WITH_2_NUMBERS,
        FAIL_CURRENT_MISSION,
        GET_CLOSEST_OBJECT_OF_TYPE,
        PLACE_OBJECT_RELATIVE_TO_OBJECT,
        SET_ALL_OCCUPANTS_OF_CAR_LEAVE_CAR,
        SET_INTERPOLATION_PARAMETERS,
        GET_CLOSEST_CAR_NODE_WITH_HEADING_TOWARDS_POINT,
        GET_CLOSEST_CAR_NODE_WITH_HEADING_AWAY_POINT,
        GET_DEBUG_CAMERA_POINT_AT,
        ATTACH_CHAR_TO_CAR,
        DETACH_CHAR_FROM_CAR,
        SET_CAR_CHANGE_LANE,
        CLEAR_CHAR_LAST_WEAPON_DAMAGE,
        CLEAR_CAR_LAST_WEAPON_DAMAGE,
        GET_RANDOM_COP_IN_AREA,
        GET_RANDOM_COP_IN_ZONE,
        SET_CHAR_OBJ_FLEE_CAR,
        GET_DRIVER_OF_CAR,
        GET_NUMBER_OF_FOLLOWERS,
        GIVE_REMOTE_CONTROLLED_MODEL_TO_PLAYER,
        GET_CURRENT_PLAYER_WEAPON,
        GET_CURRENT_CHAR_WEAPON,
        LOCATE_CHAR_ANY_MEANS_OBJECT_2D,
        LOCATE_CHAR_ON_FOOT_OBJECT_2D,
        LOCATE_CHAR_IN_CAR_OBJECT_2D,
        LOCATE_CHAR_ANY_MEANS_OBJECT_3D,
        LOCATE_CHAR_ON_FOOT_OBJECT_3D,
        LOCATE_CHAR_IN_CAR_OBJECT_3D,
        SET_CAR_HANDBRAKE_TURN_LEFT,
        SET_CAR_HANDBRAKE_TURN_RIGHT,
        SET_CAR_HANDBRAKE_STOP,
        IS_CHAR_ON_ANY_BIKE,
        LOCATE_SNIPER_BULLET_2D,
        LOCATE_SNIPER_BULLET_3D,
        GET_NUMBER_OF_SEATS_IN_MODEL,
        IS_PLAYER_ON_ANY_BIKE,
        IS_CHAR_LYING_DOWN,
        CAN_CHAR_SEE_DEAD_CHAR,
        SET_ENTER_CAR_RANGE_MULTIPLIER,
        SET_THREAT_REACTION_RANGE_MULTIPLIER,
    };

    public static class OpcodeExtensions
    {
        public static bool IsJump(this Opcode op)
        {
            switch (op)
            {
                case Opcode.GOTO:
                case Opcode.GOTO_IF_FALSE:
                case Opcode.GOTO_IF_TRUE:
                case Opcode.GOSUB:
                case Opcode.GOSUB_FILE:
                    return true;
            }

            return false;
        }
    }
}
