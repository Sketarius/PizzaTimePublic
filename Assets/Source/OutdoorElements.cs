using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorElements : MonoBehaviour
{
    public Accountant theAccountant;
    public Light[] sunPoints;

    public enum timeOfDay {
        MORNING = 0,
        MID_MORNING = 1,
        AFTERNOON = 2,
        MID_AFTERNOON = 3,
        EVENING = 4,
        NIGHT = 5,
        START,
        END
    };

    private enum rgbi {
        R, G, B, I
    }

    public Dictionary<timeOfDay, Dictionary<timeOfDay, int>> timeOfDayStartEnd = new Dictionary<timeOfDay, Dictionary<timeOfDay, int>>() {
        { timeOfDay.MORNING, new Dictionary<timeOfDay, int>() { { timeOfDay.START, 7 }, { timeOfDay.END, 10 } } },
        { timeOfDay.MID_MORNING, new Dictionary<timeOfDay, int>() { { timeOfDay.START, 10 }, { timeOfDay.END, 11 } } },
        { timeOfDay.AFTERNOON, new Dictionary<timeOfDay, int>() { { timeOfDay.START, 11 }, { timeOfDay.END, 13 } } },
        { timeOfDay.MID_AFTERNOON, new Dictionary<timeOfDay, int>() { { timeOfDay.START, 13 }, { timeOfDay.END, 17 } } },
        { timeOfDay.EVENING, new Dictionary<timeOfDay, int>() { { timeOfDay.START, 17 }, { timeOfDay.END, 19 } } },
        { timeOfDay.NIGHT, new Dictionary<timeOfDay, int>() { { timeOfDay.START, 19 }, { timeOfDay.END, 7 } } }
    };

    public int getTimeOfDay() {
        int ret = 0;
        System.DateTime gameTime = theAccountant.getGameTime();

        if ((gameTime.Hour >= getTimeOfDayStartHourFromEnum(timeOfDay.MORNING)) && (gameTime.Hour < getTimeOfDayEndHourFromEnum(timeOfDay.MORNING))) {
            ret = (int)timeOfDay.MORNING;
        } else if ((gameTime.Hour >= getTimeOfDayStartHourFromEnum(timeOfDay.MID_MORNING)) && (gameTime.Hour < getTimeOfDayEndHourFromEnum(timeOfDay.MID_MORNING))) {
            ret = (int)timeOfDay.MID_MORNING;
        } else if ((gameTime.Hour >= getTimeOfDayStartHourFromEnum(timeOfDay.AFTERNOON)) && (gameTime.Hour < getTimeOfDayEndHourFromEnum(timeOfDay.AFTERNOON))) {
            ret = (int)timeOfDay.AFTERNOON;
        } else if ((gameTime.Hour >= getTimeOfDayStartHourFromEnum(timeOfDay.MID_AFTERNOON)) && (gameTime.Hour < getTimeOfDayEndHourFromEnum(timeOfDay.MID_AFTERNOON))) {
            ret = (int)timeOfDay.MID_AFTERNOON;
        } else if ((gameTime.Hour >= getTimeOfDayStartHourFromEnum(timeOfDay.EVENING)) && (gameTime.Hour < getTimeOfDayEndHourFromEnum(timeOfDay.EVENING))) {
            ret = (int)timeOfDay.EVENING;
        } else if ((gameTime.Hour >= getTimeOfDayStartHourFromEnum(timeOfDay.NIGHT)) || (gameTime.Hour % 24 < getTimeOfDayEndHourFromEnum(timeOfDay.NIGHT))) {
            ret = (int)timeOfDay.NIGHT;
        }

        return ret;
    }

    public int getTimeOfDayStartHourFromEnum(timeOfDay hour) {
        return timeOfDayStartEnd[hour][timeOfDay.START];
    }

    public int getTimeOfDayEndHourFromEnum(timeOfDay hour) {
        return timeOfDayStartEnd[hour][timeOfDay.END];
    }

    public timeOfDay getNearestTimeOfDayStart(System.DateTime gameTime) {
        foreach (KeyValuePair<timeOfDay, Dictionary<timeOfDay, int>> tod in timeOfDayStartEnd) {
            if (gameTime.Hour >= tod.Value[timeOfDay.START] && gameTime.Hour < tod.Value[timeOfDay.END]) {
                return tod.Key;
            }
        }

        // Return night because of it's wrapping
        return timeOfDay.NIGHT;
    }

    /// <summary>
    /// Returns a value of 0 to 1 to interpolate between different sun times
    /// </summary>
    /// <returns>0 to 1 between two times</returns>
    public float lerpTimeOfDay() {
        var lerpRatio = 0.0f;
        System.DateTime gameTime = theAccountant.getGameTime();
        timeOfDay nearestTimeOfDay = getNearestTimeOfDayStart(gameTime);
        float LAST_MAJOR_TIME_START = timeOfDayStartEnd[nearestTimeOfDay][timeOfDay.START];
        float NEXT_MAJOR_TIME_START = timeOfDayStartEnd[nearestTimeOfDay][timeOfDay.END];

        if (nearestTimeOfDay != timeOfDay.NIGHT) {
            lerpRatio = ((gameTime.Hour + (gameTime.Minute / 60.0f)) - LAST_MAJOR_TIME_START) / (NEXT_MAJOR_TIME_START - LAST_MAJOR_TIME_START);
        } else {
            var morningLerpStart = getTimeOfDayStartHourFromEnum(timeOfDay.MORNING) - 1;
            if (gameTime.Hour == morningLerpStart) {
                lerpRatio = (
                    (gameTime.Hour + (gameTime.Minute / 60.0f)) - morningLerpStart) / (NEXT_MAJOR_TIME_START - morningLerpStart);
            } else {
                lerpRatio = 0.0f;
            }
        }

        return lerpRatio;
    }

    private void setLightIntensity(Light[] lights, float intensity, float r, float g, float b) {
        for (int i = 0; i < lights.Length; i++) {
            lights[i].intensity = intensity;
            lights[i].color = new Color(r, g, b, 255);
        }
    }

    private Dictionary<timeOfDay, Dictionary<rgbi, float>> timeOfDayRGBI = new Dictionary<timeOfDay, System.Collections.Generic.Dictionary<rgbi, float>>() { 
        { timeOfDay.MORNING, 
            new Dictionary<rgbi, float>() {
                { rgbi.I, 3.58f },
                { rgbi.R, 1f },
                { rgbi.G, 0.6064516f },
                { rgbi.B, 0f },
            } 
        },
        { timeOfDay.MID_MORNING,
            new Dictionary<rgbi, float>() {
                { rgbi.I, 3.58f },
                { rgbi.R, 1f },
                { rgbi.G, 0.7599899f },
                { rgbi.B, 0.3915094f },
            }
        },
        { timeOfDay.AFTERNOON,
            new Dictionary<rgbi, float>() {
                { rgbi.I, 5.71f },
                { rgbi.R, 1f },
                { rgbi.G, 1f },
                { rgbi.B, 1f },
            }
        },
        { timeOfDay.MID_AFTERNOON,
            new Dictionary<rgbi, float>() {
                { rgbi.I, 5.71f },
                { rgbi.R, 0.9433962f },
                { rgbi.G, 0.8452166f },
                { rgbi.B, 0.3337487f },
            }
        },
        { timeOfDay.EVENING,
            new Dictionary<rgbi, float>() {
                { rgbi.I, 2.15f },
                { rgbi.R, 0.4528302f },
                { rgbi.G, 0.4456418f },
                { rgbi.B, 0.4079744f },
            }
        },
        { timeOfDay.NIGHT,
            new Dictionary<rgbi, float>() {
                { rgbi.I, 4.28f },
                { rgbi.R, 0.25f },
                { rgbi.G, 0.5065942f },
                { rgbi.B, 1f },
            }
        },
    };

    private void updateOutDoorLights() {
        setLightIntensity(
            sunPoints,
            timeOfDayRGBI[(timeOfDay)getTimeOfDay()][rgbi.I] + (timeOfDayRGBI[(timeOfDay)((getTimeOfDay() + 1) % 6)][rgbi.I] - timeOfDayRGBI[(timeOfDay)getTimeOfDay()][rgbi.I]) * lerpTimeOfDay(),
            timeOfDayRGBI[(timeOfDay)getTimeOfDay()][rgbi.R] + (timeOfDayRGBI[(timeOfDay)((getTimeOfDay() + 1) % 6)][rgbi.R] - timeOfDayRGBI[(timeOfDay)getTimeOfDay()][rgbi.R]) * lerpTimeOfDay(),
            timeOfDayRGBI[(timeOfDay)getTimeOfDay()][rgbi.G] + (timeOfDayRGBI[(timeOfDay)((getTimeOfDay() + 1) % 6)][rgbi.G] - timeOfDayRGBI[(timeOfDay)getTimeOfDay()][rgbi.G]) * lerpTimeOfDay(),
            timeOfDayRGBI[(timeOfDay)getTimeOfDay()][rgbi.B] + (timeOfDayRGBI[(timeOfDay)((getTimeOfDay() + 1) % 6)][rgbi.B] - timeOfDayRGBI[(timeOfDay)getTimeOfDay()][rgbi.B]) * lerpTimeOfDay()
        );
    }
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        updateOutDoorLights();
    }
}
