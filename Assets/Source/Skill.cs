using System;
using UnityEngine;

public class Skill
{
    public const int MONEY_INTERVAL_EARN_SKILL = 0;
    public const int XP_INTERVAL_EARN_SKILL = 1;
    public const int PLAYER_SPEED_SKILL = 2;
    public const int ARCADE_FIX_COST_REDUCTION_SKILL = 3;
    public const int CHEAPER_ENERGYDRINK_SKILL = 4;

    public const int UNAVAILABLE_SPRITE = 0;
    public const int AVAILABLE_SPRITE = 1;
    public const int SELECTED_SPRITE = 2;
    public const int SELECTED_ACTIVATED_SPRITE = 3;
    public const int ACTIVATED_SPRITE = 4;

    private int skillType = -1;
    private int skillLevel = 1;

    private string skillDescription;

    private Sprite unavailableSprite;
    private Sprite availableSprite;
    private Sprite selectedSprite;
    private Sprite selectedActivatedSprite;
    private Sprite activatedSprite;

    private bool selected = false;

    public Skill(int skillType) {
        this.skillType = skillType;
    }

    public Skill(int skillType, string spriteName) {
        this.skillType = skillType;
        unavailableSprite = Resources.Load<Sprite>("Sprites/" + spriteName + "_" + UNAVAILABLE_SPRITE);
        availableSprite = Resources.Load<Sprite>("Sprites/" + spriteName + "_" + AVAILABLE_SPRITE);
        selectedSprite = Resources.Load<Sprite>("Sprites/" + spriteName + "_" + SELECTED_SPRITE);
        selectedActivatedSprite = Resources.Load<Sprite>("Sprites/" + spriteName + "_" + SELECTED_ACTIVATED_SPRITE);
        activatedSprite = Resources.Load<Sprite>("Sprites/" + spriteName + "_" + ACTIVATED_SPRITE);
    }

    public void setSkillDescription(string skillDescription) {
        this.skillDescription = skillDescription;
    }

    public void setSkillLevel(int skillLevel) {
        this.skillLevel = skillLevel;
    }

    public string getSkillDescription() {
        return this.skillDescription;
    }

    public int getSkillLevel() {
        return this.skillLevel;
    }

    public int getSkillType() {
        return this.skillType;
    }

    public Sprite getSprite(int spriteInt) {
        Sprite ret;

        switch(spriteInt) {
            case UNAVAILABLE_SPRITE:
                ret = unavailableSprite;
                break;
            case AVAILABLE_SPRITE:
                ret = availableSprite;
                break;
            case SELECTED_SPRITE:
                ret = selectedSprite;
                break;
            case SELECTED_ACTIVATED_SPRITE:
                ret = selectedActivatedSprite;
                break;
            case ACTIVATED_SPRITE:
                ret = activatedSprite;
                break;
            default:
                ret = unavailableSprite;
                break;
        }

        return ret;
    }
}
