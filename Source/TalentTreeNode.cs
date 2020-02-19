using System;
using System.Collections.Generic;
public class TalentTreeNode
{
    private bool isRootNode = false;
    private bool unlocked = false;
    private Skill skill;
	public List<TalentTreeNode> branches;

    public TalentTreeNode(bool isRootNode, Skill newSkill) {
        branches = new List<TalentTreeNode>();
        this.isRootNode = isRootNode;
        this.skill = newSkill;
	}

    public TalentTreeNode addTalentNode(Skill newSkill) {
        TalentTreeNode newNode = new TalentTreeNode(false, newSkill);
        branches.Add(newNode);
        return newNode;
    }

    public Skill getSkill() {
		return this.skill;
	}

}
